namespace PreLuna.EnemyHealthBar.BFVFeedback.Control
{
    public static class BfvFeedbackControlApi
    {
        const string Snapshot = "{\"targetId\":\"PreLuna.EnemyHealthBar.BFVFeedback\",\"displayName\":\"Fake BFV 反馈\",\"version\":\"0.2.0\",\"status\":0,\"revision\":\"4\",\"capabilities\":3,\"settings\":[" +
            "{\"key\":\"enabled\",\"displayName\":\"启用 BFV 反馈\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"styleIndex\",\"displayName\":\"风格\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":1},\"minimum\":0,\"maximum\":2}," +
            "{\"key\":\"colorNormal\",\"displayName\":\"普通命中颜色\",\"kind\":3,\"defaultValue\":{\"kind\":3,\"string\":\"#FFFFFF\"}}]," +
            "\"saved\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"styleIndex\":{\"kind\":1,\"number\":1},\"colorNormal\":{\"kind\":3,\"string\":\"#FFFFFF\"}}," +
            "\"effective\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"styleIndex\":{\"kind\":1,\"number\":1},\"colorNormal\":{\"kind\":3,\"string\":\"#FFFFFF\"}}}";

        public static string GetSnapshotJson() => Snapshot;
        public static string ValidateJson(string changeJson) => "{\"outcome\":0,\"message\":\"ok\",\"issues\":[],\"snapshot\":" + Snapshot + "}";
        public static string SetPersistentJson(string changeJson) => "{\"outcome\":0,\"message\":\"saved\",\"issues\":[],\"snapshot\":" + Snapshot.Replace("\"revision\":\"4\"", "\"revision\":\"5\"") + "}";
    }
}
