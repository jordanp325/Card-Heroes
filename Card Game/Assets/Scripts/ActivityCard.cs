using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityCard : Card
{
    public override void OnClick()
    {
        if (cardName == "Health") Server.server.healChoice(true);
        if (cardName == "Resource") Server.server.healChoice(false);
        if (cardName == "Fight") Server.server.fightMimic(true);
        if (cardName == "Run") Server.server.fightMimic(false);
    }
}
