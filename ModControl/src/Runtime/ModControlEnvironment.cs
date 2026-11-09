using PreLuna.ModControl.Features;
using UnityEngine;

namespace PreLuna.ModControl.Runtime
{
    /// <summary>
    /// Read-only environment facts for C6. Display/focus are reported, not changed:
    /// TheArchive owns the settings menu and its input focus.
    /// </summary>
    internal static class ModControlEnvironment
    {
        internal static string ScreenSummary()
        {
            try
            {
                return Screen.width + "×" + Screen.height + (Screen.fullScreen ? " 全屏" : " 窗口");
            }
            catch
            {
                return "显示信息不可用";
            }
        }

        internal static string InputSummary()
        {
            try
            {
                return ModControlPageSupport.IsMenuOpen()
                    ? "菜单已打开：输入焦点由 TheArchive 持有；ModControl 未注册全局热键"
                    : "菜单未打开：ModControl 未注册全局热键";
            }
            catch
            {
                return "输入状态不可用";
            }
        }
    }
}
