using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// 一定時間待機するChain
    /// </summary>
    public class ChainDelay : Chain
    {
        int delayMilliseconds;
        CancellationTokenSource cts;

        /// <summary>
        /// 秒指定
        /// </summary>
        public ChainDelay(float seconds)
        {
            delayMilliseconds = (int)(seconds * 1000);
        }

        /// <summary>
        /// 開始
        /// </summary>
        protected override void StartInternal()
        {
            _ = DelayAsync();
        }

        /// <summary>
        /// スキップ
        /// </summary>
        protected override void SkipInternal()
        {
            cts?.Cancel();
        }

        /// <summary>
        /// 遅延処理
        /// </summary>
        private async Task DelayAsync()
        {
            try
            {
                if (isWillSkip)
                {
                    // 直後にSkipする場合はなにもしない
                    return;
                }
                cts = new CancellationTokenSource();
                await Task.Delay(delayMilliseconds, cts.Token);
                Complete();
            }
            catch (OperationCanceledException)
            {
                // Skip された (正常)
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
