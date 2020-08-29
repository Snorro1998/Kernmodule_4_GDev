using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageGiveTurn : Message
{
    public override MessageType Type => MessageType.gameGiveTurn;
}
