using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daltonmonitor.Mappers;

namespace Daltonmonitor.Application.Config;

public class ConfigManager
{
    private const string ConfigPath = "./config1.ini";
    private List<ConfigEntryData> ConfigEntryDatas { get; } = [];

    public ConfigManager()
    {
        Setup();
        InitializeConfigData();
    }

    public string[] GetConfigListValue(ConfigIdentifier configIdentifier)
    {
        return configIdentifier == ConfigIdentifier.None ? [] : 
            ConfigEntryDatas.FirstOrDefault(ced => ced.Identifier == configIdentifier)!.CurrentValue.ToArray();
    }
    
    public string GetConfigValue(ConfigIdentifier configIdentifier)
    {
        if (configIdentifier == ConfigIdentifier.None) return string.Empty;

        ConfigEntryData configEntryData = ConfigEntryDatas.FirstOrDefault(ced => ced.Identifier == configIdentifier)!;
        return configEntryData.CurrentValue.Count > 0 ? configEntryData.CurrentValue[0] : string.Empty;
    }

    private void InitializeConfigData()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                Dictionary<ConfigIdentifier, List<string>> existingConfigDatas = GetDataFromConfigFile();
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

    private void UpdateConfig(Dictionary<ConfigIdentifier, List<string>> configDatas)
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
                
