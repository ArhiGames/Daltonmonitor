using System.IO;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator;
using Daltonmonitor.Models.Timetable;
using Daltonmonitor.Models.Types.Common.Result;

namespace Daltonmonitor.Application;

public class Daltonmonitor
{
    private ConfigManager ConfigManager { get; } = new();
    private SubstitutionReader SubstitutionReader { get; }
    private HtmlGenerator HtmlGenerator { get; }

    public Daltonmonitor()
    {
        SubstitutionReader = new SubstitutionReader(ConfigManager);
        HtmlGenerator = new HtmlGenerator(ConfigManager);
    }

    public void Process()
    {
        Result<Timetable> timetableResult = SubstitutionReader.Process();
        if (!timetableResult.IsSuccess)
        {
            return;
        }
        
        Timetable timetable = timetableResult.Value!;
        string htmlStructure = HtmlGenerator.GenerateHtmlStructure(timetable);

        string outputFile = ConfigManager.GetConfigValue(ConfigType.OutputPath);
        File.WriteAllText(outputFile, htmlStructure);
    }
}