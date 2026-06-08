using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daltonmonitor.Application.Config;

public class ConfigManager
{
    private const string ConfigPath = "./config1.ini";
    private List<ConfigEntryData> ConfigEntryDatas { get; } = [];

    public ConfigManager()
    {
        /* Paths */
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigType.GPU001, ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigType.GPU014, ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigType.GPU018, ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigType.OutputPath, "./output.html"));
        
        /* Runtime */
        ConfigEntryDatas.Add(new ConfigEntryData("Runtime", ConfigType.CheckInterval, "30", "check interval in seconds"));
        
        /* Data */
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigType.ApplicationName, "Daltonmonitor"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigType.DaltonIdentifiers, "DAL"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigType.WorkshopIdentifiers, "DAL+"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigType.MentorIdentifiers, ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigType.FloorCount, "4", "The amount of floors that should be baked into the html"));
        
        /* Html */
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.ShowWorkshops, "true"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.ShowVacationDays, "true"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.ShowLabels, "true"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.PreviewingDaysCount, "2", "the amount of future days that should be baked into the html"));
        
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.SpecialLabel, "SP", "Will only be used if the dalton lesson doesn't fit any of the predefined tags"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.WorkshopLabel, "WS", "The string that will be baked into a tag if the dalton lesson is a workshop"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.BoundDaltonLabel, "GEB", "The string that will be baked into a tag if the dalton lesson is a bound dalton lesson"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.MentorLabel, "MEN", "The string that will be baked into a tag if the dalton lesson is a dalton lesson with the mentor"));
        
        /* Style */
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigType.StyleSourcePath, "./style.css", "The path to the css file that will be baked into the generated html"));
        
        InitializeConfigData();
    }

    public string[] GetConfigAsList(ConfigType configType)
    {
        return ConfigEntryDatas.FirstOrDefault(ced => ced.Identifier == configType)!.AsList();
    }
    
    public string GetConfigValue(ConfigType configType)
    {
        return ConfigEntryDatas.FirstOrDefault(ced => ced.Identifier == configType)!.CurrentValue;
    }

    private void InitializeConfigData()
    {
        if (File.Exists(ConfigPath))
        {
            ReadConfigFile();
        }
        else
        {
            WriteConfigFile();
        }
    }

    private void WriteConfigFile()
    {
        Dictionary<string, List<ConfigEntryData>> configEntryDatas = ConfigEntryDatas
            .GroupBy(ced => ced.Category)
            .ToDictionary(
                group => group.Key,
                group => group.ToList());

        StringBuilder stringBuilder = new();
        foreach (KeyValuePair<string, List<ConfigEntryData>> categoryConfigEntryDatas in configEntryDatas)
        {
            stringBuilder.Append($"[{categoryConfigEntryDatas.Key}]\n");
            foreach (ConfigEntryData configEntryData in categoryConfigEntryDatas.Value)
            {
                if (configEntryData.Comment is not null)
                {
                    stringBuilder.Append($";{configEntryData.Comment}\n");
                }
                stringBuilder.Append($"{configEntryData.Identifier.ToString()}={configEntryData.CurrentValue}\n");
            }
            stringBuilder.Append('\n');
        }
        
        File.WriteAllText(ConfigPath, stringBuilder.ToString());
    }

    public void ReadConfigFile()
    {
        string[] configDataLines = File.ReadAllLines(ConfigPath);

        foreach (string configData in configDataLines)
        {
            if (configData.StartsWith(';') || configData.StartsWith('[') || configData.IsWhiteSpace())
            {
                continue;
            }

            string identifier = configData[..configData.IndexOf('=')];
            bool parsedSuccessfully = Enum.TryParse(identifier, false, out ConfigType configType);
            if (!parsedSuccessfully)
            {
                continue;
            }
            
            string value = configData[(configData.IndexOf('=') + 1)..];
            
            ConfigEntryData? configEntryData = ConfigEntryDatas.Find(ced => ced.Identifier == configType);
            configEntryData?.UpdateCurrentValue(value);
        }
    }
}