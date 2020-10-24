using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    private GameObject hpBar;
    private ChampionInfo cInfo;
    private Image bar;

    private void Awake()
    {
        Transform tf = transform;
        while (tf)
        {
            cInfo = tf.GetComponent<ChampionInfo>();
            if (cInfo) break;
            tf = tf.parent;
        }

        bar = transform.Find("Gage").GetComponent<Image>();
    }

    public void Show()
    {
        if (cInfo)
        {
            float gage = cInfo.hp / (float)cInfo.maxHp;
            if (gage < 0) gage = 0;
            bar.rectTransform.localScale = new Vector2 (gage, 1f);
        }
    }
}
