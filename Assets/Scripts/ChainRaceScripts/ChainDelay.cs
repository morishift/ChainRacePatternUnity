
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chain that waits for a specified duration
    /// </summary>
    public class ChainDelay : Chain
    {
        int delayMilliseconds;
        CancellationTokenSource cts;

        /// <summary>
        /// Creates a delay chain with duration in seconds
        /// </summary>
        public ChainDelay(float seconds)
        {
            delayMilliseconds = (int)(seconds * 1000);
        }

        /// <summary>
        /// Starts execution
        /// </summary>
        protected override void StartInternal()
        {
            _ = DelayAsync();
        }

        /// <summary>
        /// Called when skipped
        /// </summary>
        protected override void SkipInternal()
        {
            cts?.Cancel();
        }

        /// <summary>
        /// Performs the delay asynchronously
        /// </summary>
        private async Task DelayAsync()
        {
            try
            {
                if (isWillSkip)
                {
                    // Do nothing if it will be skipped immediately
                    return;
                }
                cts = new CancellationTokenSource();
                await Task.Delay(delayMilliseconds, cts.Token);
                Complete();
            }
            catch (OperationCanceledException)
            {
                // Skipped (normal behavior)
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in ChainDelay: {ex}");
                Complete();
            }
            finally
            {
                cts?.Dispose();
                cts = null;
            }
        }
    }
}
