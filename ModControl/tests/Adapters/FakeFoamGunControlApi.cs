namespace PreLuna.FoamGunCustom.Control
{
    public static class FoamGunControlApi
    {
        const string Snapshot = "{\"targetId\":\"PreLuna.FoamGunCustom\",\"displayName\":\"Fake 泡沫枪\",\"version\":\"1.0.2\",\"status\":0,\"revision\":\"5\",\"capabilities\":3,\"settings\":[" +
            "{\"key\":\"enabled\",\"displayName\":\"启用\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"shotInterval\",\"displayName\":\"发射间隔\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":0.08},\"minimum\":0.02,\"maximum\":2}," +
            "{\"key\":\"launchForce\",\"displayName\":\"发射力度\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":32},\"minimum\":1,\"maximum\":100}," +
            "{\"key\":\"pelletSpread\",\"displayName\":\"散布\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":1},\"minimum\":1,\"maximum\":45}," +
            "{\"key\":\"ammoCost\",\"displayName\":\"每发资源\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":0.5},\"minimum\":0.01,\"maximum\":50}]," +
            "\"saved\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"shotInterval\":{\"kind\":1,\"number\":0.08},\"launchForce\":{\"kind\":1,\"number\":32},\"pelletSpread\":{\"kind\":1,\"number\":1},\"ammoCost\":{\"kind\":1,\"number\":0.5}}," +
            "\"effective\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"shotInterval\":{\"kind\":1,\"number\":0.08},\"launchForce\":{\"kind\":1,\"number\":32},\"pelletSpread\":{\"kind\":1,\"number\":1},\"ammoCost\":{\"kind\":1,\"number\":0.5}}}";

        public static string GetSnapshotJson() => Snapshot;
        public static string ValidateJson(string changeJson) => "{\"outcome\":0,\"message\":\"ok\",\"issues\":[],\"snapshot\":" + Snapshot + "}";
        public static string SetPersistentJson(string changeJson) => "{\"outcome\":0,\"message\":\"saved\",\"issues\":[],\"snapshot\":" + Snapshot.Replace("\"revision\":\"5\"", "\"revision\":\"6\"") + "}";
    }
}
