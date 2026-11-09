using System.Collections.Generic;

namespace PreLuna.ModControl.Features
{
    internal static class ReadOnlyCfgLine
    {
        internal static string At(IReadOnlyList<string> lines, int index)
            => lines != null && index >= 0 && index < lines.Count ? lines[index] : string.Empty;
    }
}
