using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars
{
    public Dictionary<string, string> LogMessages = new Dictionary<string, string>
    {
        { "clientStarting",         "De client wordt gestart" },
        { "serverConnected",        "De client is nu verbonden met de server" },
        { "serverReceiveData",      "De client ontvangt data van de server" },
        { "serverDisconnect",       "De server heeft de verbinding verbroken"},
        { "messageUnknown",         "Onbekend of corrupt berichttype" },
        { "messageSendDefault",     "Snaterdijke jongen"},
        { "errorLoginUserAlready",  "De opgegeven gebruikersnaam is reeds ingelogd"},
        { "errorLoginUserNonExist", "De opgegeven gebruikersnaam bestaat niet"},
        { "errorLoginWrongPassword","Verkeerd wachtwoord"},
        { "loginSucces",            "Succesvol ingelogd"},
        { "lobbyListReceive",       "Ontvangt lobbylijst van de server" },
        { "lobbyEnterSuccess",      "Lobby succesvol binnengekomen" },
        { "lobbyCantEnterNonExist", "Lobby bestaat niet!" },
        { "lobbyCantEnterBegun",    "Kan lobby niet binnenkomen, het spel is reeds begonnen" }
    };
}
