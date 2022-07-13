using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TooltipSpawner : MonoBehaviour {

    RaycastHit2D[] hits;
    GameObject target;
    public GameObject tooltip;
    GameObject temp;
    public int timer = 0;
    public int tooltipTime = 30;
    public GameObject tooltipPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.TransformDirection(Vector3.forward));
        string tagName = "";
        string targetName = "";

        string debugData = "";
        foreach(RaycastHit2D r in hits)
        {
            debugData += r.transform.gameObject.tag + " ";
        }

        if (hits.Length == 0) 
        {
            target = null;
            tagName = "";
            targetName = "";
            timer = 0;
        }
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.tag == "Tooltip")
            {
                target = hits[i].transform.gameObject;
                tagName = target.tag;
                targetName = target.name;
            }
            else if (hits[i].transform.gameObject.tag == "Skill" && tagName != "Tooltip")
            {
                target = hits[i].transform.gameObject;
                tagName = target.tag;
                targetName = target.name;
            }
            else if (hits[i].transform.gameObject.tag == "Item" && tagName != "Skill" && tagName != "Tooltip")
            {
                target = hits[i].transform.gameObject;
                tagName = target.tag;
                targetName = target.name;
            }
            else if (hits[i].transform.gameObject.tag == "Buff" && tagName != "Item" && tagName != "Skill" && tagName != "Tooltip")
            {
                target = hits[i].transform.gameObject;
                tagName = target.tag;
                targetName = target.name;
            }
            else if (hits[i].transform.gameObject.tag == "Enemy" && tagName != "Item" && tagName != "Buff" && tagName != "Skill" && tagName != "Tooltip")
            {
                target = hits[i].transform.gameObject;
                tagName = target.tag;
                targetName = target.name;
            }
            else if (tagName != "Enemy" && tagName != "Buff" && tagName != "Item" && tagName != "Skill" && tagName != "Tooltip")
            {
                target = null;
                tagName = "";
                targetName = "";
            }
        }

        if (tagName != "Tooltip" && tooltip != null)
        {
            if(tooltip.GetComponent<Tooltip>().parentName != targetName)
            {
                Destroy(tooltip);
                timer = 0;
            }
        }
        if (target != null && tooltip == null)
        {
            if (timer <= tooltipTime) timer++;
            else timer = 0;

            if (timer == tooltipTime && target.tag == "Enemy")
            {
                tooltip = Instantiate(tooltipPrefab, target.transform.parent);
                tooltip.transform.position = target.transform.position;
                tooltip.name = "EnemyTooltip";
                tooltip.GetComponent<Tooltip>().title.text = target.GetComponent<Enemy>().enemyName;
                tooltip.GetComponent<Tooltip>().description.text = target.GetComponent<Enemy>().enemyDesc;
                tooltip.GetComponent<Tooltip>().parentName = target.transform.name;
                AdjustTooltip();
            }
            if (timer == tooltipTime && target.tag == "Buff")
            {
                tooltip = Instantiate(tooltipPrefab, target.transform.parent.parent.parent);
                tooltip.transform.position = target.transform.position;
                tooltip.name = "BuffTooltip";
                tooltip.GetComponent<Tooltip>().title.text = target.name;
                tooltip.GetComponent<Tooltip>().parentName = target.transform.name;

                string description = "";
                if (Descriptions.descriptions.buffDescs.ContainsKey(target.name)) description = Descriptions.descriptions.buffDescs[target.name];
                else description = Descriptions.descriptions.skillDescs[target.name];

                description = description.Replace("[X]", target.GetComponentsInChildren<Text>()[1].text);
                if (target.GetComponentsInChildren<Text>()[1].text != "") if (int.Parse(target.GetComponentsInChildren<Text>()[1].text) < 0) description = description.Replace("-", "");

                if (target.GetComponentsInChildren<Text>()[0].text == "1") description = description.Replace("([Y] turns)", "(1 turn)");
                else if (target.GetComponentsInChildren<Text>()[0].text != "") description = description.Replace("[Y]", target.GetComponentsInChildren<Text>()[0].text);
                else description = description.Replace("([Y] turns)", "(Endless)");

                if (target.GetComponentsInChildren<Text>()[0].text != "") description = description.Replace("[Y/10]", (int.Parse(target.GetComponentsInChildren<Text>()[0].text) / 10 - 1).ToString());
                tooltip.GetComponent<Tooltip>().description.text = description;

                try
                {
                    StatObject s = target.GetComponent<StatObject>();
                    if(s != null)
                    {
                        description += "\n" + "<b>Sources:</b>";
                        foreach (Buff b in s.components)
                        {
                            if (b.decaying.Contains(s.statName))
                            {
                                if (b.duration == 1) description += ("\n" + b.values[s.statName] + " " + s.statName + " " + "(1 turn, decaying)");
                                else description += ("\n" + b.values[s.statName] + " " + s.statName + " " + "(" + b.duration + " turns, decaying)");
                            }
                            else
                            {
                                if (b.duration == -1) description += ("\n" + b.values[s.statName] + " " + s.statName + " " + "(Endless)");
                                else if (b.duration == 1) description += ("\n" + b.values[s.statName] + " " + s.statName + " " + "(1 turn)");
                                else description += ("\n" + b.values[s.statName] + " " + s.statName + " " + "(" + b.duration + " turns)");
                            }
                        }
                        tooltip.GetComponent<Tooltip>().description.text = description;
                    }
                }
                catch { }
                AdjustTooltip();
            }
            if (timer == tooltipTime && target.tag == "Item")
            {
                tooltip = Instantiate(tooltipPrefab, target.transform.parent);
                tooltip.transform.position = target.transform.position;
                tooltip.name = "ItemTooltip";
                tooltip.GetComponent<Tooltip>().title.text = target.GetComponent<Item>().itemName;
                tooltip.GetComponent<Tooltip>().description.text = target.GetComponent<Item>().GetItemDescription();
                tooltip.GetComponent<Tooltip>().parentName = target.transform.name;
                AdjustTooltip();
            }
            if (timer == tooltipTime && target.tag == "Skill")
            {
                tooltip = Instantiate(tooltipPrefab, target.transform.parent.parent);
                tooltip.transform.position = target.transform.position;
                tooltip.name = "SkillTooltip";
                switch (Server.server.playerList.Me().playerClass)
                {
                    case "Warrior":
                        tooltip.GetComponent<Tooltip>().title.text = Server.server.skillTree.WarriorSkills[target.name];
                        tooltip.GetComponent<Tooltip>().description.text = Descriptions.descriptions.skillDescs[Server.server.skillTree.WarriorSkills[target.name]];
                        break;
                    case "Rogue":
                        tooltip.GetComponent<Tooltip>().title.text = Server.server.skillTree.RogueSkills[target.name];
                        tooltip.GetComponent<Tooltip>().description.text = Descriptions.descriptions.skillDescs[Server.server.skillTree.RogueSkills[target.name]];
                        break;
                    case "Mage":
                        tooltip.GetComponent<Tooltip>().title.text = Server.server.skillTree.MageSkills[target.name];
                        tooltip.GetComponent<Tooltip>().description.text = Descriptions.descriptions.skillDescs[Server.server.skillTree.MageSkills[target.name]];
                        break;
                    case "Cleric":
                        tooltip.GetComponent<Tooltip>().title.text = Server.server.skillTree.ClericSkills[target.name];
                        tooltip.GetComponent<Tooltip>().description.text = Descriptions.descriptions.skillDescs[Server.server.skillTree.ClericSkills[target.name]];
                        break;
                }
                tooltip.GetComponent<Tooltip>().parentName = target.transform.name;
                AdjustTooltip();
            }
        }
        else timer = 0;
    }

    public void AdjustTooltip()
    {
        if (tooltip.transform.position.x > 14) tooltip.transform.position = new Vector3(14, tooltip.transform.position.y);
        if (tooltip.transform.position.x < -7.6f) tooltip.transform.position = new Vector3(-7.6f, tooltip.transform.position.y);
        if (tooltip.transform.position.y > 7.5f) tooltip.transform.position = new Vector3(tooltip.transform.position.x, 7.5f);
        if (tooltip.transform.position.y < -3.4f) tooltip.transform.position = new Vector3(tooltip.transform.position.x, -3.4f);

        Canvas.ForceUpdateCanvases();
        int lines = tooltip.GetComponent<Tooltip>().description.cachedTextGenerator.lines.Count;
        if (lines > 5) tooltip.GetComponent<Tooltip>().scrollView.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 25 + (lines * 16));
        tooltip.GetComponent<Tooltip>().scrollView.transform.localPosition = new Vector3(0, 55 - (tooltip.GetComponent<Tooltip>().scrollView.GetComponent<RectTransform>().rect.height / 2));
        Canvas.ForceUpdateCanvases();
    }
}
