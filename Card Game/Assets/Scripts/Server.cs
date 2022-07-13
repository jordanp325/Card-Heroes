using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;
using System;

public class Server : MonoBehaviour { //USE Server.server TO ACCESS!!!!!!!!! @@LOGAN
	public static Server server;
	public Chat chat;
	public PlayerList playerList;
	public CardPanel cardPanel;
    public SkillTree skillTree;
    public EnemyManager enemyManager;
    public TooltipSpawner tooltipSpawner;
    public GameControl gameControl;
    public DungeonMap dungeonMap;
    public Activities activities;
    public ItemManager itemManager;
	string username;
    string userID;
    string lobbyname;
    long timestamp;

    //for testing the server without building it:
    public bool test;
    public bool localTest;
    public string testUsername;
    public bool dataDebug;
    ClientWebSocket ws;
	

	void Awake(){
		server = this;
        Application.targetFrameRate = 30;
	}

	void Start () {
        if(!test) Application.ExternalCall("webGLloaded");
        else {
            playerList.username = testUsername;
            username = testUsername;
            startTest();
        }
    }

    async void startTest(){
        Uri target;
        if (localTest) target = new Uri("ws://127.0.0.1/ws");
        else target = new Uri("ws://24.16.27.52/ws");
        ws = new ClientWebSocket();
        ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(90);
        ws.Options.Cookies = new CookieContainer();
        ws.Options.Cookies.Add(new Cookie("userID", testUsername) { Domain = target.Host });
        await ws.ConnectAsync(target, System.Threading.CancellationToken.None);
        Debug.Log("websocket connected");
        while (ws.State == WebSocketState.Open){
            byte[] byteArr = new byte[1024];
            ArraySegment<Byte> storage = new ArraySegment<byte>(byteArr);
            var task = await ws.ReceiveAsync(storage, System.Threading.CancellationToken.None);
            byte[] result = new byte[task.Count];
            Array.Copy(storage.Array, result, task.Count);
            //Debug.Log("Recieved message of " + task.Count + " length:\n" + System.Text.Encoding.UTF8.GetString(result));
            recieveData(System.Text.Encoding.UTF8.GetString(result));
        }
        Debug.Log("websocket disconnected");
    }
	
	void Update () {
		
	}

	string msgQueue = "";
	public void recieveData(string data){
		msgQueue += data;
		msgCheck();
	}
	void msgCheck(){
		while(msgQueue.Contains(";")){
			handleServerCommunications(msgQueue.Substring(0, msgQueue.IndexOf(";")));
			msgQueue = msgQueue.Substring(msgQueue.IndexOf(";")+1);
		}
	}



	public void recieveInfo(string info){ 
		username = info.Split(':')[0];
        userID = info.Split(':')[1];
        lobbyname = info.Split(':')[2];
		//Debug.Log("Recieved: "+info);
        playerList.username = username;
	}

    void sendData(string data){
        if(!test) Application.ExternalCall("sendData", data);
        else {
            //Debug.Log("SENT: "+data);
            ws.SendAsync(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(data)), WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
        }
    }

    Dictionary<string, int> parseString(string str){
        Dictionary<string, int> d = new Dictionary<string, int>();
        foreach (string s in str.Split(',')) {
            if(s == "") continue;
            string[] a = s.Split('>');
            d.Add(a[0], int.Parse(a[1]));
        }
        return d;
    }

