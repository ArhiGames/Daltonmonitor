using System.Collections.Generic;

namespace Daltonmonitor.Application.Generator.Components.Lib;

public abstract class HtmlComponent
{
    protected List<HtmlComponent> Children { get; } = [];
    protected int MaxChildrenCount = -1;
    
    public abstract string GenerateHtml();

    public int AddChildrenToComponent(HtmlComponent htmlComponent)
    {
        if (MaxChildrenCount >= Children.Count)
        {
            return -1;
        }
        
        int idx = Children.Count;
        Children.Add(htmlComponent);
        return idx;
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