using System;
using System.Collections.Generic;
using System.IO;
using Daltonmonitor.Mappers;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types;
using Daltonmonitor.Models.Types.Common.Result;

namespace Daltonmonitor.Application;

public class SubstitutionReader
{
    private const string FileName = @"";
    private Timetable? Timetable { get; set; } = null;

    public Result StartProcess()
    {
       Result readDaltonDataResult = ReadRegularDaltonData();
       if (!readDaltonDataResult.IsSuccess)
       {
           return readDaltonDataResult;
       }

       return readDaltonDataResult;
    }

    private Result ReadRegularDaltonData()
    {
        string[] lines = File.ReadAllLines(FileName);
        Timetable = new Timetable(lines.Length);

        foreach (string line in lines)
        {
            List<string> lineContents = line.Replace('"', ' ').EnhancedSplit(',');

            // Todo read dalton specifier that should be ignored from config
            if (!lineContents[3].Equals("DAL") && !lineContents[3].Equals("DAL+"))
            {
                continue;
            }
            
            int lessonId = Convert.ToInt32(lineContents[0]);
            Teacher teacher = new(lineContents[2]);
            // Todo read relevant data from config
            DaltonType daltonType = 
                lineContents[3] == "DAL+" ? DaltonType.Workshop : 
                lineContents[3] == "DAL" ? DaltonType.Dalton : 
                DaltonType.None;
            Room room = new(lineContents[4]);
            EDay day = (EDay)Convert.ToInt32(lineContents[5]);
            int lesson = Convert.ToInt32(lineContents[6]);

            TimetableLessonData timetableLessonData = new(lessonId, daltonType, teacher, room, day, lesson);
            Timetable.AddDaltonLesson(timetableLessonData);
        }

        return Result.Success();
    }
}