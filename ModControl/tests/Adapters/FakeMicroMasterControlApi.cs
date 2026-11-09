namespace PreLuna.MicroMaster.Control
{
    public static class MicroMasterControlApi
    {
        const string Snapshot = "{\"targetId\":\"PreLuna.MicroMaster\",\"displayName\":\"Fake 微操大师\",\"version\":\"0.5.18\",\"status\":0,\"revision\":\"7\",\"capabilities\":3,\"settings\":[" +
            "{\"key\":\"enabled\",\"displayName\":\"启用\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"doubleClickWindow\",\"displayName\":\"双击间隔\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":0.3},\"minimum\":0.18,\"maximum\":0.6}," +
            "{\"key\":\"walkHaste\",\"displayName\":\"步行速度档位\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":0.25},\"minimum\":0.05,\"maximum\":0.5}," +
            "{\"key\":\"mouseSensitivity\",\"displayName\":\"鼠标灵敏度\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":1},\"minimum\":0.05,\"maximum\":10}," +
            "{\"key\":\"invertMouse\",\"displayName\":\"反转纵轴\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":false}}," +
            "{\"key\":\"nativeLight\",\"displayName\":\"原生手电常亮\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}]," +
            "\"saved\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"doubleClickWindow\":{\"kind\":1,\"number\":0.3},\"walkHaste\":{\"kind\":1,\"number\":0.25},\"mouseSensitivity\":{\"kind\":1,\"number\":1},\"invertMouse\":{\"kind\":0,\"boolean\":false},\"nativeLight\":{\"kind\":0,\"boolean\":true}}," +
            "\"effective\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"doubleClickWindow\":{\"kind\":1,\"number\":0.3},\"walkHaste\":{\"kind\":1,\"number\":0.25},\"mouseSensitivity\":{\"kind\":1,\"number\":1},\"invertMouse\":{\"kind\":0,\"boolean\":false},\"nativeLight\":{\"kind\":0,\"boolean\":true}}}";

        public static string GetSnapshotJson() => Snapshot;
        public static string ValidateJson(string changeJson) => "{\"outcome\":0,\"message\":\"ok\",\"issues\":[],\"snapshot\":" + Snapshot + "}";
        public static string SetPersistentJson(string changeJson) => "{\"outcome\":0,\"message\":\"saved\",\"issues\":[],\"snapshot\":" + Snapshot.Replace("\"revision\":\"7\"", "\"revision\":\"8\"") + "}";
    }
}
