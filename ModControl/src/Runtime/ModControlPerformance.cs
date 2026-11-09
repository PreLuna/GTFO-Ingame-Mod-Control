using System.Globalization;

namespace PreLuna.ModControl.Runtime
{
    /// <summary>
    /// Lightweight, allocation-free counters used by the diagnostics page. They do not
    /// drive behavior; they only make menu-update cost visible during C6 verification.
    /// </summary>
    internal static class ModControlPerformance
    {
        static int updateCount;
        static int pageRefreshCount;
        static float lastUpdateMilliseconds;

        internal static void RecordUpdate(float milliseconds)
        {
            updateCount++;
            lastUpdateMilliseconds = milliseconds;
        }

        internal static void RecordPageRefresh()
        {
            pageRefreshCount++;
        }

        internal static string Summary()
            => "运行：更新 " + updateCount.ToString(CultureInfo.InvariantCulture) +
               " 次，最近更新 " + lastUpdateMilliseconds.ToString("0.000", CultureInfo.InvariantCulture) +
               " ms；页面刷新 " + pageRefreshCount.ToString(CultureInfo.InvariantCulture) + " 次";
    }
}
