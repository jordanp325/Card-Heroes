using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour {

    public GameObject newCard;
    public GameObject newLootCard;
    public GameObject newDeckCard;
    public GameObject newRemovalCard;
    public GameObject cardPanels;
    public Dictionary<string, GameObject> DrawPilePanels = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> DiscardPilePanels = new Dictionary<string, GameObject>();
    public int playerNumberPile;
    public GameObject RemoveCardPanel;
    public GameObject CardPanelScroll;

    GameObject card;
    GameObject destroyCard;
    GameObject removalCard;

    public string CardName;
    public string CardID;

    Dictionary<string, GameObject> CardDict = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> LootCardDict = new Dictionary<string, GameObject>();
    List<GameObject> Cards = new List<GameObject>();
    List<GameObject> RemovalCards = new List<GameObject>();

    public Dictionary<string,Dictionary<string, GameObject>> DrawCards = new Dictionary<string, Dictionary<string, GameObject>>();
    public Dictionary<string, Dictionary<string, GameObject>> DiscardCards = new Dictionary<string, Dictionary<string, GameObject>>();

    int drawCardNumber;
    int discardCardNumber;
    int cardNumber;
    int totalCards;
    int cardsPlayed;
    public int lootCards;
    string[] lootString = new string[2];

    public Sprite[] CardColors = new Sprite[5];

    public bool cardSelected;
    public string selectedCardID;
    public char? cardTargeting;
    public int[,] targets = new int[2,4];
    public string cardTarget;
    bool lootCard;

    public GameObject EnemyPanel;

    public GameObject InfoPanel;
    public GameObject ConfirmButton;
    public GameObject ConfirmRemovalButton;
    public GameObject SpecialButton;
    public GameObject DrawPileButton;
    public GameObject DiscardPileButton;

    // Use this for initialization
    void Start () {
        
    }

    public void CardSelected(string cn, string ci, char? ct, bool lc)
    {
        Server.server.enemyManager.CorrectHealthBars(); //Placed here to prevent unexplainable bugged health bar stretching

        CardName = cn;
        CardID = ci;
        lootCard = lc;
        
        if (lootCard)
        {
            HighlightLootCard(cn);
            ConfirmButton.SetActive(true);
        }
        else
        {
            cardTargeting = ct;
            cardTarget = null;
            cardSelected = true;
            ClearTargets();

            if (cardTargeting == null)
            {
                if (!lc) InfoPanel.GetComponentInChildren<Text>().text = "Confirm Turn";
                else InfoPanel.GetComponentInChildren<Text>().text = "Claim Card";
                ConfirmButton.SetActive(true);
            }
            else
            {
                InfoPanel.GetComponentInChildren<Text>().text = "Select a Target";
                ConfirmButton.SetActive(false);
            }
        }
        Server.server.itemManager.DeselectItem();
    }

    public void DeselectCard()
    {
        cardSelected = false;
        if (Server.server.playerList.myTurn()) InfoPanel.GetComponentInChildren<Text>().text = "Select a Card";
        else InfoPanel.GetComponentInChildren<Text>().text = "It is " + Server.server.playerList.getPlayerTurn().playerName + "'s Turn";
        ConfirmButton.SetActive(false);
        ClearTargets();
    }

    public void ConfirmCard()
    {
        if (lootCard)
        {
            if(lootCards > 1)
            {
                lootCards--;
                cardSelected = false;
                if (CardName == "None")
                {
                    lootString[1] = "";
                    HighlightLootCard("");
                }
                else
                {
                    totalCards--;
                    LootCardDict.Remove(CardName);
                    foreach (GameObject c in Cards)
                    {
                        if (c.GetComponent<Card>().cardName == CardName)
                        {
                            Destroy(c);
                            Cards.Remove(c);
                            break;
                        }
                    }
                    lootString[1] = CardName;
                }
            }
            else
            {
                if (CardName == "None") lootString[0] = "";
                else lootString[0] = CardName;
                Server.server.pickCardFromPool(lootString[0], lootString[1]);
                lootString[0] = "";
                lootString[1] = "";
            }
        }
        else
        {
            Server.server.activateCard(CardID, cardTarget);
            ClearTargets();
        }
        ConfirmButton.SetActive(false);
        SpecialButton.SetActive(false);
    }

    public void ActivateSpecial()
    {
        Server.server.activateClassAbility();
        SpecialButton.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0) && cardSelected && cardTargeting != null)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.TransformDirection(Vector3.forward));
            foreach(RaycastHit2D r in hits)
            {
                if(r.collider != null)
                {
                    if(r.transform.gameObject.tag == "Enemy" || r.transform.gameObject.tag == "Self" || r.transform.gameObject.tag == "Ally")
                    {
                        GameObject target = r.transform.gameObject;
                        cardTarget = null;

                        //if (target.tag == "Tooltip")
                        //{
                        //    for (int i = 0; i < EnemyPanel.transform.childCount; i++)
                        //    {
                        //        if (EnemyPanel.transform.GetChild(i).name == target.GetComponent<Tooltip>().parentName)
                        //        {
                        //            target = EnemyPanel.transform.GetChild(i).gameObject;
                        //            break;
                        //        }
                        //    }
                        //}

                        if (target.tag == "Enemy" && (cardTargeting == 'e' || cardTargeting == 'n'))
                        {
                            if (cardTarget == target.GetComponent<Enemy>().enemyID)
                            {
                                cardTarget = "";
                                Server.server.selectTarget(target.GetComponent<Enemy>().enemyID, false);
                            }
                            else
                            {
                                cardTarget = target.GetComponent<Enemy>().enemyID;
                                Server.server.selectTarget(target.GetComponent<Enemy>().enemyID, true);
                            }
                        }
                        if (target.tag == "Self" && (cardTargeting == 'p' || cardTargeting == 'n'))
                        {
                            if (cardTarget == target.GetComponentInChildren<Text>().text)
                            {
                                cardTarget = "";
                                Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, false);
                            }
                            else
                            {
                                cardTarget = target.GetComponentsInChildren<Text>()[1].text;
                                Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, true);
                            }
                        }
                        if (target.tag == "Ally" && (cardTargeting == 'p' || cardTargeting == 'a' || cardTargeting == 'n'))
                        {
                            if (cardTarget == target.GetComponentInChildren<Text>().text)
                            {
                                cardTarget = "";
                                Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, false);
                            }
                            else
                            {
                                cardTarget = target.GetComponentsInChildren<Text>()[1].text;
                                Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, true);
                            }
                        }
                        if ((target.tag == "Ally" || target.tag == "Self") && cardTargeting == 'd')
                        {
                            if(!Server.server.playerList.Players[target.GetComponentsInChildren<Text>()[1].text].alive)
                            {
                                if (cardTarget == target.GetComponentInChildren<Text>().text)
                                {
                                    cardTarget = "";
                                    Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, false);
                                }
                                else
                                {
                                    cardTarget = target.GetComponentsInChildren<Text>()[1].text;
                                    Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, true);
                                }
                            }
                        }

                        if (cardTarget != "" && cardTarget != null)
                        {
                            InfoPanel.GetComponentInChildren<Text>().text = "Confirm Turn";
                            ConfirmButton.SetActive(true);
                        }
                        else if (cardTarget != null)
                        {
                            InfoPanel.GetComponentInChildren<Text>().text = "Select a Target";
                            ConfirmButton.SetActive(false);
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ViewDrawPile(false);
            ViewDiscardPile(false);
        }
    }

    public void HighlightTarget(string ID, bool targeted)
    {
        ClearTargets();
        foreach(Player p in Server.server.playerList.Players.Values)
        {
            if(p.playerName == ID)
            {
                if (targeted)
                {
                    p.character.GetComponentsInChildren<Text>()[1].color = new Color(1, 1, 1);
                    p.character.GetComponentsInChildren<Outline>()[1].effectColor = new Color(0, 0, 0);
                }
                else
                {
                    p.character.GetComponentsInChildren<Text>()[1].color = new Color(0, 0, 0);
                    p.character.GetComponentsInChildren<Outline>()[1].effectColor = new Color(1, 1, 1);
                }
            }
        }
        foreach(Enemy e in Server.server.enemyManager.Enemies.Values)
        {
            if (e.enemyID == ID)
            {
                if (targeted)
                {
                    e.GetComponentsInChildren<Text>()[1].color = new Color(1, 1, 1);
                    e.GetComponentsInChildren<Outline>()[1].effectColor = new Color(0, 0, 0);
                }
                else
                {
                    e.GetComponentsInChildren<Text>()[1].color = new Color(0, 0, 0);
                    e.GetComponentsInChildren<Outline>()[1].effectColor = new Color(1, 1, 1);
                }
            }
        }
    }

    public void ClearTargets()
    {
        foreach (Player p in Server.server.playerList.Players.Values)
        {
            p.character.GetComponentsInChildren<Text>()[1].color = new Color(0, 0, 0);
            p.character.GetComponentsInChildren<Outline>()[1].effectColor = new Color(1, 1, 1);
        }
        foreach (Enemy e in Server.server.enemyManager.Enemies.Values)
        {
            e.GetComponentsInChildren<Text>()[1].color = new Color(0, 0, 0);
            e.GetComponentsInChildren<Outline>()[1].effectColor = new Color(1, 1, 1);
        }
    }

    public void AddCard(string cardID, char? cardTargeting)
    {
        card = Instantiate(newCard, CardPanelScroll.transform);
        card.GetComponent<Card>().cardID = cardID;
        card.GetComponent<Card>().cardName = DrawCards[Server.server.playerList.getPlayerTurn().playerName][cardID].GetComponent<Card>().cardName;
        card.GetComponent<Card>().color = DrawCards[Server.server.playerList.getPlayerTurn().playerName][cardID].GetComponent<Card>().color;
        card.GetComponent<Card>().locals = DrawCards[Server.server.playerList.getPlayerTurn().playerName][cardID].GetComponent<Card>().locals;

        if (Server.server.playerList.getPlayerTurn().playerClass == "Mage" && Server.server.playerList.getPlayerTurn().activeSpecial)
        {
            card.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[card.GetComponent<Card>().cardName + " (Unleashed)"];
        }
        else
        {
            card.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[card.GetComponent<Card>().cardName];
        }
        card.GetComponent<Card>().cardTargeting = cardTargeting;
        CardDict.Add(card.GetComponent<Card>().cardID, card);
        Cards.Add(card);
        card.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = card.GetComponent<Card>().cardName;
        card.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Text>().text = card.GetComponent<Card>().cardName;
        card.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>().text = card.GetComponent<Card>().description;
        switch(card.GetComponent<Card>().color)
        {
            case 'r': card.GetComponent<Image>().sprite = CardColors[0]; break;
            case 'y': card.GetComponent<Image>().sprite = CardColors[1]; break;
            case 'g': card.GetComponent<Image>().sprite = CardColors[2]; break;
            case 'b': card.GetComponent<Image>().sprite = CardColors[3]; break;
        }

        DrawCard(card.GetComponent<Card>().cardID);
        if (DrawCards[Server.server.playerList.getPlayerTurn().playerName].Count == 0) ReshuffleDiscard(Server.server.playerList.getPlayerTurn().playerName);

        UpdateCardDescriptions();

        cardNumber = 0;
        foreach(GameObject c in Cards)
        {
            c.transform.localPosition = new Vector3(170 * cardNumber - 85 * totalCards, 0);
            cardNumber++;
        }
        totalCards++;

        CardPanelScroll.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (totalCards * 170) + 20);

        if(Server.server.playerList.myTurn() && Server.server.playerList.Me().activeSpecial == false && Server.server.playerList.Me().resource == 100)
        {
            SpecialButton.SetActive(true);
        }

        if (Server.server.playerList.myTurn()) InfoPanel.GetComponentInChildren<Text>().text = "Select a Card";
        else InfoPanel.GetComponentInChildren<Text>().text = "It is " + Server.server.playerList.getPlayerTurn().playerName + "'s Turn";
    }

    public void UpdateCardDescriptions()
    {
        foreach(GameObject c in CardDict.Values)
        {
            if (Server.server.playerList.getPlayerTurn().playerClass == "Mage" && Server.server.playerList.getPlayerTurn().activeSpecial)
            {
                c.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[c.GetComponent<Card>().cardName + " (Unleashed)"];
            }
            else c.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[c.GetComponent<Card>().cardName];

            if(c.GetComponent<Card>().locals != null)
            {
                if (c.GetComponent<Card>().locals.ContainsKey("x")) c.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[c.GetComponent<Card>().cardName].Replace("[X]", c.GetComponent<Card>().locals["x"].ToString());
                if (c.GetComponent<Card>().locals.ContainsKey("y")) c.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[c.GetComponent<Card>().cardName].Replace("[Y]", c.GetComponent<Card>().locals["y"].ToString());
                if (c.GetComponent<Card>().locals.ContainsKey("z")) c.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[c.GetComponent<Card>().cardName].Replace("[Z]", c.GetComponent<Card>().locals["z"].ToString());
            }
            c.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>().text = c.GetComponent<Card>().description;
        }
    }

    public void AddLootCard(string cardName)
    {
        if(LootCardDict.Count == 0)
        {
            card = Instantiate(newLootCard, CardPanelScroll.transform);
            card.GetComponent<Card>().cardName = "None";
            card.GetComponent<Card>().description = "Don't pick any card";
            card.GetComponent<Card>().color = 'n';
            LootCardDict.Add("None", card);
            Cards.Add(card);
            card.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = "None";
            card.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Text>().text = "None";
            card.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>().text = "Don't pick any card";
            card.GetComponent<Image>().sprite = CardColors[4];
            cardNumber = 0;
            foreach (GameObject c in Cards)
            {
                c.transform.localPosition = new Vector3(170 * cardNumber - 85 * totalCards, 0);
                cardNumber++;
            }
            totalCards++;
        }
        card = Instantiate(newLootCard, CardPanelScroll.transform);
        card.GetComponent<Card>().cardName = cardName;
        if (Descriptions.descriptions.cardDescs.ContainsKey(cardName)) card.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[cardName];
        else card.GetComponent<Card>().description = "ERROR: Description not found";
        card.GetComponent<Card>().color = GetClassColor(Server.server.playerList.username);
        if (!LootCardDict.ContainsKey(cardName)) LootCardDict.Add(cardName, card);
        Cards.Add(card);
        card.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = cardName;
        card.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Text>().text = cardName;
        card.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>().text = card.GetComponent<Card>().description;
        switch (card.GetComponent<Card>().color)
        {
            case 'r': card.GetComponent<Image>().sprite = CardColors[0]; break;
            case 'y': card.GetComponent<Image>().sprite = CardColors[1]; break;
            case 'g': card.GetComponent<Image>().sprite = CardColors[2]; break;
            case 'b': card.GetComponent<Image>().sprite = CardColors[3]; break;
        }

        cardNumber = 0;
        foreach (GameObject c in Cards)
        {
            c.transform.localPosition = new Vector3(170 * cardNumber - 85 * totalCards, 0);
            cardNumber++;
        }
        totalCards++;

        InfoPanel.GetComponentInChildren<Text>().text = "Select a Card";
    }

	public void HighlightLootCard(string cardName)
    {
        foreach (GameObject c in LootCardDict.Values)
        {
            c.GetComponent<Image>().color = new Color(1, 1, 1);
            foreach (Image image in c.transform.GetChild(0).GetComponentsInChildren<Image>()) image.color = new Color(1, 1, 1, 0);
            foreach (Image image in c.transform.GetChild(1).GetComponentsInChildren<Image>()) image.color = new Color(1, 1, 1, 0);
        }
        if(LootCardDict.ContainsKey(cardName))
        {
            LootCardDict[cardName].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            foreach (Image image in LootCardDict[cardName].transform.GetChild(0).GetComponentsInChildren<Image>()) image.color = new Color(0.5f, 0.5f, 0.5f, 0);
            foreach (Image image in LootCardDict[cardName].transform.GetChild(1).GetComponentsInChildren<Image>()) image.color = new Color(0.5f, 0.5f, 0.5f, 0);
        }
    }

    public void HighlightCard(string cardID)
    {
        selectedCardID = cardID;
        foreach(GameObject c in CardDict.Values)
        {
            if(c.GetComponent<Card>().cardID == cardID)
            {
                c.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                foreach (Image image in c.transform.GetChild(0).GetComponentsInChildren<Image>()) image.color = new Color(0.5f, 0.5f, 0.5f, 0);
                foreach (Image image in c.transform.GetChild(1).GetComponentsInChildren<Image>()) image.color = new Color(0.5f, 0.5f, 0.5f, 0);
            }
            else
            {
                c.GetComponent<Image>().color = new Color(1, 1, 1);
                foreach (Image image in c.transform.GetChild(0).GetComponentsInChildren<Image>()) image.color = new Color(1, 1, 1, 0);
                foreach (Image image in c.transform.GetChild(1).GetComponentsInChildren<Image>()) image.color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void CardLooted()
    {
        totalCards = 0;
        LootCardDict.Clear();
        foreach (GameObject c in Cards) Destroy(c);
        Cards.Clear();
        cardSelected = false;
    }

    public void DisplayCardRemoval()
    {
        foreach(GameObject g in RemovalCards)
        {
            Destroy(g);
        }
        RemovalCards.Clear();
        RemoveCardPanel.SetActive(true);
        int removeCardNumber = 0;
        foreach(GameObject g in DrawCards[Server.server.playerList.username].Values)
        {
            GameObject card = Instantiate(newRemovalCard, RemoveCardPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0));
            card.GetComponent<Card>().cardID = g.GetComponent<Card>().cardID;
            card.GetComponent<Card>().cardName = g.GetComponent<Card>().cardName;
            card.GetComponent<Card>().description = g.GetComponent<Card>().description;
            card.GetComponent<Card>().color = g.GetComponent<Card>().color;
            card.GetComponent<Image>().sprite = g.GetComponent<Image>().sprite;
            card.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = card.GetComponent<Card>().cardName;
            card.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Text>().text = card.GetComponent<Card>().cardName;
            card.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>().text = card.GetComponent<Card>().description;
            RemovalCards.Add(card);
            RemoveCardPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1260, (removeCardNumber / 6) * 260 + 390);
            card.transform.localPosition = new Vector3(180 * (removeCardNumber % 6) - 450, -260 * (removeCardNumber / 6) + 110);
            removeCardNumber++;
        }
        InfoPanel.GetComponentInChildren<Text>().text = "Pick a Card";
    }

    public void HighlightRemovalCard(string cardID)
    {
        foreach (GameObject c in RemovalCards)
        {
            if (c.GetComponent<Card>().cardID == cardID)
            {
                removalCard = c;
                c.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                foreach (Image image in c.transform.GetChild(0).GetComponentsInChildren<Image>()) image.color = new Color(0.5f, 0.5f, 0.5f, 0);
                foreach (Image image in c.transform.GetChild(1).GetComponentsInChildren<Image>()) image.color = new Color(0.5f, 0.5f, 0.5f, 0);
            }
            else
            {
                c.GetComponent<Image>().color = new Color(1, 1, 1);
                foreach (Image image in c.transform.GetChild(0).GetComponentsInChildren<Image>()) image.color = new Color(1, 1, 1, 0);
                foreach (Image image in c.transform.GetChild(1).GetComponentsInChildren<Image>()) image.color = new Color(1, 1, 1, 0);
            }
        }
        ConfirmRemovalButton.SetActive(true);
        InfoPanel.GetComponentInChildren<Text>().text = "Confirm Removal";
    }

    public void SelectRemovalCard()
    {
        string[] removalCardIDs = new string[1]; //You're gonna need to change this later
        removalCardIDs[0] = removalCard.GetComponent<Card>().cardID;
        Server.server.removeCard(Server.server.skillTree.selectedSkill, removalCardIDs);
        ConfirmRemovalButton.SetActive(false);
    }

    public void RemoveCard(string playerName, string cardID)
    {
        if(playerName == Server.server.playerList.username) RemoveCardPanel.SetActive(false);
        GameObject removeCard = new GameObject();
        removeCard = DrawCards[playerName][cardID];
        DrawCards[playerName].Remove(cardID);
        Destroy(removeCard);
        RearrangeDecks(playerName);
    }

	public void PlayCard(string playerName, string cardID){
        cardsPlayed++;
        ClearTargets();

        if (Server.server.playerList.Players[playerName].playerClass == "Rogue" && Server.server.playerList.Players[playerName].activeSpecial && cardsPlayed == 1)
        {
            if (Server.server.playerList.myTurn()) InfoPanel.GetComponentInChildren<Text>().text = "Select a Card";
            else InfoPanel.GetComponentInChildren<Text>().text = "It is " + Server.server.playerList.PlayerOrder[Server.server.playerList.PlayerTurn] + "'s Turn";
            bool removeCard = true;
            foreach (GameObject c in Cards)
            {
                if (c.GetComponent<Card>().cardID == cardID && c.GetComponent<Card>().cardID == selectedCardID && removeCard)
                {
                    DiscardCard(c.GetComponent<Card>().cardID);
                    destroyCard = c;
                    removeCard = false;
                }
            }
            Cards.Remove(destroyCard);
            CardDict.Remove(destroyCard.GetComponent<Card>().cardID);
            Destroy(destroyCard);
        }
        else
        {
            ClearHand();
        }
        cardSelected = false;
        foreach (Player p in Server.server.playerList.Players.Values) p.character.GetComponentInChildren<Text>().color = new Color(0, 0, 0);
        foreach (Enemy e in Server.server.enemyManager.Enemies.Values) e.GetComponentInChildren<Text>().color = new Color(0, 0, 0);
    }

    public void ClearHand()
    {
        cardsPlayed = 0;
        totalCards = 0;
        CardDict.Clear();
        foreach (GameObject c in Cards)
        {
            DiscardCard(c.GetComponent<Card>().cardID);
            Destroy(c);
        }
        Cards.Clear();
        Server.server.playerList.IncrementTurn();
    }

    public void DeleteCards()
    {
        cardsPlayed = 0;
        totalCards = 0;
        CardDict.Clear();
        foreach (GameObject c in Cards)  Destroy(c);
        Cards.Clear();
        Server.server.playerList.IncrementTurn();
    }

    public void RecievePlayerDeck(string playerName, string[] ids, string[] names, Dictionary<string,int>[] locals)
    {
        for(int i = 0; i < names.Length; i++)
        {
            card = Instantiate(newDeckCard, DrawPilePanels[playerName].transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0));
            
            DrawCards[playerName][ids[i]] = card;
            card.GetComponent<Card>().cardID = ids[i];
            card.GetComponent<Card>().cardName = names[i];
            card.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[names[i]];
            card.GetComponent<Card>().color = GetClassColor(playerName);
            card.GetComponent<Card>().locals = locals[i];
            card.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = names[i];
            card.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Text>().text = names[i];
            card.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>().text = Descriptions.descriptions.cardDescs[names[i]];
            switch (card.GetComponent<Card>().color)
            {
                case 'r': card.GetComponent<Image>().sprite = CardColors[0]; break;
                case 'y': card.GetComponent<Image>().sprite = CardColors[1]; break;
                case 'g': card.GetComponent<Image>().sprite = CardColors[2]; break;
                case 'b': card.GetComponent<Image>().sprite = CardColors[3]; break;
            }
        }
        RearrangeDecks(playerName);
    }

    public void AddCardToDeck(string playerName, string cardID, string cardName, Dictionary<string,int> locals)
    {
        card = Instantiate(newDeckCard, DrawPilePanels[playerName].transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0));
        DrawCards[playerName][cardID] = card;
        card.GetComponent<Card>().cardID = cardID;
        card.GetComponent<Card>().cardName = cardName;
        card.GetComponent<Card>().description = Descriptions.descriptions.cardDescs[cardName];
        card.GetComponent<Card>().color = GetClassColor(playerName);
        card.GetComponent<Card>().locals = locals;
        card.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = cardName;
        card.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Text>().text = cardName;
        card.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>().text = card.GetComponent<Card>().description;
        switch (card.GetComponent<Card>().color)
        {
            case 'r': card.GetComponent<Image>().sprite = CardColors[0]; break;
            case 'y': card.GetComponent<Image>().sprite = CardColors[1]; break;
            case 'g': card.GetComponent<Image>().sprite = CardColors[2]; break;
            case 'b': card.GetComponent<Image>().sprite = CardColors[3]; break;
        }
        ReshuffleDiscard(playerName);
        UpdateCardDescriptions();
    }

    public void ChangeLocals(string playerName, string cardID, Dictionary<string,int> locals)
    {
        GameObject changeCard = new GameObject();
        for(int i = 0; i < Cards.Count; i++) if (Cards[i].GetComponent<Card>().cardID == cardID) changeCard = Cards[i];
        if (DrawCards[playerName].ContainsKey(cardID)) changeCard = DrawCards[playerName][cardID];
        if (DiscardCards[playerName].ContainsKey(cardID)) changeCard = DiscardCards[playerName][cardID];
        changeCard.GetComponent<Card>().locals = locals;
        UpdateCardDescriptions();
    }

    public void DrawCard(string cardID)
    {
        foreach (GameObject g in DrawCards[Server.server.playerList.getPlayerTurn().playerName].Values)
        {
            if (g.GetComponent<Card>().cardID == cardID) { DrawCards[Server.server.playerList.getPlayerTurn().playerName].Remove(g.GetComponent<Card>().cardID); Destroy(g); break; }
        }
        RearrangeDecks(Server.server.playerList.getPlayerTurn().playerName);
    }

    public void DiscardCard(string cardID)
    {
        foreach (GameObject g in Cards)
        {
            if (g.GetComponent<Card>().cardID == cardID)
            {
                card = Instantiate(newDeckCard, DiscardPilePanels[Server.server.playerList.getPlayerTurn().playerName].transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0));
                DiscardCards[Server.server.playerList.getPlayerTurn().playerName][cardID] = card;
                card.GetComponent<Card>().cardID = g.GetComponent<Card>().cardID;
                card.GetComponent<Card>().cardName = g.GetComponent<Card>().cardName;
                card.GetComponent<Card>().description = g.GetComponent<Card>().description;
                card.GetComponent<Card>().color = g.GetComponent<Card>().color;
                card.GetComponent<Card>().locals = g.GetComponent<Card>().locals;
                card.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Text>().text = g.GetComponent<Card>().cardName;
                card.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Text>().text = g.GetComponent<Card>().cardName;
                card.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>().text = g.GetComponent<Card>().description;
                switch (g.GetComponent<Card>().color)
                {
                    case 'r': card.GetComponent<Image>().sprite = CardColors[0]; break;
                    case 'y': card.GetComponent<Image>().sprite = CardColors[1]; break;
                    case 'g': card.GetComponent<Image>().sprite = CardColors[2]; break;
                    case 'b': card.GetComponent<Image>().sprite = CardColors[3]; break;
                }
                break;
            }
        }
        RearrangeDecks(Server.server.playerList.getPlayerTurn().playerName);
    }

    public void ReshuffleDiscard(string playerName)
    {
        foreach (GameObject g in DiscardCards[playerName].Values)
        {
            DrawCards[playerName][g.GetComponent<Card>().cardID] = g;
            g.transform.SetParent(DrawPilePanels[playerName].transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0));
        }
        DiscardCards[playerName].Clear();
        RearrangeDecks(playerName);
    }

    public void ReshuffleAll()
    {
        foreach (Player p in Server.server.playerList.Players.Values)
        {
            foreach (GameObject g in DiscardCards[p.playerName].Values)
            {
                DrawCards[p.playerName][g.GetComponent<Card>().cardID] = g;
                g.transform.SetParent(DrawPilePanels[p.playerName].transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0));
            }
            DiscardCards[p.playerName].Clear();
            RearrangeDecks(p.playerName);
        }
    }

    public void RearrangeDecks(string playerName)
    {
        drawCardNumber = 0;
        discardCardNumber = 0;
        foreach(GameObject g in DrawCards[playerName].Values)
        {
            DrawPilePanels[playerName].transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1580, (drawCardNumber / 8) * 260 + 390);
            g.transform.localPosition = new Vector3(180 * (drawCardNumber % 8) - 630, -260 * (drawCardNumber / 8) + 250);
            drawCardNumber++;
        }
        foreach (GameObject g in DiscardCards[playerName].Values)
        {
            DiscardPilePanels[playerName].transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1580, (discardCardNumber / 8) * 260 + 390);
            g.transform.localPosition = new Vector3(180 * (discardCardNumber % 8) - 630, -260 * (discardCardNumber / 8) + 250);
            discardCardNumber++;
        }
    }

    public void ResetPileNumber()
    {
        for (int i = 0; i < Server.server.playerList.Players.Count; i++)
        {
            if (Server.server.playerList.PlayerOrder[i] == Server.server.playerList.username) playerNumberPile = i;
        }
    }

    public void ViewDrawPile(bool visible)
    {
        cardPanels.SetActive(visible);
        EnemyIntentionsDisplay(!visible);

        if (visible)
        {
            DiscardPilePanels[Server.server.playerList.PlayerOrder[playerNumberPile]].SetActive(false);
            DrawPilePanels[Server.server.playerList.PlayerOrder[playerNumberPile]].SetActive(true);
        }
        else
        {
            foreach (GameObject g in DrawPilePanels.Values) g.SetActive(false);
            ResetPileNumber();
        }
    }

    public void ViewDiscardPile(bool visible)
    {
        cardPanels.SetActive(visible);
        EnemyIntentionsDisplay(!visible);

        if (visible)
        {
            DrawPilePanels[Server.server.playerList.PlayerOrder[playerNumberPile]].SetActive(false);
            DiscardPilePanels[Server.server.playerList.PlayerOrder[playerNumberPile]].SetActive(true);
        }
        else
        {
            foreach (GameObject g in DiscardPilePanels.Values) g.SetActive(false);
            ResetPileNumber();
        }
    }

    public void IncrementDrawPile(int increment)
    {
        DrawPilePanels[Server.server.playerList.PlayerOrder[playerNumberPile]].SetActive(false);
        playerNumberPile += increment;
        if (playerNumberPile < 0) playerNumberPile = Server.server.playerList.Players.Count - 1;
        if (playerNumberPile >= Server.server.playerList.Players.Count) playerNumberPile = 0;
        DrawPilePanels[Server.server.playerList.PlayerOrder[playerNumberPile]].SetActive(true);
    }

    public void IncrementDiscardPile(int increment)
    {
        DiscardPilePanels[Server.server.playerList.PlayerOrder[playerNumberPile]].SetActive(false);
        playerNumberPile += increment;
        if (playerNumberPile < 0) playerNumberPile = Server.server.playerList.Players.Count - 1;
        if (playerNumberPile >= Server.server.playerList.Players.Count) playerNumberPile = 0;
        DiscardPilePanels[Server.server.playerList.PlayerOrder[playerNumberPile]].SetActive(true);
    }

    public void RemovePiles(string playerName)
    {
        DrawPilePanels.Remove(playerName);
        DiscardPilePanels.Remove(playerName);
        DrawCards.Remove(playerName);
        DiscardCards.Remove(playerName);
        ResetPileNumber();
    }

    public void EnemyIntentionsDisplay(bool visible)
    {
        foreach(Enemy e in Server.server.enemyManager.Enemies.Values)
        {
            e.transform.GetChild(2).gameObject.SetActive(visible);
        }
    }

    public char GetClassColor(string playerName)
    {
        switch (Server.server.playerList.Players[playerName].playerClass)
        {
            case "Warrior": return 'r';
            case "Rogue": return 'g';
            case "Mage": return 'b';
            case "Cleric": return 'y';
        }
        return ' ';
    }
}
