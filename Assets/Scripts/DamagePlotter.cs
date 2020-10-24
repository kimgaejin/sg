using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePlotter : MonoBehaviour
{
    public GameObject damageTextPrefab;

    private List<GameObject> damageEffects;

    private Vector2 incVec2UpPos = new Vector2(0, 5);

    private void Awake()
    {
        damageEffects = new List<GameObject>();
        foreach (Transform tf in transform)
        {
            damageEffects.Add(tf.gameObject);
            tf.gameObject.SetActive(false);
        }
    }

    public void ShowDamage(Vector2 pos, int value)
    {
        StartCoroutine(Show(pos, value));
    }

    IEnumerator Show(Vector2 pos, int value)
    {
        WaitForSeconds wait01 = new WaitForSeconds(0.1f);

        GameObject target = null;
        foreach (Transform tf in transform)
        {
            if (tf.gameObject.activeSelf == false)
            {
                target = tf.gameObject;
                break;
            }
        }
        if (target == null)
        {
            target = Instantiate(damageTextPrefab);
            target.transform.parent = transform;
        }
        target.SetActive(true);

        RectTransform targetRect = target.GetComponent<RectTransform>();
        targetRect.anchoredPosition = pos;
        target.GetComponent<Text>().text = value.ToString();

        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return wait01;
                targetRect.anchoredPosition += incVec2UpPos;
            }
            break;
        }
        target.SetActive(false);

        yield break;
    }
}
