using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeManager : MonoBehaviour
{
    public GameObject textPrefab;
    private GameObject mainNotice;
    private WaitForSeconds wait01;
    private Color alphaDelta;

    private void Awake()
    {
        mainNotice = transform.Find("MainNotice").gameObject;
        wait01 = new WaitForSeconds(0.1f);
        alphaDelta = new Color(0, 0, 0, 0.1f);
    }

    public void ShowNotice(string body, int area)
    {
        StartCoroutine( ShowNoticeInTime(body, area, 5.0f) );
    }

    IEnumerator ShowNoticeInTime(string body, int area, float time)
    {
        // 일정시간동안 메시지를 표시하고 삭제. time 이 0과 같거나 작을경우 무한지속
        Transform target = null;
        foreach (Transform tf in mainNotice.transform)
        {
            if (tf.gameObject.activeSelf == false)
            {
                target = tf;
                break;
            }
        }

        if (target == null)
        {
            target = Instantiate(textPrefab, mainNotice.transform).transform;
        }

        target.GetComponent<Text>().color = Color.white;
        target.GetComponent<Text>().text = body;
        target.GetComponent<RectTransform>().position = mainNotice.GetComponent<RectTransform>().position;
        target.gameObject.SetActive(true);


        float t = 0;
        if (0 < time)
        {
            while (true)
            {
                yield return wait01;
                t += 0.1f;
                if (time < t) break;
            }

            // 1초간 흐려지면서 사라짐
            for (int i = 0; i < 10; i++)
            {
                yield return wait01;
                target.GetComponent<Text>().color -= alphaDelta;
            }

            target.gameObject.SetActive(false);
        }
        else
        {

        }
        yield break;
    }
}
