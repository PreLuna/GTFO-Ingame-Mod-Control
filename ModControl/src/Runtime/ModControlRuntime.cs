using System;
using System.Collections.Generic;
using BepInEx.Logging;
using PreLuna.ModControl.Core;
using TheArchive;
using TheArchive.Interfaces;

namespace PreLuna.ModControl.Runtime
{
    internal static class ModControlRuntime
    {
        static ManualLogSource log;
        static IArchiveLogger archiveLogger;

        internal static IReadOnlyList<InstalledPlugin> Targets { get; private set; } = Array.Empty<InstalledPlugin>();
        internal static string LastError { get; private set; } = string.Empty;
        internal static bool RegistrationSucceeded { get; private set; }
        internal static string RegistrationDetail { get; private set; } = "尚未注册";
        internal static bool ArchiveInitialized { get; private set; }
        internal static string ArchiveVersion { get; private set; } = "未知";

        internal static void AttachLog(ManualLogSource source)
        {
            log = source;
        }

        internal static void AttachArchiveLogger(IArchiveLogger logger)
        {
            archiveLogger = logger;
        }

        internal static void MarkRegistered(bool setupAlreadyRan)
        {
            RegistrationSucceeded = true;
            RegistrationDetail = setupAlreadyRan ? "成功，框架已完成初始化" : "成功，框架初始化后生效";
            log?.LogInfo("Archive module registered (" + RegistrationDetail + ").");
            archiveLogger?.Info("PreLuna.ModControl archive module registered.");
        }

        internal static void MarkRegistrationFailed(Exception error)
        {
            RegistrationSucceeded = false;
            RegistrationDetail = "失败";
            RecordError("Archive 模块注册", error);
        }

        internal static void RefreshFramework()
        {
            try
            {
                var modules = ArchiveMod.Modules;
                ArchiveInitialized = modules != null && modules.Count > 0;
                ArchiveVersion = string.IsNullOrWhiteSpace(ArchiveMod.VERSION_STRING) ? "未知" : ArchiveMod.VERSION_STRING;
            }
            catch (Exception error)
            {
                RecordError("读取 TheArchive 状态", error);
            }
        }

        internal static void RefreshTargets()
        {
            try
            {
                Targets = PluginCatalog.ReadInstalledPreLuna();
            }
            catch (Exception error)
            {
                RecordError("读取已加载插件", error);
            }
        }

        internal static ModControlStatus CreateStatus()
        {
            RefreshFramework();
            RefreshTargets();
            var verified = string.Equals(ArchiveVersion, ModControlInfo.ExpectedArchiveVersion, StringComparison.OrdinalIgnoreCase);
            return new ModControlStatus(
                ModControlInfo.Version,
                ArchiveVersion,
                ArchiveInitialized,
                verified,
                RegistrationSucceeded,
                RegistrationDetail,
                Targets,
                LastError);
        }

        internal static void RecordError(string context, Exception error)
        {
            LastError = context + "：" + (error?.Message ?? "未知错误");
            log?.LogError(LastError + Environment.NewLine + error);
            archiveLogger?.Error(LastError);
        }
    }
}