    public void handleServerCommunications(string data){
        if(dataDebug) Debug.Log(data);
        string[] arr = data.Split(':');
        switch (arr[0]){
			case "msg":
            string msg = WebUtility.UrlDecode(data.Substring(4));
            if (msg.Contains("<b>" + username + ":</b>")) msg = msg.Replace("<color=#666666ff>", "<color=#333333ff>");
            chat.RecieveMSG(msg);
			break;
			case "adduser":
			playerList.AddPlayer(arr[1]);
			break;
			case "removeuser":
			playerList.RemovePlayer(arr[1]);
			break;
			case "changeuser": //ex: changeuser:D3SYNC:class:w
			switch(arr[2]){
				case "class":
				playerList.ChangePlayerClass(arr[1], arr[3][0]);
				break;
				case "ready":
                if (!playerList.gameStarted) playerList.ChangePlayerReadyState(arr[1]);
                else {
                    playerList.PlayerDone(arr[1]);
                    if(username == arr[1] && (dungeonMap.partyRoom == 'X' || dungeonMap.partyRoom == 'M' || dungeonMap.partyRoom == 'B')) cardPanel.CardLooted();
                }
				break;
			}
			break;
			case "initialize":
            playerList.StartGame(arr[1].Split(','));
            break;
			case "endEncounter":
			gameControl.EndEncounter(arr[1] == "1");
			break;
            case "startEncounter":
            gameControl.StartEncounter();
            break;
            case "kickable":
            playerList.ToggleKickable(arr[1] == "1");
            break;
            case "votecount":
            playerList.UpdateVotes(int.Parse(arr[1]));
            break;
            case "addCard":
            arr = arr[1].Split('!');
            cardPanel.AddCard(arr[0], (arr[1] == "_" ? null : (char?)arr[1][0]));
            break;
			case "highlight":
            cardPanel.HighlightCard(arr[1]);
            break;
            case "addEnemy":
            arr = arr[1].Split('!');
            enemyManager.AddEnemy(arr[0], arr[1], int.Parse(arr[2]));
			break;
            case "cardPool":
            var arr2 = arr[1].Split('!');
            foreach(string s in arr2)
                cardPanel.AddLootCard(s); 
            break;
			case "playerClassAbility":
			playerList.ChangePlayerAbilityActive(arr[1], arr[2] == "1");
			break;
            case "deckInitialization":
            List<string> names = new List<string>();
            List<string> ids = new List<string>();
            List<Dictionary<string, int>> locals = new List<Dictionary<string, int>>();
            for (int i = 2; i < arr.Length; i++)
            {
                string[] pair = arr[i].Split('!');
                ids.Add(pair[0]);
                names.Add(pair[1]);
                locals.Add(parseString(pair[2]));
            }
            cardPanel.RecievePlayerDeck(arr[1], ids.ToArray(), names.ToArray(), locals.ToArray());
            break;
            case "skillchoice":
            skillTree.DisplaySkillTree(arr[1].Split(','));
            break;
            case "skillConfirm":
            skillTree.AddSkill(arr[1]);
            break;
            case "addCardToDeck":
            cardPanel.AddCardToDeck(arr[1], arr[2], arr[3], parseString(arr[4]));
            break;
            case "removeCard":
            cardPanel.RemoveCard(arr[1], arr[2]);
            break;
            case "initMap":
            dungeonMap.InitializeMap(int.Parse(arr[1][0]+""), int.Parse(arr[1][1]+""));
            break;
            case "mapReveal":
            dungeonMap.TileVisible(int.Parse(arr[1][0]+""), int.Parse(arr[1][1]+""), arr[2][0]);
            break;
            case "voteEnable":
            dungeonMap.EnableVoting(arr[1] == "1");
            break;
            case "canVoteToggle":
            dungeonMap.RecieveVotingToggle(arr[1]);
            break;
            case "voteMove":
            dungeonMap.RecieveRoomVote(arr[1], int.Parse(arr[2][0]+""), int.Parse(arr[2][1]+""));
            break;
            case "mimic":
            activities.MimicRoom();
            break;
            case "mimicVote":
            activities.RecieveMimicVote(arr[1], arr[2] == "1");
            break;
            case "addItem":
            itemManager.AddItem(arr[2], arr[1], arr[3][0] == ' ' ? null : (char?)arr[3][0], arr[4][0], int.Parse(arr[5]));
            break;
            case "itemUsed":
            itemManager.ItemUsed(arr[1]);
            break;
            //case "instantUsed":
			//usedInstantItem(arr[1]);
			//break;
            case "moveParty":
            int[] Xs = new int[arr[1].Length/2];
            int[] Ys = new int[arr[1].Length/2];
            for(int i = 0; i < arr[1].Length; i+=2){
                Xs[i / 2] = int.Parse(arr[1][i] + "");
                Ys[i / 2] = int.Parse(arr[1][i + 1] + "");
            }
            dungeonMap.MoveParty(Xs, Ys);
            break;
            case "updateStats":
            string[] arr4 = arr[1].Split('!');
            for(int i = 0; i < arr4.Length; i++){
                string[] arr3 = arr4[i].Split('>');
                playerList.UpdatePlayerStats(arr3[0], int.Parse(arr3[1]), int.Parse(arr3[2]), int.Parse(arr3[3]));
            }
            break;
            case "updateGold":
            string[] arr5 = arr[1].Split('!');
            for (int i = 0; i < arr5.Length; i++)
            {
                string[] arr3 = arr5[i].Split('>');
                playerList.UpdatePlayerGold(arr3[0], int.Parse(arr3[1]));
            }
            break;
            case "shopinit":
            for (int i = 1; i < arr.Length; i++){
                string[] args = arr[i].Split('>');
                if(args[0][0] == 'l') activities.AddShopCard(args[1], int.Parse(args[2]));
                else activities.AddShopItem(args[1], int.Parse(args[2]), args[0][0]);
            }
            break;
            case "s":
			cardPanel.HighlightTarget(arr[1].Substring(1), arr[1][0] == '1');
			break;
			case "update":
			for(int i = 1; i < arr.Length; i++){
				string[] args = arr[i].Split('!');
				switch(args[0]){
					case "0":
					for(int j = 1; j < args.Length; j++){
						string[] arr3 = args[j].Split('>');
						if(arr3[0] == "p") playerList.UpdatePlayerStats(arr3[1], int.Parse(arr3[2]), int.Parse(arr3[3]), int.Parse(arr3[4]));
						if(arr3[0] == "e") enemyManager.UpdateEnemyStats(arr3[1], int.Parse(arr3[2]), int.Parse(arr3[3]));
					}
					break;
					/*case "0":
					//enemyTurnTaken(args[1]);
					break;*/
					case "1":
					Dictionary<string, int> locals1 = parseString(args[5]);
                    if (playerList.Players.ContainsKey(args[1])) playerList.AddBuff(args[1], args[2], args[3], int.Parse(args[4]), locals1, new List<string>(args[6].Split(',')));
                    if (enemyManager.Enemies.ContainsKey(args[1])) enemyManager.AddBuff(args[1], args[2], args[3], int.Parse(args[4]), locals1, new List<string>(args[6].Split(',')));
					break;
					case "2":
                    if (playerList.Players.ContainsKey(args[1])) playerList.UpdateBuffDuration(args[1], args[2], int.Parse(args[3]));
                    if (enemyManager.Enemies.ContainsKey(args[1])) enemyManager.UpdateBuffDuration(args[1], args[2], int.Parse(args[3]));
                    break;
					case "3":
                    if (playerList.Players.ContainsKey(args[1])) playerList.UpdateBuffLocal(args[1], args[2], args[3], int.Parse(args[4]));
                    if (enemyManager.Enemies.ContainsKey(args[1])) enemyManager.UpdateBuffLocal(args[1], args[2], args[3], int.Parse(args[4]));
                    break;
					case "4":
                    if (playerList.Players.ContainsKey(args[1])) playerList.RemoveBuff(args[1], args[2]);
                    if (enemyManager.Enemies.ContainsKey(args[1])) enemyManager.RemoveBuff(args[1], args[2]);
                    break;
					case "5":
					if(args[2] == "0") playerList.PlayerDied(args[1]);
					else playerList.PlayerRevived(args[1]);
					break;
					case "6":
					enemyManager.EnemyDied(args[1]);
					break;
					case "7":
					cardPanel.PlayCard(args[1], args[2]);
					break;
					case "8":
					bool[] arrB = new bool[5];
					for(int j = 0; j < arrB.Length; j++) arrB[j] = args[3][j] == '1';
                    enemyManager.ChangeEnemyIntention(args[1], (args[2] == "" ? new string[0] : args[2].Split(',')), arrB, int.Parse(args[4]), int.Parse(args[5]));
                    break;
                    case "9":
                    enemyManager.ChangeAttackSeverity(args[1], int.Parse(args[2]));
                    break;
                    case "a":
                    cardPanel.ChangeLocals(args[1], args[2], parseString(args[3]));
                    break;
                    case "b":
                    if (playerList.Players.ContainsKey(args[1])) playerList.UpdateLocalDecay(args[1], args[2], args[3]);
                    if (enemyManager.Enemies.ContainsKey(args[1])) enemyManager.UpdateLocalDecay(args[1], args[2], args[3]);
                    break;
                    case "c":
                    playerList.UpdatePlayerGold(args[1], int.Parse(args[2]));
                    break;
                    }
			}
			break;
			default: throw new Exception("Server protocol "+arr[0] +" not recognized");
		}
	}

