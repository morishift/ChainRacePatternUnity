using ChainPattern;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Sample
{
    /// <summary>
    /// a dialog that shows results
    /// </summary>
    public class ResultDialog : MonoBehaviour
    {
        /// <summary>
        /// VerticalLayoutGroup that holds ranking entries
        /// </summary>
        [SerializeField]
        VerticalLayoutGroup rankingVerticalLayoutGroup;
        /// <summary>
        /// RankingPlayer that serves as a source for instantiation
        /// </summary>
        [SerializeField]
        RankingPlayer sourceRankingPlayer;
        /// <summary>
        /// Animator for animating the dialog
        /// </summary>
        [SerializeField]
        public Animator animator;

        /// <summary>
        /// Instances of RankingPlayer
        /// </summary>
        public List<RankingPlayer> rankingPlayers = new List<RankingPlayer>();
        /// <summary>
        /// Initial position of the instantiated RankingPlayer
        /// </summary>
        List<Vector2> rankingPlayerAnchoredPositions = new List<Vector2>();
        /// <summary>
        /// Offset amount for the starting position when the RankingPlayer is displayed
        /// </summary>
        readonly Vector2 rankingPlayerOffset1 = new Vector2(0.0f, -600.0f);
        readonly Vector2 rankingPlayerOffset2 = new Vector2(0.0f, 600.0f);

        private void Awake()
        {
            sourceRankingPlayer.gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the number of players disdplayed in the ranking area
        /// </summary>
        public void UpdatePlayerNumber(int number)
        {
            // Destroy old RankingPlayer instances
            rankingVerticalLayoutGroup.enabled = true;
            foreach (RankingPlayer player in rankingPlayers)
            {
                player.gameObject.SetActive(false);
                Destroy(player.gameObject);
            }
            rankingPlayers.Clear();
            rankingPlayerAnchoredPositions.Clear();

            // Create the specified number of RankingPlayer instances
            for (int i = 0; i < number; ++i)
            {
                var go = Instantiate<GameObject>(sourceRankingPlayer.gameObject, rankingVerticalLayoutGroup.transform);
                go.gameObject.SetActive(true);
                rankingPlayers.Add(go.GetComponent<RankingPlayer>());
            }

            // Force update the layout to get the correct positions
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rankingVerticalLayoutGroup.GetComponent<RectTransform>());

            // Save positions
            foreach (RankingPlayer r in rankingPlayers)
            {
                Vector2 position = r.GetComponent<RectTransform>().anchoredPosition;
                Debug.Log($"{rankingPlayerAnchoredPositions.Count}:({position.x}, {position.y})");
                rankingPlayerAnchoredPositions.Add(r.GetComponent<RectTransform>().anchoredPosition);
                // add offset
                r.GetComponent<RectTransform>().anchoredPosition += rankingPlayerOffset1;
            }
            rankingVerticalLayoutGroup.enabled = false;
        }

        /// <summary>
        /// Show the dialog with animation and displaying RankingPlayers
        /// </summary>
        public Chain ChainShowDialog()
        {
            return new ChainParallel(
                ChainRankingPlayers(true),
                new ChainAnimator(animator, "ResultDialogShowAnim")
            );
        }

        /// <summary>
        /// Show the dialog with animation and displaying RankingPlayers
        /// </summary>
        public Chain ChainHideDialog()
        {
            return new ChainParallel(
                ChainRankingPlayers(false),
                new ChainAnimator(animator, "ResultDialogHideAnim")
            );
        }

        /// <summary>
        /// Create a chain to display RankingPlayers
        /// </summary>
        public Chain ChainRankingPlayers(bool show)
        {
            var parallel = new ChainParallel();
            var curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
            for (int i = 0; i < rankingPlayers.Count; ++i)
            {
                RectTransform rectTransform = rankingPlayers[i].GetComponent<RectTransform>();
                Vector2 endPosition = show ? rankingPlayerAnchoredPositions[i] : rankingPlayerAnchoredPositions[i] + rankingPlayerOffset2;
                parallel.Add(new ChainSequence(
                    new ChainDelay(0.25f * i),
                    Utility.ChainMoveTween(rectTransform, endPosition, 0.7f, curve)
                ));
            }
            return parallel;
        }
    }
}

