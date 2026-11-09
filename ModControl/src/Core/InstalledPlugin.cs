namespace PreLuna.ModControl.Core
{
    public sealed class InstalledPlugin
    {
        public InstalledPlugin(string guid, string name, string version, string location)
        {
            Guid = guid ?? string.Empty;
            Name = name ?? string.Empty;
            Version = version ?? string.Empty;
            Location = location ?? string.Empty;
        }

        public string Guid { get; }
        public string Name { get; }
        public string Version { get; }
        public string Location { get; }
    }
}