                bool exists = configDatas.TryGetValue(configEntryData.Identifier, out List<string>? existingValues);
                WriteConfigValue(configEntryData, ref stringBuilder, exists ? existingValues! : configEntryData.CurrentValue);
            }
            stringBuilder.Append('\n');
        }
        
        File.WriteAllText(ConfigPath, stringBuilder.ToString());
    }

    private void WriteConfigValue(ConfigEntryData configEntryData, ref StringBuilder stringBuilder, List<string> existingValues)
    {
        switch (configEntryData.ConfigType)
        {
            case ConfigType.SingleValue:
                stringBuilder.Append($"{configEntryData.Identifier.ToString()}={existingValues[0]}\n");
                break;
            case ConfigType.InlineListValue:
                stringBuilder.Append($"{configEntryData.Identifier.ToString()}=");
                for (int i = 0; i < existingValues.Count; i++)
                {
                    stringBuilder.Append($"\"{existingValues[i]}\"");
                    if (existingValues.Count - 1 > i)
                    {
                        stringBuilder.Append(',');
                    }
                }
                stringBuilder.Append('\n');
                break;
            case ConfigType.ComplexListValue:
                foreach (string value in existingValues)
                {
                    stringBuilder.Append($"+{configEntryData.Identifier.ToString()}={value}\n");
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Dictionary<ConfigIdentifier, List<string>> GetDataFromConfigFile()
    {
        Dictionary<ConfigIdentifier, List<string>> configDatas = [];
        configDatas.EnsureCapacity(ConfigEntryDatas.Count);
        
        string[] configDataLines = File.ReadAllLines(ConfigPath);
        foreach (string configData in configDataLines)
        {
            if (configData.StartsWith(';') || configData.StartsWith('[') || configData.IsWhiteSpace())
            {
                continue;
            }

            bool startsWithPlus = configData.StartsWith('+');
            
            string identifier = configData.Substring(startsWithPlus ? 1 : 0, configData.IndexOf('=') - (startsWithPlus ? 1 : 0));
            bool parsedSuccessfully = Enum.TryParse(identifier, false, out ConfigIdentifier configIdentifier);
            if (!parsedSuccessfully)
            {
                continue;
            }
            
            ConfigEntryData configEntryData = ConfigEntryDatas.FirstOrDefault(ced => ced.Identifier == configIdentifier)!;
            string value = configData[(configData.IndexOf('=') + 1)..];

            switch (configEntryData.ConfigType)
            {
                case ConfigType.SingleValue when value.StartsWith('"') && value.EndsWith('"'):
                    configDatas.Add(configIdentifier, [value.Substring(1, value.Length - 2)]);
                    continue;
                case ConfigType.SingleValue:
                    configDatas.Add(configIdentifier, [value]);
                    break;
                case ConfigType.ComplexListValue when startsWithPlus:
                {
                    bool found = configDatas.TryGetValue(configIdentifier, out List<string>? configValues);
                    if (found)
                    {
                        configValues?.Add(value);
                    }
                    else
                    {
                        configDatas.Add(configIdentifier, [value]);
                    }

                    break;
                }
                case ConfigType.InlineListValue:
                    configDatas.Add(configIdentifier, value.CsvSplit(',').ToList());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return configDatas;
    }

    public void ReadConfigFile()
    {
        Dictionary<ConfigIdentifier, List<string>> configDatas = GetDataFromConfigFile();
        foreach (KeyValuePair<ConfigIdentifier, List<string>> configData in configDatas)
        {
            ConfigEntryData? configEntryData = ConfigEntryDatas.Find(ced => ced.Identifier == configData.Key);
            configEntryData?.UpdateCurrentValue(configData.Value);
        }
    }

    private void Setup()
    {
        /* Paths */
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigIdentifier.GPU001, ""));
        ConfigEntryDatas.Add(new ConfigEntryData("Paths", ConfigIdentifier.GPU002, ""));
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
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.GpuSplitCharacter, ",", 
            "How are the gpu files split, e. g. using a ','; might also be for example a ';'. But only *one* character"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.DaltonIdentifiers, ["DAL"]));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.WorkshopIdentifiers, ["DAL+"]));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.MentorIdentifiers,
            ["M5A","M5B","M5C","M5D","M5E", "M6A","M6B","M6C","M6D","M6E","M7A","M7B","M7C","M7D","M7E",
                "M8A","M8B","M8C","M8D","M8E","M9A","M9B","M9C","M9D","M9E","M10A","M10B","M10C","M10D","M10E","MEF","MQ1","MQ2"]));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.MentorPattern, "M{Class}",
            "The pattern to extract the class to show from the mentor identifiers, e. g. M{Class} when the mentor identifier is like M5A"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.BoundDaltonLessonPattern, "geb. {Information}",
            "The pattern to extract bound dalton lesson information to show on the website. {Information} will be data shown in the app. Must be in the row 'Text for processing'"));
        ConfigEntryDatas.Add(new ConfigEntryData("Data", ConfigIdentifier.FloorCount, "4", 
            "The amount of floors that should be baked into the html"));
        
        /* Variants */
        ConfigEntryDatas.Add(new ConfigEntryData("Variants", ConfigIdentifier.Variants, ["A-Wo", "B-Wo"], 
            ConfigType.InlineListValue, "Defines all the variants, have to be in the order they should be used in 'VariantOverride'"));
        ConfigEntryDatas.Add(new ConfigEntryData("Variants", ConfigIdentifier.VariantOverride, 
            ["Override$yyyyMMdd$\"A-Wo\""], ConfigType.ComplexListValue,
            "THE DATES MUST BE IN ORDER! This should be used as a list of overrides for A/B/... weeks. Using this, you can override your default a/b scheme. " +
            "Normally, the A/B week just counts up normally. Using this list, the behaviour can be overriden. See the example to learn the syntax rules."));
        
        /* Floors */
        ConfigEntryDatas.Add(new ConfigEntryData("Floors", ConfigIdentifier.DefaultFloor, "1",
            "All lessons, that cannot be assigned to a specific floor, are assigned to this floor"));
        ConfigEntryDatas.Add(new ConfigEntryData("Floors", ConfigIdentifier.FloorOverride,
            ["\"\\b1\\d+\\b\"$1"], ConfigType.ComplexListValue, "Before the first $: regex pattern that is used as a rule, if the regex matches the room identifier, the floor after the $ is taken (must be a number)"));
        
        /* Html */
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.ShowWorkshops, "true"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.ShowVacationDays, "false",
            "If show vacation days is set to true, vacation days are not skipped but highlighted in a special way. Weekends are always skipped"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.HighlightMentorDalton, "true", 
            "If set to true, dalton lessons with mentors are highlight in a special way"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.PreviewingDaysCount, "5",
            "The amount of future days that should be baked into the html"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.MaxFutureDaysAfterOffDay, "3",
            "This config is to prevent that after long summer vacations (for example) there are still shown school days. After x free days no other day is shown anymore. Zero to ignore"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.MaxShowingDaysCount, "2",
            "The amount of days that should be shown at max on in an environment which supports JavaScript"));
        
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.SpecialLabel, "SP", 
            "Will only be used if the dalton lesson doesn't fit any of the predefined tags. If empty the label isn't shown"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.WorkshopLabel, "WS", 
            "The string that will be baked into a tag if the dalton lesson is a workshop. If empty the label isn't shown"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.BoundDaltonLabel, "GEB", 
            "The string that will be baked into a tag if the dalton lesson is a bound dalton lesson. If empty the label isn't shown"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.MentorLabel, "", 
            "The string that will be baked into a tag if the dalton lesson is a dalton lesson with the mentor. If empty the label isn't shown"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.EnableInlineTags, "true",
            "If this option is enabled, tags are generated into the same column as the lessons themselves. If 'true', tags get an extra column"));
        
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.ReorderWorkshopsWithLabelToBottom, "true",
            "If this is set to true, dalton lessons (with a label in front of it) that are workshops are always displayed at the bottom"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.ReorderMentorDaltonWithLabelToBottom, "",
            "If this is set to true, dalton lessons (with a label in front of it) that are mentor dalton lessons are always displayed at the bottom"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.ReorderBoundDaltonWithLabelToBottom, "",
            "If this is set to true, dalton lessons (with a label in front of it) that are bound dalton lessons are always displayed at the bottom"));
        ConfigEntryDatas.Add(new ConfigEntryData("Html", ConfigIdentifier.ExtraColumnForWorkshops, "true",
            "If set to 'true', workshops won't get shown in the normal column, but rather in an extra column just for workshops"));
        
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
    }
}