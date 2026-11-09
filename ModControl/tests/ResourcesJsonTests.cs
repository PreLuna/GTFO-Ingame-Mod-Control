using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

static class ResourcesJsonTests
{
    sealed record Result(string Name, bool Passed, string Error);
    static readonly List<Result> Results = new();

    static void Check(bool value, string message = "assertion failed")
    {
        if (!value) throw new Exception(message);
    }

    static void Case(string name, Action test)
    {
        try { test(); Results.Add(new(name, true, null)); }
        catch (Exception error) { Results.Add(new(name, false, error.ToString())); }
    }

    static int Main(string[] args)
    {
        var output = args.Length > 0 ? args[0] : "ResourcesJsonTests.json";
        var resourcePath = args.Length > 1 ? args[1] : "Module_PreLuna.ModControl_Group_Definition.json";

        Case("module-group-definition-has-preluna-localization", () =>
        {
            using var document = JsonDocument.Parse(File.ReadAllText(resourcePath));
            var root = document.RootElement;
            var moduleGroup = root.GetProperty("ModuleGroup");
            var localization = moduleGroup.GetProperty("LocalizationData");
            var displayName = localization.GetProperty("DisplayName");
            Check(displayName.GetProperty("English").GetString() == "PreLuna", "English display name");
            Check(displayName.GetProperty("Chinese").GetString() == "PreLuna", "Chinese display name");
            Check(localization.GetProperty("Description").GetProperty("Chinese").GetString() == "游戏内统一 MOD 管理", "Chinese description");
            Check(moduleGroup.GetProperty("SubGroups").GetArrayLength() == 0, "no subgroups");
            Check(root.GetProperty("TopLevelGroups").GetArrayLength() == 0, "no top-level groups");
        });

        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
