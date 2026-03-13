using ChainPattern;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Sample
{
    public static class Utility
    {
        /// <summary>
        /// Creates a tween chain that animates RectTransform's anchoredPosition using the specified curve
        /// </summary>
        public static Chain ChainMoveTween(RectTransform rect, Vector2 endPosition, float duration, AnimationCurve curve)
        {            
            Vector2 startPosition = Vector2.zero;
            ChainWork work = new ChainWork();
            float startTime = 0.0f;
            duration = Mathf.Max(duration, 0.01f);
            work.onStart += () =>
            {                
                startPosition = rect.anchoredPosition;
                startTime = Time.time;
            };
            work.onUpdate += () =>
            {
                float t = curve.Evaluate((Time.time - startTime) / duration);
                rect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
                if (t >= 1.0f)
                {
                    work.End();
                }
            };
            work.onSkip += () =>
            {
                rect.anchoredPosition = endPosition;
            };
            return work;
        }

        /// <summary>
        /// Creates a tween chain that animates RectTransform's anchoredPosition
        /// </summary>
        public static Chain ChainMoveTween(RectTransform rect, Vector2 endPosition, float duration)
        {
            return ChainMoveTween(rect, endPosition, duration, AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f));
        }

        /// <summary>
        /// Creates a chain that counts a numeric text from start to end over the specified duration using the given format
        /// </summary>
        public static Chain ChainTextUpdate(TextMeshProUGUI ugui, string format, int start, int end, float duration)
        {
            ChainWork work = new ChainWork();
            float startTime = 0.0f;
            duration = Mathf.Max(duration, 0.01f);
            work.onStart += () =>
            {
                startTime = Time.time;                
            };
            work.onUpdate += () =>
            {
                float t = (Time.time - startTime) / duration;
                int n = (int)Mathf.Lerp(start, end, t);
                if (t >= 1.0f)
                {
                    ugui.text = string.Format(format, n);
                    work.End();
                }
                else
                {
                    ugui.text = string.Format(format, n);
                }
            };
            work.onSkip += () =>
            {
                ugui.text = string.Format(format, end);
            };
            return work;
        }

        /// <summary>
        /// Creates a chain that plays a sound
        /// </summary>
        public static Chain ChainPlaySound(SoundType soundType)
        {
            return new ChainAction(fastForward =>
            {
                if (!fastForward)
                {
                    // Only play if not skipped immediately
                    if (SoundPlayer.Get() != null)
                    {
                        SoundPlayer.Get().PlaySound(soundType);
                    }
                }
            });
        }

        /// <summary>
        /// Creates a chain that animates alpha of a graphic 
        /// </summary>
        public static Chain ChainAlphaAnimation(Graphic graphic, float alphaEnd, float duration)
        { 
            ChainWork work = new ChainWork();
            float alphaStart = 0.0f;
            AnimationCurve curve = null;
            work.onStart += () =>
            {
                alphaStart = graphic.color.a;
                curve = AnimationCurve.Linear(Time.time, 0, Time.time + duration, 1);
            };
            work.onUpdate += () =>
            {
                float t = curve.Evaluate(Time.time);
                Color c = graphic.color;
                c.a = Mathf.Lerp(alphaStart, alphaEnd, t);
                graphic.color = c;
                if (t >= 1.0f)
                {
                    work.End();
                }
            };
            work.onSkip += () =>
            {
                Color c = graphic.color;
                c.a = alphaEnd;
                graphic.color = c;
            };
            return work;
        }
    }
}

