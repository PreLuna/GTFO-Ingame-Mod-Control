using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Core;

static class SliderValueFormatterTests
{
    sealed record Result(string Name, bool Passed, string Error);
    static readonly List<Result> Results = new();

    static void Check(bool value, string message = "assertion failed")
    {
        if (!value) throw new Exception(message);
    }

    static void Equal(string expected, string actual, string message)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
            throw new Exception(message + "：期望 \"" + expected + "\"，实际 \"" + actual + "\"");
    }

    static void Case(string name, Action test)
    {
        try { test(); Results.Add(new(name, true, null)); }
        catch (Exception error) { Results.Add(new(name, false, error.ToString())); }
    }

    static int Main(string[] args)
    {
        Case("NoDecimal-formats-actual-integer", () =>
            Equal("157", SliderValueFormatter.Format(157d, SliderValueStyle.NoDecimal), "157"));

        Case("OneDecimal-keeps-one-decimal", () =>
        {
            Equal("9.3", SliderValueFormatter.Format(9.3d, SliderValueStyle.OneDecimal), "9.3");
            Equal("5.0", SliderValueFormatter.Format(5d, SliderValueStyle.OneDecimal), "5.0");
        });

        Case("TwoDecimal-keeps-two-decimals", () =>
            Equal("0.59", SliderValueFormatter.Format(.59d, SliderValueStyle.TwoDecimal), ".59"));

        Case("Percent-formats-fraction-as-whole-percent", () =>
        {
            Equal("59%", SliderValueFormatter.Format(.59d, SliderValueStyle.Percent), ".59");
            Equal("100%", SliderValueFormatter.Format(1d, SliderValueStyle.Percent), "1");
        });

        Case("Integer-rounds-away-from-zero", () =>
            Equal("5", SliderValueFormatter.Format(5.4d, SliderValueStyle.Integer), "5.4"));

        var output = args.Length > 0 ? args[0] : "SliderValueFormatterTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
