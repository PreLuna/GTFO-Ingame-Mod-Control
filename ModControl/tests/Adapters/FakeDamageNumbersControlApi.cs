namespace PreLuna.EnemyHealthBar.DamageNumbers.Control
{
    public static class DamageNumberControlApi
    {
        const string Snapshot = "{\"targetId\":\"PreLuna.EnemyHealthBar.DamageNumbers\",\"displayName\":\"Fake 伤害数字\",\"version\":\"0.1.1\",\"status\":0,\"revision\":\"4\",\"capabilities\":3,\"settings\":[" +
            "{\"key\":\"enabled\",\"displayName\":\"启用\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}," +
            "{\"key\":\"maxRows\",\"displayName\":\"最大行数\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":12},\"minimum\":1,\"maximum\":12}," +
            "{\"key\":\"lifetime\",\"displayName\":\"生命周期\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":0.95},\"minimum\":0.3,\"maximum\":3}," +
            "{\"key\":\"fontSize\",\"displayName\":\"字号\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":24},\"minimum\":12,\"maximum\":48}," +
            "{\"key\":\"maxDistance\",\"displayName\":\"最大距离\",\"kind\":1,\"defaultValue\":{\"kind\":1,\"number\":100},\"minimum\":0,\"maximum\":300}," +
            "{\"key\":\"showArmor\",\"displayName\":\"显示护甲\",\"kind\":0,\"defaultValue\":{\"kind\":0,\"boolean\":true}}]," +
            "\"saved\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"maxRows\":{\"kind\":1,\"number\":12},\"lifetime\":{\"kind\":1,\"number\":0.95},\"fontSize\":{\"kind\":1,\"number\":24},\"maxDistance\":{\"kind\":1,\"number\":100},\"showArmor\":{\"kind\":0,\"boolean\":true}}," +
            "\"effective\":{\"enabled\":{\"kind\":0,\"boolean\":true},\"maxRows\":{\"kind\":1,\"number\":12},\"lifetime\":{\"kind\":1,\"number\":0.95},\"fontSize\":{\"kind\":1,\"number\":24},\"maxDistance\":{\"kind\":1,\"number\":100},\"showArmor\":{\"kind\":0,\"boolean\":true}}}";

        public static string GetSnapshotJson() => Snapshot;
        public static string ValidateJson(string changeJson) => "{\"outcome\":0,\"message\":\"ok\",\"issues\":[],\"snapshot\":" + Snapshot + "}";
        public static string SetPersistentJson(string changeJson) => "{\"outcome\":0,\"message\":\"saved\",\"issues\":[],\"snapshot\":" + Snapshot.Replace("\"revision\":\"4\"", "\"revision\":\"5\"") + "}";
    }
}
