using System.IO;
using System.Text;
using Daltonmonitor.Application.Generator.Components.Lib;
using Daltonmonitor.Models.Timetable;

namespace Daltonmonitor.Application.Generator.Components.User;

public class HtmlRootComponent : HtmlComponent
{
    private readonly Timetable _timetable;

    public HtmlRootComponent(Timetable timetable)
    {
        _timetable = timetable;
        SetAsRootComponent();
    }
    
    public override string GenerateHtml()
    {
        // todo read application name from config
        const string applicationName = "Daltonmonitor";
        const string htmlHead = $"<!DOCTYPE html><html lang=\"de\"><head><meta charset=\"UTF-8\"><title>{applicationName}</title><link rel=\"icon\" type=\"image/x-icon\" href=\"Icon.png\"></head>";
        string htmlStyle = $"<style>{GetCssString()}</style>";
        const string htmlBack = "</body></html>";

        StringBuilder stringBuilder = new();
        stringBuilder.Append(htmlHead);
        stringBuilder.Append(htmlStyle);
        foreach (HtmlComponent htmlComponent in Children)
        {
            stringBuilder.Append(htmlComponent.GenerateHtml());
        }
        stringBuilder.Append(htmlBack);

        return stringBuilder.ToString();
    }

    protected override void Initialize()
    {
        MainHtmlComponent mainHtmlComponent = new(_timetable);
        AddChildrenToComponent(mainHtmlComponent);
    }
    
    private string GetCssString()
    {
        // todo read path from config
        
        string css = File.ReadAllText(cssPath);
        return css.Replace('\n', ' ');
    }
}