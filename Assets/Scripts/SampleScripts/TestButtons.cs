using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButtons : MonoBehaviour
{
    [SerializeField]
    Button srcButton;

    private void Awake()
    {
        srcButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// ƒ{ƒ^ƒ“‚ð’Ç‰Á‚·‚é
    /// </summary>
    public Button AddButton(string caption, Action onclick)
    {
        GameObject go = Instantiate<GameObject>(srcButton.gameObject, srcButton.transform.parent);
        go.SetActive(true);
        Button button = go.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            onclick?.Invoke();
        });
        TMPro.TextMeshProUGUI text = go.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = caption;
        return button;
    }
}
