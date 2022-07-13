using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

    public string cardID;
    public string cardName;
    public string description;
    public char color;
    public Dictionary<string, int> locals;

    public char? cardTargeting;
    public char cardTarget;

    public GameObject front;
    public GameObject back;
    public GameObject hitbox;
    public bool mouseEnter;
    public bool mouseExit;
    public bool rotateFront;
    public bool rotateBack;
    int rotation;
    int rotateSpeed;

	// Use this for initialization
	public virtual void Start () {
        rotateSpeed = 15; //15 is default, 1 for slow-mo testing
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        rotation = Mathf.RoundToInt(gameObject.transform.rotation.eulerAngles.y);

        if (!(rotateBack || rotateFront))
        {
            if (mouseEnter) rotateBack = true;
            if (mouseExit) rotateFront = true; 
        }

        if (rotateBack && rotation < 180 && rotation >= 0) gameObject.transform.Rotate(0, rotateSpeed, 0);
        if (rotateBack && rotation < 180 && rotation >= 0) hitbox.transform.eulerAngles = new Vector3(0, 0, 0);
        if (rotateBack && rotation == 90)
        {
            back.SetActive(true);
            front.SetActive(false);
        }
        if (rotateBack && rotation == 180)
        {
            rotateBack = false;
            mouseEnter = false;
        }

        if (rotateFront && rotation <= 180 && rotation > 0) gameObject.transform.Rotate(0, -rotateSpeed, 0);
        if (rotateFront && rotation <= 180 && rotation > 0) hitbox.transform.eulerAngles = new Vector3(0, 0, 0);
        if (rotateFront && rotation == 90)
        {
            front.SetActive(true);
            back.SetActive(false);
        }
        if (rotateFront && rotation == 0)
        {
            rotateFront = false;
            mouseExit = false;
        }

        if (rotation == 90) //This *looks* like it does nothing but we need it or everything breaks
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public void OnMouseEnter()
    {
        mouseEnter = true;
        mouseExit = false;
    }

    public void OnMouseExit()
    {
        mouseExit = true;
        mouseEnter = false;
    }

    public virtual void OnClick()
    {
        if(Server.server.playerList.PlayerOrder[Server.server.playerList.PlayerTurn] == Server.server.playerList.username)
        {
            if(cardID == Server.server.cardPanel.selectedCardID && Server.server.cardPanel.cardSelected)
            {
                Server.server.chooseCard("");
                Server.server.cardPanel.DeselectCard();
            }
            else
            {
                Server.server.chooseCard(cardID);
                Server.server.cardPanel.CardSelected(cardName, cardID, cardTargeting, false);
            }
        }
        else
        {
            Server.server.suggestCard(cardName);
        }
    }
}
