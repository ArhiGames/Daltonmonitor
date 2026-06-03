namespace Daltonmonitor.Models.Types;

public class Class(string classDescriptor)
{
    /** For example, 5A, 5B, EF, Q1 */
    public string ClassDescriptor { get; init; } = classDescriptor;
}