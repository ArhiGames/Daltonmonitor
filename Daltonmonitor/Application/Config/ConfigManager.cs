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
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigIdentifier.GPU001, ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigIdentifier.GPU014, ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigIdentifier.GPU018, ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigIdentifier.OutputPath, "./output.html"));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigIdentifier.ScriptSourcePath, "./script.js",
            "The path to the javascript file that will be baked into the generated html"));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigIdentifier.StyleSourcePath, "./style.css", 
            "The path to the css file that will be baked into the generated html"));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigIdentifier.LogoSvgSourcePath, "./logo.svg",
            "The path to the svg logo file that should be displayed in the top left corner of the html website. Must be an svg"));
        
        /* Runtime */
        ConfigEntryDatas.Add(new ConfigEntryData("Runtime", ConfigIdentifier.CheckInterval, "30", 
            "check interval in seconds"));
        ConfigEntryDatas.Add(new ConfigEntryData("Runtime", ConfigIdentifier.UserMode, "KEEP", 
            "DEL: deletes GPU files after usage. KEEP: keeps GPU files after usage"));
        
        /* Data */
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.ApplicationName, "Daltonmonitor"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.DaltonIdentifiers, "DAL"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.WorkshopIdentifiers, "DAL+"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.MentorIdentifiers,
            "M5A,M5B,M5C,M5D,M6A,M6B,M6C,M6D,M7A,M7B,M7C,M7D,M8A,M8B,M8C,M8D,M9A,M9B,M9C,M9D,M10A,M10B,M10C,M10D,MEF,MQ1,MQ2"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.MentorPattern, "M{Class}",
            "The pattern to extract the class to show from the mentor identifiers, e. g. M{Class} when the mentor identifier is like M5A"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.BoundDaltonLessonPattern, "geb. {Information}",
            "The pattern to extract bound dalton lesson information to show on the website. {Information} will be data shown in the app. Must be in the row 'Text for processing'"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.FloorCount, "4", 
            "The amount of floors that should be baked into the html"));
        
        /* Html */
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.ShowWorkshops, "true"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.ShowVacationDays, "true",
            "If show vacation days is set to true, vacation days are not skipped but highlighted in a special way. Weekends are always skipped"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.HighlightMentorDalton, "true", 
            "If set to true, dalton lessons with mentors are highlight in a special way"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.PreviewingDaysCount, "5", 
            "the amount of future days that should be baked into the html"));
        
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.SpecialLabel, "SP", 
            "Will only be used if the dalton lesson doesn't fit any of the predefined tags. If empty the label isn't shown"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.WorkshopLabel, "WS", 
            "The string that will be baked into a tag if the dalton lesson is a workshop. If empty the label isn't shown"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.BoundDaltonLabel, "GEB", 
            "The string that will be baked into a tag if the dalton lesson is a bound dalton lesson. If empty the label isn't shown"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.MentorLabel, "", 
            "The string that will be baked into a tag if the dalton lesson is a dalton lesson with the mentor. If empty the label isn't shown"));
        
        /* Style */
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.BackgroundColor, "#00bbff"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.ElementColor, "#90caf9"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.BorderColor, "#bbdefb"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.TagColor, "#bbdefb"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.DateColor, "#f48c06"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.BorderColorDate, "#ffba08"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.OffDayColor, "#70e000"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.BorderColorOffDay, "#9ef01a"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.TextColor, "#0d0d0d"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.TextColorSubstitution, "#c1121f"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.LessonTextSize, "0.9rem"));
        ConfigEntryDatas.Add(new ConfigEntryData("Style", ConfigIdentifier.BorderSize, "0.1rem"));
        
        InitializeConfigData();
    }

    public string[] GetConfigAsList(ConfigIdentifier configIdentifier)
    {
        return configIdentifier == ConfigIdentifier.None ? [] : ConfigEntryDatas.FirstOrDefault(ced => ced.Identifier == configIdentifier)!.AsList();
    }
    
    public string GetConfigValue(ConfigIdentifier configIdentifier)
    {
        return configIdentifier == ConfigIdentifier.None ? "" : ConfigEntryDatas.FirstOrDefault(ced => ced.Identifier == configIdentifier)!.CurrentValue;
    }

    private void InitializeConfigData()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                Dictionary<ConfigIdentifier, string> existingConfigDatas = GetDataFromConfigFile();
                UpdateConfig(existingConfigDatas);

                ReadConfigFile();
            }
            else
            {
                UpdateConfig([]);
            }
        }
        catch
        {
            // ignored
        }
    }

    private void UpdateConfig(Dictionary<ConfigIdentifier, string> configDatas)
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

                bool exists = configDatas.TryGetValue(configEntryData.Identifier, out string? existingValue);
                if (exists)
                {
                    stringBuilder.Append($"{configEntryData.Identifier.ToString()}={existingValue}\n");
                }
                else
                {
                    stringBuilder.Append($"{configEntryData.Identifier.ToString()}={configEntryData.CurrentValue}\n");
                }
            }
            stringBuilder.Append('\n');
        }
        
        File.WriteAllText(ConfigPath, stringBuilder.ToString());
    }

    private Dictionary<ConfigIdentifier, string> GetDataFromConfigFile()
    {
        Dictionary<ConfigIdentifier, string> configDatas = [];
        configDatas.EnsureCapacity(ConfigEntryDatas.Count);
        
        string[] configDataLines = File.ReadAllLines(ConfigPath);
        foreach (string configData in configDataLines)
        {
            if (configData.StartsWith(';') || configData.StartsWith('[') || configData.IsWhiteSpace())
            {
                continue;
            }

            string identifier = configData[..configData.IndexOf('=')];
            bool parsedSuccessfully = Enum.TryParse(identifier, false, out ConfigIdentifier configType);
            if (!parsedSuccessfully)
            {
                continue;
            }

            string value = configData[(configData.IndexOf('=') + 1)..];
            if (value.StartsWith('"') && value.EndsWith('"'))
            {
                configDatas.Add(configType, value.Substring(1, value.Length - 2));
                continue;
            }
            configDatas.Add(configType, value);
        }

        return configDatas;
    }

    public void ReadConfigFile()
    {
        Dictionary<ConfigIdentifier, string> configDatas = GetDataFromConfigFile();
        foreach (KeyValuePair<ConfigIdentifier, string> configData in configDatas)
        {
            ConfigEntryData? configEntryData = ConfigEntryDatas.Find(ced => ced.Identifier == configData.Key);
            configEntryData?.UpdateCurrentValue(configData.Value);
        }
    }
}