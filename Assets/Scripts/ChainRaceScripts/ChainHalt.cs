using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// 実行開始後停止するChainクラス
    /// </summary>
    public class ChainHalt: Chain
    {
        /// <summary>
        /// 開始
        /// </summary>
        protected override void StartInternal()
        {
            // Complete()しない
        }

        /// <summary>
        /// スキップ時
        /// </summary>
        protected override void SkipInternal()
        {
        }
    }
}
