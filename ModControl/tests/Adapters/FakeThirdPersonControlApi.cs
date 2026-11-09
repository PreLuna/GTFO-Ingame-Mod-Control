namespace PreLuna.ThirdPerson.Control
{
    // Test double with the same full name as the target API. The ModControl adapter resolves
    // it by name only and maps the JSON contract without a compile reference.
    public static class ThirdPersonControlApi
    {
        const string Snapshot = "{\"targetId\":\"PreLuna.ThirdPerson\",\"displayName\":\"Fake 第三人称镜头\",\"version\":\"0.6.3\",\"status\":0,\"revision\":\"7\",\"capabilities\":3,\"settings\":[" +
            "{\"key\":\"distance\",\"displayName\":\"镜头距离\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":2.25},\"minimum\":0.6,\"maximum\":4}," +
            "{\"key\":\"shoulder\",\"displayName\":\"右肩偏移\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":0.48},\"minimum\":-1,\"maximum\":1}," +
            "{\"key\":\"height\",\"displayName\":\"支点高度偏移\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":0.1},\"minimum\":-0.35,\"maximum\":0.6}," +
            "{\"key\":\"shoulderDistance\",\"displayName\":\"肩射距离\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":1.35},\"minimum\":0.6,\"maximum\":3}," +
            "{\"key\":\"leftShoulder\",\"displayName\":\"左肩\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":false}}]," +
            "\"saved\":{\"distance\":{\"kind\":1,\"number\":2.25},\"shoulder\":{\"kind\":1,\"number\":0.48},\"height\":{\"kind\":1,\"number\":0.1},\"shoulderDistance\":{\"kind\":1,\"number\":1.35},\"leftShoulder\":{\"kind\":0,\"boolean\":false}}," +
            "\"effective\":{\"distance\":{\"kind\":1,\"number\":2.25},\"shoulder\":{\"kind\":1,\"number\":0.48},\"height\":{\"kind\":1,\"number\":0.1},\"shoulderDistance\":{\"kind\":1,\"number\":1.35},\"leftShoulder\":{\"kind\":0,\"boolean\":false}}}";

        public static string GetSnapshotJson() => Snapshot;

        public static string ValidateJson(string changeJson)
            => "{\"outcome\":0,\"message\":\"ok\",\"issues\":[],\"snapshot\":" + Snapshot + "}";

        public static string SetPersistentJson(string changeJson)
            => "{\"outcome\":0,\"message\":\"saved\",\"issues\":[],\"snapshot\":" +
               Snapshot.Replace("\"revision\":\"7\"", "\"revision\":\"8\"") + "}";
    }
}
