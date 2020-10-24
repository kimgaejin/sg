using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionInfo : MonoBehaviour
{
    // references
    private Image img;
    protected BattleManager battleManager;
    protected HpBar hpBar;

    // values
    public bool isDead;
    public int team;
    public int location;
    private string name;
    public List<SkillCommon> skills;
    public int curSkillIndex;

    public int maxHp;
    public int hp;
    public int atk;
    public int def;
    public int spd;
    public List<int> buff;  // 1 atk 2 def

    // functions
    private void Awake()
    {
        img = transform.GetComponent<Image>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        Transform tfHpBar = transform.Find("HpBar");
        if (tfHpBar) hpBar = tfHpBar.GetComponent<HpBar>();
        name = transform.name;

        skills = new List<SkillCommon>();
    }

    public virtual void StartBattle(int team, int location)
    {
        img.color = Color.white;
        hp = maxHp;
        isDead = false;
        this.team = team;
        this.location = location;
    }

    public virtual void Attacked(int damage)
    {
        // 공격당했을 때 호출

        int appDef = this.def;
        if (buff.Contains(2) == true) appDef = (int)(appDef * 1.5f);    // 방어버프시 방어력 1.5배

        int totalDmg = damage -= appDef;
        if (totalDmg < 0) totalDmg = 0; // 공격력보다 방어력이 높을 시 데미지 0
        battleManager.ShowDamage(transform, totalDmg);

        this.hp -= totalDmg;
        if (hp <= 0)    // 사망
        {
            Debug.Log(name + "은 죽었습니다" + location);
            img.color = Color.gray;
            isDead = true;
            battleManager.AdjustLocationForDead();
        }

        if (hpBar) hpBar.Show();
    }

    public void AddSkill(SkillCommon skill)
    {
        skills.Add(skill);
    }
}
