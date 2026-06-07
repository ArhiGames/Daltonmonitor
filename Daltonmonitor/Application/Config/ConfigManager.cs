using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daltonmonitor.Application.Config;

public class ConfigManager
{
    private const string ConfigPath = "./config1.ini"; 
    public List<ConfigEntryData> ConfigEntryDatas { get; } = [];

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
        
        /* Html */
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.ShowWorkshops, "true"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.ShowVacationDays, "true"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.ShowPreviewingDays, "2", "the amount of future days that should be baked into the html"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigType.ShowLabels, "true"));
        
        InitializeConfigData();
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

    private void ReadConfigFile()
    {
        string[] configDataLines = File.ReadAllLines(ConfigPath);

        foreach (string configData in configDataLines)
        {
            if (configData.StartsWith(';') || configData.StartsWith('['))
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