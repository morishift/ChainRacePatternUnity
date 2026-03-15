using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ChainPattern;

namespace Sample
{
    /// <summary>
    /// A class representing a player in the ranking
    /// </summary>
    public class RankingPlayer : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI textName;
        [SerializeField]
        TextMeshProUGUI textPoint;
        int playerPoint = 0;

        private void Awake()
        {
            textPoint.text = "0pt";
        }

        /// <summary>
        /// set player name to display
        /// </summary>
        public void SetPlayerName(string playerName)
        { 
            textName.text = playerName;
        }

        /// <summary>
        /// Creates a chain that animates the point text area from start to end
        /// </summary>
        public Chain ChainPointAnimation(int end)
        {
            int start = playerPoint;
            playerPoint = end;
            return Utility.ChainTextUpdate(textPoint, "{0}pt", start, end, 0.5f);
        }
    }
}
