using System;
using System.Collections.Generic;
using System.Linq;
using PreLuna.ModControl.Core.Operations;

namespace PreLuna.ModControl.Core.Keybinds
{
    internal sealed record KeybindAction(string Property, string Target, string Key, string Name);
    internal sealed record KeybindDuplicate(int Code, IReadOnlyList<KeybindAction> Actions);

    // A catalogue of explicitly adapted actions, not a second config store or input dispatcher.
    internal sealed class KeybindSession
    {
        readonly IReadOnlyDictionary<string, Func<ITargetProvider>> factories;
        readonly Dictionary<string, ITargetProvider> providers = new(StringComparer.Ordinal);
        readonly Dictionary<string, TargetSnapshot> snapshots = new(StringComparer.Ordinal);
        internal IReadOnlyList<KeybindAction> Actions { get; }
        internal Dictionary<string, int> Values { get; } = new(StringComparer.Ordinal);

        internal KeybindSession(IReadOnlyList<KeybindAction> actions,
            IReadOnlyDictionary<string, Func<ITargetProvider>> factories)
        {
            Actions = actions;
            this.factories = factories;
        }

        internal void Refresh()
        {
            Values.Clear();
            snapshots.Clear();
            foreach (var pair in factories)
            {
                try
                {
                    if (!providers.TryGetValue(pair.Key, out var provider) || provider == null)
                        providers[pair.Key] = provider = pair.Value();
                    var snapshot = provider?.ReadSnapshot();
                    if (snapshot?.Descriptor?.Status != TargetStatus.Available) continue;
                    snapshots[pair.Key] = snapshot;
                    foreach (var action in Actions.Where(a => a.Target == pair.Key))
                        if (snapshot.Descriptor.Find(action.Key) != null &&
                            snapshot.TryGetEffective(action.Key, out var value) && value.TryGetInteger(out var code))
                            Values[action.Property] = code;
                }
                catch
                {
                    // One absent/incompatible optional target must not hide the others.
                    providers.Remove(pair.Key);
                }
            }
        }

        internal string DescribeTarget(string target)
        {
            var supported = Actions.Count(a => a.Target == target && Values.ContainsKey(a.Property));
            return snapshots.TryGetValue(target, out var snapshot)
                ? snapshot.Descriptor.Version + " / 已接入 " + supported + " 项"
                : "未安装或控制接口不可用";
        }

        internal string Set(string property, int code)
        {
            var action = Actions.FirstOrDefault(a => a.Property == property);
            if (action == null) return "未知按键，未保存。";
            Refresh();
            if (!Values.ContainsKey(property) || !providers.TryGetValue(action.Target, out var provider) || provider == null)
                return "目标未提供此按键接口，请安装配套版本；未保存。";
            try
            {
                var coordinator = new OperationCoordinator(provider);
                var result = coordinator.SetImmediate(action.Key, SettingValue.Integer(code));
                return result.Outcome == OperationOutcome.Success
                    ? action.Name + "：已保存并生效"
                    : action.Name + "：未保存（" + (result.Outcome == OperationOutcome.StateConflict ? "配置冲突" : "操作失败") + "）" + result.Message +
                      (result.Issues.Count == 0 ? "" : "；" + string.Join("；", result.Issues.Select(i => i.Message)));
            }
            finally { Refresh(); }
        }

        internal IReadOnlyList<KeybindDuplicate> Duplicates()
            => Actions.Where(a => Values.TryGetValue(a.Property, out var code) && code != 0)
                .GroupBy(a => Values[a.Property]).Where(g => g.Count() > 1).OrderBy(g => g.Key)
                .Select(g => new KeybindDuplicate(g.Key, g.ToArray())).ToArray();
    }
}
