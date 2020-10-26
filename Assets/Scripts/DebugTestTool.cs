using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTestTool : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Q)) Time.timeScale *= 2.0f;
            else if (Input.GetKeyDown(KeyCode.W)) Time.timeScale = 1.0f;
        }
    }
}
