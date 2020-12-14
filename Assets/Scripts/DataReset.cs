using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DataReset : MonoBehaviour
{
    public void ResetInfo()
    {
        EventManager.Instance.events.Clear();
        EventManager.Instance.infos.Clear();
        CSVReader.Write(null, "Assets/Resources/Data/playerInfo.csv");

        SceneManager.LoadScene("Assets/0Game/Scenes/Lobby");
    }

}
