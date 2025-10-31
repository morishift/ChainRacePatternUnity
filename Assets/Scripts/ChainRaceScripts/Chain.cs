using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.U2D;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chainクラスの基底クラス
    /// </summary>
    public abstract class Chain
    {
        enum State
        {
            Ready,
            Started,
            Skipped,
            Completed,
        }

        TaskCompletionSource<bool> currentTcs;
        State state;        
        Action onComplete;

        public Chain()
        {
            state = State.Ready;
        }

        /// <summary>
        /// Chainの実行を開始
        /// </summary>
        public Task Start()
        {
            if (state != State.Ready)
            {                
                Debug.Log("Chain alrestarted");
                return Task.FromResult(true);
            }           
            currentTcs = new TaskCompletionSource<bool>();
            state = State.Started;
            StartInternal();
            return currentTcs.Task;
        }

        /// <summary>
        /// Chainをスキップして即座に完了
        /// </summary>
        public void Skip()
        {
            if (state != State.Started)
            {
                return;
            }
            state = State.Skipped;
            onComplete = null; // スキップ時は完了コールバックを呼ばない
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
        /// スタート直後にSkipされるかどうかを設定
        /// </summary>
        public void SetIsWillSkip(bool willskip)
        {
            isWillSkip = willskip;
        }

        /// <summary>
        /// Chainを完了状態にする。派生クラスから呼ぶ
        /// </summary>
        protected void Complete()
        {
            if (state != State.Started)
            {
                return;
            }
            state = State.Completed;            
            currentTcs?.TrySetResult(true);
            onComplete?.Invoke();
            onComplete = null;
        }

        /// <summary>
        /// Start直後にSkipされるかどうか
        /// </summary>
        protected bool isWillSkip
        {
            get;
            private set;
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

