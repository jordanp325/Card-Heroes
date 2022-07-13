using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public string itemID;
    public string itemDesc;
    public char? itemTargeting;

    public bool active;
    public char cooldownType;
    public int cooldown;
    public int currentCooldown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetItemDescription()
    {
        string cooldownDesc = "Cooldown: " + cooldown + " ";
        if (cooldownType == 'r') cooldownDesc += "Rounds";
        if (cooldownType == 'c') cooldownDesc += "Combats";
        if (cooldownType == 's') cooldownDesc = "Single Use";
        if (cooldownType == 'i') cooldownDesc = "Instant Use";
        return cooldownDesc + "\n" + itemDesc;
    }

    public void OnClick()
    {
        if (itemID == Server.server.itemManager.selectedItemID && Server.server.itemManager.itemSelected)
        {
            Server.server.itemManager.DeselectItem();
        }
        else if(active)
        {
            Server.server.itemManager.ItemSelected(itemName, itemID, itemTargeting);
        }
    }

    public void DecreaseCooldown()
    {
        if (currentCooldown > 0) currentCooldown--;
        if (currentCooldown == 0)
        {
            active = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    public void UseItem()
    {
        currentCooldown = cooldown;
        active = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
    }
}
