using System;
using System.Collections.Generic;

namespace PreLuna.ModControl.Core.Operations
{
    public enum OperationOutcome
    {
        Success,
        ValidationRejected,
        PermissionDenied,
        StateConflict,
        CapabilityUnsupported,
        TargetUnavailable,
        SaveFailed,
        Reentrant
    }

    public sealed class OperationResult
    {
        public OperationResult(
            OperationOutcome outcome,
            string message,
            IReadOnlyList<ValidationIssue> issues = null)
        {
            Outcome = outcome;
            Message = message ?? string.Empty;
            Issues = issues ?? Array.Empty<ValidationIssue>();
        }

        public OperationOutcome Outcome { get; }
        public string Message { get; }
        public IReadOnlyList<ValidationIssue> Issues { get; }
        public bool Succeeded => Outcome == OperationOutcome.Success;

        public override string ToString() => Outcome + (Message.Length == 0 ? string.Empty : ": " + Message);
    }
}
