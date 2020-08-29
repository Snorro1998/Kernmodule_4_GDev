using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestPlayerData
{
    public string userName;
    public string password;

    public TestPlayerData(string _userName, string _password)
    {
        userName = _userName;
        password = _password;
    }
}
