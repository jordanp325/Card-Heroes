using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    public Text title;
    public Text description;
    public GameObject scrollView;
    public string parentName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClickTooltip()
    {
        if (gameObject.transform.parent.name == "SkillTree")
        {
            Dictionary<string, string> skills = new Dictionary<string, string>();
            switch (Server.server.playerList.Me().playerClass)
            {
                case "Warrior": skills = Server.server.skillTree.WarriorSkills; break;
                case "Rogue": skills = Server.server.skillTree.RogueSkills; break;
                case "Mage": skills = Server.server.skillTree.MageSkills; break;
                case "Cleric": skills = Server.server.skillTree.ClericSkills; break;
            }
            foreach (string skill in skills.Keys)
            {
                if (skills[skill] == gameObject.GetComponent<Tooltip>().title.text)
                {
                    if(Server.server.skillTree.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(int.Parse("" + skill[0])).GetChild(4 * int.Parse("" + skill[1]) + int.Parse("" + skill[2])).GetComponent<Button>().interactable)
                    {
                        Server.server.skillTree.SelectSkill(skill);
                        break;
                    }
                }
            }
        }
    }
}
