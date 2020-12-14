using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager instance = null;

    public Queue<string> events;        // 플레이어가 진행하면서 발생시킨 이벤트
    public List<string[]> infos;   // 이미 플레이어가 플레이한 기록을 불러온 것
    
    // Start is called before the first frame update
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            if (events == null)
            {
                events = new Queue<string>();
            }
            if (infos == null)
            {
                List<Dictionary<string, object>> data;
                infos = new List<string[]>();
                data = CSVReader.Read("data/playerinfo");
                for (int i = 0; i < data.Count; i++)
                {
                    string[] line = { data[i]["event"].ToString() };
                    infos.Add(line);
                }
            }
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {   // 중복방지
            Destroy(this.gameObject);
        }
    }

    public bool FindInfo(string target)
    {
        foreach (string[] s in infos)
        {
            if (s[0] == target) return true;
        }
        return false;
    }

    public bool AddInfo(string target)
    {
        foreach (string[] s in infos)
        {
            if (s[0] == target) return false;
        }

        string[] line = { target };
        infos.Add(line);

        CSVReader.Write(infos, "Assets/Resources/Data/playerInfo.csv");

        return true;
    }

    public static EventManager Instance
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
}
