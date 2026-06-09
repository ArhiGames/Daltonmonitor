using System;
using Daltonmonitor.Application.Generator.Components.Lib;

namespace Daltonmonitor.Application.Generator.Components.User;

public enum MetadataType
{
    NotSpecified = 0,
    Mentor
}

public class MetaLessonInfoHtmlComponent(MetadataType metadataType, string metadata) : HtmlComponent
{
    protected override void Initialize() { }

    public override string GenerateHtml()
    {
        string metadataString = metadataType switch
        {
            MetadataType.NotSpecified => metadata,
            MetadataType.Mentor => $"({metadata})",
            _ => throw new ArgumentOutOfRangeException(nameof(metadataType), metadataType, null)
        };
        string html = $"<div class=\"meta-data\">{metadataString}</div>";

        return html;
    }
}