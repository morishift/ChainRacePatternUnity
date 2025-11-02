using ChainPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample
{
    public static class Utility
    {
        /// <summary>
        /// Creates a tween chain that animates RectTransform's anchoredPosition
        /// </summary>
        public static Chain ChainMoveTween(RectTransform rect, Vector2 endPosition, float duration)
        {
            AnimationCurve curve = null;
            Vector2 startPosition = Vector2.zero;
            ChainWork work = new ChainWork();
            work.onStart += () =>
            {
                curve = AnimationCurve.Linear(Time.time, 0, Time.time + duration, 1);
                startPosition = rect.anchoredPosition;
            };
            work.onUpdate += () =>
            {
                float t = curve.Evaluate(Time.time);
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
        /// Creates a chain that plays a sound
        /// </summary>
        public static Chain ChainPlaySound(SoundType soundType)
        {
            return new ChainAction(willSkip =>
            {
                if (!willSkip)
                {
                    // Only play if not skipped immediately
                    if (SoundPlayer.Get() != null)
                    {
                        SoundPlayer.Get().PlaySound(soundType);
                    }
                }
            });
        }
    }
}

