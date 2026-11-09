using System;
using System.Collections.Generic;

namespace PreLuna.ModControl.Core.Operations
{
    public enum ProviderOutcome
    {
        Success,
        Conflict,
        Failed,
        Unavailable
    }

    public sealed class ProviderResult
    {
        public ProviderResult(
            ProviderOutcome outcome,
            string message = "",
            TargetSnapshot snapshot = null,
            IReadOnlyList<ValidationIssue> issues = null)
        {
            Outcome = outcome;
            Message = message ?? string.Empty;
            Snapshot = snapshot;
            Issues = issues ?? Array.Empty<ValidationIssue>();
        }

        public ProviderOutcome Outcome { get; }
        public string Message { get; }
        public TargetSnapshot Snapshot { get; }
        public IReadOnlyList<ValidationIssue> Issues { get; }

        public static ProviderResult Success(TargetSnapshot snapshot, string message = "")
            => new ProviderResult(ProviderOutcome.Success, message, snapshot);

        public static ProviderResult Conflict(string message)
            => new ProviderResult(ProviderOutcome.Conflict, message);

        public static ProviderResult Failed(string message, IReadOnlyList<ValidationIssue> issues = null)
            => new ProviderResult(ProviderOutcome.Failed, message, null, issues);

        public static ProviderResult Unavailable(string message)
            => new ProviderResult(ProviderOutcome.Unavailable, message);
    }

    public interface ITargetProvider
    {
        TargetSnapshot ReadSnapshot();
        ValidationResult Validate(ChangeSet changes, TargetSnapshot snapshot);
        ProviderResult SetPersistent(ChangeSet changes, TargetSnapshot snapshot, string expectedRevision);
    }
}
