using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChainPattern;

public class SampleScene1 : MonoBehaviour
{
    [SerializeField]
    TestButtons testButtons;
    [SerializeField]
    RectTransform moveRect;
    [SerializeField]
    TMPro.TextMeshProUGUI moveText;

    Button startButton1;
    Button startButton2;
    Button skipButton;
    ChainSequence mainChain;

    const int MoveWidth = 400;
    const int MoveLeft = -MoveWidth / 2;
    const int MoveWidth3 = 400 / 3;

    readonly Vector2 Position1 = new Vector2(MoveLeft + MoveWidth3 * 0, 0.0f);
    readonly Vector2 Position2 = new Vector2(MoveLeft + MoveWidth3 * 1, 0.0f);
    readonly Vector2 Position3 = new Vector2(MoveLeft + MoveWidth3 * 2, 0.0f);
    readonly Vector2 Position4 = new Vector2(MoveLeft + MoveWidth3 * 3, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        startButton1 = testButtons.AddButton("Start1", OnClickStartButton1);
        startButton2 = testButtons.AddButton("Start2", OnClickStartButton2);
        skipButton = testButtons.AddButton("Skip", null);
        skipButton.interactable = false;
    }

    private async void OnClickStartButton1()
    {
        mainChain?.Skip();
        mainChain = new ChainSequence(
            ChainStartButtonEnabled(false),
            new ChainAction(() => moveRect.anchoredPosition = Position1),
            ChainSetMoveText("1"),
            new ChainRace(
                new ChainButton(skipButton),
                new ChainSequence(
                    Utility.ChainMoveTween(moveRect, Position2, 0.7f),
                    Utility.ChainPlaySound(SoundType.Pong1),
                    ChainSetMoveText("2"),
                    new ChainDelay(0.2f),
                    Utility.ChainMoveTween(moveRect, Position3, 0.7f),
                    Utility.ChainPlaySound(SoundType.Pong2),
                    ChainSetMoveText("3"),
                    new ChainDelay(0.2f),
                    Utility.ChainMoveTween(moveRect, Position4, 0.7f),
                    Utility.ChainPlaySound(SoundType.Pong3),
                    ChainSetMoveText("4"),
                    new ChainDelay(0.2f)
                )
            ),
            ChainStartButtonEnabled(true)
        );
        
        await mainChain.Start();
    }

    private async void OnClickStartButton2()
    {
        mainChain?.Skip();
        mainChain = new ChainSequence(
            ChainStartButtonEnabled(false),
            new ChainAction(() => moveRect.anchoredPosition = Position1),
            ChainSetMoveText("1"),
            new ChainRace(
                new ChainButton(skipButton),
                new ChainSequence(
                    Utility.ChainMoveTween(moveRect, Position2, 0.7f),
                    Utility.ChainPlaySound(SoundType.Pong1),
                    ChainSetMoveText("2"),
                    new ChainDelay(0.2f)
                )
            ),
            new ChainRace(
                new ChainButton(skipButton),
                new ChainSequence(
                    Utility.ChainMoveTween(moveRect, Position3, 0.7f),
                    Utility.ChainPlaySound(SoundType.Pong2),
                    ChainSetMoveText("3"),
                    new ChainDelay(0.2f)
                )
            ),
            new ChainRace(
                new ChainButton(skipButton),
                new ChainSequence(
                    Utility.ChainMoveTween(moveRect, Position4, 0.7f),
                    Utility.ChainPlaySound(SoundType.Pong3),
                    ChainSetMoveText("4"),
                    new ChainDelay(0.2f)                
                )
            ),
            ChainStartButtonEnabled(true)
        );            
        await mainChain.Start();
    }

    private Chain ChainStartButtonEnabled(bool flg)
    { 
        return new ChainAction(() => 
        {
            if (startButton1 != null)
            {
                startButton1.interactable = flg;
            }
            if (startButton2 != null)
            {
                startButton2.interactable = flg;
            }
        });
    }


    /// <summary>
    /// 移動するテキストの変更
    /// </summary>
    private Chain ChainSetMoveText(string str)
    {
        return new ChainAction(() => {
            moveText.text = str;
        });
    }

    void OnDestroy()
    {
        mainChain?.Skip();
    }

}
