using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManagerTest : MonoBehaviour
{
    public Image BlackScreen;
    public Transform CameraStartPosition, CameraEndPosition;
    public CanvasGroup TitlePanel, LobbyPanel, SelectWorldPanel;
    public GameObject ConfigPopup;
    public Image SelectArrow;

    Transform MapCamera;
    CanvasGroup ActivePanel;

    void Start()
    {
        MapCamera = Camera.main.transform;
        TitlePanel.gameObject.SetActive(true);
        LobbyPanel.gameObject.SetActive(false);
        ConfigPopup.SetActive(false);
        SelectWorldPanel.gameObject.SetActive(false);
        StartCoroutine(SelectArrowAnimation());
        StartCoroutine(ChangePanel(TitlePanel));
        StartCoroutine(MoveCamera());

        EffectManager.Instance.IfFadeIn();
    }

    public void TitlePanelAnyKeyPressed()
	{
        StartCoroutine(ChangePanel(LobbyPanel));
	}

    public void LobbyPanelConfigPopupButton()
    {
        ConfigPopup.SetActive(true);
    }

    public void ConfigPopupCloseButton()
	{
        ConfigPopup.SetActive(false);
	}

    public void LobbyPanelSelectWorldButton()
    {
        StartCoroutine(ChangePanel(SelectWorldPanel));
    }

    public void SelectWorldPanelBackButton()
    {
        StartCoroutine(ChangePanel(LobbyPanel));
    }

    public void SelectWorldPanelWorld1Button()
	{
        SceneManager.LoadScene("Scenes/SampleScene01103");
        Debug.Log("todo : 월드1 선택");
	}

    IEnumerator MoveCamera()
    {
        float MoveDuration = 120f;
        float BlackScreenAlpha = BlackScreen.color.a;
        BlackScreen.color = Color.black;
        yield return new WaitForSeconds(1.4f);
        while (true)
        {
            MapCamera.transform.position = CameraStartPosition.position;
            MapCamera.transform.DOKill();
            MapCamera.DOMove(CameraEndPosition.position, MoveDuration).SetEase(Ease.Linear);
            BlackScreen.DOFade(BlackScreenAlpha, 5f);
            yield return new WaitForSeconds(MoveDuration - 5f);
            yield return BlackScreen.DOFade(1f, 5f).WaitForCompletion();
        }
    }

    IEnumerator SelectArrowAnimation()
    {
        Vector3 SelectArrowInitialPosition = SelectArrow.transform.localPosition;
        float AnimationTime = 1.2f;
        while (true)
        {
            SelectArrow.transform.DOLocalMoveY(SelectArrowInitialPosition.y - 30f, AnimationTime);
            yield return SelectArrow.DOFade(0f, AnimationTime * 0.5f).SetDelay(AnimationTime * 0.5f).WaitForCompletion();
            SelectArrow.transform.localPosition = SelectArrowInitialPosition;
            SelectArrow.color = Color.white;
        }
    }

    IEnumerator ChangePanel(CanvasGroup PanelTo)
    {
        if(ActivePanel)
        {
            yield return ActivePanel.DOFade(0f, 0.8f).WaitForCompletion();
            ActivePanel.gameObject.SetActive(false);
        }
        if(PanelTo)
        {
            ActivePanel = PanelTo;
            PanelTo.gameObject.SetActive(true);
            PanelTo.alpha = 0f;
            yield return PanelTo.DOFade(1f, 0.8f).WaitForCompletion();
        }
    }
}
