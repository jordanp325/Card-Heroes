using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Chat : MonoBehaviour {

    public Text chatLog;
    public InputField input;
    public GameObject chatBox;
    public EventSystem eventSystem;
    public Scrollbar chatScroll;
    public bool scrollBarAdjust;
    public int timer;
    long timestamp;

	// Use this for initialization
	void Start () {
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (scrollBarAdjust) timer++;
        if (timer > 1)
        {
            scrollBarAdjust = false;
            timer = 0;
            chatScroll.value = 0;
        }
	}

    public void EnterText (string message)
    {

        if(Input.GetKey(KeyCode.Return) && message != "" && new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - timestamp > 1000)
        {
            input.text = "";
            eventSystem.SetSelectedGameObject(input.gameObject);
            input.ActivateInputField();
            Server.server.sendMessage(message);
            //RecieveMSG(message); //temporary
            timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
    }

    public void RecieveMSG (string message)
    {
        if (chatLog.text == "") chatLog.text += message;
        else chatLog.text += "\n" + message;
        Canvas.ForceUpdateCanvases();
        int lines = chatLog.cachedTextGenerator.lines.Count;
        if (lines > 11)
        {
            chatBox.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lines * 16);
            Canvas.ForceUpdateCanvases();
            scrollBarAdjust = true;
        }
    }
}
