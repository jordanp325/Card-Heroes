using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Activities : MonoBehaviour
{
    public GameObject FountainPanel;
    public GameObject FountainCards;
    public GameObject TreasurePanel;
    public GameObject TreasureCards;
    public GameObject ShopPanel;
    public GameObject ShopCards;
    public GameObject Cards;

    public char queuedRoom;

    public bool isMimic;
    public bool openChest;
    public GameObject TreasureKey;
    public GameObject TreasureChest;
    public int treasureTimer = 0;

    public GameObject mimicCountdownDisplay;
    public float mimicCountdown;
    public float red;
    public float green;
    public bool mimicReady;
    public Dictionary<string, bool> mimicVotes = new Dictionary<string, bool>();
    public int runners;
    public int fighters;

    // Start is called before the first frame update
    void Start()
    {
        mimicCountdown = 3;
        red = 0;
        green = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!Server.server.dungeonMap.fade)
        {
            if (openChest && treasureTimer <= 50)
            {
                treasureTimer++;
                if (treasureTimer > 0 && treasureTimer <= 20) TreasureKey.GetComponent<Image>().color = new Color(1, 1, 1, (float)treasureTimer / 20);
                if (treasureTimer > 20 && treasureTimer < 40) TreasureKey.transform.localPosition = new Vector3(-190 + (treasureTimer - 20) * 5, -20);
                if (treasureTimer == 50)
                {
                    TreasureKey.SetActive(false);
                    TreasureKey.transform.localPosition = new Vector3(-190, 0);
                    if (isMimic)
                    {
                        TreasureChest.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Map/Mimic");
                        TreasureCards.SetActive(true);
                    }
                    else TreasureChest.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Map/ChestOpen");
                    openChest = false;
                    treasureTimer = 0;
                }
            }
            if (mimicReady && !mimicCountdownDisplay.activeSelf)
            {
                mimicCountdownDisplay.SetActive(true);
            }
            if (mimicReady && mimicCountdown >= 0)
            {
                mimicCountdown -= .02f;
                mimicCountdownDisplay.GetComponent<Image>().fillAmount = mimicCountdown / 3;
                if (mimicCountdown <= 3 && mimicCountdown > 1.5f) red += (1 / 75);
                if (mimicCountdown <= 1.5f && mimicCountdown > 0) green -= (1 / 75);
                mimicCountdownDisplay.GetComponent<Image>().color = new Color(red, green, 0);
                mimicCountdownDisplay.GetComponentInChildren<Text>().color = new Color(red, green, 0);
                mimicCountdownDisplay.GetComponentInChildren<Text>().text = Mathf.Round(mimicCountdown + 0.5f).ToString();
            }
            if (!mimicReady && mimicCountdown < 3 && mimicCountdown > 0)
            {
                mimicCountdown = 3;
                red = 0;
                green = 1;
                mimicCountdownDisplay.SetActive(false);
            }
        }
    }

    public void ResetMimicCountdown()
    {
        mimicCountdown = 3;
        red = 0;
        green = 1;
    }

    public void MimicRoom()
    {
        isMimic = true;
    }

    public void DisplayActivity()
    {
        FountainPanel.SetActive(false);
        FountainCards.SetActive(false);
        TreasurePanel.SetActive(false);
        TreasureCards.SetActive(false);
        ShopPanel.SetActive(false);
        ShopCards.SetActive(false);
        switch (queuedRoom)
        {
            case 'X':
                Cards.SetActive(true);
                break;
            case 'H':
                FountainPanel.SetActive(true);
                FountainCards.SetActive(true);
                switch (Server.server.playerList.Me().playerClass)
                {
                    case "Warrior": FountainCards.transform.GetChild(1).GetComponent<Image>().sprite = Server.server.cardPanel.CardColors[0]; break;
                    case "Rogue": FountainCards.transform.GetChild(1).GetComponent<Image>().sprite = Server.server.cardPanel.CardColors[1]; break;
                    case "Mage": FountainCards.transform.GetChild(1).GetComponent<Image>().sprite = Server.server.cardPanel.CardColors[2]; break;
                    case "Cleric": FountainCards.transform.GetChild(1).GetComponent<Image>().sprite = Server.server.cardPanel.CardColors[3]; break;
                }
                break;
            case 'T':
                TreasurePanel.SetActive(true);
                TreasureChest.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Map/Chest");
                if (Server.server.dungeonMap.keys > 0)
                {
                    isMimic = false;
                    openChest = true;
                    Server.server.dungeonMap.RemoveKey();
                }
                break;
            case 'S':
                ShopPanel.SetActive(true);
                ShopCards.SetActive(true);
                break;
            case 'M':
                Cards.SetActive(true);
                break;
            case 'B':
                Server.server.dungeonMap.RemoveKey();
                Cards.SetActive(true);
                break;
            default:

                break;
        }
    }

    public void HideActvities()
    {
        FountainPanel.SetActive(false);
        FountainCards.SetActive(false);
        TreasurePanel.SetActive(false);
        TreasureCards.SetActive(false);
        ShopPanel.SetActive(false);
        ShopCards.SetActive(false);
    }

    public void SendMimicVote(bool fight)
    {
        Server.server.fightMimic(fight);
    }

    public void RecieveMimicVote(string playerName, bool fight)
    {
        if (mimicVotes.ContainsKey(playerName)) mimicVotes[playerName] = fight;
        else mimicVotes.Add(playerName, fight);
        foreach(bool b in mimicVotes.Values)
        {
            if (b) fighters++;
            else runners++;
        }
        mimicCountdownDisplay.transform.GetChild(1).GetComponent<Text>().text = "Votes to Run: " + runners;
        mimicCountdownDisplay.transform.GetChild(2).GetComponent<Text>().text = "Votes to Run: " + fighters;
        if (fighters > Server.server.playerList.Players.Count / 2 || runners > Server.server.playerList.Players.Count / 2) mimicReady = true;
        else mimicReady = false;
    }
}
