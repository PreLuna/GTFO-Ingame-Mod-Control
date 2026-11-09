namespace PreLuna.ThirdPersonHUD.Control
{
    // Test double with the same full name as the target API. The ModControl adapter must
    // resolve it by name only and map its JSON contract without a compile reference.
    public static class ThirdPersonHudControlApi
    {
        const string Snapshot = "{\"targetId\":\"PreLuna.ThirdPersonHUD\",\"displayName\":\"Fake HUD\",\"version\":\"0.13.1\",\"status\":0,\"revision\":\"7\",\"capabilities\":3,\"settings\":[" +
            "{\"key\":\"panel\",\"displayName\":\"角色旁面板\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"personal\",\"displayName\":\"个人读数\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"team\",\"displayName\":\"队伍摘要\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"scene\",\"displayName\":\"场景提示\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"alerts\",\"displayName\":\"提示区\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"panelScale\",\"displayName\":\"面板缩放\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":1},\"minimum\":0.65,\"maximum\":1.6}," +
            "{\"key\":\"panelWidth\",\"displayName\":\"面板宽度\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":360},\"minimum\":260,\"maximum\":420}]," +
            "\"saved\":{\"panel\":{\"kind\":0,\"boolean\":true},\"personal\":{\"kind\":0,\"boolean\":true},\"team\":{\"kind\":0,\"boolean\":true},\"scene\":{\"kind\":0,\"boolean\":true},\"alerts\":{\"kind\":0,\"boolean\":true},\"panelScale\":{\"kind\":1,\"number\":1},\"panelWidth\":{\"kind\":1,\"number\":360}}," +
            "\"effective\":{\"panel\":{\"kind\":0,\"boolean\":true},\"personal\":{\"kind\":0,\"boolean\":true},\"team\":{\"kind\":0,\"boolean\":true},\"scene\":{\"kind\":0,\"boolean\":true},\"alerts\":{\"kind\":0,\"boolean\":true},\"panelScale\":{\"kind\":1,\"number\":1},\"panelWidth\":{\"kind\":1,\"number\":360}}}";

        public static string GetSnapshotJson() => Snapshot;

        public static string ValidateJson(string changeJson)
            => "{\"outcome\":0,\"message\":\"ok\",\"issues\":[],\"snapshot\":" + Snapshot + "}";

        public static string SetPersistentJson(string changeJson)
            => "{\"outcome\":0,\"message\":\"saved\",\"issues\":[],\"snapshot\":" +
               Snapshot.Replace("\"revision\":\"7\"", "\"revision\":\"8\"") + "}";
    }
}
