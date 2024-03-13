using System;

namespace ArchCore.Pooling
{
    [Obsolete]
    public interface IPrototype : ICloneable
    {
        void ResetToProto();
        void SetProto(IPrototype prototype);
    }
}