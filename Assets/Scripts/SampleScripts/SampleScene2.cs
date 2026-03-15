
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
        PlayerInfo[] playerInfos;

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

            //testButtons.AddButton("FadeIn", () =>
            //{
            //    fadePanel.ChainFade(true).Start();
            //});
            //testButtons.AddButton("FadeOut", () =>
            //{
            //    fadePanel.ChainFade(false).Start();
            //});
            //testButtons.AddButton("Show", () =>
            //{
            //    resultDialog.UpdatePlayerNumber(5);
            //    //resultDialog.ChainRankingPlayers(true).Start();
            //    resultDialog.ChainShowDialog().Start();
            //});
            //testButtons.AddButton("SetPlayerInfos", () =>
            //{
            //    //resultDialog.SetPlayerInfos(playerInfos);
            //    //resultDialog.ChainRankingPlayers(true).Start();
            //});

            //testButtons.AddButton("AnimTest", () =>
            //{
            //    new ChainRace(
            //        new ChainButton(skipButton),                    
            //        new ChainSequence(                        
            //            new ChainAnimator(resultDialog.animator, "ResultDialogShowAnim"),
            //            new ChainAnimator(resultDialog.animator, "ResultDialogHideAnim")
            //        )
            //    ).Start();
            //});

            //testButtons.AddButton("All", () =>
            //{
            //    resultDialog.UpdatePlayerNumber(5);
            //    //new ChainSequence(
            //    //    new ChainRace(
            //    //        new ChainButton(skipButton),
            //    //        resultDialog.ChainShowDialog()
            //    //    ),
            //    //    new ChainRace(
            //    //        new ChainButton(skipButton),
            //    //        resultDialog.ChainHideDialog()
            //    //    ),
            //    //    new ChainNop()
            //    //).Start();
            //    new ChainSequence(
            //        new ChainRace(
            //            new ChainButton(skipButton),
            //            new ChainSequence(
            //                resultDialog.ChainShowDialog(),
            //                resultDialog.ChainHideDialog()
            //            )
            //        )
            //    ).Start();
            //});

            testButtons.AddButton("All", async () =>
            {
                resultDialog.SetPlayerInfos(playerInfos);
                await ChainResult().Start();
                //resultDialog.SetPlayerInfos(playerInfos);
                //resultDialog.ChainRankingPlayers(true).Start();
            });

            testButtons.AddButton("SetPanelInitialPosition", () =>
            {
                resultDialog.SetPanelInitialPosition();
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

        /// <summary>
        /// shows the result dialog with ranking player animation and fade in effect, 
        /// then hides the dialog with fade out effect. The skip button can be used to skip the animations and effects.
        /// </summary>
        public Chain ChainResult()
        {
            return new ChainSequence(
                new ChainAction(() =>
                {
                    fadePanel.gameObject.SetActive(true);
                    fadePanel.SetAlpha(1.0f);                    
                    resultDialog.SetPanelInitialPosition();
                }),
                new ChainDelay(0.5f),
                new ChainRace(
                    new ChainButton(skipButton),
                    new ChainParallel(
                        fadePanel.ChainFade(false),
                        new ChainSequence(
                            new ChainDelay(0.1f),
                            resultDialog.ChainShowDialog()
                       )
                    )
                ),
                new ChainRace(
                    new ChainButton(skipButton),
                    resultDialog.ChainShowBonus()
                ),
                new ChainRace(
                    new ChainButton(skipButton),
                    resultDialog.ChainHideDialog()
                ),
                fadePanel.ChainFade(true)
            );
        }
    }
}

