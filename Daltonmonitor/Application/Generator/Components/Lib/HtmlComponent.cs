using System.Collections.Generic;

namespace Daltonmonitor.Application.Generator.Components.Lib;

public abstract class HtmlComponent
{
    private HtmlComponent? _parent;
    
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
        htmlComponent._parent = this;
        Children.Add(htmlComponent);
        
        htmlComponent.Initialize();
        
        return idx;
    }

    protected T? GetOuter<T>() where T : HtmlComponent
    {
        HtmlComponent? currentOuter = _parent;
        while (currentOuter is not null)
        {
            if (currentOuter is T castedOuter)
            {
                return castedOuter;
            }
            
            currentOuter = currentOuter._parent;
        }
        return null;
    }

    protected void SetAsRootComponent()
    {
        Initialize();
    }
}