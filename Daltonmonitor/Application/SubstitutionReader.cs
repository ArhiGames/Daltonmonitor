using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Mappers;
using Daltonmonitor.Models.Substitution;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types;
using Daltonmonitor.Models.Types.Common.Errors;
using Daltonmonitor.Models.Types.Common.Result;

namespace Daltonmonitor.Application;

public class SubstitutionReader(ConfigManager configManager)
{
    private Timetable? Timetable { get; set; }

    private char CsvSplitCharacter { get; set; } = '\0';
    private string[] DaltonIdentifiers { get; set; } = [];
    private string[] WorkshopIdentifiers { get; set; } = [];
    private string[] MentorIdentifiers { get; set; } = [];

    public Result<Timetable> Process()
    {
        string csvSplitString = configManager.GetConfigValue(ConfigIdentifier.GpuSplitCharacter);
        if (csvSplitString.Length < 1)
        {
            return Errors.Unknown;
        }
        CsvSplitCharacter = csvSplitString[0];
        
        DaltonIdentifiers = configManager.GetConfigListValue(ConfigIdentifier.DaltonIdentifiers);
        WorkshopIdentifiers = configManager.GetConfigListValue(ConfigIdentifier.WorkshopIdentifiers);
        MentorIdentifiers = configManager.GetConfigListValue(ConfigIdentifier.MentorIdentifiers);

        Result readDaltonDataResult = ReadRegularDaltonData();
        if (!readDaltonDataResult.IsSuccess)
        {
           return readDaltonDataResult.Error!;
        }

        // These may fail (result is not checked) as they are not mandatory
        ReadSubstitutionData();
        ReadVariantsData();
        ReadVacationData();

        return Timetable!;
    }

    public Result HandleUserMode()
    {
        string userModeString = configManager.GetConfigValue(ConfigIdentifier.UserMode);
        switch (userModeString)
        {
            case "DEL":
                DeleteGpuFiles();
                break;
            case "KEEP":
                break;
            default:
                return Errors.InvalidUserMode;
        }

        return Result.Success();
    }

    private void DeleteGpuFiles()
    {
        string gpu001Path = configManager.GetConfigValue(ConfigIdentifier.GPU001);
        string gpu002Path = configManager.GetConfigValue(ConfigIdentifier.GPU002);
        string gpu014Path = configManager.GetConfigValue(ConfigIdentifier.GPU014);
        string gpu018Path = configManager.GetConfigValue(ConfigIdentifier.GPU018);

        try
        {
            File.Delete(gpu001Path);
            File.Delete(gpu002Path);
            File.Delete(gpu014Path);
            File.Delete(gpu018Path);
        }
        catch
        {
            // ignored
        }
    }

