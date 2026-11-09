namespace PreLuna.Captain.Control
{
    public static class CaptainControlApi
    {
        const string Snapshot = "{\"targetId\":\"PreLuna.Captain\",\"displayName\":\"Fake 队长中文显示\",\"version\":\"0.5.4\",\"status\":0,\"revision\":\"3\",\"capabilities\":3,\"settings\":[" +
            "{\"key\":\"chineseMenus\",\"displayName\":\"通讯菜单\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"chineseItems\",\"displayName\":\"携带物品\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}]," +
            "\"saved\":{\"chineseMenus\":{\"kind\":0,\"boolean\":true},\"chineseItems\":{\"kind\":0,\"boolean\":true}}," +
            "\"effective\":{\"chineseMenus\":{\"kind\":0,\"boolean\":true},\"chineseItems\":{\"kind\":0,\"boolean\":true}}}";

        public static string GetSnapshotJson() => Snapshot;
        public static string ValidateJson(string changeJson) => "{\"outcome\":0,\"message\":\"ok\",\"issues\":[],\"snapshot\":" + Snapshot + "}";
        public static string SetPersistentJson(string changeJson) => "{\"outcome\":0,\"message\":\"saved\",\"issues\":[],\"snapshot\":" + Snapshot.Replace("\"revision\":\"3\"", "\"revision\":\"4\"") + "}";
    }
}
