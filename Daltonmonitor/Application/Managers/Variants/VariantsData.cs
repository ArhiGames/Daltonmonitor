using System;

namespace Daltonmonitor.Application.Managers.Variants;

public enum VariantType
{
    Override
}

public record VariantsData(VariantType VariantType, DateTime StartingDate, string VariantIdentifier);