using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Mappers;

namespace Daltonmonitor.Application.Managers.Variants;

public class VariantsManager : Manager
{
    private List<VariantsData> VariantsDatas { get; set; } = [];
    private string[] VariantIdentifiers { get; set; } = [];

    public VariantsManager(ConfigManager configManager)
        : base(configManager)
    {
        Initialize();
    }

    private void Initialize()
    {
        VariantIdentifiers = ConfigManager.GetConfigListValue(ConfigIdentifier.Variants);
        string[] variantRules = ConfigManager.GetConfigListValue(ConfigIdentifier.VariantOverride);

        try
        {
            foreach (string variantRule in variantRules)
            {
                string[] overrideParts = variantRule.CsvSplit('$');
                bool parsedSuccessfully = Enum.TryParse(overrideParts[0], out VariantType variantType);
                if (!parsedSuccessfully)
                {
                    continue;
                }

                DateTime startDateTime =
                    DateTime.ParseExact(overrideParts[1], "yyyyMMdd", CultureInfo.InvariantCulture);
                string identifier = overrideParts[2];

                VariantsData variantsData = new(variantType, startDateTime, identifier);
                VariantsDatas.Add(variantsData);
            }
        }
        catch
        {
            VariantIdentifiers = [];
            VariantsDatas.Clear();
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
        if (latestVariantIdentifierIdx == -1)
        {
            return string.Empty;
        }

        TimeSpan timeSpan = dateTime.Subtract(latestVariantData.StartingDate);
        int totalWeeks = (int)timeSpan.TotalDays * 7;
        return VariantIdentifiers[(latestVariantIdentifierIdx + totalWeeks) % VariantIdentifiers.Length];
    }
}