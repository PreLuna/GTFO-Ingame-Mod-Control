using System;
using System.Collections.Generic;

namespace PreLuna.ModControl.Core.Operations
{
    public sealed class OperationCoordinator
    {
        readonly ITargetProvider provider;
        TargetSnapshot snapshot;
        bool busy;

        public OperationCoordinator(ITargetProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Refresh();
        }

        public TargetSnapshot Snapshot => snapshot;

        public TargetSnapshot Refresh()
        {
            try
            {
                var latest = provider.ReadSnapshot();
                if (latest != null) snapshot = latest;
            }
            catch
            {
                snapshot = null;
            }

            return snapshot;
        }

        public OperationResult SetImmediate(string key, SettingValue value)
        {
            if (busy) return Remember(Reentrant("上一操作尚未完成"));
            busy = true;
            try
            {
                var current = ReadSnapshot();
                if (current == null || current.Descriptor == null || current.Descriptor.Status != TargetStatus.Available)
                    return Remember(Result(OperationOutcome.TargetUnavailable, "目标当前不可用"));
                snapshot = current;
                if (!current.Descriptor.PermissionGranted)
                    return Remember(Result(OperationOutcome.PermissionDenied, current.Descriptor.PermissionReason));

                var definition = current.Descriptor.Find(key);
                if (definition == null)
                    return Remember(Result(OperationOutcome.ValidationRejected, "目标未注册该设置"));
                var issue = definition.Validate(value);
                if (issue != null)
                    return Remember(Result(OperationOutcome.ValidationRejected, "改动未通过校验", new[] { issue }));
                if (!current.Descriptor.Has(TargetCapabilities.Validate) ||
                    !current.Descriptor.Has(TargetCapabilities.SavePersistent))
                    return Remember(Result(OperationOutcome.CapabilityUnsupported, "目标不支持即时保存"));

                var values = new Dictionary<string, SettingValue>(StringComparer.Ordinal) { [key] = value };
                var changes = new ChangeSet(current.Revision, values);
                try
                {
                    var validation = provider.Validate(changes, current);
                    if (validation != null && !validation.IsValid)
                        return Remember(Result(OperationOutcome.ValidationRejected, "目标校验拒绝", validation.Issues));
                    var result = provider.SetPersistent(changes, current, current.Revision);
                    return Remember(Map(result, current));
                }
                catch (Exception error)
                {
                    RefreshAfterFailure();
                    return Remember(Result(OperationOutcome.SaveFailed, "即时保存异常：" + error.Message));
                }
            }
            finally
            {
                busy = false;
            }
        }

        TargetSnapshot ReadSnapshot()
        {
            try
            {
                return provider.ReadSnapshot();
            }
            catch
            {
                return null;
            }
        }

        OperationResult Map(ProviderResult result, TargetSnapshot current)
        {
            if (result == null)
            {
                RefreshAfterFailure();
                return Result(OperationOutcome.SaveFailed, "目标没有返回结果");
            }

            if (result.Outcome == ProviderOutcome.Success)
            {
                snapshot = result.Snapshot ?? current;
                return Result(OperationOutcome.Success, string.IsNullOrEmpty(result.Message) ? "已保存" : result.Message);
            }

            if (result.Outcome == ProviderOutcome.Conflict)
            {
                RefreshAfterFailure();
                return Result(OperationOutcome.StateConflict, result.Message);
            }

            if (result.Outcome == ProviderOutcome.Unavailable)
            {
                snapshot = null;
                return Result(OperationOutcome.TargetUnavailable, result.Message);
            }

            RefreshAfterFailure();
            return Result(OperationOutcome.SaveFailed, result.Message, result.Issues);
        }

        void RefreshAfterFailure()
        {
            try
            {
                var latest = provider.ReadSnapshot();
                if (latest != null) snapshot = latest;
            }
            catch
            {
                // Keep the last known snapshot; the result already reports failure.
            }
        }

        OperationResult Reentrant(string message)
            => Result(OperationOutcome.Reentrant, message);

        static OperationResult Remember(OperationResult result) => result;

        static OperationResult Result(
            OperationOutcome outcome,
            string message,
            IReadOnlyList<ValidationIssue> issues = null)
            => new OperationResult(outcome, message, issues);
    }
}
