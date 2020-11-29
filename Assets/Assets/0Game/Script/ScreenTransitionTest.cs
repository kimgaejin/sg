using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionTest : MonoBehaviour
{
    public Transform ScreenTransitionParent;
    public List<RectTransform> AllDiamonds;

    void Start()
    {
        // 16:9 비율 유지하기위해서 스케일
        float ScreenRatio = (float)Screen.width / Screen.height;
        float TargetScreenRatio = 16f / 9f;
        ScreenTransitionParent.localScale = new Vector3(ScreenRatio / TargetScreenRatio, 1f, 1f);

        StartCoroutine(FadeOut());
        IEnumerator FadeOut()
        {
            yield return null;
            foreach (RectTransform Diamond in AllDiamonds)
            {
                Diamond.localScale = Vector3.zero;
                Vector2 TopLeftRelativePosition = new Vector2(Diamond.anchoredPosition.x / Screen.width, -(Diamond.anchoredPosition.y / Screen.height) + 1f);
                float DelayAmount = TopLeftRelativePosition.x * 0.66f + TopLeftRelativePosition.y * 0.33f;
                Diamond.DOScale(1f, 0.6f).SetDelay(DelayAmount * 0.8f);
            }
        }
    }
}
