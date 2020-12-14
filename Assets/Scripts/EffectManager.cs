using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    // EventManager와 헷갈리지 말자
    private static EffectManager instance = null;
    private Coroutine fadeCoroutine;
    private Image fade;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            fade = this.transform.Find("Fade").GetComponent<Image>();

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {   // 중복방지
            Destroy(this.gameObject);
        }
    }

    public static EffectManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) FadeOut();
        if (Input.GetKeyDown(KeyCode.R)) FadeIn();
    }

    public void IfFadeIn()
    {   // 화면이 어두운 상태일때만 FadeIn
        if (0.5f < fade.color.a)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(Fade(-1));
        }
    }

    public void FadeIn()
    {   // 검은화면에서 점점 밝아짐
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(Fade(-1));
    }

    public void IfFadeOut()
    {   // 화면이 밝은 상태일때만 FadeOut
        if (0.5f < fade.color.a)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(Fade(1));
        }
    }

    public void FadeOut()
    {   // 밝은화면에서 점점 어두워짐
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(Fade(1));
    }

    private IEnumerator Fade(int type)
    {
        // type: color.a 증가/감소 값. 0 제외
        if (type == 0) yield break;

        float deltaTime = 0.05f;
        float fadeTime = 0.5f;
        float time = 0.0f;
        WaitForSeconds waitDelta = new WaitForSeconds(deltaTime);

        if (0 < type) { fade.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);}
        else if (type < 0) { fade.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);}

        Color deltaColor = new Color(0.0f, 0.0f, 0.0f, (float)type * deltaTime / fadeTime);
        while (time < fadeTime)
        {
            fade.color += deltaColor;
            yield return waitDelta;
            if (fade.color.a < 0) { fade.color = new Color(0.0f, 0.0f, 0.0f, 0.0f); break; }
            else if (1.0f < fade.color.a) { fade.color = new Color(0.0f, 0.0f, 0.0f, 1.0f); break; }
            time += deltaTime;
        }
        Debug.LogWarning(deltaColor);

        yield break;
    }

}
