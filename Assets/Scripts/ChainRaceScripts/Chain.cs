using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.U2D;
using UnityEngine;

namespace ChainPattern
{
    public abstract class Chain
    {
        enum ChainState
        {
            Ready,
            Started,
            Skipped,
            Completed,
        }

        TaskCompletionSource<bool> currentTcs;
        ChainState chainState;
        Action onComplete;

        public Chain()
        {
            chainState = ChainState.Ready;
        }

        /// <summary>
        /// Chainの実行を開始
        /// </summary>
        public Task Start()
        {
            if (chainState != ChainState.Ready)
            {                
                Debug.Log("Chain alrestarted");
                return Task.FromResult(true);
            }           
            currentTcs = new TaskCompletionSource<bool>();
            chainState = ChainState.Started;
            StartInternal();
            return currentTcs.Task;
        }

        /// <summary>
        /// Chainをスキップして即座に完了
        /// </summary>
        public void Skip()
        {
            if (chainState != ChainState.Started)
            {
                return;
            }
            onComplete = null; // スキップ時は完了コールバックを呼ばない
            chainState = ChainState.Skipped;
            SkipInternal();
            currentTcs?.TrySetResult(true);
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
            if (chainState != ChainState.Started)
            {
                return;
            }
            chainState = ChainState.Completed;
            currentTcs?.TrySetResult(true);
            onComplete?.Invoke();
            onComplete = null;
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

