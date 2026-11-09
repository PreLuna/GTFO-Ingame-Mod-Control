using System;
using System.Collections.Generic;
using PreLuna.ModControl.Core.Operations;

namespace PreLuna.ModControl.Adapters
{
    internal enum ReadOnlyCfgFormat
    {
        Text,
        Boolean,
        Number,
        Integer
    }

    internal sealed class ReadOnlyCfgField
    {
        internal ReadOnlyCfgField(string section, string key, string label, ReadOnlyCfgFormat format)
        {
            Section = section ?? string.Empty;
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Label = label ?? key;
            Format = format;
        }

        internal string Section { get; }
        internal string Key { get; }
        internal string Label { get; }
        internal ReadOnlyCfgFormat Format { get; }
    }

    internal sealed class ReadOnlyCfgTarget
    {
        internal ReadOnlyCfgTarget(
            string id,
            string displayName,
            string version,
            string fileName,
            string permissionReason,
            IReadOnlyList<ReadOnlyCfgField> fields)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = displayName ?? id;
            Version = version ?? "frozen";
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            PermissionReason = permissionReason ?? "冻结模块只读：未提供运行控制接口。";
            Fields = fields ?? Array.Empty<ReadOnlyCfgField>();
        }

        internal string Id { get; }
        internal string DisplayName { get; }
        internal string Version { get; }
        internal string FileName { get; }
        internal string PermissionReason { get; }
        internal IReadOnlyList<ReadOnlyCfgField> Fields { get; }
    }

    internal sealed class ReadOnlyCfgSnapshot
    {
        internal ReadOnlyCfgSnapshot(TargetDescriptor descriptor, string revision, IReadOnlyList<string> values)
        {
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            Revision = revision ?? string.Empty;
            Values = values ?? Array.Empty<string>();
        }

        internal TargetDescriptor Descriptor { get; }
        internal string Revision { get; }
        internal IReadOnlyList<string> Values { get; }
    }
}
