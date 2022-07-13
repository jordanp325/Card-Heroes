using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat{

    public string statName;
    public List<Buff> components;
    public string description;
    public int duration;
    public int power;

    public Stat(string nam, List<Buff> com, string des, int dur, int pow)
    {
        statName = nam;
        components = com;
        description = des;
        duration = dur;
        power = pow;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
