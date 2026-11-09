using System;
using System.Collections.Generic;
using System.Globalization;
using BepInEx.Logging;
using CellMenu;
using PreLuna.ModControl.Core;
using TheArchive.Core.Attributes.Feature.Settings;
using TheArchive.Core.FeaturesAPI;
using TheArchive.Core.FeaturesAPI.Settings;

namespace PreLuna.ModControl.Features
{
    internal static class SliderActualValueDisplay
    {
        static readonly ManualLogSource Log = Logger.CreateLogSource("PreLuna.ModControl.Slider");
        static readonly Dictionary<int, Binding> Bindings = new();
        static int lastLoggedFound = -1;
        static int lastLoggedTotal = -1;

        sealed class Binding
        {
            internal Binding(NumberSetting setting, CM_SettingScrollReceiver receiver)
            {
                Setting = setting;
                Receiver = receiver;
            }

            internal NumberSetting Setting { get; }
            internal CM_SettingScrollReceiver Receiver { get; }
        }

        internal static bool Register(FeatureSettingsHelper helper)
        {
            Bindings.Clear();
            if (helper == null) return false;

            var total = 0;
            var found = 0;
            foreach (var setting in helper.Settings)
            {
                if (!(setting is NumberSetting number) || number.Slider == null) continue;
                if (number.Prop?.DeclaringType != typeof(EnemyHealthBarConfig)) continue;
                total++;

                var receiver = FindReceiver(number);
                var id = InstanceId(receiver);
                if (id == 0) continue;

                Bindings[id] = new Binding(number, receiver);
                found++;
            }

            if (found != lastLoggedFound || total != lastLoggedTotal)
            {
                Log.LogInfo("滑块显示桥：登记 " + found + "/" + total + " 个滑块。");
                lastLoggedFound = found;
                lastLoggedTotal = total;
            }

            if (found == 0) return false;
            RefreshText();
            return found == total;
        }

        internal static void RefreshText()
        {
            foreach (var binding in Bindings.Values)
            {
                try { binding.Receiver.UpdateText(); }
                catch (Exception error) { Log.LogWarning("刷新滑块文本失败：" + binding.Setting.Prop?.Name + "：" + error.Message); }
            }
        }

        internal static void Clear()
        {
            if (Bindings.Count > 0) Log.LogInfo("滑块显示桥：清理 " + Bindings.Count + " 个滑块。");
            Bindings.Clear();
            lastLoggedFound = -1;
            lastLoggedTotal = -1;
        }

        internal static void Apply(CM_SettingScrollReceiver receiver)
        {
            var id = InstanceId(receiver);
            if (id == 0 || !Bindings.TryGetValue(id, out var binding)) return;
            Write(receiver, binding.Setting);
        }

        static void Write(CM_SettingScrollReceiver receiver, NumberSetting setting)
        {
            var progress = receiver?.m_progress;
            if (progress == null) return;

            string text;
            try { text = Format(setting); }
            catch (Exception error)
            {
                Log.LogWarning("格式化滑块实际值失败：" + setting.Prop?.Name + "：" + error.Message);
                return;
            }

            if (!string.Equals(progress.text, text, StringComparison.Ordinal)) progress.text = text;
        }

        static string Format(NumberSetting setting)
        {
            var value = Convert.ToDouble(setting.GetValue(), CultureInfo.InvariantCulture);
            return SliderValueFormatter.Format(value, ToStyle(setting.Slider.Style));
        }

        static SliderValueStyle ToStyle(FSSlider.SliderStyle style)
        {
            switch (style)
            {
                case FSSlider.SliderStyle.FloatPercent: return SliderValueStyle.Percent;
                case FSSlider.SliderStyle.FloatNoDecimal: return SliderValueStyle.NoDecimal;
                case FSSlider.SliderStyle.FloatOneDecimal: return SliderValueStyle.OneDecimal;
                case FSSlider.SliderStyle.FloatTwoDecimal: return SliderValueStyle.TwoDecimal;
                default: return SliderValueStyle.NoDecimal;
            }
        }

        static CM_SettingScrollReceiver FindReceiver(FeatureSetting setting)
        {
            try
            {
                if (!(setting?.CM_SettingsItem is CM_SettingsItem item)) return null;
                var align = item.m_inputAlign;
                return align == null ? null : align.GetComponentInChildren<CM_SettingScrollReceiver>(true);
            }
            catch (Exception error)
            {
                Log.LogWarning("定位滑块控件失败：" + setting?.Prop?.Name + "：" + error.Message);
                return null;
            }
        }

        static int InstanceId(CM_SettingScrollReceiver receiver)
        {
            if (receiver == null) return 0;
            try { return receiver.GetInstanceID(); }
            catch { return 0; }
        }
    }
}
