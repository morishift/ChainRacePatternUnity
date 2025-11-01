using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// 一つの関数を実行するChain
    /// </summary>
    public class ChainWork : Chain
    {
        /// <summary>
        /// 開始時に呼び出されるイベント
        /// </summary>
        public event Action onStart;
        /// <summary>
        /// スキップ時に呼び出されるイベント
        /// </summary>
        public event Action onSkip;
        /// <summary>
        /// 毎フレーム呼び出されるイベント
        /// </summary>
        public event Action onUpdate;

        bool startedFlg;

        public ChainWork()
        {            
        }

        /// <summary>
        /// workがスキップされるかどうか
        /// </summary>
        public bool isWorkWillSkip => isWillSkip;

        /// <summary>
        /// Workの終了
        /// </summary>
        public void End()
        {
            if (startedFlg)
            {
                startedFlg = false;
                CustomUpdateComponent.RemoveUpdateListener(OnCustomUpdateComponent);
                Complete();
            }
        }

        /// <summary>
        /// 開始
        /// </summary>
        protected override void StartInternal()
        {
            startedFlg = true;
            CustomUpdateComponent.AddUpdateListener(OnCustomUpdateComponent);
            onStart?.Invoke();
        }

        /// <summary>
        /// スキップ時
        /// </summary>
        protected override void SkipInternal()
        {
            startedFlg = false;
            CustomUpdateComponent.RemoveUpdateListener(OnCustomUpdateComponent);
            onSkip?.Invoke();
        }

        /// <summary>
        /// ChainWorkの開始後毎フレーム呼ばれる
        /// </summary>
        private void OnCustomUpdateComponent()
        {
            onUpdate?.Invoke();
        }
    }
}



