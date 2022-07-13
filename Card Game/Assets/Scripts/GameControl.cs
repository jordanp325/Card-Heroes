using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

    public GameObject EncounterOutcome;
    public GameObject FullscreenButton;
    public GameObject FloorIndicator;
    //public int encounterNumber;

    public GameObject HelpPanel;
    public GameObject GeneralHelp;
    public GameObject WarriorHelp;
    public GameObject RogueHelp;
    public GameObject MageHelp;
    public GameObject ClericHelp;
    public GameObject DebuffHelp;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
    }

    public void EndEncounter(bool win)
    {
        if (Server.server.dungeonMap.partyRoom == 'M' || Server.server.dungeonMap.partyRoom == 'B')
        {
            Server.server.cardPanel.lootCards = 2;
            Server.server.dungeonMap.AddKey();
        }
        else Server.server.cardPanel.lootCards = 1;
        EncounterOutcome.SetActive(true);
        if (win)
        {
            EncounterOutcome.GetComponent<Text>().text = "Victory!";
            EncounterOutcome.GetComponent<Text>().color = new Color(0.2f, 0.6f, 0.2f);
        }
        else
        {
            EncounterOutcome.GetComponent<Text>().text = "Defeat...";
            EncounterOutcome.GetComponent<Text>().color = new Color(0.6f, 0.2f, 0.2f);
        }
        foreach (Player p in Server.server.playerList.Players.Values)
        {
            p.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        Server.server.itemManager.UpdateCombatCooldowns();
        Server.server.cardPanel.ClearHand();
        Server.server.cardPanel.ReshuffleAll();
        Server.server.pingServer();
        foreach(Player p in Server.server.playerList.Players.Values) p.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    public void StartEncounter()
    {
        Server.server.playerList.PlayerTurn = 0;
        Server.server.playerList.AllCardsPicked();
        Server.server.playerList.Players[Server.server.playerList.PlayerOrder[0]].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        EncounterOutcome.SetActive(false);
        foreach(Player p in Server.server.playerList.Players.Values)
        {
            p.Buffs = new Dictionary<string, Buff>();
            Server.server.playerList.ArrangeBuffs(p.playerName);
        }
        Server.server.enemyManager.CorrectHealthBars();
        Server.server.cardPanel.ResetPileNumber();
    }

    public void ToggleFullScreen()
    {
        if (Server.server.full) FullscreenButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Fullscreen");
        else FullscreenButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Fullscreen2");
        Server.server.fullscreenToggle();
    }

    public void DisplayHelp(bool open)
    {
        if(open)
        {
            HelpPanel.SetActive(true);
            if (Server.server.playerList.Me().playerClass == "")
            {
                GeneralHelp.SetActive(true);
            }
            else
            {
                switch (Server.server.playerList.Me().playerClass)
                {
                    case "Warrior": WarriorHelp.SetActive(true); break;
                    case "Rogue": RogueHelp.SetActive(true); break;
                    case "Mage": MageHelp.SetActive(true); break;
                    case "Cleric": ClericHelp.SetActive(true); break;
                }
            }
        }
        else
        {
            GeneralHelp.SetActive(false);
            WarriorHelp.SetActive(false);
            RogueHelp.SetActive(false);
            MageHelp.SetActive(false);
            ClericHelp.SetActive(false);
            DebuffHelp.SetActive(false);
            HelpPanel.SetActive(false);
        }
    }

    public void SwitchHelp(string playerClass)
    {
        GeneralHelp.SetActive(false);
        WarriorHelp.SetActive(false);
        RogueHelp.SetActive(false);
        MageHelp.SetActive(false);
        ClericHelp.SetActive(false);
        DebuffHelp.SetActive(false);
        switch (playerClass)
        {
            case "General": GeneralHelp.SetActive(true); break;
            case "Warrior": WarriorHelp.SetActive(true); break;
            case "Rogue": RogueHelp.SetActive(true); break;
            case "Mage": MageHelp.SetActive(true); break;
            case "Cleric": ClericHelp.SetActive(true); break;
            case "Debuff": DebuffHelp.SetActive(true); break;
        }
    }
}
