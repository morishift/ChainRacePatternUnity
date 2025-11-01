using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// 実行開始後即座に完了するChainクラス
    /// </summary>
    public class ChainNop : Chain
    {
        /// <summary>
        /// 開始
        /// </summary>
        protected override void StartInternal()
        {
            Complete();
        }

        /// <summary>
        /// スキップ時
        /// </summary>
        protected override void SkipInternal()
        {
        }
    }
}
