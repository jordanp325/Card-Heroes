using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public string playerName;
    public string playerClass;
    public int maxHealth;
    public int health;
    public int resource;
    public int gold;
    public bool ready;
    public bool voting;
    public int roomVote;
    public bool alive;
    public bool activeSpecial;
    public Dictionary<string, Buff> Buffs = new Dictionary<string, Buff>();
    public Dictionary<string, Item> Items = new Dictionary<string, Item>();

    public GameObject readyButton;
    public GameObject character;
    public Slider healthBar;
    public Slider resourceBar;
    public GameObject playerNumber;

    // Use this for initialization
    void Start () {
        voting = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
