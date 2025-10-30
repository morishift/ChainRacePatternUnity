using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ChainPattern
{
    public abstract class Chain
    {
        TaskCompletionSource<bool> currentTcs;
        bool isStarted;
        bool isCompleted;
        Action onComplete;

        /// <summary>
        /// Chainの実行を開始
        /// </summary>
        public Task Start()
        {
            if (isStarted)
            {                
                Debug.Log("Chain already started");
                return Task.FromResult(true);
            }

            isStarted = true;
            isCompleted = false;
            currentTcs = new TaskCompletionSource<bool>();
            StartInternal();
            return currentTcs.Task;
        }

        /// <summary>
        /// Chainをスキップして即座に完了
        /// </summary>
        public void Skip()
        {
            if (!isStarted || isCompleted)
            {
                return;
            }
            onComplete = null; // スキップ時は完了コールバックを呼ばない
            SkipInternal();
            Complete();
        }

        /// <summary>
        /// 完了コールバックを設定
        /// </summary>
        public void SetCompleteCallback(Action callback)
        {
            onComplete = callback;
        }

        /// <summary>
        /// Chainを完了状態にする。派生クラスから呼ぶ
        /// </summary>
        protected void Complete()
        {
            if (isCompleted)
            {
                return;
            }
            isCompleted = true;
            currentTcs?.TrySetResult(true);
            onComplete?.Invoke();
        }

        /// <summary>
        /// 実際の開始処理。派生クラスで実装
        /// </summary>
        protected abstract void StartInternal();

        /// <summary>
        /// 実際のスキップ処理。派生クラスで実装
        /// </summary>
        protected abstract void SkipInternal();
    }
}

