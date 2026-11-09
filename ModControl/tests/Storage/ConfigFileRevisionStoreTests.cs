using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Core.Storage;

static class ConfigFileRevisionStoreTests
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

    static string TempFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), "preluna-modcontrol-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "shared.cfg");
    }

    static int Main(string[] args)
    {
        Case("managed-write-preserves-comments-and-unknown-keys", () =>
        {
            var path = TempFile();
            File.WriteAllText(path, "# header\n[General]\n\n## comment\nEnabled = true\nUnknown = 42\n\n[Other]\nKeep = yes\n");
            var hash = ConfigFileRevisionStore.Capture(path);
            ConfigFileRevisionStore.Save(path, hash, new[]
            {
                new ConfigFileWrite("General", "Enabled", "false"),
                new ConfigFileWrite("General", "Added", "7")
            });
            var text = File.ReadAllText(path);
            Check(text.Contains("# header") && text.Contains("## comment"), "comments");
            Check(text.Contains("Enabled = false") && text.Contains("Added = 7"), "managed");
            Check(text.Contains("Unknown = 42") && text.Contains("[Other]") && text.Contains("Keep = yes"), "other content");
        });

        Case("external-change-is-a-conflict-and-file-is-unchanged", () =>
        {
            var path = TempFile();
            File.WriteAllText(path, "[General]\nEnabled = true\n");
            var hash = ConfigFileRevisionStore.Capture(path);
            File.AppendAllText(path, "External = 1\n");
            var before = File.ReadAllText(path);
            var threw = false;
            try
            {
                ConfigFileRevisionStore.Save(path, hash, new[]
                {
                    new ConfigFileWrite("General", "Enabled", "false")
                });
            }
            catch (ConfigFileConflictException) { threw = true; }
            Check(threw, "expected conflict");
            Check(File.ReadAllText(path) == before, "file unchanged");
        });

        Case("missing-file-is-created", () =>
        {
            var path = TempFile();
            var hash = ConfigFileRevisionStore.Capture(path);
            Check(hash.Length == 0, "missing hash");
            ConfigFileRevisionStore.Save(path, hash, new[]
            {
                new ConfigFileWrite("General", "Enabled", "true")
            });
            Check(File.Exists(path), "created");
            Check(File.ReadAllText(path).Contains("[General]") && File.ReadAllText(path).Contains("Enabled = true"), "content");
        });

        Case("two-writers-with-same-revision-cannot-both-succeed", () =>
        {
            var path = TempFile();
            File.WriteAllText(path, "[General]\nA = 1\n");
            var first = ConfigFileRevisionStore.Capture(path);
            var second = first;
            ConfigFileRevisionStore.Save(path, first, new[] { new ConfigFileWrite("General", "A", "2") });
            var threw = false;
            try
            {
                ConfigFileRevisionStore.Save(path, second, new[] { new ConfigFileWrite("General", "B", "3") });
            }
            catch (ConfigFileConflictException) { threw = true; }
            Check(threw, "second writer must conflict");
            Check(!File.ReadAllText(path).Contains("B = 3"), "second write rejected");
        });

        var output = args.Length > 0 ? args[0] : "ConfigFileRevisionStoreTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
