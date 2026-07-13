using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Mappers;

namespace Daltonmonitor.Application.Managers.Floors;

public class FloorsManager : Manager
{
    private List<FloorsData> FloorsDatas { get; } = [];
    
    public FloorsManager(ConfigManager configManager)
        : base(configManager)
    {
        Initialize();
    }
    
    private void Initialize()
    {
        string[] floorOverrides = ConfigManager.GetConfigListValue(ConfigIdentifier.FloorOverride);
        
        foreach (string floorOverride in floorOverrides)
        {
            string[] splitParameters = floorOverride.CsvSplit('$');

            Regex regex = new(splitParameters[0]);
            int floor = Convert.ToInt32(splitParameters[1]);

            FloorsData floorsData = new(regex, floor);
            FloorsDatas.Add(floorsData);
        }
    }

    public int GetFloorByIdentifier(string identifier)
    {
        foreach (FloorsData floorsData in FloorsDatas)
        {
            if (floorsData.RuleRegex.IsMatch(identifier))
            {
                return floorsData.Floor;
            }
        }
        return Convert.ToInt32(ConfigManager.GetConfigValue(ConfigIdentifier.DefaultFloor));
    }
}