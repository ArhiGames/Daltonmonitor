using System.Collections.Generic;

namespace Daltonmonitor.Application.Generator.Components.Lib;

public abstract class HtmlComponent
{
    protected HtmlComponent? Parent;
    
    protected List<HtmlComponent> Children { get; } = [];
    protected int MaxChildrenCount = -1;
    
    protected abstract void Initialize();
    public abstract string GenerateHtml();

    protected int AddChildrenToComponent(HtmlComponent htmlComponent)
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
    
    public HtmlComponent? GetChildrenAtIndex(int idx)
    {
        return Children.Count > idx ? Children[idx] : null;
    }

    public bool RemoveChildrenAtIndex(int idx)
    {
        if (Children.Count <= idx)
        {
            return false;
        }

        Children.RemoveAt(idx);
        return true;
    }

    public void ClearChildren()
    {
        Children.Clear();
    }
}