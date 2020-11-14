using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Vector2 gageOriginSize;
    private SpriteRenderer gageSpr;

    private void Awake()
    {
        gageSpr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        gageOriginSize = gageSpr.size;
        SetGage(0.4f);
   
    }

    public void SetGage(float percentage)
    {
        if (percentage < 0) percentage = 0;
        gageSpr.size = gageOriginSize * percentage;
    }
}
