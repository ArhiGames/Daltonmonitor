using System.Collections.Generic;

namespace Daltonmonitor.Application.Generator.Components.Lib;

public abstract class HtmlComponent
{
    protected HtmlComponent? Parent;
    
    protected List<HtmlComponent> Children { get; } = [];
    protected int MaxChildrenCount = -1;
    
    protected abstract void Initialize();
    public abstract string GenerateHtml();

    protected int AddChildToComponent(HtmlComponent htmlComponent)
    {
        if (MaxChildrenCount >= Children.Count)
        {
            return -1;
        }
        
        int idx = Children.Count;
        htmlComponent.Parent = this;
        Children.Add(htmlComponent);
        
        htmlComponent.Initialize();
        
        return idx;
    }

    protected T? GetOuter<T>() where T : HtmlComponent
    {
        HtmlComponent? currentOuter = Parent;
        while (currentOuter is not null)
        {
            if (currentOuter is T castedOuter)
            {
                return castedOuter;
            }
            
            currentOuter = currentOuter.Parent;
        }
        return null;
    }

    protected void SetAsRootComponent()
    {
        Initialize();
    }
}