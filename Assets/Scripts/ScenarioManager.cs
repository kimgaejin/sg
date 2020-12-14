using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    private GameObject scenarioPanel;
    private Image leftSprite;
    private Image rightSprite;
    private GameObject leftNameObject;
    private GameObject rightNameObject;
    private Text leftNameText;
    private Text rightNameText;
    private Text senarioText;
    // 인게임에서 출력되는 캐릭터 대사 관리
    private List<Dictionary<string, object>> data;
    private int curIndex = 0;

    private void Awake()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        Transform tfScenarioPanel = canvas.Find("ScenarioPanel");
        scenarioPanel = tfScenarioPanel.gameObject;
        senarioText = tfScenarioPanel.Find("scenario").GetComponentInChildren<Text>();
        leftSprite = tfScenarioPanel.Find("leftSprite").GetComponent<Image>();
        rightSprite = tfScenarioPanel.Find("rightSprite").GetComponent<Image>();
        leftNameObject = tfScenarioPanel.Find("leftName").gameObject;
        rightNameObject = tfScenarioPanel.Find("rightName").gameObject;
        leftNameText = tfScenarioPanel.Find("leftName").GetComponentInChildren<Text>();
        rightNameText = tfScenarioPanel.Find("rightName").GetComponentInChildren<Text>();
        
        scenarioPanel.SetActive(false);
    }

    public void GetCsvTable(string filename)
    {
        data = CSVReader.Read(filename);
        curIndex = 0;
    }

    public bool ReadCurrentData(string bunch)
    {
        // 테이블의 현재 줄을 읽는다. 다음 줄도 읽어야한다면 true 반환
        while (curIndex < data.Count)
        {
            Debug.Log(data[curIndex]["bunch"] + " " + bunch + " " + data[curIndex]["content"]);

            string thisBunch = data[curIndex]["bunch"].ToString();
            if (thisBunch != bunch) break;
            if (scenarioPanel.activeSelf == false) scenarioPanel.SetActive(true);
            string index = data[curIndex]["index"].ToString();
            string leftSprite = data[curIndex]["leftSprite"].ToString();
            string rightSprite = data[curIndex]["rightSprite"].ToString();
            string leftName = data[curIndex]["leftName"].ToString();
            string rightName = data[curIndex]["rightName"].ToString();
            string effect = data[curIndex]["effect"].ToString();
            string content = data[curIndex]["content"].ToString();
            string sound = data[curIndex]["sound"].ToString();

            if (leftSprite != "") SetCharacterSprite(this.leftSprite, leftSprite);
            if (rightSprite != "") SetCharacterSprite(this.rightSprite, rightSprite);
            if (leftName != "")
            {   // 왼쪽 인물 대사
                this.leftNameObject.SetActive(true);
                this.leftNameText.text = leftName;
                this.rightNameObject.SetActive(false);

                this.leftSprite.color = Color.white;
                this.rightSprite.color = Color.gray;
            }
            else if (rightName != "")
            {
                this.rightNameObject.SetActive(true);
                this.rightNameText.text = rightName;
                this.leftNameObject.SetActive(false);

                this.leftSprite.color = Color.gray;
                this.rightSprite.color = Color.white;
            }
            else
            {
                this.leftNameObject.SetActive(false);
                this.rightNameObject.SetActive(false);

                this.leftSprite.color = Color.gray;
                this.rightSprite.color = Color.gray;
            }
            content = content.Replace("<br>", "\n");
            senarioText.text = content;

            if (this.leftSprite.sprite == null) this.leftSprite.color = new Color(1,1,1,0);
            if (this.rightSprite.sprite == null) this.rightSprite.color = new Color(1,1,1,0);


            //Debug.Log(data[curIndex]["bunch"] + " " + bunch + " " +data[curIndex]["content"]);
            curIndex++;
            return true;
        }
        scenarioPanel.SetActive(false);
        return false;
    }

    public void SetCharacterSprite(Image target, string fileName)
    {
        target.sprite = Resources.Load<Sprite>("CharacterSprite/" + fileName);
        target.SetNativeSize();
    }
}
