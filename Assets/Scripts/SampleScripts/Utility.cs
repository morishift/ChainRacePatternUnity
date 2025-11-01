using ChainPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    /// <summary>
    /// RectTransformのanchordedPositionを移動するTween
    /// </summary>
    public static Chain ChainMoveTween(RectTransform rect, Vector2 endpos, float duration)
    {
        AnimationCurve curve = null;
        Vector2 startpos = Vector2.zero;
        ChainWork work = new ChainWork();
        work.onStart += () =>
        {
            curve = AnimationCurve.Linear(Time.time, 0, Time.time + duration, 1);
            startpos = rect.anchoredPosition;
        };
        work.onUpdate += () => {
            float t = curve.Evaluate(Time.time);
            rect.anchoredPosition = Vector2.Lerp(startpos, endpos, t);
            if (t >= 1.0f)
            {
                work.End();
            }
        };
        work.onSkip += () =>
        {
            rect.anchoredPosition = endpos;
        };
        return work;
    }

    /// <summary>
    /// サウンドの再生
    /// </summary>
    public static Chain ChainPlaySound(SoundType soundtype)
    {
        return new ChainAction(_willSkip =>
        {
            if (!_willSkip)
            {
                // 直後に完了する場合は無視する
                // スキップされなかった場合のみ再生する
                if (SoundPlayer.Get() != null)
                {
                    SoundPlayer.Get().PlaySound(soundtype);
                }
            }
        });
    }
}


