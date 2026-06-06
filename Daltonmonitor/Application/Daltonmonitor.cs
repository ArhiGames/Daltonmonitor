using System.IO;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types.Common.Result;

namespace Daltonmonitor.Application;

public class Daltonmonitor
{
    private SubstitutionReader SubstitutionReader { get; } = new();
    private HtmlGenerator HtmlGenerator { get; } = new();
    private ConfigManager ConfigManager { get; } = new();

    public void Process()
    {
        Result<Timetable> timetableResult = SubstitutionReader.Process();
        if (!timetableResult.IsSuccess)
        {
            return;
        }
        
        Timetable timetable = timetableResult.Value!;
        string htmlStructure = HtmlGenerator.GenerateHtmlStructure(timetable);

        // todo read output path from config
        File.WriteAllText(file, htmlStructure);
    }
}