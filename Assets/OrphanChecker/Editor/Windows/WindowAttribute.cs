using System;

namespace OrphanChecker.Editor.Windows
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class WindowAttribute : Attribute
    {
        public string Label { get; }
        public int Order { get; }
        public bool IsDefault { get; }

        public WindowAttribute(string label, int order, bool isDefault = false)
        {
            Label = label;
            Order = order;
            IsDefault = isDefault;
        }
    }
}
