using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Daltonmonitor.Mappers;
using Daltonmonitor.Models.Substitution;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types;
using Daltonmonitor.Models.Types.Common.Result;

namespace Daltonmonitor.Application;

public class SubstitutionReader
{

    private Timetable? Timetable { get; set; }

    public Result<Timetable> Process()
    {
       Result readDaltonDataResult = ReadRegularDaltonData();
       if (!readDaltonDataResult.IsSuccess)
       {
           return readDaltonDataResult.Error!;
       }

       Result readSubstitutionDataResult = ReadSubstitutionData();
       return readSubstitutionDataResult.IsSuccess ? Timetable! : readSubstitutionDataResult.Error!;
    }

    private Result ReadRegularDaltonData()
    {
        string[] lines = File.ReadAllLines(File001);
        Timetable = new Timetable(lines.Length);

        foreach (string line in lines)
        {
            string[] lineContents = line.Replace('"', ' ').EnhancedSplit(',');

            // Todo idx from config
            if (!IsDalton(lineContents[3]))
            {
                continue;
            }
            
            int lessonId = Convert.ToInt32(lineContents[0]);
            List<Teacher> teachers = [new(lineContents[2])];
            DaltonType daltonType = GetDaltonType(lineContents[3]);
            
            string[] roomsIdentifiers = lineContents[4].EnhancedSplit('~');
            List<Room> rooms = roomsIdentifiers.Select(roomIdentifier => new Room(roomIdentifier)).ToList();

            DayOfWeek day = (DayOfWeek)Convert.ToInt32(lineContents[5]);
            int lesson = Convert.ToInt32(lineContents[6]);

            TimetableLessonData? existingTimetableLessonData = Timetable.TimetableLessonDatas.FirstOrDefault(tld =>
                tld.LessonId == lessonId &&
                tld.DaltonType == daltonType &&
                tld.Day == day &&
                tld.Lesson == lesson);
            if (existingTimetableLessonData is null)
            {
                TimetableLessonData timetableLessonData = new(lessonId, daltonType, teachers, rooms, day, lesson);
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
        string[] lines = File.ReadAllLines(File014);
        
        foreach (string line in lines)
        {
            string[] lineContents = line.Replace('"', ' ').EnhancedSplit(',');

            if (!IsDalton(lineContents[7]))
            {
                continue;
            }

            int substitutionId = Convert.ToInt32(lineContents[0]);
            DateTime dateTime = DateTime.ParseExact(lineContents[1], "yyyyMMdd", CultureInfo.InvariantCulture);
            int lesson = Convert.ToInt32(lineContents[2]);
            int lessonId = Convert.ToInt32(lineContents[4]);
            Teacher substituteTeacher = new(lineContents[6]);

            string[] roomsIdentifiers = lineContents[12].EnhancedSplit('~');
            List<Room> substituteRooms = roomsIdentifiers.Select(roomIdentifier => new Room(roomIdentifier)).ToList();

            SubstitutionFlags substitutionFlags = (SubstitutionFlags)Convert.ToInt32(lineContents[17]);
            SubstitutionType substitutionType = GetSubstitutionType(lineContents[19]);

            SubstitutionData substitutionData = new(substitutionId, dateTime, lesson, substituteTeacher,
                substituteRooms, substitutionFlags, substitutionType);

            TimetableLessonData? timetableLessonData =
                Timetable!.TimetableLessonDatas.FirstOrDefault(tld => tld.LessonId == lessonId &&
                                                                      tld.Day == dateTime.DayOfWeek &&
                                                                      tld.Lesson == lesson);
            timetableLessonData?.AddSubstitutionData(substitutionData);
        }

        return Result.Success();
    }

    private static DaltonType GetDaltonType(string identifier)
    {
        return identifier switch
        {
            "DAL+" => DaltonType.Workshop,
            "DAL" => DaltonType.Dalton,
            _ => DaltonType.None
        };
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
    
    private static bool IsDalton(string identifier)
    {
        // Todo read DAL+ / DAL identifier from config
        return identifier.Equals("DAL+") || identifier.Equals("DAL");
    }
}