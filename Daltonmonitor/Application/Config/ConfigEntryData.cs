using System.Collections.Generic;

namespace Daltonmonitor.Application.Config;

public enum ConfigIdentifier
{
    None = 0,
    GPU001,
    GPU002,
    GPU014,
    GPU018,
    OutputPath,
    ScriptSourcePath,
    StyleSourcePath,
    LogoSvgSourcePath,
    
    CheckInterval,
    UserMode,
    
    ApplicationName,
    GpuSplitCharacter,
    DaltonIdentifiers,
    WorkshopIdentifiers,
    MentorIdentifiers,
    MentorPattern,
    BoundDaltonLessonPattern,
    FloorCount,
    
    Variants,
    VariantOverride,
    
    ShowWorkshops,
    ShowVacationDays,
    HighlightMentorDalton,
    PreviewingDaysCount,
    SpecialLabel,
    WorkshopLabel,
    BoundDaltonLabel,
    MentorLabel,
    EnableInlineTags,
    ReorderWorkshopsWithLabelToBottom,
    ReorderMentorDaltonWithLabelToBottom,
    ReorderBoundDaltonWithLabelToBottom,
    
    BackgroundColor,
    ElementColor,
    BorderColor,
    TagColor,
    DateColor,
    BorderColorDate,
    OffDayColor,
    BorderColorOffDay,
    TextColor,
    TextColorSubstitution,
    LessonTextSize,
    BorderSize
}

public enum ConfigType
{
    SingleValue,
    InlineListValue,
    ComplexListValue
}

public class ConfigEntryData
{
    public string Category { get; init; }

    public ConfigIdentifier Identifier { get; init; }

    public ConfigType ConfigType { get; init; } = ConfigType.SingleValue;

    public string? Comment { get; init; }

    private List<string> DefaultValue { get; init; } = [string.Empty];

    public List<string> CurrentValue { get; private set; } = [string.Empty];

    public ConfigEntryData(string category, ConfigIdentifier identifier, string defaultValue, string? comment = null)
    {
        Category = category;
        Identifier = identifier; 
        DefaultValue[0] = defaultValue;
        CurrentValue[0] = defaultValue;
        Comment = comment;
    }

    public ConfigEntryData(string category, ConfigIdentifier identifier, List<string> defaultValue, 
        ConfigType defaultConfigListType = ConfigType.InlineListValue, string? comment = null)
    {
        Category = category;
        ConfigType = defaultConfigListType;
        Identifier = identifier;
        DefaultValue = defaultValue;
        CurrentValue = defaultValue;
        Comment = comment;
    }
    
    public void UpdateCurrentValue(List<string> currentValue)
    {
        CurrentValue = currentValue;
    }

    public void ResetToDefault()
    {
        CurrentValue = DefaultValue;
    }
}