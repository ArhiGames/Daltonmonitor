using System.Collections.Generic;
using System.Text;

namespace Daltonmonitor.Mappers;

public static class StringMapper
{
    public static string[] CsvSplit(this string value, char separator)
    {
        List<string> splits = [];

        StringBuilder stringBuilder = new();

        bool isStartingOfWord = true;
        bool isWordEndedCorrectly = true;
        
        foreach (char character in value)
        {
            if (character == '"')
            {
                isWordEndedCorrectly = !isStartingOfWord;
                continue;
            }

            isStartingOfWord = false;
            
            if (character == separator && isWordEndedCorrectly)
            {
                splits.Add(stringBuilder.ToString());
                stringBuilder.Clear();
                
                isStartingOfWord = true;
                isWordEndedCorrectly = true;
                
                continue;
            }
            stringBuilder.Append(character);
        }
        splits.Add(stringBuilder.ToString());

        return splits.ToArray();
    }
}