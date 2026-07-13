using System.Text.RegularExpressions;

namespace Daltonmonitor.Application.Managers.Floors;

public record FloorsData(Regex RuleRegex, int Floor);