	public void activateItem(string itemId, string targetId){
		sendData("useItem:"+itemId+":"+targetId);
	}

	public void fightMimic(bool fight){
		sendData("mimicVote:"+(fight?"1":"0"));
	}

	public void healChoice(bool heal){
		sendData("heal:"+(heal?"1":"0"));
	}

	public void toggleVoting(){
		sendData("canVoteToggle");
	}

	public void voteRoom(int x, int y){
		sendData("voteMove:"+x.ToString()+y.ToString());
	}

	public void removeCard(string skillKey, string[] cardIds){
        sendData("cardRemoval:"+skillKey+":"+string.Join(",", cardIds));
	}

	public void sendMessage(string message){
        sendData($"msg:{message}");
	}

    public void pickCardFromPool(string cardName, string cardName2) {
        sendData("cardPoolPick:" + cardName + ":" + cardName2);
    }

    public void chooseCard(string id){
		sendData("highlight:"+id);
	}

    public void suggestCard(string cardName)
    {
        if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - timestamp > 3000)
        {
            sendData("suggest:" + cardName);
            timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
    }

    public void activateCard(string id, string targetName){
		sendData($"selectcard:{id}:" + targetName);
	}

	public void activateClassAbility(){
		sendData("activateClassAbility");
	}

	public void chooseClass(string playerClass){
        char cls;
		switch(playerClass){
			case "Warrior": cls = 'w'; break;
			case "Rogue": cls = 'r'; break;
			case "Mage": cls = 'm'; break;
			case "Cleric": cls = 'c'; break;
			case "Random": cls = '?'; break;
			default: cls = '?'; break;
		}
        sendData($"setclass:{cls}");
	}

	public void changeReadyState(){
        sendData("toggleready");
	}

    public void chooseSkill(string skillKey){
        sendData("treeRequest:" + skillKey);
    }
    
    public void selectTarget(string name, bool selected){
		sendData("s:"+(selected?"1":"0")+name);
    }
    
    public void voteKick(){
		sendData("voteKick");
    }

    public void pingServer(){
        Application.ExternalCall("ping");
    }

	public bool full = false;
	public void fullscreenToggle(){
		if(!full) Application.ExternalCall("fullscreenOn");
		else Application.ExternalCall("fullscreenOff"); 
		full = !full;
	}

}
