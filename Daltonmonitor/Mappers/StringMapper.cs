using System.Collections.Generic;
using System.Text;

namespace Daltonmonitor.Mappers;

public static class StringMapper
{
    public static string[] EnhancedSplit(this string value, char separator, bool ignoreWhitespace = true)
    {
        List<string> splits = [];

        StringBuilder stringBuilder = new();
        foreach (char character in value)
        {
            if (ignoreWhitespace && char.IsWhiteSpace(character))
            {
                continue;
            }
            
            if (character == separator)
            {
                splits.Add(stringBuilder.ToString());
                stringBuilder.Clear();
                continue;
            }
            stringBuilder.Append(character);
        }
        splits.Add(stringBuilder.ToString());

        return splits.ToArray();
    }
}