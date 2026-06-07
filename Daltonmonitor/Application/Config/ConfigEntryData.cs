namespace Daltonmonitor.Application.Config;

public enum ConfigType
{
    None = 0,
    GPU001,
    GPU014,
    GPU018,
    OutputPath,
    CheckInterval,
    ApplicationName,
    DaltonIdentifiers,
    WorkshopIdentifiers,
    MentorIdentifiers,
    ShowWorkshops,
    ShowVacationDays,
    ShowPreviewingDays,
    ShowLabels,
    StyleSourcePath
}

public class ConfigEntryData(string category, ConfigType identifier, string defaultValue, string? comment = null)
{
    public string Category { get; init; } = category;

    public ConfigType Identifier { get; init; } = identifier;

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
}