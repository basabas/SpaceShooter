using System;

namespace Bas.Serialization
{
    [Flags]
    public enum SerializableTypeOptions
    {
        None = 0,
        DrawName = 1,
        IncludePrimitives = 2
    }
}