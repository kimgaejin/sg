using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StoryProgressTestManager : MonoBehaviour
{
    [System.Serializable]
    public class MessageInfo
    {
        public Sprite ProfileSprite;
        public string Name;
        [TextArea] public string MessageText;
    }
    public List<MessageInfo> MessageInfos; 
    public GameObject BlackScreen;
    public GameObject MessagePanel;
    public Image MessagePanelProfileImage;
    public Text MessagePanelName;
    public Text MessagePanelText;
    public GameObject BattleTemp;
    public Transform CameraTransform;
    public List<Transform> CameraLocations;
    public List<Animator> PlayerCharacterAnimators;
    public List<GameObject> EnemyCharacters;

    bool NextMessageTrigger;

    void Start()
    {
        StartCoroutine(PlayCoroutine());
    }

    IEnumerator PlayCoroutine()
    {
        // 까만화면에서 대화
        Image BlackScreenImage = BlackScreen.GetComponent<Image>();
        BlackScreen.SetActive(true);
        BlackScreenImage.color = Color.black;
        yield return StartCoroutine(ShowMessage(MessageInfos[0]));
        yield return StartCoroutine(ShowMessage(MessageInfos[1]));
        // 페이드인, 왼쪽에서 캐릭터 둘 등장
        yield return BlackScreenImage.DOFade(0f, 1.5f).WaitForCompletion();
        BlackScreen.SetActive(false);
        yield return StartCoroutine(ShowMessage(MessageInfos[2]));
        yield return StartCoroutine(ShowMessage(MessageInfos[3]));
        yield return StartCoroutine(ShowMessage(MessageInfos[4]));
        yield return StartCoroutine(ShowMessage(MessageInfos[5]));
        yield return StartCoroutine(ShowMessage(MessageInfos[6]));
        yield return StartCoroutine(ShowMessage(MessageInfos[7]));
        yield return StartCoroutine(ShowMessage(MessageInfos[8]));
        // 전투
        yield return StartCoroutine(BattleProgressTemp());
        // 앞으로 나아간다
        yield return StartCoroutine(ProceedCharacters(1));
        // 대화
        yield return StartCoroutine(ShowMessage(MessageInfos[9]));
        yield return StartCoroutine(ShowMessage(MessageInfos[10]));
        yield return StartCoroutine(ShowMessage(MessageInfos[11]));
        yield return StartCoroutine(ShowMessage(MessageInfos[12]));
        // 전투
        yield return StartCoroutine(BattleProgressTemp());
        // 대화
        yield return StartCoroutine(ShowMessage(MessageInfos[13]));
        yield return StartCoroutine(ShowMessage(MessageInfos[14]));
        yield return StartCoroutine(ShowMessage(MessageInfos[15]));
        yield return StartCoroutine(ShowMessage(MessageInfos[16]));
        yield return StartCoroutine(ShowMessage(MessageInfos[17]));
        yield return StartCoroutine(ShowMessage(MessageInfos[18]));
        yield return StartCoroutine(ShowMessage(MessageInfos[19]));
    }

    IEnumerator ShowMessage(MessageInfo Message)
    {
        MessagePanel.SetActive(true);
        MessagePanelName.text = Message.Name;
        MessagePanelProfileImage.sprite = Message.ProfileSprite;
        for (int i = 0; i <= Message.MessageText.Length; i++)
        {
            MessagePanelText.text = Message.MessageText.Substring(0, i);
            yield return new WaitForSeconds(0.01f);
        }
        NextMessageTrigger = false;
        yield return new WaitUntil(() => NextMessageTrigger);
        NextMessageTrigger = false;
        MessagePanel.SetActive(false);
    }

    IEnumerator BattleProgressTemp()
    {
        EnemyCharacters.ForEach((enemy) => enemy.SetActive(true));
        PlayerCharacterAnimators.ForEach((animator) => animator.Play("BattleIdle"));
        BattleTemp.SetActive(true);
        yield return new WaitForSeconds(3f);
        BattleTemp.SetActive(false);
        PlayerCharacterAnimators.ForEach((animator) => animator.Play("Idle"));
        EnemyCharacters.ForEach((enemy) => enemy.SetActive(false));
    }

    IEnumerator ProceedCharacters(int LocationIndex)
    {
        PlayerCharacterAnimators.ForEach((animator) => animator.Play("Run"));
        yield return CameraTransform.DOMove(CameraLocations[LocationIndex].position, 1.8f).SetEase(Ease.Linear).WaitForCompletion();
        PlayerCharacterAnimators.ForEach((animator) => animator.Play("Idle"));
    }

    public void NextMessage()
    {
        NextMessageTrigger = true;
    }
}
