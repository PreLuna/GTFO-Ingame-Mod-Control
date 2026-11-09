using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Core.Operations;

static class ValidationTests
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
        var boolean = new SettingDefinition("enabled", "Enabled", SettingKind.Boolean, SettingValue.Boolean(true));
        var number = new SettingDefinition("distance", "Distance", SettingKind.Number, SettingValue.Number(60), minimum: 5, maximum: 300);

        Case("accepts-valid-values", () =>
        {
            Check(boolean.Validate(SettingValue.Boolean(false)) == null, "bool");
            Check(number.Validate(SettingValue.Number(60)) == null, "number");
            Check(number.Validate(SettingValue.Number(5)) == null, "min boundary");
            Check(number.Validate(SettingValue.Number(300)) == null, "max boundary");
        });

        Case("rejects-kind-mismatch-and-null", () =>
        {
            Check(boolean.Validate(SettingValue.Number(1)) != null, "bool kind");
            Check(number.Validate(SettingValue.Integer(10)) != null, "number kind");
            Check(number.Validate(null) != null, "null");
        });

        Case("rejects-out-of-range-and-non-finite", () =>
        {
            Check(number.Validate(SettingValue.Number(4.9)) != null, "below min");
            Check(number.Validate(SettingValue.Number(300.1)) != null, "above max");
            Check(number.Validate(SettingValue.Number(double.NaN)) != null, "nan");
            Check(number.Validate(SettingValue.Number(double.PositiveInfinity)) != null, "infinity");
        });

        Case("value-equality-compares-kind-and-number-tolerance", () =>
        {
            Check(SettingValue.Boolean(true).ValueEquals(SettingValue.Boolean(true)), "bool equal");
            Check(!SettingValue.Boolean(true).ValueEquals(SettingValue.Boolean(false)), "bool differ");
            Check(SettingValue.Number(1.0).ValueEquals(SettingValue.Number(1.0 + 1e-12)), "number tolerance");
            Check(!SettingValue.Number(1.0).ValueEquals(SettingValue.Integer(1)), "kind differ");
        });

        var output = args.Length > 0 ? args[0] : "ValidationTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
