namespace PreLuna.HeadCamera.Control
{
    public static class HeadCameraControlApi
    {
        const string Snapshot = "{\"targetId\":\"PreLuna.HeadCamera\",\"displayName\":\"Fake 头戴摄像头\",\"version\":\"0.3.16\",\"status\":0,\"revision\":\"7\",\"capabilities\":3,\"settings\":[" +
            "{\"key\":\"enabled\",\"displayName\":\"启用\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"width\",\"displayName\":\"渲染宽度\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":1280},\"minimum\":160,\"maximum\":1920}," +
            "{\"key\":\"fps\",\"displayName\":\"刷新帧率\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":30},\"minimum\":5,\"maximum\":60}," +
            "{\"key\":\"fov\",\"displayName\":\"视野角度\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":80},\"minimum\":45,\"maximum\":110}," +
            "{\"key\":\"viewDistance\",\"displayName\":\"最大绘制距离\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":80},\"minimum\":15,\"maximum\":120}," +
            "{\"key\":\"windowWidth\",\"displayName\":\"小窗宽度\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":480},\"minimum\":200,\"maximum\":960}]," +
            "\"saved\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"width\":{\"kind\":1,\"number\":1280},\"fps\":{\"kind\":1,\"number\":30},\"fov\":{\"kind\":1,\"number\":80},\"viewDistance\":{\"kind\":1,\"number\":80},\"windowWidth\":{\"kind\":1,\"number\":480}}," +
            "\"effective\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"width\":{\"kind\":1,\"number\":1280},\"fps\":{\"kind\":1,\"number\":30},\"fov\":{\"kind\":1,\"number\":80},\"viewDistance\":{\"kind\":1,\"number\":80},\"windowWidth\":{\"kind\":1,\"number\":480}}}";

        public static string GetSnapshotJson() => Snapshot;
        public static string ValidateJson(string changeJson) => "{\"outcome\":0,\"message\":\"ok\",\"issues\":[],\"snapshot\":" + Snapshot + "}";
        public static string SetPersistentJson(string changeJson) => "{\"outcome\":0,\"message\":\"saved\",\"issues\":[],\"snapshot\":" + Snapshot.Replace("\"revision\":\"7\"", "\"revision\":\"8\"") + "}";
    }
}
