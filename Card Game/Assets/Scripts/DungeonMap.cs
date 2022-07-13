using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMap : MonoBehaviour
{
    public GameObject MapPanel;
    public GameObject GamePanel;
    public GameObject InfoPanel;
    public GameObject[] Rooms;
    public GameObject[] VoteIndicators;
    public GameObject PartyIndicator;
    public List<int> VisibleRooms;

    public GameObject[] PlayerGoldDisplays;

    public int keys;
    public GameObject Key1;
    public GameObject Key2;

    public bool ready;
    public float readyTimer;
    public GameObject readyTimerDisplay;
    public float red;
    public float green;

    public char partyRoom = 'S';

    public List<Vector3> movementQueue;
    public int[] partyCoords = new int[2];
    public bool move;
    public float movementTimer = 0;

    public bool mapLock;
    public bool mapDisplay;
    public bool fade = false;
    public bool fadeToMap;
    public GameObject FadePanel;
    public int transitionTimer = 0;

    public bool votingEnabled;
    public int voters;

    void Start()
    {
        readyTimer = 3;
        red = 0;
        green = 1;
        mapLock = true;
    }

    void FixedUpdate()
    {
        if (ready && !readyTimerDisplay.activeSelf)
        {
            readyTimerDisplay.SetActive(true);
        }
        if (ready && readyTimer >= 0)
        {
            readyTimer -= .02f;
            readyTimerDisplay.GetComponent<Image>().fillAmount = readyTimer / 3;
            if (readyTimer <= 3 && readyTimer > 1.5f) red += (1/75);
            if (readyTimer <= 1.5f && readyTimer > 0) green -= (1/75);
            readyTimerDisplay.GetComponent<Image>().color = new Color(red, green, 0);
            readyTimerDisplay.GetComponentInChildren<Text>().color = new Color(red, green, 0);
            readyTimerDisplay.GetComponentInChildren<Text>().text = Mathf.Round(readyTimer + 0.5f).ToString();
        }
        if (!ready && readyTimer < 3 && readyTimer > 0)
        {
            readyTimer = 3;
            red = 0;
            green = 1;
            readyTimerDisplay.SetActive(false);
        }
        if (readyTimer <= 0)
        {
            ready = false;
            readyTimerDisplay.SetActive(false);
            readyTimer = 3;
            red = 0;
            green = 1;
        }
        if (move)
        {
            PartyIndicator.transform.localPosition += movementQueue[0] / 20;
            movementTimer++;
            if (movementTimer == 20)
            {
                movementTimer = 0;
                movementQueue.RemoveAt(0);
                if (movementQueue.Count == 0) move = false;
            }
        }
        else if (fade && transitionTimer <= 150)
        {
            transitionTimer++;
            if (transitionTimer > 25 && transitionTimer <= 50) FadePanel.GetComponent<Image>().color = new Color(0, 0, 0, ((float)transitionTimer - 25) / 25);
            if (transitionTimer == 75)
            {
                if (fadeToMap)
                {
                    MapPanel.SetActive(true);
                    Server.server.activities.Cards.SetActive(false);
                    mapDisplay = false;
                    MapPanel.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    Server.server.dungeonMap.DisplayPlayerGold();
                    UpdateInfoPanel();
                }
                else
                {
                    MapPanel.SetActive(false);
                    ResetMap();
                    Server.server.activities.DisplayActivity();
                }
            }
            if (transitionTimer > 75 && transitionTimer <= 100) FadePanel.GetComponent<Image>().color = new Color(0, 0, 0, (100 - (float)transitionTimer) / 25);
            if (transitionTimer == 100)
            {
                fade = false;
                transitionTimer = 0;
            }
        }
    }

    public void UpdateInfoPanel()
    {
        InfoPanel.GetComponentInChildren<Text>().text = "Pick a Room";
    }

    public void ResetTimer()
    {
        readyTimer = 2.98f;
        red = 0;
        green = 1;
    }

    public void ResetMap()
    {
        for(int i = 0; i < 8; i++)
        {
            VoteIndicators[i].SetActive(false);
        }
    }

    public void DisplayMap()
    {
        if (!mapLock)
        {
            mapDisplay = !mapDisplay;
            if (mapDisplay)
            {
                MapPanel.SetActive(true);
                Server.server.dungeonMap.DisplayPlayerGold();
                MapPanel.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            else
            {
                MapPanel.SetActive(false);
                MapPanel.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void InitializeMap(int x, int y)
    {
        int roomNumber = x + (5 * y);
        PartyIndicator.transform.localPosition = new Vector3((x * 100) - 200, (y * 100) - 200);
        partyCoords[0] = x;
        partyCoords[1] = y;
        Rooms[roomNumber].GetComponent<Image>().color = new Color(0, 1, 1, 1);
        voters = Server.server.playerList.Players.Count;
        Rooms[roomNumber].GetComponent<Room>().roomType = 'E';
        VisibleRooms.Add(roomNumber);
        for (int i = 0; i < Server.server.playerList.Players.Count; i++)
        {
            PlayerGoldDisplays[i].SetActive(true);
            PlayerGoldDisplays[i].GetComponent<Text>().text = Server.server.playerList.PlayerOrder[i] + "\nGold: 0";
        }
        ViewMap();
    }

    public void DisplayPlayerGold()
    {
        for (int i = 0; i < Server.server.playerList.Players.Count; i++)
        {
            PlayerGoldDisplays[i].GetComponent<Text>().text = Server.server.playerList.PlayerOrder[i] + "\nGold: " + Server.server.playerList.Players[Server.server.playerList.PlayerOrder[i]].gold + "g";
        }
    }

    public void ViewMap()
    {
        Server.server.activities.HideActvities();
        fadeToMap = true;
        fade = true;
        mapLock = true;
    }

    public void TileVisible(int x, int y, char room)
    {
        int roomNumber = x + (5 * y);
        Color roomColor = new Color();
        switch (room)
        {
            case 'B': roomColor = new Color(0.9f, 0, 0, 1); break;
            case 'M': roomColor = new Color(0.9f, 0.4f, 0, 1); break;
            case 'X': roomColor = new Color(0.9f, 0.4f, 0.4f, 1); break;
            case 'T': roomColor = new Color(0.9f, 0.9f, 0, 1); break;
            case 'S': roomColor = new Color(0.9f, 0, 0.9f, 1); break;
            case 'H': roomColor = new Color(0, 0.9f, 0, 1); break;
            case 'E': roomColor = new Color(0.9f, 0.9f, 0.9f, 1); break;
            default: roomColor = new Color(0.4f, 0.4f, 0.4f, 1); break;
        }
        Rooms[roomNumber].GetComponent<Image>().color = roomColor;
        Rooms[roomNumber].GetComponent<Room>().roomType = room;
        if(!VisibleRooms.Contains(roomNumber)) VisibleRooms.Add(roomNumber);
    }

    public void EmptyRoom()
    {
        int roomNumber = partyCoords[0] + (5 * partyCoords[1]);
        if(Rooms[roomNumber].GetComponent<Room>().roomType != 'E') TileVisible(partyCoords[0], partyCoords[1], 'E');
    }

    public void SendVotingToggle()
    {
        Server.server.toggleVoting();
    }

    public void RecieveVotingToggle(string playerName)
    {
        Server.server.playerList.Players[playerName].voting = !Server.server.playerList.Players[playerName].voting;
        voters = 0;
        foreach (Player p in Server.server.playerList.Players.Values) if (p.voting) voters++;
        CalculateMajority();
    }

    public void EnableVoting(bool enabled)
    {
        votingEnabled = enabled;
        if (enabled)
        {
            ViewMap();
            EmptyRoom();
        }
    }

    public void SendRoomVote(string roomNumber)
    {
        if (VisibleRooms.Contains(int.Parse(roomNumber)) && votingEnabled && Server.server.playerList.Me().voting)
        {
            int x = int.Parse(roomNumber)%5;
            int y = int.Parse(roomNumber)/5;
            Server.server.voteRoom(x, y);
        }
    }

    public void RecieveRoomVote(string playerName, int x, int y)
    {
        int roomNumber = x + (5 * y);
        Server.server.playerList.Players[playerName].roomVote = roomNumber;
        foreach (GameObject r in Rooms) r.GetComponent<Room>().votes = 0;
        DisplayVotes();
        CalculateMajority();
    }

    public void CalculateMajority()
    {
        ready = false;
        foreach (GameObject r in Rooms)
        {
            if (r.GetComponent<Room>().votes > (float)voters / 2 && !(r.GetComponent<Room>().x == partyCoords[0] && r.GetComponent<Room>().y == partyCoords[1])) ready = true;
        }
        ResetTimer();
    }

    public void DisplayVotes()
    {
        foreach(Player p in Server.server.playerList.Players.Values)
        {
            int playerNumber = int.Parse(p.playerNumber.GetComponent<Text>().text);
            if (p.voting && p.roomVote != (partyCoords[0] + (5 * partyCoords[1])))
            {
                Rooms[p.roomVote].GetComponent<Room>().votes++;
                VoteIndicators[playerNumber].transform.localPosition = new Vector3(Rooms[p.roomVote].transform.localPosition.x, Rooms[p.roomVote].transform.localPosition.y);
                Vector3 adjustment = new Vector3(-30 + 30 * ((Rooms[p.roomVote].GetComponent<Room>().votes - 1) % 3), 30 - 30 * ((Rooms[p.roomVote].GetComponent<Room>().votes - 1) / 3));
                VoteIndicators[playerNumber].transform.localPosition += adjustment;

                VoteIndicators[playerNumber].SetActive(true);
                VoteIndicators[playerNumber].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/IntentionIcons/" + p.playerClass + "Mini");
                VoteIndicators[playerNumber].GetComponentInChildren<Text>().text = playerNumber.ToString();
            }
            else VoteIndicators[playerNumber].SetActive(false);
        }
    }

    public void MoveParty(int[] xCoords, int[] yCoords)
    {
        partyCoords[0] = xCoords[xCoords.Length - 1];
        partyCoords[1] = yCoords[yCoords.Length - 1];
        for (int i = 0; i < xCoords.Length; i++)
        {
            Vector3 currentPos = PartyIndicator.transform.localPosition;
            if (i == 0) movementQueue.Add(new Vector3(((xCoords[i] * 100) - 200) - currentPos.x, ((yCoords[i] * 100) - 200) - currentPos.y));
            else movementQueue.Add(new Vector3(((xCoords[i] * 100) - 200) - ((xCoords[i - 1] * 100) - 200), ((yCoords[i] * 100) - 200) - ((yCoords[i - 1] * 100) - 200)));
        }
        move = true;
        DisplayRoom(Rooms[partyCoords[0] + (5 * partyCoords[1])].GetComponent<Room>().roomType);
        mapLock = false;
        foreach (Player p in Server.server.playerList.Players.Values)
        {
            p.roomVote = partyCoords[0] + (5 * partyCoords[1]);
            DisplayVotes();
        }
    }

    public void DisplayRoom(char roomType)
    {
        partyRoom = roomType;
        if(roomType != 'E')
        {
            fadeToMap = false;
            fade = true;
            Server.server.activities.queuedRoom = roomType;
        }
    }

    public void AddKey()
    {
        if (keys < 2) keys++;
        DisplayKeys();
    }

    public void RemoveKey()
    {
        if (keys > 0) keys--;
        DisplayKeys();
    }

    public void DisplayKeys()
    {
        if (keys == 0)
        {
            Key1.SetActive(false);
            Key2.SetActive(false);
        }
        if (keys == 1)
        {
            Key1.SetActive(true);
            Key2.SetActive(false);
        }
        if (keys == 2)
        {
            Key1.SetActive(true);
            Key2.SetActive(true);
        }
    }
}
