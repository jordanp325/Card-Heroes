using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public GameObject item;
    public GameObject newItem;

    public GameObject ItemPanel;

    public string selectedItemID;
    public bool itemSelected;

    public string itemTarget;
    public string ItemID;
    public char? itemTargeting;

    public GameObject ConfirmButton;
    public GameObject InfoPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && itemSelected && itemTargeting != null)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.TransformDirection(Vector3.forward));
            foreach (RaycastHit2D r in hits)
            {
                if (r.collider != null)
                {
                    if (r.transform.gameObject.tag == "Enemy" || r.transform.gameObject.tag == "Self" || r.transform.gameObject.tag == "Ally")
                    {
                        GameObject target = r.transform.gameObject;
                        itemTarget = null;

                        if (target.tag == "Enemy" && (itemTargeting == 'e' || itemTargeting == 'n'))
                        {
                            if (itemTarget == target.GetComponent<Enemy>().enemyID)
                            {
                                itemTarget = "";
                                Server.server.selectTarget(target.GetComponent<Enemy>().enemyID, false);
                            }
                            else
                            {
                                itemTarget = target.GetComponent<Enemy>().enemyID;
                                Server.server.selectTarget(target.GetComponent<Enemy>().enemyID, true);
                            }
                        }
                        if (target.tag == "Self" && (itemTargeting == 'p' || itemTargeting == 'n'))
                        {
                            if (itemTarget == target.GetComponentInChildren<Text>().text)
                            {
                                itemTarget = "";
                                Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, false);
                            }
                            else
                            {
                                itemTarget = target.GetComponentsInChildren<Text>()[1].text;
                                Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, true);
                            }
                        }
                        if (target.tag == "Ally" && (itemTargeting == 'p' || itemTargeting == 'a' || itemTargeting == 'n'))
                        {
                            if (itemTarget == target.GetComponentInChildren<Text>().text)
                            {
                                itemTarget = "";
                                Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, false);
                            }
                            else
                            {
                                itemTarget = target.GetComponentsInChildren<Text>()[1].text;
                                Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, true);
                            }
                        }
                        if ((target.tag == "Ally" || target.tag == "Self") && itemTargeting == 'd')
                        {
                            if (!Server.server.playerList.Players[target.GetComponentsInChildren<Text>()[1].text].alive)
                            {
                                if (itemTarget == target.GetComponentInChildren<Text>().text)
                                {
                                    itemTarget = "";
                                    Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, false);
                                }
                                else
                                {
                                    itemTarget = target.GetComponentsInChildren<Text>()[1].text;
                                    Server.server.selectTarget(target.GetComponentsInChildren<Text>()[1].text, true);
                                }
                            }
                        }

                        if (itemTarget != "" && itemTarget != null)
                        {
                            InfoPanel.GetComponentInChildren<Text>().text = "Confirm Item Use";
                            ConfirmButton.SetActive(true);
                        }
                        else if (itemTarget != null)
                        {
                            InfoPanel.GetComponentInChildren<Text>().text = "Select a Target";
                            ConfirmButton.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void AddItem(string itemName, string itemID, char? itemTargeting, char cooldownType, int cooldown)
    {
        item = Instantiate(newItem, ItemPanel.transform);
        item.transform.localPosition = new Vector3(-580 + (Server.server.playerList.Me().Items.Count * 65), 0);
        item.GetComponent<Item>().itemName = itemName;
        item.GetComponent<Item>().itemID = itemID;
        item.GetComponent<Item>().itemTargeting = itemTargeting;
        item.GetComponent<Item>().cooldownType = cooldownType;
        item.GetComponent<Item>().cooldown = cooldown;
        item.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Icons/ItemIcons/" + itemName);
        item.GetComponentInChildren<Text>().text = cooldown.ToString();
        Server.server.playerList.Me().Items.Add(itemID, item.GetComponent<Item>());
    }

    public void UpdateRoundCooldowns()
    {
        foreach (Item i in Server.server.playerList.Me().Items.Values)
        {
            if (i.cooldownType == 'r' && !i.active) i.currentCooldown--;
        }
    }

    public void UpdateCombatCooldowns()
    {
        foreach (Item i in Server.server.playerList.Me().Items.Values)
        {
            if (i.cooldownType == 'c' && !i.active) i.currentCooldown--;
        }
    }

    public void ConfirmItem()
    {
        Server.server.activateItem(ItemID, itemTarget);
    }

    public void ItemUsed(string itemID)
    {
        Server.server.playerList.Me().Items[itemID].UseItem();
    }

    public void ItemSelected(string itemN, string itemID, char? itemT)
    {
        ItemID = itemID;
        itemTargeting = itemT;
        itemTarget = null;
        itemSelected = true;
        ClearTargets();
        if (itemTargeting == null)
        {
            InfoPanel.GetComponentInChildren<Text>().text = "Confirm Item Use";
            ConfirmButton.SetActive(true);
        }
        else
        {
            InfoPanel.GetComponentInChildren<Text>().text = "Select a Target";
            ConfirmButton.SetActive(false);
        }
        Server.server.cardPanel.DeselectCard();
    }

    public void DeselectItem()
    {
        itemSelected = false;
        ConfirmButton.SetActive(false);
        ClearTargets();
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
}
