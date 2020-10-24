using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarManager : MonoBehaviour
{
    public GameObject HpBarPrefab;
    public GameObject teamA;
    public GameObject teamB;

    private void Start()
    {
        foreach (Transform tf in teamA.transform)
        {
            GameObject hpBar = Instantiate(HpBarPrefab);
            hpBar.transform.SetParent(transform);
            hpBar.transform.localRotation = Quaternion.identity;
            hpBar.transform.localScale = Vector3.one;
            tf.GetComponent<ChampionInfo>().LinkHpBar(hpBar.GetComponent<HpBar>());
        }

        foreach (Transform tf in teamB.transform)
        {
            GameObject hpBar = Instantiate(HpBarPrefab);
            hpBar.transform.SetParent(transform);
            hpBar.transform.localRotation = Quaternion.identity;
            hpBar.transform.localScale = Vector3.one;
            tf.GetComponent<ChampionInfo>().LinkHpBar(hpBar.GetComponent<HpBar>());
        }
    }

}
