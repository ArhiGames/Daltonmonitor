namespace Daltonmonitor.Application.Config;

public enum ConfigIdentifier
{
    None = 0,
    GPU001,
    GPU014,
    GPU018,
    OutputPath,
    
    CheckInterval,
    UserMode,
    
    ApplicationName,
    DaltonIdentifiers,
    WorkshopIdentifiers,
    MentorIdentifiers,
    MentorPattern,
    FloorCount,
    
    ShowWorkshops,
    ShowVacationDays,
    HighlightMentorDalton,
    PreviewingDaysCount,
    SpecialLabel,
    WorkshopLabel,
    BoundDaltonLabel,
    MentorLabel,
    
    StyleSourcePath,
    LogoSvgSourcePath
}

public class ConfigEntryData(string category, ConfigIdentifier identifier, string defaultValue, string? comment = null)
{
    public string Category { get; init; } = category;

    public ConfigIdentifier Identifier { get; init; } = identifier;

    public string? Comment { get; init; } = comment;

    private string DefaultValue { get; init; } = defaultValue;

    public string CurrentValue { get; private set; } = defaultValue;

    public void UpdateCurrentValue(string currentValue)
    {
        CurrentValue = currentValue;
    }

    public void ResetToDefault()
    {
        CurrentValue = DefaultValue;
    }

    public string[] AsList()
    {
        return CurrentValue.Split(',');
    }
}