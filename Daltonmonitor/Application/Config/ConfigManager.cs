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
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", "GPU001", ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", "GPU014", ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", "GPU018", ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", "OutputPath", "./output.html"));
        
        /* Runtime */
        ConfigEntryDatas.Add(new ConfigEntryData("Runtime", "CheckInterval", "30", "check interval in seconds"));
        
        /* Data */
        ConfigEntryDatas.Add(new ConfigEntryData("Data", "ApplicationName", "Daltonmonitor"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", "DaltonIdentifiers", "DAL"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", "WorkshopIdentifiers", "DAL+"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", "MentorIdentifiers", ""));
        
        /* Html */
        ConfigEntryDatas.Add(new ConfigEntryData("Html", "ShowWorkshops", "true"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", "ShowVacationDays", "true"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", "PreviewingDays", "2", "the amount of future days that should be baked into the html"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", "ShowLabels", "true"));
        
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
                stringBuilder.Append($"{configEntryData.Identifier}={configEntryData.CurrentValue}\n");
            }
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
            string value = configData[(configData.IndexOf('=') + 1)..];
            
            ConfigEntryData? configEntryData = ConfigEntryDatas.Find(ced => ced.Identifier == identifier);
            configEntryData?.UpdateCurrentValue(value);
        }
    }
}