using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chainの順次実行
    /// </summary>
    public class ChainSequence : Chain
    {
        List<Chain> chainList = new List<Chain>();
        Chain currentChain;
        bool enableFlg;        

        public ChainSequence(params Chain[] chains)
        {
            enableFlg = true;
            chainList.AddRange(chains);
        }

        /// <summary>
        /// 追加
        /// </summary>        
        public ChainSequence Add(Chain chain)
        {
            if (enableFlg)
            {
                chainList.Add(chain);
            }
            return this;
        }

        /// <summary>
        /// 開始
        /// </summary>
        protected override void StartInternal()
        { 
            NextChain();
        }

        /// <summary>
        /// スキップ
        /// </summary>
        protected override void SkipInternal()
        {
            currentChain.Skip();
            currentChain = null;
            while (chainList.Count > 0)
            {
                Chain c = chainList[0];
                chainList.RemoveAt(0);
                bool end = false;
                c.SetCompleteCallback(() => end = true);
                c.SetIsWillSkip(true);
                c.Start();
                if (!end)
                {
                    c.Skip();
                }
            }
            enableFlg = false;
        }

        /// <summary>
        /// 次のChainを実行
        /// </summary>
        private void NextChain()
        {
            if (chainList.Count <= 0)
            {
                enableFlg = false;
                Complete();
                return;
            }
            currentChain = chainList[0];
            chainList.RemoveAt(0);
            currentChain.SetIsWillSkip(isWillSkip);
            currentChain.SetCompleteCallback(() =>
            {
                currentChain = null;
                NextChain();
            });
            currentChain.Start();
        }
    }
}


