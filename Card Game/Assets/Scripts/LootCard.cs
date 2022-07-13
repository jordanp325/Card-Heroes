using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCard : Card {

    public override void Start()
    {
        base.Start();
    }

    public override void OnClick()
    {
        Server.server.cardPanel.CardSelected(cardName, cardID, cardTargeting, true);
    }
}
