using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public int maxHealth;
    public int health;
    public string enemyName;
    public string enemyID;
    public string enemyDesc;
    public Dictionary<string, Buff> Buffs = new Dictionary<string, Buff>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
