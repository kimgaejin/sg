using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToTurnAble : MonoBehaviour
{
    public void TurnSetActive()
    {
        this.transform.gameObject.SetActive(!this.transform.gameObject.activeSelf);
    }
}
