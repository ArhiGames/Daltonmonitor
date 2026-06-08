using System;
using Daltonmonitor.Application.Config;
using Daltonmonitor.Application.Generator.Components.Lib;

namespace Daltonmonitor.Application.Generator.Components.User;

public enum LabelType
{
    Special = 0,
    Workshop,
    Bound,
    Mentor,
}

public class LabelHtmlComponent(LabelType labelType) : HtmlComponent
{
    protected override void Initialize() { }

    public override string GenerateHtml()
    {
        HtmlRootComponent htmlRootComponent = GetOuter<HtmlRootComponent>()!;
        ConfigType configType = labelType switch
        {
            LabelType.Special => ConfigType.SpecialLabel,
            LabelType.Workshop => ConfigType.WorkshopLabel,
            LabelType.Bound => ConfigType.BoundDaltonLabel,
            LabelType.Mentor => ConfigType.MentorLabel,
            _ => throw new ArgumentOutOfRangeException(nameof(labelType), labelType, null)
        };
        string labelTypeString = labelType.ToString();
        string labelString = htmlRootComponent.ConfigManager.GetConfigValue(configType);

        string html = $"<div class=\"tag {labelTypeString}\">{labelString}</div>";
        return html;
    }
}