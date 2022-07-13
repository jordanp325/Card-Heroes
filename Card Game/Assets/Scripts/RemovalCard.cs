using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalCard : Card
{
    public override void Start()
    {
        base.Start();
    }

    public override void OnClick()
    {
        Server.server.cardPanel.HighlightRemovalCard(cardID);
    }
}
