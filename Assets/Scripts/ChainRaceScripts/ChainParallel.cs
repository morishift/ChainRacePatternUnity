using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ChainPattern
{
    /// <summary>
    /// Chainの並列実行
    /// </summary>
    public class ChainParallel : Chain
    {
        List<Chain> chainList = new List<Chain>();
        List<Chain> startedChainList = new List<Chain>();
        bool enableFlg;
        bool startingFlg;
        bool startedFlg;
        bool consumeFlg;

        public ChainParallel(params Chain[] chains)
        {
            enableFlg = true;
            chainList.AddRange(chains);
        }

        /// <summary>
        /// 追加
        /// </summary>        
        public ChainParallel Add(Chain chain)
        {
            if (!enableFlg)
            {
                // 無視する
            }
            else if (startingFlg || consumeFlg)
            {
                chainList.Add(chain);
            }
            else if (startedFlg)
            {
                startedChainList.Add(chain);
                chain.SetCompleteCallback(() => OnChainComplete(chain));
                chain.SetIsWillSkip(isWillSkip);
                chain.Start();
            }
            else
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
            startingFlg = true;
            while (chainList.Count > 0 && enableFlg)
            {
                Chain c = chainList[0];
                chainList.RemoveAt(0);
                startedChainList.Add(c);
                c.SetCompleteCallback(() => OnChainComplete(c));
                c.SetIsWillSkip(isWillSkip);                
                c.Start();
            }
            if (enableFlg)
            {
                startingFlg = false;
                startedFlg = true;
            }
        }

        /// <summary>
        /// スキップ
        /// </summary>
        protected override void SkipInternal()
        {
            consumeFlg = true;
            ConsumeStartedChainListAndChainList();
            enableFlg = false;
            startingFlg = false;
            startedFlg = false;
            consumeFlg = false;
        }

        /// <summary>
        /// Chainの完了コールバック
        /// </summary>        
        private void OnChainComplete(Chain chain)
        {
            if (startedChainList.Contains(chain))
            {
                startedChainList.Remove(chain);
                if (chainList.Count <= 0 && startedChainList.Count <= 0)
                {
                    enableFlg = false;
                    startingFlg = false;
                    startedFlg = false;
                    consumeFlg = false;
                    Complete();
                }
            }
        }

        /// <summary>
        /// 開始済みChainと未開始Chainの消化
        /// </summary>
        private void ConsumeStartedChainListAndChainList()
        {
            while (startedChainList.Count > 0)
            {
                Chain c = startedChainList[0];
                startedChainList.RemoveAt(0);
                c.Skip();
            }
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
        }
    }
}
