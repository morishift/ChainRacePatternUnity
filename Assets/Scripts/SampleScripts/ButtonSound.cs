using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    /// <summary>
    /// plays a sound when a button is clicked
    /// </summary>
    public class ButtonSound : MonoBehaviour
    {
        [SerializeField]
        SoundType soundType;

        Button button;
        
        void Start()
        {
            button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnClickButton);
            }
        }

        /// <summary>
        /// plays a sound when the button is clicked
        /// </summary>
        private void OnClickButton()
        {
            if (SoundPlayer.Get() != null)
            {
                SoundPlayer.Get().PlaySound(soundType);
            }        
        }
    }
}
