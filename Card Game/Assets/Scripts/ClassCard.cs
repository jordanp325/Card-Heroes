using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassCard : Card
{
    public override void OnClick()
    {
        Server.server.chooseClass(cardName);
    }
}
