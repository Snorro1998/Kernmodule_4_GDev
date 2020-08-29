using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTypes
{
    public enum Server
    {
        invalid = 0,
        message,
        aliveCheck
    }

    public enum Client
    {
        invalid = 0,
        message,
        aliveResponse
    }
}
