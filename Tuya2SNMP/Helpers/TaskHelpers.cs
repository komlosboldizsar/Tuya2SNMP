using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuya2SNMP.Helpers
{
    internal static class TaskHelpers
    {

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
        public static async Task<bool> WithTimeout(this Task task, int timeout, CancellationToken cancellationToken)
        {
            CancellationTokenSource timeoutCancellationTokenSource = new();
            CancellationTokenSource combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);
            Task timeoutTask = Task.Delay(timeout, combinedCancellationTokenSource.Token);
            Task anyTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
            timeoutCancellationTokenSource.Cancel();
            return anyTask != timeoutTask;
        }
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods

    }
}
