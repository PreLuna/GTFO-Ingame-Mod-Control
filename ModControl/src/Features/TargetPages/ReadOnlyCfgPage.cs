using System;
using System.Collections.Generic;
using PreLuna.ModControl.Adapters;
using PreLuna.ModControl.Core.Operations;
using TheArchive.Core.FeaturesAPI.Components;
using TheArchive.Core.FeaturesAPI.Settings;

namespace PreLuna.ModControl.Features
{
    public interface IReadOnlyCfgSettings
    {
        FLabel Status { get; }
        FLabel Reason { get; }
        void SetLines(IReadOnlyList<string> lines);
    }

    /// <summary>
    /// Reusable read-only page for a frozen target's CFG. The target owns the file; this
    /// page only formats already-selected keys and never exposes an editable control.
    /// </summary>
    internal sealed class ReadOnlyCfgPage<TSettings> : ITargetPage
        where TSettings : class, IReadOnlyCfgSettings
    {
        readonly ReadOnlyCfgTarget target;

        internal ReadOnlyCfgPage(ReadOnlyCfgTarget target)
        {
            this.target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public Type SettingsType => typeof(TSettings);

        public void Refresh(object settings)
        {
            if (settings is not TSettings typed) return;

            var snapshot = ReadOnlyCfgReader.Read(target);
            if (snapshot.Descriptor.Status != TargetStatus.Available)
            {
                ModControlPageSupport.SetLabel(typed.Status, target.DisplayName + "（只读）：未找到 " + target.FileName);
                ModControlPageSupport.SetLabel(typed.Reason, snapshot.Descriptor.PermissionReason);
                typed.SetLines(snapshot.Values);
                return;
            }

            var lines = new List<string>(target.Fields.Count);
            for (var index = 0; index < target.Fields.Count; index++)
                lines.Add(target.Fields[index].Label + "：" + (index < snapshot.Values.Count ? snapshot.Values[index] : "未知"));

            ModControlPageSupport.SetLabel(
                typed.Status,
                target.DisplayName + "（只读）/ 冻结模块 / " + ModControlText.TargetStatusText(snapshot.Descriptor.Status));
            ModControlPageSupport.SetLabel(typed.Reason, snapshot.Descriptor.PermissionReason);
            typed.SetLines(lines);
        }

        public void OnSettingChanged(FeatureSetting setting, object settings)
        {
            // Read-only: TheArchive has no editable control on these pages.
        }

        public void OnOpened()
        {
        }

        public void OnClosed()
        {
        }
    }
}
