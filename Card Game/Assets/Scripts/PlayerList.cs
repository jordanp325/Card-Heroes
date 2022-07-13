using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour {

    public GameObject PlayerPanel;
    public GameObject AllyPanel;
    public GameObject newBuff;
    GameObject buff;
    GameObject newPlayer;
    public GameObject PlayerListPanel;
    public GameObject ClassPanel;
    public GameObject SkillPanel;
    public GameObject InfoPanel;
    public GameObject DrawPileButton;
    public GameObject DiscardPileButton;
    public GameObject ReadyButton;
    public GameObject CardPanels;

    public string username;
    
    public GameObject[] ClassCards = new GameObject[4];
    public GameObject[] Characters = new GameObject[8];
    GameObject character;
    public Dictionary<string, Player> Players = new Dictionary<string, Player>();
    public string[] PlayerOrder = new string[8];
    public int PlayerTurn;
    
    bool allReady;
    public bool gameStarted;

    public float countdown;
    public float red;
    public float green;
    public GameObject countdownDisplay;
    
    string[] statNames = new string[] { "Strength Up" , "Intellect Up", "Armor Up", "Resolve Up", "Shield", "Dodge", "Regen" };
    string[] statDownNames = new string[] { "Strength Down", "Intellect Down", "Armor Down", "Resolve Down" };

    // Use this for initialization
    void Start () {
        countdown = 5;
        red = 0;
        green = 1;
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    void FixedUpdate()
    {
        if (allReady && !countdownDisplay.activeSelf)
        {
            countdownDisplay.SetActive(true);
            ClassPanel.SetActive(false);
        }
        if (allReady && countdown > 0)
        {
            countdown -= .02f;
            countdownDisplay.GetComponent<Image>().fillAmount = countdown / 5;
            if (countdown <= 5 && countdown > 2.5f) red += 0.008f;
            if (countdown <= 2.5f && countdown > 0) green -= 0.008f;
            countdownDisplay.GetComponent<Image>().color = new Color(red, green, 0);
            countdownDisplay.GetComponentInChildren<Text>().color = new Color(red, green, 0);
            countdownDisplay.GetComponentInChildren<Text>().text = Mathf.Round(countdown + 0.5f).ToString();
        }
        if (!allReady && countdown < 5 && countdown > 0 && !gameStarted)
        {
            countdown = 5;
            red = 0;
            green = 1;
            countdownDisplay.SetActive(false);
            ClassPanel.SetActive(true);
        }
    }

    public Player getPlayerTurn()
    {
        return Players[PlayerOrder[PlayerTurn]];
    }

    public Player Me()
    {
        return Players[username];
    }

    public bool myTurn()
    {
        return PlayerOrder[PlayerTurn] == username;
    }

    public void IncrementTurn()
    {
        PlayerTurn++;
        if (PlayerTurn >= Players.Count)
        {
            PlayerTurn = 0;
            Server.server.itemManager.UpdateRoundCooldowns();
        }
        int playersDead = 0;
        foreach(Player p in Players.Values)
        {
            if (!p.alive) playersDead++;
        }
        if (playersDead != Players.Count && !Players[PlayerOrder[PlayerTurn]].alive) IncrementTurn();
        foreach (Player p in Players.Values)
        {
            if (p.playerName == PlayerOrder[PlayerTurn]) p.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            else p.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }

    public void PlayerDone (string playerName)
    {
        Players[playerName].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        if(playerName == username) InfoPanel.GetComponentInChildren<Text>().text = "Waiting for Allies...";
    }

    public void AllCardsPicked ()
    {
        foreach (Player p in Players.Values) p.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    public void ReadyButtonPressed ()
    {
        if (Players[username].playerClass != "") Server.server.changeReadyState();
    }

    public void ToggleStats (bool stats)
    {
        foreach(Player p in Players.Values)
        {
            if(stats)
            {
                p.transform.GetChild(2).gameObject.SetActive(false);
                p.transform.GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                p.transform.GetChild(2).gameObject.SetActive(true);
                p.transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    public void StartGame(string[] playerOrder)
    {
        gameStarted = true;
        PlayerOrder = playerOrder;
        countdownDisplay.SetActive(false);
        ClassPanel.SetActive(false);
        ReadyButton.SetActive(false);
        allReady = false;
        ToggleStats(true);
        DrawPileButton.SetActive(true);
        DiscardPileButton.SetActive(true);
        foreach(Player p in Players.Values)
        {
            if (p.playerClass == "Warrior")
            {
                p.transform.GetChild(3).transform.GetChild(4).GetComponent<Text>().text = "Rage";
                p.health = 100;
                p.maxHealth = 100;
                p.healthBar.maxValue = 100;
                p.character.GetComponentInChildren<Slider>().maxValue = 100;
            }
            if (p.playerClass == "Rogue")
            {
                p.transform.GetChild(3).transform.GetChild(4).GetComponent<Text>().text = "Focus";
                p.health = 80;
                p.maxHealth = 80;
                p.healthBar.maxValue = 80;
                p.character.GetComponentInChildren<Slider>().maxValue = 80;
            }
            if (p.playerClass == "Mage")
            {
                p.transform.GetChild(3).transform.GetChild(4).GetComponent<Text>().text = "Aether";
                p.health = 80;
                p.maxHealth = 80;
                p.healthBar.maxValue = 80;
                p.character.GetComponentInChildren<Slider>().maxValue = 80;
            }
            if (p.playerClass == "Cleric")
            {
                p.transform.GetChild(3).transform.GetChild(4).GetComponent<Text>().text = "Faith";
                p.health = 100;
                p.maxHealth = 100;
                p.healthBar.maxValue = 100;
                p.character.GetComponentInChildren<Slider>().maxValue = 100;
            }
            p.transform.GetChild(3).transform.GetChild(2).GetComponent<Text>().text = p.health + " / " + p.maxHealth;
            p.transform.GetChild(3).transform.GetChild(5).GetComponent<Text>().text = "0 / 100";
        }
        Server.server.skillTree.ColorSkillTree();
        AssignCardPanels();
    }

    public void AssignCardPanels()
    {
        for(int i = 0; i < Players.Count; i++)
        {
            Server.server.cardPanel.DrawPilePanels[PlayerOrder[i]] = CardPanels.transform.GetChild(i * 2).gameObject;
            Server.server.cardPanel.DiscardPilePanels[PlayerOrder[i]] = CardPanels.transform.GetChild((i * 2) + 1).gameObject;
            Server.server.cardPanel.DrawPilePanels[PlayerOrder[i]].transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = PlayerOrder[i] + "'s Draw Pile";
            Server.server.cardPanel.DiscardPilePanels[PlayerOrder[i]].transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = PlayerOrder[i] + "'s Discard Pile";
            Server.server.cardPanel.DrawCards[PlayerOrder[i]] = new Dictionary<string, GameObject>();
            Server.server.cardPanel.DiscardCards[PlayerOrder[i]] = new Dictionary<string, GameObject>();
        }
    }

    public void AddPlayer (string playerName)
    {
        if(!Players.ContainsKey(playerName))
        {
            PlayerOrder[Players.Count] = playerName;

            if (playerName == username) newPlayer = Instantiate(PlayerPanel, PlayerListPanel.transform);
            else newPlayer = Instantiate(AllyPanel, PlayerListPanel.transform);
            newPlayer.GetComponentInChildren<Text>().text = playerName;
            newPlayer.GetComponentInChildren<Button>().onClick.AddListener(ReadyButtonPressed);

            Players[playerName] = newPlayer.GetComponent<Player>();
            Players[playerName].playerName = playerName;
            Players[playerName].alive = true;

            Players[playerName].transform.localPosition = new Vector3(0, 240 - (70 * (Players.Count - 1)), 0);
            allReady = false;

            for (int i = 0; i < Players.Count; i++)
            {
                Players[PlayerOrder[i]].GetComponentsInChildren<Text>()[2].text = (i + 1).ToString();
            }
        }
    }

    public void RemovePlayer(string playerName)
    {
        bool moveUp = false;
        bool clearHand = false;
        if (PlayerOrder[PlayerTurn] == playerName) clearHand = true;

        int playerNumber = 0;
        for(int i = 0; i < Players.Count; i++)
        {
            if (PlayerOrder[i] == playerName)
            {
                moveUp = true;
                playerNumber = i;
            }
            if (moveUp) Players[PlayerOrder[i]].gameObject.transform.localPosition += new Vector3(0, 70, 0);
        }
        if (playerNumber < PlayerTurn) PlayerTurn--;

        if(Players[playerName].character != null) Players[playerName].character.SetActive(false);
        Destroy(Players[playerName].gameObject);
        Players.Remove(playerName);
        
        List<string> newPlayerOrder = new List<string>(PlayerOrder);
        newPlayerOrder.Remove(playerName);
        PlayerOrder = newPlayerOrder.ToArray();

        for (int i = 0; i < Players.Count; i++)
        {
            Players[PlayerOrder[i]].playerNumber.GetComponent<Text>().text = (i + 1).ToString();
        }
        if (clearHand) Server.server.cardPanel.DeleteCards();
        Server.server.cardPanel.RemovePiles(playerName);
    }

    public void ChangePlayerClass(string playerName, char playerClass)
    {
        if(Players.ContainsKey(playerName))
        {
            switch (playerClass)
            {
                case 'w': Players[playerName].playerClass = "Warrior"; break;
                case 'r': Players[playerName].playerClass = "Rogue"; break;
                case 'm': Players[playerName].playerClass = "Mage"; break;
                case 'c': Players[playerName].playerClass = "Cleric"; break;
            }
            if (Players[playerName].ready) ChangePlayerReadyState(playerName);

            Players[playerName].gameObject.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Sprites/Icons/PlayerIcons/" + Players[playerName].playerClass + "Icon");

            if (Players[playerName].playerClass == "Warrior")
            {
                Players[playerName].gameObject.GetComponent<Image>().color = new Color(1, 0.7f, 0.7f);
                Players[playerName].resourceBar.transform.GetChild(1).GetComponentInChildren<Image>().color = new Color(1, 0, 0);
            }
            if (Players[playerName].playerClass == "Rogue")
            {
                Players[playerName].gameObject.GetComponent<Image>().color = new Color(0.7f, 1, 0.7f);
                Players[playerName].resourceBar.transform.GetChild(1).GetComponentInChildren<Image>().color = new Color(0, 1, 0);
            }
            if (Players[playerName].playerClass == "Mage")
            {
                Players[playerName].gameObject.GetComponent<Image>().color = new Color(0.7f, 0.7f, 1);
                Players[playerName].resourceBar.transform.GetChild(1).GetComponentInChildren<Image>().color = new Color(0, 0.5f, 1);
            }
            if (Players[playerName].playerClass == "Cleric")
            {
                Players[playerName].gameObject.GetComponent<Image>().color = new Color(1, 1, 0.7f);
                Players[playerName].resourceBar.transform.GetChild(1).GetComponentInChildren<Image>().color = new Color(1, 1, 0);
            }

            int[] ClassCounts = new int[4];
            foreach (GameObject g in Characters) g.SetActive(false);

            foreach (Player p in Players.Values)
            {
                if (p.playerClass != "")
                {
                    if (p.playerClass == "Warrior")
                    {
                        ClassCounts[0]++;
                        if (!Characters[6].activeSelf) character = Characters[6];
                        else character = Characters[7];
                    }
                    if (p.playerClass == "Rogue")
                    {
                        ClassCounts[1]++;
                        if (!Characters[0].activeSelf) character = Characters[0];
                        else character = Characters[1];
                    }
                    if (p.playerClass == "Mage")
                    {
                        ClassCounts[2]++;
                        if (!Characters[2].activeSelf) character = Characters[2];
                        else character = Characters[3];
                    }
                    if (p.playerClass == "Cleric")
                    {
                        ClassCounts[3]++;
                        if (!Characters[4].activeSelf) character = Characters[4];
                        else character = Characters[5];
                    }

                    p.character = character;
                    character.SetActive(true);
                    character.GetComponentsInChildren<Text>()[1].text = p.playerName;
                    if (!ReadyButton.activeSelf)
                    {
                        ReadyButton.SetActive(true);
                        InfoPanel.GetComponentInChildren<Text>().text = "Ready Up!";
                    }

                    if (p.playerName == username) character.tag = "Self";
                    else character.tag = "Ally";
                }
            }

            int maxDuplicates = 0;
            if (Players.Count >= 1 && Players.Count <= 3) maxDuplicates = 1;
            if (Players.Count >= 4 && Players.Count <= 8) maxDuplicates = 2;

            for (int i = 0; i < 4; i++)
            {
                ClassCards[i].GetComponent<Image>().color = new Color(1, 1, 1);
                foreach (Image image in ClassCards[i].transform.GetChild(0).GetComponentsInChildren<Image>()) image.color = new Color(1, 1, 1);
                foreach (Image image in ClassCards[i].transform.GetChild(1).GetComponentsInChildren<Image>()) image.color = new Color(1, 1, 1);
                if (ClassCounts[i] >= maxDuplicates)
                {
                    ClassCards[i].GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                    foreach (Image image in ClassCards[i].transform.GetChild(0).GetComponentsInChildren<Image>()) image.color = new Color(.5f, .5f, .5f);
                    foreach (Image image in ClassCards[i].transform.GetChild(1).GetComponentsInChildren<Image>()) image.color = new Color(.5f, .5f, .5f);
                }
            }
        }
    }

    public void UpdatePlayerStats(string playerName, int health, int maxHealth, int resource)
    {
        Players[playerName].health = health;
        Players[playerName].maxHealth = maxHealth;
        Players[playerName].resource = resource;

        Players[playerName].healthBar.maxValue = maxHealth;
        Players[playerName].healthBar.value = health;
        Players[playerName].healthBar.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = Color.HSVToRGB((float)health / Players[playerName].maxHealth / 3f, 1, 1);
        Players[playerName].resourceBar.value = resource;
        
        Players[playerName].transform.GetChild(3).transform.GetChild(2).GetComponent<Text>().text = health + " / " + Players[playerName].maxHealth;
        Players[playerName].transform.GetChild(3).transform.GetChild(5).GetComponent<Text>().text = resource + " / 100";

        Players[playerName].character.GetComponentInChildren<Slider>().maxValue = maxHealth;
        Players[playerName].character.GetComponentInChildren<Slider>().value = health;
        Players[playerName].character.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = health + " / " + Players[playerName].maxHealth;
        Players[playerName].character.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().color = Color.HSVToRGB((float)health / Players[playerName].maxHealth / 3f, 1, 1);
    }

    public void UpdatePlayerGold(string playerName, int gold)
    {
        Players[playerName].gold = gold;
        Server.server.dungeonMap.DisplayPlayerGold();
    }

    public void PlayerDied(string playerName)
    {
        Players[playerName].alive = false;
        Players[playerName].character.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        Players[playerName].gameObject.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Sprites/Icons/PlayerIcons/DeadIcon");
        if (PlayerOrder[PlayerTurn] == playerName) IncrementTurn();
    }

    public void PlayerRevived(string playerName)
    {
        Players[playerName].alive = true;
        Players[playerName].character.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        Players[playerName].gameObject.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Sprites/Icons/PlayerIcons/" + Players[playerName].playerClass + "Icon");
    }

    public void AddBuff(string playerName, string buffName, string buffId, int duration, Dictionary<string, int> values, List<string> decaying)
    {
        bool statsOnly;
        if (buffName == "Strength" || buffName == "Intellect" || buffName == "Armor" || buffName == "Resolve" || buffName == "Shield" || buffName == "Dodge" || buffName == "Regen") statsOnly = true;
        else statsOnly = false;
        int power = 0;
        if (values.ContainsKey("x")) power = values["x"];
        if (Descriptions.descriptions.buffDescs.ContainsKey(buffName)) Players[playerName].Buffs.Add(buffId, new Buff(buffName, buffId, Descriptions.descriptions.buffDescs[buffName], duration, power, statsOnly, false, values, decaying));
        else Players[playerName].Buffs.Add(buffId, new Buff(buffName, buffId, Descriptions.descriptions.skillDescs[buffName], duration, power, statsOnly, true, values, decaying));

        ArrangeBuffs(playerName);
    }

    //public void UpdateBuff(string playerName, string buffId, int duration)
    //{
    //    Players[playerName].Buffs[buffId].duration = duration;
    //    ArrangeBuffs(playerName);
    //}

    public void UpdateBuffDuration(string playerName, string buffID, int duration)
    {
        Players[playerName].Buffs[buffID].duration = duration;
        ArrangeBuffs(playerName);
    }

    public void UpdateBuffLocal(string playerName, string buffID, string localName, int power)
    {
        if (Players[playerName].Buffs[buffID].values.ContainsKey(localName)) Players[playerName].Buffs[buffID].values[localName] = power;
        else Players[playerName].Buffs[buffID].values.Add(localName, power);
        if(localName == "x") Players[playerName].Buffs[buffID].power = power;
        ArrangeBuffs(playerName);
    }

    public void RemoveBuff(string playerName, string buffId)
    {
        Players[playerName].Buffs.Remove(buffId);
        ArrangeBuffs(playerName);
    }

    //public void ActivateBuff(string playerName, string buffId, int duration, Dictionary<string, int> values)
    //{
    //    Players[playerName].Buffs[buffId].duration = duration;
    //    Players[playerName].Buffs[buffId].values = values;
    //    int power = 0;
    //    if (values.ContainsKey("x")) power = values["x"];
    //    Players[playerName].Buffs[buffId].power = power;
    //    ArrangeBuffs(playerName);
    //}

    public void UpdateLocalDecay(string playerName, string buffId, string decaying)
    {
        Players[playerName].Buffs[buffId].decaying.Add(decaying);
    }

    public void ArrangeBuffs(string playerName)
    {
        for (int i = Players[playerName].character.transform.GetChild(3).childCount; i > 0; i--)
        {
            Destroy(Players[playerName].character.transform.GetChild(3).GetChild(i - 1).gameObject);
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

        foreach (Buff b in Players[playerName].Buffs.Values)
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
                buff = Instantiate(newBuff, Players[playerName].character.transform.GetChild(3));
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

        if (playerName == Server.server.playerList.username)
        {
            for (int i = 0; i < SkillPanel.transform.childCount; i++)
            {
                SkillPanel.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
            }
        }

        foreach (Buff b in Players[playerName].Buffs.Values)
        {
            if (b.skill)
            {
                for (int i = 0; i < SkillPanel.transform.childCount; i++)
                {
                    if (SkillPanel.transform.GetChild(i).name == b.buffName)
                    {
                        SkillPanel.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                    }
                }
            }
            else if (!b.statsOnly)
            {
                buff = Instantiate(newBuff, Players[playerName].character.transform.GetChild(3));
                if (b.duration > 0) buff.GetComponentsInChildren<Text>()[0].text = b.duration.ToString();
                if (b.power > 0) buff.GetComponentsInChildren<Text>()[1].text = b.power.ToString();
                if (!b.buffName.Contains(" (Unleashed)")) buff.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/BuffIcons/" + b.buffName);
                else
                {
                    int pos = b.buffName.IndexOf(" (Unleashed)");
                    string newBuffName = b.buffName.Remove(pos);
                    buff.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/BuffIcons/" + newBuffName);
                }
                buff.name = b.buffName;
                buffObjects++;
                buff.transform.localPosition = new Vector2((buffObjects - 1) % 4 * 35 - 52.5f, (buffObjects - 1) / 4 * 35 - 52.5f);
            }
        }
    }

    public void ChangePlayerAbilityActive(string playerName, bool active)
    {
        Players[playerName].activeSpecial = active;
        Server.server.cardPanel.UpdateCardDescriptions();
        if (active) Players[playerName].character.transform.GetChild(0).gameObject.SetActive(true);
        else Players[playerName].character.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ChangePlayerReadyState(string playerName){
        if(Players.ContainsKey(playerName))
        {
            Players[playerName].ready = !Players[playerName].ready;

            ColorBlock buttonColors = Players[playerName].gameObject.GetComponentInChildren<Button>().colors;
            if (Players[playerName].ready)
            {
                buttonColors.normalColor = new Color(0, 1, 0);
                buttonColors.highlightedColor = new Color(0, 0.9f, 0);
                buttonColors.pressedColor = new Color(0, 0.8f, 0);
                buttonColors.selectedColor = new Color(0, 0.9f, 0);
                buttonColors.disabledColor = new Color(0, 1, 0);
                Players[playerName].gameObject.GetComponentsInChildren<Text>()[1].text = "READY";
            }
            else
            {
                buttonColors.normalColor = new Color(1, 0, 0);
                buttonColors.highlightedColor = new Color(0.9f, 0, 0);
                buttonColors.pressedColor = new Color(0.8f, 0, 0);
                buttonColors.selectedColor = new Color(0.9f, 0, 0);
                buttonColors.disabledColor = new Color(1, 0, 0);
                Players[playerName].gameObject.GetComponentsInChildren<Text>()[1].text = "NOT READY";
            }
            Players[playerName].gameObject.GetComponentInChildren<Button>().colors = buttonColors;

            if (playerName == username)
            {
                //buttonColors = ReadyButton.GetComponentInChildren<Button>().colors;
                if (Players[username].ready)
                {
                    //buttonColors.normalColor = new Color(0, 1, 0);
                    //buttonColors.highlightedColor = new Color(0, 0.9f, 0);
                    //buttonColors.pressedColor = new Color(0, 0.8f, 0);
                    //buttonColors.disabledColor = new Color(0, 1, 0);
                    ReadyButton.gameObject.GetComponentInChildren<Text>().text = "READY";
                }
                else
                {
                    //buttonColors.normalColor = new Color(1, 0, 0);
                    //buttonColors.highlightedColor = new Color(0.9f, 0, 0);
                    //buttonColors.pressedColor = new Color(0.8f, 0, 0);
                    //buttonColors.disabledColor = new Color(1, 0, 0);
                    ReadyButton.gameObject.GetComponentInChildren<Text>().text = "NOT READY";
                }
                ReadyButton.GetComponentInChildren<Button>().colors = buttonColors;
            }

            int numReady = 0;

            foreach (Player p in Players.Values)
            {
                if (p.ready) numReady++;
            }

            if (numReady == Players.Count) allReady = true;
            else allReady = false;
        }
    }

    public void ToggleKickable(bool kickable)
    {
        if(kickable)
        {
            Players[PlayerOrder[PlayerTurn]].transform.GetChild(3).gameObject.SetActive(false);
            Players[PlayerOrder[PlayerTurn]].transform.GetChild(5).gameObject.SetActive(true);
            Players[PlayerOrder[PlayerTurn]].transform.GetChild(5).GetComponentInChildren<Text>().text = "KICK? (0/" + (Players.Count - 1) + " Votes)";
            Players[PlayerOrder[PlayerTurn]].transform.GetChild(5).GetComponent<Button>().onClick.AddListener(VoteKick);
            ColorBlock buttonColors = Players[PlayerOrder[PlayerTurn]].gameObject.GetComponentInChildren<Button>().colors;
            buttonColors.normalColor = new Color(1, 1, 0);
            buttonColors.highlightedColor = new Color(0.9f, 0.9f, 0);
            buttonColors.pressedColor = new Color(0.8f, 0.8f, 0);
            buttonColors.disabledColor = new Color(1, 1, 0);
            Players[PlayerOrder[PlayerTurn]].gameObject.GetComponentInChildren<Button>().colors = buttonColors;
        }
        else
        {
            Players[PlayerOrder[PlayerTurn]].transform.GetChild(3).gameObject.SetActive(true);
            Players[PlayerOrder[PlayerTurn]].transform.GetChild(5).gameObject.SetActive(false);
        }
    }

    public void VoteKick()
    {
        if(Server.server.playerList.PlayerOrder[Server.server.playerList.PlayerTurn] != Server.server.playerList.username)
        {
            Server.server.voteKick();
            ColorBlock buttonColors = Players[PlayerOrder[PlayerTurn]].gameObject.GetComponentInChildren<Button>().colors;
            buttonColors.normalColor = new Color(1, 0, 0);
            buttonColors.highlightedColor = new Color(0.9f, 0, 0);
            buttonColors.pressedColor = new Color(0.8f, 0, 0);
            buttonColors.disabledColor = new Color(1, 0, 0);
            Players[PlayerOrder[PlayerTurn]].gameObject.GetComponentInChildren<Button>().colors = buttonColors;
        }
    }

    public void UpdateVotes(int votes)
    {
        Players[PlayerOrder[PlayerTurn]].transform.GetChild(5).GetComponentInChildren<Text>().text = "KICK? (" + votes + "/" + (Players.Count - 1) + " Votes)";
    }
}
