
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChainPattern;

namespace Sample
{
    /// <summary>
    /// Main script for Sample Scene 2
    /// </summary>
    public class SampleScene2 : MonoBehaviour
    {
        [SerializeField]
        TestButtons testButtons;
        [SerializeField]
        FadePanel fadePanel;
        [SerializeField]
        ResultDialog resultDialog;

        List<Button> sequenceButtons = new List<Button>();
        Button skipButton;
        ChainSequence chainSequence;
        

        void Start()
        {
            //sequenceButtons.Add(testButtons.AddButton("Sequence1", OnClickSequence1Button));
            //sequenceButtons.Add(testButtons.AddButton("Sequence2", OnClickSequence2Button));
            //sequenceButtons.Add(testButtons.AddButton("Sequence3", OnClickSequence3Button));

            testButtons.AddButton("FadeIn", () =>
            {
                fadePanel.ChainFade(true).Start();
            });
            testButtons.AddButton("FadeOut", () =>
            {
                fadePanel.ChainFade(false).Start();
            });
            testButtons.AddButton("Set player number", () =>
            {
                resultDialog.UpdatePlayerNumber(5);
                resultDialog.ChainShowRankingPlayers().Start();
            });
            testButtons.AddButton("ChainPointAnimation ", () =>
            {
                resultDialog.rankingPlayers[0].ChainPointAnimation(100).Start();
            });
            skipButton = testButtons.AddButton("Skip");
            skipButton.interactable = false;
        }


        /// <summary>
        /// Enables or disables the sequence buttons        
        /// </summary>
        private Chain ChainSetButtonsEnabled(bool enabed)
        {
            return new ChainAction(() =>
            {
                foreach (Button button in sequenceButtons)
                {
                    button.interactable = enabed;
                }
            });
        }

    }
}



