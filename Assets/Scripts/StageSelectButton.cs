using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectButton : MonoBehaviour
{
    // "goStage
    // ;stageName
    // ;stageDesc
    // ;playerChara(/)
    // ;roundEnemy(/+)
    // ;scenarioName
    // ;mapType
    // ;openCondition
    // ;clearEvent"
    public string stageName;
    public string stageDesc;
    public string [] playerChara;
    public string [] roundEnemy;
    public string scenarioName;
    public string mapType;
    public string openCondition;
    public string clearEvent;

    public void StartStage()
    {
        WriteStageInfo();
        SceneManager.LoadScene("Scenes/SampleScene01103");
    }

    private void WriteStageInfo()
    {
        string stageInfo = "goStage" + ";";
        stageInfo += stageName + ";";
        stageInfo += stageDesc + ";";
        foreach(string target in playerChara) stageInfo += target + "/";
        stageInfo += ";";
        foreach (string target in roundEnemy) stageInfo += target + "/";
        stageInfo += ";";
        stageInfo += scenarioName + ";";
        stageInfo += mapType + ";";
        stageInfo += openCondition + ";";
        stageInfo += clearEvent;

        EventManager.Instance.events.Enqueue(stageInfo);
    }
}
