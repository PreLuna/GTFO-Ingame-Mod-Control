using System;

namespace PreLuna.ModControl.Core.Operations
{
    [Flags]
    public enum TargetCapabilities
    {
        None = 0,
        Validate = 1,
        SavePersistent = 2,
        ReadOnly = 4
    }

    public enum TargetStatus
    {
        Available,
        Missing,
        Incompatible,
        Uninitialized,
        Destroyed
    }
}
