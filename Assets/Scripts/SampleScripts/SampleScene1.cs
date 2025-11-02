
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChainPattern;

namespace Sample
{
    /// <summary>
    /// Main script for Sample Scene 1
    /// </summary>
    public class SampleScene1 : MonoBehaviour
    {
        [SerializeField]
        TestButtons testButtons;
        [SerializeField]
        RectTransform moveRect;
        [SerializeField]
        TMPro.TextMeshProUGUI moveRectText;

        Button startButton1;
        Button startButton2;
        Button skipButton;
        ChainSequence chainSequence;

        const int MoveWidth = 400;
        const int MoveLeft = -MoveWidth / 2;
        const int MoveWidth3 = MoveWidth / 3;

        readonly Vector2 Position1 = new Vector2(MoveLeft + MoveWidth3 * 0, 0.0f);
        readonly Vector2 Position2 = new Vector2(MoveLeft + MoveWidth3 * 1, 0.0f);
        readonly Vector2 Position3 = new Vector2(MoveLeft + MoveWidth3 * 2, 0.0f);
        readonly Vector2 Position4 = new Vector2(MoveLeft + MoveWidth3 * 3, 0.0f);

        void Start()
        {
            startButton1 = testButtons.AddButton("Start1", OnClickStartButton1);
            startButton2 = testButtons.AddButton("Start2", OnClickStartButton2);
            skipButton = testButtons.AddButton("Skip");
            skipButton.interactable = false;
        }

        /// <summary>
        /// Sample that skips to the end when the skip button is pressed
        /// </summary>
        private async void OnClickStartButton1()
        {
            chainSequence?.Skip();
            chainSequence = new ChainSequence(
                ChainSetStartButtonsEnabled(false),
                new ChainAction(() => moveRect.anchoredPosition = Position1),
                ChainSetMoveRectText("1"),
                new ChainRace(
                    new ChainButton(skipButton),
                    new ChainSequence(
                        Utility.ChainMoveTween(moveRect, Position2, 0.7f),
                        Utility.ChainPlaySound(SoundType.Pong1),
                        ChainSetMoveRectText("2"),
                        new ChainDelay(0.2f),
                        Utility.ChainMoveTween(moveRect, Position3, 0.7f),
                        Utility.ChainPlaySound(SoundType.Pong1),
                        ChainSetMoveRectText("3"),
                        new ChainDelay(0.2f),
                        Utility.ChainMoveTween(moveRect, Position4, 0.7f),
                        Utility.ChainPlaySound(SoundType.Pong1),
                        ChainSetMoveRectText("4"),
                        new ChainDelay(0.2f)
                    )
                ),
                ChainSetStartButtonsEnabled(true)
            );

            await chainSequence.Start();
        }

        /// <summary>
        /// Sample that skips each section when the skip button is pressed
        /// </summary>
        private async void OnClickStartButton2()
        {
            chainSequence?.Skip();
            chainSequence = new ChainSequence(
                ChainSetStartButtonsEnabled(false),
                new ChainAction(() => moveRect.anchoredPosition = Position1),
                ChainSetMoveRectText("1"),
                new ChainRace(
                    new ChainButton(skipButton),
                    new ChainSequence(
                        Utility.ChainMoveTween(moveRect, Position2, 0.7f),
                        Utility.ChainPlaySound(SoundType.Pong1),
                        ChainSetMoveRectText("2"),
                        new ChainDelay(0.2f)
                    )
                ),
                new ChainRace(
                    new ChainButton(skipButton),
                    new ChainSequence(
                        Utility.ChainMoveTween(moveRect, Position3, 0.7f),
                        Utility.ChainPlaySound(SoundType.Pong1),
                        ChainSetMoveRectText("3"),
                        new ChainDelay(0.2f)
                    )
                ),
                new ChainRace(
                    new ChainButton(skipButton),
                    new ChainSequence(
                        Utility.ChainMoveTween(moveRect, Position4, 0.7f),
                        Utility.ChainPlaySound(SoundType.Pong1),
                        ChainSetMoveRectText("4"),
                        new ChainDelay(0.2f)
                    )
                ),
                ChainSetStartButtonsEnabled(true)
            );
            await chainSequence.Start();
        }

        /// <summary>
        /// Enables or disables the start buttons
        /// </summary>
        private Chain ChainSetStartButtonsEnabled(bool enabed)
        {
            return new ChainAction(() =>
            {
                if (startButton1 != null)
                {
                    startButton1.interactable = enabed;
                }
                if (startButton2 != null)
                {
                    startButton2.interactable = enabed;
                }
            });
        }

        /// <summary>
        /// Sets the thext of the moving rectangle
        /// </summary>
        private Chain ChainSetMoveRectText(string text)
        {
            return new ChainAction(() =>
            {
                moveRectText.text = text;
            });
        }
    }
}

