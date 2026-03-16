using ChainPattern;
using UnityEngine;
using UnityEngine.UI;

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
        ResultDialog resultDialog;
        [SerializeField]
        GameObject touchScreen;
        [SerializeField]
        Button screenButton;

        [SerializeField]
        FadePanel fadePanel;
        [SerializeField]
        TestButtons testButtons;

        Button startButton;

        void Start()
        {
            startButton = testButtons.AddButton("Start", async () =>
            {
                resultDialog.SetPlayerInfos(playerInfos);
                Chain chainResult = ChainResult();
                await new ChainSequence(
                    new ChainAction(() => startButton.interactable = false),
                    chainResult,
                    new ChainAction(() => startButton.interactable = true)
                ).Start();
            });
            touchScreen.SetActive(false);
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
                    new ChainSequence(
                        new ChainDelay(0.1f), // Prevent skipping immediately after the start
                        new ChainButton(screenButton),
                        Utility.ChainPlaySound(SoundType.Pong4) // Button press sound effect
                    ),
                    new ChainParallel(
                        fadePanel.ChainFade(false),
                        new ChainSequence(
                            new ChainDelay(0.1f),
                            resultDialog.ChainShowDialog()
                       )
                    )
                ),
                new ChainRace(
                    new ChainSequence(
                        new ChainButton(screenButton),
                        Utility.ChainPlaySound(SoundType.Pong4)
                    ),
                    resultDialog.ChainShowBonus()
                ),
                ChainTouchScreen(),
                new ChainParallel(
                    resultDialog.ChainHideDialog(),
                    new ChainSequence(
                        new ChainDelay(0.3f + playerInfos.Length * 0.1f),
                        fadePanel.ChainFade(true)
                    )
                )
            );
        }

        /// <summary>
        /// show the touch screen and wait until the screen button is pressed, then hide the touch screen
        /// </summary>
        private Chain ChainTouchScreen()
        {
            return new ChainSequence(
                new ChainAction(() =>
                {
                    touchScreen.SetActive(true);
                }),
                new ChainButton(screenButton),
                Utility.ChainPlaySound(SoundType.Pong4),
                new ChainAction(() =>
                {
                    touchScreen.SetActive(false);
                })
            );
        }
    }
}

