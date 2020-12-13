using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager instance = null;

    public Queue<string> events;
    
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
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {   // 중복방지
            Destroy(this.gameObject);
        }
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
