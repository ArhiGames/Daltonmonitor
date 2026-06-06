namespace Daltonmonitor.Application.Config;

public class ConfigEntryData(string category, string identifier, string defaultValue, string? comment = null)
{
    public string Category { get; init; } = category;

    public string Identifier { get; init; } = identifier;

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