using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChainPattern;

namespace Sample
{
    /// <summary>
    /// a panel that fades in and out
    /// </summary>
    public class FadePanel : MonoBehaviour
    {
        [SerializeField]
        Image image;

        /// <summary>
        /// Creates a chain that fades in and out
        /// the fade-in changes the alpha value from 0 to1
        /// </summary>
        public Chain ChainFade(bool fadeIn)
        {
            if (fadeIn)
            {
                return new ChainSequence(
                    new ChainAction(() =>
                    {
                        gameObject.SetActive(true);
                        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
                    }),
                    Utility.ChainAlphaAnimation(image, 1.0f, 0.5f)
                );
            }
            else
            {
                return new ChainSequence(
                    new ChainAction(() =>
                    {
                        gameObject.SetActive(true);
                        image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
                    }),
                    Utility.ChainAlphaAnimation(image, 0.0f, 0.5f),
                    new ChainAction(() =>
                    {
                        gameObject.SetActive(false);
                    })
                );
            }
        }

    }
}
