using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    private Image bar;

    private void Awake()
    {
        bar = transform.Find("Gage").GetComponent<Image>();
    }
    
    public void Show(Vector3 position, float percentage)
    {
        transform.position = position + new Vector3 (0, 2, 0);
        bar.rectTransform.localScale = new Vector2 (percentage, 1f);
    }
}
