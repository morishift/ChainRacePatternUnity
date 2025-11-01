using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.UI;

namespace ChainPattern
{
    /// <summary>
    /// ボタンが押されたら終了するChain
    /// </summary>
    public class ChainButton : Chain
    {
        Button targetButton;

        public ChainButton(Button button)
        {
            targetButton = button;
        }

        /// <summary>
        /// 開始
        /// </summary>
        protected override void StartInternal()
        {
            if (targetButton == null)
            {
                Complete();
                return;
            }
            targetButton.interactable = true;
            targetButton.onClick.AddListener(OnClickButton);
        }

        /// <summary>
        /// スキップ時
        /// </summary>
        protected override void SkipInternal()
        {
            if (targetButton != null)
            {
                targetButton.onClick.RemoveListener(OnClickButton);
            }
            targetButton.interactable = false;
        }

        /// <summary>
        /// ボタンをクリックした
        /// </summary>
        private void OnClickButton()
        {
            if (targetButton != null)
            {
                targetButton.onClick.RemoveListener(OnClickButton);
            }
            targetButton.interactable = false;
            Complete();
        }
    }
}
