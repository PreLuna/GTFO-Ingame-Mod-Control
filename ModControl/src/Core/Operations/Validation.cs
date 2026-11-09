using System.Collections.Generic;

namespace PreLuna.ModControl.Core.Operations
{
    public sealed class ValidationIssue
    {
        public ValidationIssue(string key, string message)
        {
            Key = key ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public string Key { get; }
        public string Message { get; }

        public override string ToString() => (Key.Length == 0 ? string.Empty : Key + ": ") + Message;
    }

    public sealed class ValidationResult
    {
        ValidationResult(bool isValid, IReadOnlyList<ValidationIssue> issues)
        {
            IsValid = isValid;
            Issues = issues ?? System.Array.Empty<ValidationIssue>();
        }

        public bool IsValid { get; }
        public IReadOnlyList<ValidationIssue> Issues { get; }

        public static ValidationResult Valid() => new ValidationResult(true, System.Array.Empty<ValidationIssue>());

        public static ValidationResult Invalid(params ValidationIssue[] issues)
            => new ValidationResult(false, issues ?? System.Array.Empty<ValidationIssue>());

        public static ValidationResult Invalid(IReadOnlyList<ValidationIssue> issues)
            => new ValidationResult(false, issues);
    }
}
