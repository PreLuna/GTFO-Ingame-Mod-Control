using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PreLuna.ModControl.Core.Operations;
using PreLuna.ModControl.Tests;

static class OperationCoordinatorTests
{
    sealed record Result(string Name, bool Passed, string Error);
    static readonly List<Result> Results = new();

    static void Check(bool value, string message = "assertion failed")
    {
        if (!value) throw new Exception(message);
    }

    static void CheckValue(SettingValue actual, SettingValue expected, string name)
    {
        Check(actual != null && expected != null, name + ": null");
        Check(actual.ValueEquals(expected), name + ": " + actual + " != " + expected);
    }

    static void Case(string name, Action test)
    {
        try { test(); Results.Add(new(name, true, null)); }
        catch (Exception error) { Results.Add(new(name, false, error.ToString())); }
    }

    static int Main(string[] args)
    {
        Case("immediate-save-writes-p-and-r", () =>
        {
            var provider = new FakeTargetProvider();
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.Success, result.ToString());
            CheckValue(provider.GetSaved("distance"), SettingValue.Number(80), "P");
            CheckValue(provider.GetEffective("distance"), SettingValue.Number(80), "R");
            Check(provider.SetPersistentCalls == 1, "write count");
        });

        Case("immediate-boolean-save", () =>
        {
            var provider = new FakeTargetProvider();
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("enabled", SettingValue.Boolean(false));
            Check(result.Outcome == OperationOutcome.Success, result.ToString());
            CheckValue(provider.GetSaved("enabled"), SettingValue.Boolean(false), "P");
            CheckValue(provider.GetEffective("enabled"), SettingValue.Boolean(false), "R");
        });

        Case("validation-kind-rejected", () =>
        {
            var provider = new FakeTargetProvider();
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Boolean(true));
            Check(result.Outcome == OperationOutcome.ValidationRejected, result.ToString());
            Check(provider.SetPersistentCalls == 0, "no write");
        });

        Case("validation-range-rejected", () =>
        {
            var provider = new FakeTargetProvider();
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Number(999));
            Check(result.Outcome == OperationOutcome.ValidationRejected, result.ToString());
            Check(provider.SetPersistentCalls == 0, "no write");
        });

        Case("unknown-setting-rejected", () =>
        {
            var provider = new FakeTargetProvider();
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("unknown", SettingValue.Boolean(true));
            Check(result.Outcome == OperationOutcome.ValidationRejected, result.ToString());
            Check(provider.SetPersistentCalls == 0, "no write");
        });

        Case("provider-validation-rejected", () =>
        {
            var provider = new FakeTargetProvider
            {
                Validation = ValidationResult.Invalid(new ValidationIssue("distance", "provider rejected"))
            };
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.ValidationRejected, result.ToString());
            Check(provider.ValidateCalls == 1, "validate called");
            Check(provider.SetPersistentCalls == 0, "no write");
        });

        Case("write-conflict-maps-state-conflict", () =>
        {
            var provider = new FakeTargetProvider { SetPersistentOutcome = ProviderOutcome.Conflict };
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.StateConflict, result.ToString());
        });

        Case("target-destroyed", () =>
        {
            var provider = new FakeTargetProvider();
            var coordinator = new OperationCoordinator(provider);
            provider.Status = TargetStatus.Destroyed;
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.TargetUnavailable, result.ToString());
            Check(provider.SetPersistentCalls == 0, "no write");
        });

        Case("target-read-failure", () =>
        {
            var provider = new FakeTargetProvider();
            var coordinator = new OperationCoordinator(provider);
            provider.ThrowRead = true;
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.TargetUnavailable, result.ToString());
            Check(provider.SetPersistentCalls == 0, "no write");
        });

        Case("capability-unsupported", () =>
        {
            var provider = new FakeTargetProvider();
            provider.Capabilities = TargetCapabilities.Validate;
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.CapabilityUnsupported, result.ToString());
            Check(provider.SetPersistentCalls == 0, "no write");
        });

        Case("permission-denied", () =>
        {
            var provider = new FakeTargetProvider { PermissionGranted = false, PermissionReason = "host only" };
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.PermissionDenied, result.ToString());
            Check(provider.SetPersistentCalls == 0, "no write");
        });

        Case("reentrant-save-rejected", () =>
        {
            var provider = new FakeTargetProvider();
            var coordinator = new OperationCoordinator(provider);
            OperationResult inner = null;
            provider.OnSetPersistent = () =>
                inner = coordinator.SetImmediate("distance", SettingValue.Number(80));
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(inner != null && inner.Outcome == OperationOutcome.Reentrant, "inner reentrant");
            Check(result.Outcome == OperationOutcome.Success, result.ToString());
            Check(provider.SetPersistentCalls == 1, "only one write");
        });

        Case("provider-failure-keeps-values", () =>
        {
            var provider = new FakeTargetProvider { SetPersistentOutcome = ProviderOutcome.Failed };
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.SaveFailed, result.ToString());
            CheckValue(provider.GetSaved("distance"), SettingValue.Number(60), "P");
            CheckValue(provider.GetEffective("distance"), SettingValue.Number(60), "R");
        });

        Case("provider-exception-keeps-values", () =>
        {
            var provider = new FakeTargetProvider { SetPersistentException = new InvalidOperationException("disk") };
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.SaveFailed, result.ToString());
            CheckValue(provider.GetSaved("distance"), SettingValue.Number(60), "P");
        });

        Case("provider-unavailable-clears-snapshot", () =>
        {
            var provider = new FakeTargetProvider { SetPersistentOutcome = ProviderOutcome.Unavailable };
            var coordinator = new OperationCoordinator(provider);
            var result = coordinator.SetImmediate("distance", SettingValue.Number(80));
            Check(result.Outcome == OperationOutcome.TargetUnavailable, result.ToString());
            Check(coordinator.Snapshot == null, "snapshot cleared");
        });

        Case("consecutive-immediate-updates", () =>
        {
            var provider = new FakeTargetProvider();
            var coordinator = new OperationCoordinator(provider);
            Check(coordinator.SetImmediate("distance", SettingValue.Number(80)).Succeeded, "first");
            Check(coordinator.SetImmediate("distance", SettingValue.Number(90)).Succeeded, "second");
            CheckValue(provider.GetSaved("distance"), SettingValue.Number(90), "P");
            CheckValue(provider.GetEffective("distance"), SettingValue.Number(90), "R");
            Check(provider.SetPersistentCalls == 2, "two writes");
        });

        var output = args.Length > 0 ? args[0] : "OperationCoordinatorTests.json";
        File.WriteAllText(output, JsonSerializer.Serialize(Results, new JsonSerializerOptions { WriteIndented = true }));
        return Results.Exists(result => !result.Passed) ? 1 : 0;
    }
}