    private Result ReadRegularDaltonData()
    {
        string gpu001Path = configManager.GetConfigValue(ConfigIdentifier.GPU001);

        string[] lines;
        try
        {
            lines = File.ReadAllLines(gpu001Path);
        }
        catch
        {
            return Errors.FileError;
        }
        
        Timetable = new Timetable(lines.Length);
        
        foreach (string line in lines)
        {
            string[] lineContents = line.CsvSplit(CsvSplitCharacter);

            List<Class> classes = [];

            DaltonType daltonType = GetDaltonType(lineContents[3]);
            switch (daltonType)
            {
                case DaltonType.None:
                    continue;
                case DaltonType.Dalton:
                case DaltonType.Workshop:
                    break;
                case DaltonType.Mentor:
                {
                    Result<string> parseMentorClassResult = GetTypedMentorClassIdentifier(lineContents[3]);
                    if (!parseMentorClassResult.IsSuccess) break;
                    
                    classes.Add(new Class(parseMentorClassResult.Value!));
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            int lessonId = Convert.ToInt32(lineContents[0]);
            List<Teacher> teachers = [new(lineContents[2])];
            
            string[] roomsIdentifiers = lineContents[4].CsvSplit('~');
            List<Room> rooms = roomsIdentifiers.Select(roomIdentifier => new Room(roomIdentifier)).ToList();

            DayOfWeek day = (DayOfWeek)Convert.ToInt32(lineContents[5]);
            int lesson = Convert.ToInt32(lineContents[6]);

            TimetableLessonData? existingTimetableLessonData = Timetable.TimetableLessonDatas.FirstOrDefault(tld =>
                tld.LessonId == lessonId &&
                tld.DaltonType == daltonType &&
                tld.Teachers == teachers &&
                tld.Day == day &&
                tld.Lesson == lesson);
            if (existingTimetableLessonData is null)
            {
                TimetableLessonData timetableLessonData = new(lessonId, daltonType, classes, teachers, rooms, day, lesson);
                Timetable.AddDaltonLesson(timetableLessonData);
            }
            else
            {
                existingTimetableLessonData.AddTeachers(teachers);
            }
        }

        return Result.Success();
    }

    private Result ReadSubstitutionData()
    {
        string gpu014Path = configManager.GetConfigValue(ConfigIdentifier.GPU014);

        string[] lines;
        try
        {
            lines = File.ReadAllLines(gpu014Path);
        }
        catch
        {
            return Errors.FileError;
        }
        
        foreach (string line in lines)
        {
            string[] lineContents = line.CsvSplit(CsvSplitCharacter);

            DaltonType overrideDaltonType = GetDaltonType(lineContents[7]);
            if (overrideDaltonType == DaltonType.None)
            {
                continue;
            }
            
            // Bound dalton lessons can't be checked using GetDaltonType, as it requires another lineContents
            string? additionalInformation = null;
            Result<string> boundDaltonLessonResult = IsBoundDaltonLesson(lineContents[16]);
            if (boundDaltonLessonResult.IsSuccess)
            {
                overrideDaltonType = DaltonType.Bound;
                additionalInformation = boundDaltonLessonResult.Value;
            }

            int substitutionId = Convert.ToInt32(lineContents[0]);
            
            DateTime dateTime = DateTime.ParseExact(lineContents[1], "yyyyMMdd", CultureInfo.InvariantCulture);
            int lesson = Convert.ToInt32(lineContents[2]);
            int lessonId = Convert.ToInt32(lineContents[4]);
            Teacher substituteTeacher = new(lineContents[6]);

            string[] roomsIdentifiers = lineContents[12].CsvSplit('~');
            List<Room> substituteRooms = roomsIdentifiers.Select(roomIdentifier => new Room(roomIdentifier)).ToList();

            SubstitutionFlags substitutionFlags = (SubstitutionFlags)Convert.ToInt32(lineContents[17]);
            SubstitutionType substitutionType = GetSubstitutionType(lineContents[19]);

            SubstitutionData substitutionData = new(substitutionId, dateTime, lesson, substituteTeacher,
                substituteRooms, overrideDaltonType, substitutionFlags, substitutionType, additionalInformation);

            TimetableLessonData? timetableLessonData =
                Timetable!.TimetableLessonDatas.FirstOrDefault(tld => tld.LessonId == lessonId &&
                                                                      tld.Day == dateTime.DayOfWeek &&
                                                                      tld.Lesson == lesson);
            timetableLessonData?.AddSubstitutionData(substitutionData);
        }

        return Result.Success();
    }

    private Result ReadVacationData()
    {
        string gpu018Path = configManager.GetConfigValue(ConfigIdentifier.GPU018);

        string[] lines;
        try
        {
            lines = File.ReadAllLines(gpu018Path);
        }
        catch
        {
            return Errors.FileError;
        }

        foreach (string line in lines)
        {
            string[] lineContents = line.CsvSplit(CsvSplitCharacter);

            string vacationName = lineContents[0];
            string vacationDescription = lineContents[1];
            DateTime vacationStartDate = DateTime.ParseExact(lineContents[2], "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime vacationEndDate = DateTime.ParseExact(lineContents[3], "yyyyMMdd", CultureInfo.InvariantCulture);
            bool isOffDay = lineContents[4] == "F";

            VacationData vacationData = new(vacationName, vacationDescription, vacationStartDate, vacationEndDate,
                isOffDay);
            Timetable!.AddVacationData(vacationData);
        }

        return Result.Success();
    }

    private DaltonType GetDaltonType(string identifier)
    {
        if (DaltonIdentifiers.Contains(identifier))
        {
            return DaltonType.Dalton;
        }
        
        if (WorkshopIdentifiers.Contains(identifier))
        {
            return DaltonType.Workshop;
        }

        if (MentorIdentifiers.Contains(identifier))
        {
            return DaltonType.Mentor;
        }
        
        return DaltonType.None;
    }
    
    private Result ReadVariantsData()
    {
        string gpu002Path = configManager.GetConfigValue(ConfigIdentifier.GPU002);

        string[] lines;
        try
        {
            lines = File.ReadAllLines(gpu002Path);
        }
        catch
        {
            return Errors.FileError;
        }

        foreach (string line in lines)
        {
            string[] lineContents = line.CsvSplit(CsvSplitCharacter);

            int lessonId = Convert.ToInt32(lineContents[0]);

            string teacherIdentifier = lineContents[5];
            Teacher teacher = new(teacherIdentifier);

            string roomIdentifier = lineContents[7];
            Room room = new(roomIdentifier);

            string variantIdentifier = lineContents[11];
            if (variantIdentifier.IsWhiteSpace())
            {
                continue;
            }

            List<TimetableLessonData> timetableLessonDatas =
                Timetable!.TimetableLessonDatas.Where(tld => tld.LessonId == lessonId && 
                                                             tld.Teachers.Contains(teacher) && 
                                                             tld.Rooms.Contains(room)).ToList();
            foreach (TimetableLessonData timetableLessonData in timetableLessonDatas)
            {
                timetableLessonData.VariantIdentifier = variantIdentifier;
            }
        }

        return Result.Success();
    }

    private Result<string> IsBoundDaltonLesson(string value)
    {
        string typedStringTemplate = configManager.GetConfigValue(ConfigIdentifier.BoundDaltonLessonPattern);
        const string templateString = "{Information}";
        int indexOfOpening = typedStringTemplate.IndexOf(templateString, StringComparison.Ordinal);
        if (indexOfOpening == -1)
        {
            return Errors.Unknown;
        }
        int indexOfClosing = indexOfOpening + templateString.Length;
        
        string first = typedStringTemplate[..indexOfOpening];
        string last = typedStringTemplate[indexOfClosing..];

        if (!value.Contains(first) || !value.Contains(last))
        {
            return Errors.Unknown;
        }
        
        if (first != string.Empty) value = value.Replace(first, "");
        if (last != string.Empty) value = value.Replace(last, "");
        return value;
    }

    private Result<string> GetTypedMentorClassIdentifier(string value)
    {
        string typedStringTemplate = configManager.GetConfigValue(ConfigIdentifier.MentorPattern);
        const string templateString = "{Class}";
        int indexOfOpening = typedStringTemplate.IndexOf(templateString, StringComparison.Ordinal);
        if (indexOfOpening == -1)
        {
            return Errors.Unknown;
        }
        int indexOfClosing = indexOfOpening + templateString.Length;
        
        string first = typedStringTemplate[..indexOfOpening];
        string last = typedStringTemplate[indexOfClosing..];

        if (first != string.Empty) value = value.Replace(first, "");
        if (last != string.Empty) value = value.Replace(last, "");

        return value;
    }
    
    private static SubstitutionType GetSubstitutionType(string identifier)
    {
        return identifier switch
        {
            "T" => SubstitutionType.Rescheduled,
            "F" => SubstitutionType.TimeChange,
            "W" => SubstitutionType.Swap,
            "S" => SubstitutionType.Supervision,
            "A" => SubstitutionType.SpecialAssignment,
            "C" => SubstitutionType.Cancelled,
            "L" => SubstitutionType.Release,
            "P" => SubstitutionType.PartialCover,
            "R" => SubstitutionType.ClassroomCover,
            "B" => SubstitutionType.BreakSupervisionCover,
            "~" => SubstitutionType.TeacherSwap,
            "E" => SubstitutionType.Exam,
            _ => SubstitutionType.None
        };
    }
}