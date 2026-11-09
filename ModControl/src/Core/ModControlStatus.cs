using System;
using System.Collections.Generic;

namespace PreLuna.ModControl.Core
{
    public sealed class ModControlStatus
    {
        public ModControlStatus(
            string modVersion,
            string archiveVersion,
            bool archiveInitialized,
            bool archiveVersionVerified,
            bool registrationSucceeded,
            string registrationDetail,
            IReadOnlyList<InstalledPlugin> targets,
            string lastError)
        {
            ModVersion = modVersion ?? string.Empty;
            ArchiveVersion = archiveVersion ?? string.Empty;
            ArchiveInitialized = archiveInitialized;
            ArchiveVersionVerified = archiveVersionVerified;
            RegistrationSucceeded = registrationSucceeded;
            RegistrationDetail = registrationDetail ?? string.Empty;
            Targets = targets == null ? Array.Empty<InstalledPlugin>() : new List<InstalledPlugin>(targets).ToArray();
            LastError = lastError ?? string.Empty;
        }

        public string ModVersion { get; }
        public string ArchiveVersion { get; }
        public bool ArchiveInitialized { get; }
        public bool ArchiveVersionVerified { get; }
        public bool RegistrationSucceeded { get; }
        public string RegistrationDetail { get; }
        public IReadOnlyList<InstalledPlugin> Targets { get; }
        public string LastError { get; }

        public IReadOnlyList<string> OverviewLines()
        {
            var lines = new List<string>
            {
                "管理模块 PreLuna.ModControl " + ModVersion,
                "框架 TheArchive " + VersionText() + (ArchiveVersionVerified ? "（已验证）" : "（未验证）"),
                ArchiveInitialized ? "框架状态：已初始化" : "框架状态：未初始化，管理入口不可用",
                "Archive 模块注册：" + (RegistrationSucceeded ? "成功" : "失败"),
                "已加载 PreLuna 目标：" + Targets.Count + " 个",
                "目标配置：白名单即时自动保存",
                "未适配目标只读；恢复默认未提供。"
            };
            AppendLastError(lines);
            return lines;
        }

        public IReadOnlyList<string> DiagnosticsLines()
        {
            var lines = new List<string>
            {
                "程序集：PreLuna.ModControl " + ModVersion,
                "TheArchive：" + VersionText() + " / 初始化：" + (ArchiveInitialized ? "是" : "否") + " / 版本：" + (ArchiveVersionVerified ? "已验证" : "未验证"),
                "Archive 模块注册：" + (RegistrationSucceeded ? "成功" : "失败") + "（" + RegistrationDetail + "）",
                "已加载 PreLuna 插件：" + Targets.Count,
                "日志源：PreLuna.ModControl",
                "写入：仅经适配器的安全行级保存；未适配目标只读。"
            };
            AppendLastError(lines);
            return lines;
        }

        string VersionText()
            => string.IsNullOrWhiteSpace(ArchiveVersion) ? "未知" : ArchiveVersion;

        void AppendLastError(List<string> lines)
        {
            lines.Add("最近错误：" + (string.IsNullOrWhiteSpace(LastError) ? "无" : LastError));
        }
    }
}
