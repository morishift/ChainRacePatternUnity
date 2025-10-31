using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChainPattern;

public class SampleScene1 : MonoBehaviour
{
    [SerializeField]
    Button startButton;
    [SerializeField]
    Button skipButton;

    Chain mainChain;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(OnClickStartButton);
    }

    private async void OnClickStartButton()
    {
        Debug.Log("=======Start");

        mainChain = new ChainRace(
            new ChainButton(skipButton),            
            //new ChainDelay(1.5f),
            new ChainRace(
                new ChainDelay(1.0f),
                new ChainAction(() => Debug.Log("Delay 1.0f")),
                new ChainDelay(1.0f),
                new ChainAction(() => Debug.Log("Delay 2.0f")),
                new ChainDelay(1.0f),
                new ChainAction(() => Debug.Log("Delay 1.0f")),
                new ChainDelay(1.0f),
                new ChainAction(() => Debug.Log("Delay 2.0f")),
                new ChainDelay(1.0f),
                new ChainAction((willSkip) => Debug.Log($"Delay 1.0f willSkip:{willSkip}")),
                new ChainDelay(1.0f),
                new ChainAction(() => Debug.Log("Delay 2.0f"))
            )
        );
        await mainChain.Start();
        Debug.Log("=======END");
    }

    // Update is called once per frame
    void OnDestroy()
    {
        mainChain.Skip();
    }
}
