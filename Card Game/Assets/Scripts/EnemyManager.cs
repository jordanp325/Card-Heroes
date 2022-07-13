using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour {

    public GameObject BlankEnemy;
    public GameObject BlankBoss;
    public GameObject newBuff;
    GameObject buff;
    public Dictionary<string, Enemy> Enemies = new Dictionary<string, Enemy>();
    int[] enemyPosX = { 225, 225, 75, 75, -75, -75, -225, -225 };
    int[] enemyPosY = { 75, -175, 125, -125, 75, -175, 125, -125 };
    string[] enemyPos = { "", "", "", "", "", "", "", "" };

    string[] statNames = new string[] { "Strength Up", "Intellect Up", "Armor Up", "Resolve Up", "Shield", "Dodge", "Regen" };
    string[] statDownNames = new string[] { "Strength Down", "Intellect Down", "Armor Down", "Resolve Down" };

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

	}

    public void AddEnemy (string enemyID, string enemyName, int enemyHealth)
    {
        if(Enemies.Count == 0)
        {
            Server.server.activities.HideActvities();
        }
        GameObject newEnemy;
        if (enemyName == "Necromaster" || enemyName == "Mother Slime" || enemyName == "Mimic") newEnemy = Instantiate(BlankBoss, this.transform);
        else newEnemy = Instantiate(BlankEnemy, this.transform);
        newEnemy.name = enemyName + enemyID;
        newEnemy.GetComponentsInChildren<Text>()[1].text = enemyName;
        newEnemy.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = enemyHealth + "/" + enemyHealth;
        newEnemy.GetComponent<Enemy>().health = enemyHealth;
        newEnemy.GetComponent<Enemy>().maxHealth = enemyHealth;
        newEnemy.GetComponent<Enemy>().enemyName = enemyName;
        newEnemy.GetComponent<Enemy>().enemyDesc = Descriptions.descriptions.enemyDescs[enemyName];
        newEnemy.GetComponent<Enemy>().enemyID = enemyID;
        newEnemy.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Enemies/" + enemyName);
        newEnemy.GetComponentInChildren<Slider>().maxValue = enemyHealth;
        Enemies.Add(enemyID, newEnemy.GetComponent<Enemy>());
        
        if(enemyName == "Necromaster" || enemyName == "Mother Slime" || enemyName == "Mimic")
        {
            newEnemy.transform.localPosition = new Vector3(150, 0);
            enemyPos[0] = enemyID;
            enemyPos[1] = enemyID;
            enemyPos[2] = enemyID;
            enemyPos[3] = enemyID;
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                if (enemyPos[i] == "")
                {
                    newEnemy.transform.localPosition = new Vector3(enemyPosX[i], enemyPosY[i]);
                    enemyPos[i] = enemyID;
                    break;
                }
            }
        }
    }

    public void EnemyDied(string enemyID)
    {
        Destroy(Enemies[enemyID].gameObject);
        Enemies.Remove(enemyID);
        for(int i = 0; i < 8; i++)
        {
            if (enemyPos[i] == enemyID) enemyPos[i] = "";
        }
    }

    public void CorrectHealthBars()
    {
        foreach(Enemy e in Enemies.Values)
        {
            e.GetComponentInChildren<Slider>().maxValue = e.maxHealth;
            e.GetComponentInChildren<Slider>().value = e.health;
            e.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().color = Color.HSVToRGB((float)e.health / e.maxHealth / 3f, 1, 1);
        }
    }

    public void UpdateEnemyStats(string enemyID, int health, int maxHealth)
    {
        Enemies[enemyID].maxHealth = maxHealth;
        Enemies[enemyID].health = health;
        Enemies[enemyID].GetComponentInChildren<Slider>().maxValue = maxHealth;
        Enemies[enemyID].GetComponentInChildren<Slider>().value = health;
        Enemies[enemyID].transform.GetChild(0).GetChild(2).GetComponent<Text>().text = Enemies[enemyID].health + "/" + Enemies[enemyID].maxHealth;
        Enemies[enemyID].transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().color = Color.HSVToRGB((float)health / Enemies[enemyID].maxHealth / 3f, 1, 1);
    }

    public void UpdateEnemyVisuals(string enemyID)
    {
        Enemy e = Enemies[enemyID];
        if(e.enemyName == "Mimic")
        {
            if(e.Buffs.ContainsKey("Shield")) e.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Enemies/MimicClosed");
            else e.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Enemies/Mimic");
        }
    }

    public void AddBuff(string enemyID, string buffName, string buffId, int duration, Dictionary<string, int> values, List<string> decaying)
    {
        bool statsOnly;
        if (buffName == "Strength" || buffName == "Intellect" || buffName == "Armor" || buffName == "Resolve" || buffName == "Shield" || buffName == "Dodge" || buffName == "Regen") statsOnly = true;
        else statsOnly = false;
        int power = 0;
        if (values.ContainsKey("x")) power = values["x"];
        Enemies[enemyID].Buffs.Add(buffId, new Buff(buffName, buffId, Descriptions.descriptions.buffDescs[buffName], duration, power, statsOnly, false, values, decaying));
        ArrangeBuffs(enemyID);
    }

    public void UpdateBuffDuration(string enemyID, string buffId, int duration)
    {
        Enemies[enemyID].Buffs[buffId].duration = duration;
        ArrangeBuffs(enemyID);
    }

    public void UpdateBuffLocal(string enemyID, string buffID, string localName, int power)
    {
        if (Enemies[enemyID].Buffs[buffID].values.ContainsKey(localName)) Enemies[enemyID].Buffs[buffID].values[localName] = power;
        else Enemies[enemyID].Buffs[buffID].values.Add(localName, power);
        if(localName == "x") Enemies[enemyID].Buffs[buffID].power = power;
        ArrangeBuffs(enemyID);
    }

    public void RemoveBuff(string enemyID, string buffId)
    {
        Enemies[enemyID].Buffs.Remove(buffId);
        ArrangeBuffs(enemyID);
    }

    //public void ActivateBuff(string enemyID, string buffId, int duration, Dictionary<string, int> values)
    //{
    //    Enemies[enemyID].Buffs[buffId].duration = duration;
    //    Enemies[enemyID].Buffs[buffId].values = values;
    //    int power = 0;
    //    if (values.ContainsKey("x")) power = values["x"];
    //    Enemies[enemyID].Buffs[buffId].power = power;
    //    ArrangeBuffs(enemyID);
    //}

    public void UpdateLocalDecay(string enemyID, string buffId, string decaying)
    {
        Enemies[enemyID].Buffs[buffId].decaying.Add(decaying);
    }

    public void ArrangeBuffs(string enemyID)
    {
        for (int i = Enemies[enemyID].transform.GetChild(3).childCount; i > 0; i--)
        {
            Destroy(Enemies[enemyID].transform.GetChild(3).GetChild(i - 1).gameObject);
        }

        Dictionary<string, Stat> stats = new Dictionary<string, Stat>();

        stats.Add("Strength", new Stat("Strength", new List<Buff>(), "", int.MaxValue, 0));
        stats.Add("Intellect", new Stat("Intellect", new List<Buff>(), "", int.MaxValue, 0));
        stats.Add("Armor", new Stat("Armor", new List<Buff>(), "", int.MaxValue, 0));
        stats.Add("Resolve", new Stat("Resolve", new List<Buff>(), "", int.MaxValue, 0));
        stats.Add("Shield", new Stat("Shield", new List<Buff>(), "", int.MaxValue, 0));
        stats.Add("Dodge", new Stat("Dodge", new List<Buff>(), "", int.MaxValue, 0));
        stats.Add("Regen", new Stat("Regen", new List<Buff>(), "", int.MaxValue, 0));

        int buffObjects = 0;

        foreach (Buff b in Enemies[enemyID].Buffs.Values)
        {
            foreach (string s in stats.Keys)
            {
                if (b.values.ContainsKey(s))
                {
                    stats[s].power += b.values[s];
                    if (b.duration < stats[s].duration && b.duration != -1) stats[s].duration = b.duration;
                    stats[s].components.Add(b);
                }
            }
        }

        foreach (Stat s in stats.Values) if (s.duration == int.MaxValue) s.duration = -1;

        foreach (Stat s in stats.Values)
        {
            if (s.components.Count > 0)
            {
                buff = Instantiate(newBuff, Enemies[enemyID].transform.GetChild(3));
                buff.AddComponent<StatObject>();
                buff.GetComponent<StatObject>().statName = s.statName;
                buff.GetComponent<StatObject>().components = s.components;
                buff.GetComponent<StatObject>().description = s.description;
                buff.GetComponent<StatObject>().duration = s.duration;
                buff.GetComponent<StatObject>().power = s.power;
                buff.GetComponentsInChildren<Text>()[1].text = s.power.ToString();
                if (s.duration > 0) buff.GetComponentsInChildren<Text>()[0].text = s.duration.ToString();
                else buff.GetComponentsInChildren<Text>()[0].text = "";
                if (s.power > 0)
                {
                    if (s.statName == "Strength" || s.statName == "Intellect" || s.statName == "Armor" || s.statName == "Resolve") buff.name = s.statName + " Up";
                    else buff.name = s.statName;
                }
                if (s.power < 0) buff.name = s.statName + " Down";
                if (s.power == 0) buff.name = s.statName;
                buff.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/BuffIcons/" + buff.name);
                buffObjects++;
                buff.transform.localPosition = new Vector2((buffObjects - 1) % 4 * 35 - 52.5f, (buffObjects - 1) / 4 * 35 - 52.5f);
            }
        }

        foreach (Buff b in Enemies[enemyID].Buffs.Values)
        {
            if (!b.statsOnly)
            {
                buff = Instantiate(newBuff, Enemies[enemyID].transform.GetChild(3));
                if (b.duration > 0) buff.GetComponentsInChildren<Text>()[0].text = b.duration.ToString();
                if (b.power > 0) buff.GetComponentsInChildren<Text>()[1].text = b.power.ToString();
                buff.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/BuffIcons/" + b.buffName);
                buff.name = b.buffName;
                buffObjects++;
                buff.transform.localPosition = new Vector2((buffObjects - 1) % 4 * 35 - 52.5f, (buffObjects - 1) / 4 * 35 - 52.5f);
            }
        }

        UpdateEnemyVisuals(enemyID);
    }

    public void ChangeEnemyIntention(string enemyID, string[] targets, bool[] intentions, int damage, int attackNumber)
    {
        GameObject targetDisplay = Enemies[enemyID].transform.GetChild(2).GetChild(0).gameObject;
        GameObject intentionDisplay = Enemies[enemyID].transform.GetChild(2).GetChild(1).gameObject;

        for (int i = 0; i < 4; i++)
        {
            targetDisplay.transform.GetChild(i).gameObject.SetActive(false);
            targetDisplay.transform.GetChild(i).transform.localPosition = new Vector3(0, 0);
            targetDisplay.transform.GetChild(i).GetComponentInChildren<Text>().text = "";

            intentionDisplay.transform.GetChild(i).gameObject.SetActive(false);
        }
        intentionDisplay.transform.localPosition = new Vector3(0, 0);
        intentionDisplay.transform.GetChild(0).GetComponentInChildren<Text>().text = "";

        string attackIcon = "";
        if (attackNumber > 0)
        {
            if (intentions[1])
            {
                if (damage < 10) attackIcon = "WeakAttack";
                else if (damage < 30) attackIcon = "RegularAttack";
                else if (damage < 60) attackIcon = "StrongAttack";
                else if (damage < 100) attackIcon = "ViciousAttack";
                else attackIcon = "BrutalAttack";
            }
            else
            {
                if (damage < 10) attackIcon = "WeakMagicAttack";
                else if (damage < 30) attackIcon = "RegularMagicAttack";
                else if (damage < 60) attackIcon = "StrongMagicAttack";
                else if (damage < 100) attackIcon = "ViciousMagicAttack";
                else attackIcon = "BrutalMagicAttack";
            }
        }

        for(int i = 0; i < targets.Length; i++)
        {
            targetDisplay.transform.GetChild(i).gameObject.SetActive(true);
            targetDisplay.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/IntentionIcons/" + Server.server.playerList.Players[targets[i]].playerClass + "Mini");
            targetDisplay.transform.GetChild(i).GetComponentInChildren<Text>().text = Server.server.playerList.Players[targets[i]].playerNumber.GetComponent<Text>().text;
        }

        switch (targets.Length)
        {
            case 0:
                targetDisplay.transform.GetChild(0).localPosition = new Vector3(0, 0);
                intentionDisplay.transform.localPosition = new Vector3(20, 0);
                break;
            case 1:
                targetDisplay.transform.GetChild(0).localPosition = new Vector3(0, 0);
                intentionDisplay.transform.localPosition = new Vector3(20, 0);
                break;
            case 2:
                targetDisplay.transform.GetChild(0).localPosition = new Vector3(-20, 0);
                targetDisplay.transform.GetChild(1).localPosition = new Vector3(20, 0);
                intentionDisplay.transform.localPosition = new Vector3(40, 0);
                break;
            case 3:
                targetDisplay.transform.GetChild(0).localPosition = new Vector3(-20, 20);
                targetDisplay.transform.GetChild(1).localPosition = new Vector3(20, 20);
                targetDisplay.transform.GetChild(2).localPosition = new Vector3(0, -20);
                intentionDisplay.transform.localPosition = new Vector3(40, 0);
                break;
            case 4:
                targetDisplay.transform.GetChild(0).localPosition = new Vector3(-20, 20);
                targetDisplay.transform.GetChild(1).localPosition = new Vector3(20, 20);
                targetDisplay.transform.GetChild(2).localPosition = new Vector3(-20, -20);
                targetDisplay.transform.GetChild(3).localPosition = new Vector3(20, -20);
                intentionDisplay.transform.localPosition = new Vector3(40, 0);
                break;
        }

        if (attackNumber > 0)
        {
            intentionDisplay.transform.GetChild(0).gameObject.SetActive(true);
            intentionDisplay.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/IntentionIcons/" + attackIcon);
            intentionDisplay.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = attackNumber.ToString();
            intentionDisplay.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = damage.ToString();
        }
        if (intentions[0])
        {
            targetDisplay.transform.GetChild(0).gameObject.SetActive(true);
            targetDisplay.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/IntentionIcons/AoE");
        }
        if (intentions[2]) intentionDisplay.transform.GetChild(1).gameObject.SetActive(true);
        if (intentions[3]) intentionDisplay.transform.GetChild(2).gameObject.SetActive(true);
        if (intentions[4]) intentionDisplay.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void ChangeAttackSeverity(string enemyID, int damage)
    {
        if(Enemies[enemyID].transform.GetChild(2).GetChild(1).GetChild(0).gameObject.activeSelf)
        {
            string attackIcon = "";
            if (!Enemies[enemyID].transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite.name.Contains("Magic"))
            {
                if (damage < 10) attackIcon = "WeakAttack";
                else if (damage < 30) attackIcon = "RegularAttack";
                else if (damage < 60) attackIcon = "StrongAttack";
                else if (damage < 100) attackIcon = "ViciousAttack";
                else attackIcon = "BrutalAttack";
            }
            else
            {
                if (damage < 10) attackIcon = "WeakMagicAttack";
                else if (damage < 30) attackIcon = "RegularMagicAttack";
                else if (damage < 60) attackIcon = "StrongMagicAttack";
                else if (damage < 100) attackIcon = "ViciousMagicAttack";
                else attackIcon = "BrutalMagicAttack";
            }
            Enemies[enemyID].transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/IntentionIcons/" + attackIcon);
            Enemies[enemyID].transform.GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = damage.ToString();
        }
    }
}
