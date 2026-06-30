using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Mappers;

namespace Daltonmonitor.Application.Managers.Variants;

public class VariantsManager(ConfigManager configManager)
{
    private ConfigManager ConfigManager { get; init; } = configManager;

    private List<VariantsData> VariantsDatas { get; set; } = [];
    private string[] VariantIdentifiers { get; set; } = [];

    public void Initialize()
    {
        VariantIdentifiers = ConfigManager.GetConfigListValue(ConfigIdentifier.Variants);
        string[] variantRules = ConfigManager.GetConfigListValue(ConfigIdentifier.VariantOverride);

        foreach (string variantRule in variantRules)
        {
            string[] overrideParts = variantRule.CsvSplit('$');
            bool parsedSuccessfully = Enum.TryParse(overrideParts[0], out VariantType variantType);
            if (!parsedSuccessfully)
            {
                continue;
            }

            DateTime startDateTime = DateTime.ParseExact(overrideParts[1], "yyyyMMdd", CultureInfo.InvariantCulture);
            string identifier = overrideParts[2];

            VariantsData variantsData = new(variantType, startDateTime, identifier);
            VariantsDatas.Add(variantsData);
        }
    }

    public string GetVariantsWeekIdentifier(DateTime dateTime)
    {
        VariantsData? latestVariantData = VariantsDatas.LastOrDefault(variantsData => variantsData.StartingDate.CompareTo(dateTime) <= 0);

        if (latestVariantData is null)
        {
            return string.Empty;
        }

        int latestVariantIdentifierIdx = VariantIdentifiers.IndexOf(latestVariantData.VariantIdentifier); 

        TimeSpan timeSpan = dateTime.Subtract(latestVariantData.StartingDate);
        int totalWeeks = (int)timeSpan.TotalDays * 7;
        return VariantIdentifiers[(latestVariantIdentifierIdx + totalWeeks) % VariantIdentifiers.Length];
    }
}