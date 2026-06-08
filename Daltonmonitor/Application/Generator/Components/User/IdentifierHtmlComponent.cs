using System;
using Daltonmonitor.Application.Generator.Components.Lib;

namespace Daltonmonitor.Application.Generator.Components.User;

public enum IdentifierType
{
    None = 0,
    Room,
    Teacher
}

public class IdentifierHtmlComponent : HtmlComponent
{
    private readonly IdentifierType _identifierType;
    private readonly string _identifier;
    private readonly bool _isSubstitute;
    
    public IdentifierHtmlComponent(IdentifierType identifierType, string identifier, bool isSubstitute)
    {
        MaxChildrenCount = 0;
        _identifierType = identifierType;
        _identifier = identifier;
        _isSubstitute = isSubstitute;
    }
    
    protected override void Initialize() {}
    
    public override string GenerateHtml()
    {
        string htmlClass = _identifierType switch
        {
            IdentifierType.None => "",
            IdentifierType.Room => _isSubstitute ? "relocated-room-number" : "room-number",
            IdentifierType.Teacher => _isSubstitute ? "substitute-shorthand" : "teacher-shorthand",
            _ => throw new ArgumentOutOfRangeException()
        };

        string htmlElement = $"<div class=\"{htmlClass}\">{_identifier}</div>";
        return htmlElement;
    }
}