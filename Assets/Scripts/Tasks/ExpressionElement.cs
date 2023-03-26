using System;

namespace Mathy.Core.Tasks
{
    [Serializable]
    public class ExpressionElement
    {
        public TaskElementType Type;
        public string Value;
        public bool IsUnknown;

        public ExpressionElement(TaskElementType type, object value, bool isUnknown = false)
        {
            Type = type;
            Value = value.ToString();
            IsUnknown = isUnknown;
        }
    }
}

