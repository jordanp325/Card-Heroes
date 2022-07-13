using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff {

    public string buffName;
    public string buffID;
    public string description;
    public int duration;
    public int power;
    public List<string> decaying;
    public bool statsOnly;
    public bool skill;
    public Dictionary<string, int> values;

    public Buff(string nam, string id, string desc, int dur, int pow, bool so, bool sk, Dictionary<string, int> va, List<string> dec)
    {
        buffName = nam;
        buffID = id;
        description = desc;
        duration = dur;
        power = pow;
        statsOnly = so;
        skill = sk;
        values = va;
        decaying = dec;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
