using ChainPattern;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
        readonly Vector2 rankingPlayerOffset = new Vector2(0.0f, -400.0f);
        
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
                r.GetComponent<RectTransform>().anchoredPosition += rankingPlayerOffset;
            }
            rankingVerticalLayoutGroup.enabled = false;           
        }

        /// <summary>
        /// Create a chain to display RankingPlayers
        /// </summary>
        public Chain ChainShowRankingPlayers()
        {
            var parallel = new ChainParallel();
            var curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
            for (int i = 0; i < rankingPlayers.Count; ++i)
            {
                parallel.Add(new ChainSequence(
                    new ChainDelay(0.25f * i),
                    Utility.ChainMoveTween(rankingPlayers[i].GetComponent<RectTransform>(), rankingPlayerAnchoredPositions[i], 0.7f, curve)
                ));
            }
            return parallel;
        }
    }
}

