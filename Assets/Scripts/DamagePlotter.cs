using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePlotter : MonoBehaviour
{
    public GameObject damageTextPrefab;

    private List<GameObject> damageEffects;

    private Vector2 seqVec2UpPos = new Vector2(0, 30);
    private Vector2 incVec2UpPos = new Vector2(0, 3);

    private void Awake()
    {
        damageEffects = new List<GameObject>();
        foreach (Transform tf in transform)
        {
            damageEffects.Add(tf.gameObject);
            tf.gameObject.SetActive(false);
        }
    }

    public void ShowDamage(Vector3 pos, int value, int sequence)
    {
        StartCoroutine(Show(pos, value.ToString(), sequence));
    }

    public void ShowDamage(Vector3 pos, string value, int sequence)
    {
        StartCoroutine(Show(pos, value, sequence));
    }

    IEnumerator Show(Vector3 pos, string value, int sequence)
    {
        WaitForSeconds wait01 = new WaitForSeconds(0.1f);
        WaitForSeconds wait005 = new WaitForSeconds(0.05f);

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
            target.transform.SetParent(transform);
            target.transform.localScale= damageTextPrefab.transform.localScale;
            yield return wait005;
        }
        target.SetActive(true);

        target.transform.position = pos;
        target.GetComponent<Text>().text = value.ToString();
        RectTransform targetRect = target.GetComponent<RectTransform>();
        targetRect.anchoredPosition += new Vector2(10, 180) + (sequence * seqVec2UpPos);

        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return wait005;
               // target.transform.position += incVec3UpPos;

                targetRect.anchoredPosition += incVec2UpPos;
            }
            break;
        }
        target.SetActive(false);

        yield break;
    }
}
