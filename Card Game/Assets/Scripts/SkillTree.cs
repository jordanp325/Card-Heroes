using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour {

    public List<string> activeSkills = new List<string>();

    public Dictionary<string, string> WarriorSkills = new Dictionary<string, string>()
    {
        {"000", "Strong"}, {"001", "Regenerative"}, {"002", "Fortified"}, {"003", "Hardy"},
        {"010", "Offense"}, {"011", "Brawler"}, {"012", "Defense"}, {"013", "Card Removal I"},
        {"020", "Furyborn"}, {"021", "Spiked Armor"}, {"022", "Unarmored Defense"}, {"023", "Taste of Blood"},
        {"030", "Flurry of Blows"}, {"031", "Culling"}, {"032", "Barricade"},
        {"100", "???"}, {"101", "???"}, {"102", "???"}, {"103", "???"},
        {"110", "???"}, {"111", "???"}, {"112", "???"}, {"113", "???"},
        {"120", "???"}, {"121", "???"}, {"122", "???"}, {"123", "???"},
        {"130", "???"}, {"131", "???"}, {"132", "???"},
        {"200", "???"}, {"201", "???"}, {"202", "???"}, {"203", "???"},
        {"210", "???"}, {"211", "???"}, {"212", "???"}, {"213", "???"},
        {"220", "???"}, {"221", "???"}, {"222", "???"}, {"223", "???"},
        {"230", "???"}, {"231", "???"}, {"232", "???"},
        {"300", "???"}, {"301", "???"}, {"302", "???"}, {"303", "???"},
        {"310", "???"}, {"311", "???"}, {"312", "???"}, {"313", "???"},
        {"320", "???"}, {"321", "???"}, {"322", "???"}, {"323", "???"},
        {"330", "???"}, {"331", "???"}, {"332", "???"},
        {"400", "???"}, {"401", "???"}, {"402", "???"}, {"403", "???"},
        {"410", "???"}, {"411", "???"}, {"412", "???"}, {"413", "???"},
        {"420", "???"}, {"421", "???"}, {"422", "???"}, {"423", "???"},
        {"430", "???"}, {"431", "???"}, {"432", "???"},
        {"500", "???"}, {"501", "???"}, {"502", "???"}, {"503", "???"},
        {"510", "???"}, {"511", "???"}, {"512", "???"}, {"513", "???"},
        {"520", "???"}, {"521", "???"}, {"522", "???"}, {"523", "???"},
        {"530", "???"}, {"531", "???"}, {"532", "???"},
        {"600", "???"}, {"601", "???"}, {"602", "???"}, {"603", "???"},
        {"610", "???"}, {"611", "???"}, {"612", "???"}, {"613", "???"},
        {"620", "???"}, {"621", "???"}, {"622", "???"}, {"623", "???"},
        {"630", "???"}, {"631", "???"}, {"632", "???"},
    };

    public Dictionary<string, string> RogueSkills = new Dictionary<string, string>()
    {
        {"000", "Prepared"}, {"001", "Poisonous"}, {"002", "Stealthed"}, {"003", "Alert"},
        {"010", "Daggers"}, {"011", "Poison"}, {"012", "Traps"}, {"013", "Card Removal I"},
        {"020", "Sharp"}, {"021", "Toxic"}, {"022", "Cunning"}, {"023", "Focused"},
        {"030", "Tempest"}, {"031", "Infect"}, {"032", "Shadow"},
        {"100", "???"}, {"101", "???"}, {"102", "???"}, {"103", "???"},
        {"110", "???"}, {"111", "???"}, {"112", "???"}, {"113", "???"},
        {"120", "???"}, {"121", "???"}, {"122", "???"}, {"123", "???"},
        {"130", "???"}, {"131", "???"}, {"132", "???"},
        {"200", "???"}, {"201", "???"}, {"202", "???"}, {"203", "???"},
        {"210", "???"}, {"211", "???"}, {"212", "???"}, {"213", "???"},
        {"220", "???"}, {"221", "???"}, {"222", "???"}, {"223", "???"},
        {"230", "???"}, {"231", "???"}, {"232", "???"},
        {"300", "???"}, {"301", "???"}, {"302", "???"}, {"303", "???"},
        {"310", "???"}, {"311", "???"}, {"312", "???"}, {"313", "???"},
        {"320", "???"}, {"321", "???"}, {"322", "???"}, {"323", "???"},
        {"330", "???"}, {"331", "???"}, {"332", "???"},
        {"400", "???"}, {"401", "???"}, {"402", "???"}, {"403", "???"},
        {"410", "???"}, {"411", "???"}, {"412", "???"}, {"413", "???"},
        {"420", "???"}, {"421", "???"}, {"422", "???"}, {"423", "???"},
        {"430", "???"}, {"431", "???"}, {"432", "???"},
        {"500", "???"}, {"501", "???"}, {"502", "???"}, {"503", "???"},
        {"510", "???"}, {"511", "???"}, {"512", "???"}, {"513", "???"},
        {"520", "???"}, {"521", "???"}, {"522", "???"}, {"523", "???"},
        {"530", "???"}, {"531", "???"}, {"532", "???"},
        {"600", "???"}, {"601", "???"}, {"602", "???"}, {"603", "???"},
        {"610", "???"}, {"611", "???"}, {"612", "???"}, {"613", "???"},
        {"620", "???"}, {"621", "???"}, {"622", "???"}, {"623", "???"},
        {"630", "???"}, {"631", "???"}, {"632", "???"},
    };

    public Dictionary<string, string> MageSkills = new Dictionary<string, string>()
    {
        {"000", "Intelligent"}, {"001", "Shatter"}, {"002", "Explosive"}, {"003", "Arcane Shield"},
        {"010", "Scaling"}, {"011", "Hex"}, {"012", "Burst"}, {"013", "Card Removal I"},
        {"020", "Peak Condition"}, {"021", "Spell Bane"}, {"022", "Arcane Burst"}, {"023", "Aether Battery"},
        {"030", "Devour Intellect"}, {"031", "Scourge"}, {"032", "Nova Charge"},
        {"100", "???"}, {"101", "???"}, {"102", "???"}, {"103", "???"},
        {"110", "???"}, {"111", "???"}, {"112", "???"}, {"113", "???"},
        {"120", "???"}, {"121", "???"}, {"122", "???"}, {"123", "???"},
        {"130", "???"}, {"131", "???"}, {"132", "???"},
        {"200", "???"}, {"201", "???"}, {"202", "???"}, {"203", "???"},
        {"210", "???"}, {"211", "???"}, {"212", "???"}, {"213", "???"},
        {"220", "???"}, {"221", "???"}, {"222", "???"}, {"223", "???"},
        {"230", "???"}, {"231", "???"}, {"232", "???"},
        {"300", "???"}, {"301", "???"}, {"302", "???"}, {"303", "???"},
        {"310", "???"}, {"311", "???"}, {"312", "???"}, {"313", "???"},
        {"320", "???"}, {"321", "???"}, {"322", "???"}, {"323", "???"},
        {"330", "???"}, {"331", "???"}, {"332", "???"},
        {"400", "???"}, {"401", "???"}, {"402", "???"}, {"403", "???"},
        {"410", "???"}, {"411", "???"}, {"412", "???"}, {"413", "???"},
        {"420", "???"}, {"421", "???"}, {"422", "???"}, {"423", "???"},
        {"430", "???"}, {"431", "???"}, {"432", "???"},
        {"500", "???"}, {"501", "???"}, {"502", "???"}, {"503", "???"},
        {"510", "???"}, {"511", "???"}, {"512", "???"}, {"513", "???"},
        {"520", "???"}, {"521", "???"}, {"522", "???"}, {"523", "???"},
        {"530", "???"}, {"531", "???"}, {"532", "???"},
        {"600", "???"}, {"601", "???"}, {"602", "???"}, {"603", "???"},
        {"610", "???"}, {"611", "???"}, {"612", "???"}, {"613", "???"},
        {"620", "???"}, {"621", "???"}, {"622", "???"}, {"623", "???"},
        {"630", "???"}, {"631", "???"}, {"632", "???"},
    };

    public Dictionary<string, string> ClericSkills = new Dictionary<string, string>()
    {
        {"000", "Divine Healing"}, {"001", "Divine Vitality"}, {"002", "Divine Strike"}, {"003", "Divine Health"},
        {"010", "Supportive"}, {"011", "Defensive"}, {"012", "Offensive"}, {"013", "Card Removal I"},
        {"020", "Protective"}, {"021", "Resolute"}, {"022", "Radiant"}, {"023", "Divine Aura"},
        {"030", "Redemption"}, {"031", "Safeguard"}, {"032", "Regicide"},
        {"100", "???"}, {"101", "???"}, {"102", "???"}, {"103", "???"},
        {"110", "???"}, {"111", "???"}, {"112", "???"}, {"113", "???"},
        {"120", "???"}, {"121", "???"}, {"122", "???"}, {"123", "???"},
        {"130", "???"}, {"131", "???"}, {"132", "???"},
        {"200", "???"}, {"201", "???"}, {"202", "???"}, {"203", "???"},
        {"210", "???"}, {"211", "???"}, {"212", "???"}, {"213", "???"},
        {"220", "???"}, {"221", "???"}, {"222", "???"}, {"223", "???"},
        {"230", "???"}, {"231", "???"}, {"232", "???"},
        {"300", "???"}, {"301", "???"}, {"302", "???"}, {"303", "???"},
        {"310", "???"}, {"311", "???"}, {"312", "???"}, {"313", "???"},
        {"320", "???"}, {"321", "???"}, {"322", "???"}, {"323", "???"},
        {"330", "???"}, {"331", "???"}, {"332", "???"},
        {"400", "???"}, {"401", "???"}, {"402", "???"}, {"403", "???"},
        {"410", "???"}, {"411", "???"}, {"412", "???"}, {"413", "???"},
        {"420", "???"}, {"421", "???"}, {"422", "???"}, {"423", "???"},
        {"430", "???"}, {"431", "???"}, {"432", "???"},
        {"500", "???"}, {"501", "???"}, {"502", "???"}, {"503", "???"},
        {"510", "???"}, {"511", "???"}, {"512", "???"}, {"513", "???"},
        {"520", "???"}, {"521", "???"}, {"522", "???"}, {"523", "???"},
        {"530", "???"}, {"531", "???"}, {"532", "???"},
        {"600", "???"}, {"601", "???"}, {"602", "???"}, {"603", "???"},
        {"610", "???"}, {"611", "???"}, {"612", "???"}, {"613", "???"},
        {"620", "???"}, {"621", "???"}, {"622", "???"}, {"623", "???"},
        {"630", "???"}, {"631", "???"}, {"632", "???"},
    };

    public GameObject SkillTreeDisplay;
    public GameObject ConfirmSkillButton;
    public GameObject SkillPanel;
    public GameObject newSkill;
    public Text InfoText;

    public Color buttonColor;
    public Color darkOutline;
    public Color lightOutline;
    public Color whiteOutline;

    public string selectedSkill;
    public string[] rememberedSkills;

    int skillObjects = 0;

    // Use this for initialization
    void Start ()
    {
        whiteOutline = new Color(1, 1, 1);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SelectSkill(string skillKey)
    {
        selectedSkill = skillKey;
        DisplaySkillTree(rememberedSkills);
        GameObject button = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(int.Parse("" + skillKey[0])).GetChild(4 * int.Parse("" + skillKey[1]) + int.Parse("" + skillKey[2])).gameObject;
        button.GetComponent<Outline>().effectColor = whiteOutline;
        ConfirmSkillButton.SetActive(true);
        InfoText.text = "Confirm Skill";
    }

    public void ChooseSkill()
    {
        if (selectedSkill.Substring(1) == "13") Server.server.cardPanel.DisplayCardRemoval();
        else Server.server.chooseSkill(selectedSkill);
        ConfirmSkillButton.SetActive(false);
        SkillTreeDisplay.SetActive(false);
    }

    public void ColorSkillTree()
    {
        switch (Server.server.playerList.Me().playerClass)
        {
            case "Warrior": buttonColor = new Color(1f, 0.7f, 0.7f); lightOutline = new Color(1f, 0f, 0f); darkOutline = new Color(0.3f, 0f, 0f);
                foreach (string key in WarriorSkills.Keys) gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(int.Parse("" + key[0])).GetChild(4 * int.Parse("" + key[1]) + int.Parse("" + key[2])).GetComponentInChildren<Text>().text = WarriorSkills[key];
                break;
            case "Rogue": buttonColor = new Color(0.7f, 1f, 0.7f); lightOutline = new Color(0f, 1f, 0f); darkOutline = new Color(0f, 0.3f, 0f);
                foreach (string key in RogueSkills.Keys) gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(int.Parse("" + key[0])).GetChild(4 * int.Parse("" + key[1]) + int.Parse("" + key[2])).GetComponentInChildren<Text>().text = RogueSkills[key];
                break;
            case "Mage": buttonColor = new Color(0.7f, 0.7f, 1f);  lightOutline = new Color(0f, 0f, 1f); darkOutline = new Color(0f, 0f, 0.3f);
                foreach (string key in MageSkills.Keys) gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(int.Parse("" + key[0])).GetChild(4 * int.Parse("" + key[1]) + int.Parse("" + key[2])).GetComponentInChildren<Text>().text = MageSkills[key];
                break;
            case "Cleric": buttonColor = new Color(1f, 1f, 0.7f); lightOutline = new Color(1f, 1f, 0f); darkOutline = new Color(0.3f, 0.3f, 0f);
                foreach (string key in ClericSkills.Keys) gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(int.Parse("" + key[0])).GetChild(4 * int.Parse("" + key[1]) + int.Parse("" + key[2])).GetComponentInChildren<Text>().text = ClericSkills[key];
                break;
        }

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetChild(j).gameObject.GetComponent<Image>().color = buttonColor;
            }
        }
        
        
    }

    public void DisplaySkillTree(string[] availableSkills)
    {
        rememberedSkills = availableSkills;
        ColorBlock buttonColors = new ColorBlock();
        buttonColors.normalColor = new Color(1f, 1f, 1f);
        buttonColors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        buttonColors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
        buttonColors.disabledColor = new Color(0.5f, 0.5f, 0.5f);
        buttonColors.colorMultiplier = 1;

        for (int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 15; j++)
            {
                GameObject button = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetChild(j).gameObject;
                button.GetComponent<Button>().interactable = false;
                button.GetComponent<Button>().colors = buttonColors;
                button.GetComponent<Outline>().effectColor = darkOutline;
            }
        }

        foreach(string skillKey in availableSkills)
        {
            GameObject button = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(int.Parse(""+skillKey[0])).GetChild(4 * int.Parse("" + skillKey[1]) + int.Parse("" + skillKey[2])).gameObject;
            button.GetComponent<Button>().interactable = true;
        }

        buttonColors.disabledColor = new Color(1f, 1f, 1f);

        foreach (string skillKey in activeSkills)
        {
            GameObject button = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(int.Parse("" + skillKey[0])).GetChild(4 * int.Parse("" + skillKey[1]) + int.Parse("" + skillKey[2])).gameObject;
            button.GetComponent<Button>().colors = buttonColors;
            button.GetComponent<Outline>().effectColor = lightOutline;
        }

        InfoText.text = "Select a Skill";
        SkillTreeDisplay.SetActive(true);
    }

    public void AddSkill(string skillKey)
    {
        if(skillKey[1] == '0' || skillKey[1] == '2')
        {
            GameObject skill = Instantiate(newSkill, SkillPanel.transform);
            string skillName = "";
            switch (Server.server.playerList.Me().playerClass)
            {
                case "Warrior": skillName = WarriorSkills[skillKey]; break;
                case "Rogue": skillName = RogueSkills[skillKey]; break;
                case "Mage": skillName = MageSkills[skillKey]; break;
                case "Cleric": skillName = ClericSkills[skillKey]; break;
            }
            skill.name = skillName;
            skillObjects++;
            skill.transform.localPosition = new Vector2((skillObjects - 1) * 35 - 595f, 0);
            skill.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/SkillIcons/" + skillName);

        }
        activeSkills.Add(skillKey);
        selectedSkill = "";
        ConfirmSkillButton.SetActive(false);
        SkillTreeDisplay.SetActive(false);
    }
}
