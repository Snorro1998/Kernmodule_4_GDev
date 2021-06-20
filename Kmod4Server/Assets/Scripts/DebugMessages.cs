using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugMessages
{
    public enum MessageTypes
    {
        SERVER_START,
        SERVER_START_ERROR_CANT_BIND_TO_PORT,
        SERVER_START_SUCCES,
        SERVER_SETTINGS_FILE_EXISTS,
        SERVER_SETTINGS_FILE_DOESNT_EXIST,
        SERVER_DATA_DETAILS_FILE_EXISTS,
        SERVER_DATA_DETAILS_FILE_DOENST_EXIST,
        PING,
        CLIENT_CONNECT,
        CLIENT_DISCONNECT,
        CLIENT_MESSAGE,
        CLIENT_LOGIN_REQUEST,
        GAME_MAZE_NEXT_FLOOR,
        GAME_BATTLE_START,
        UNKNOWN_MESSAGE_TYPE,
    }

    public static Dictionary<MessageTypes, string> debugMessages = new Dictionary<MessageTypes, string>()
    {
        {MessageTypes.SERVER_START, "De server wordt gestart op poort v1"},
        {MessageTypes.SERVER_START_ERROR_CANT_BIND_TO_PORT, "De server kon zich niet binden aan de poort v1"},
        {MessageTypes.SERVER_START_SUCCES, "De server is probleemloos opgestart!"},
        {MessageTypes.SERVER_SETTINGS_FILE_EXISTS, "Instellingenbestand voor de server bestaat. Instellingen worden nu ingeladen" },
        {MessageTypes.SERVER_SETTINGS_FILE_DOESNT_EXIST, "Instellingenbestand voor de server bestaat niet! Wordt nu aangemaakt met standaardinstellingen" },
        {MessageTypes.SERVER_DATA_DETAILS_FILE_EXISTS, "Datadetails bestaat." },
        {MessageTypes.SERVER_DATA_DETAILS_FILE_DOENST_EXIST, "Datadetails bestaat niet! Wordt nu aangemaakt." },
        {MessageTypes.PING, "Ontvangt ping van client"},
        {MessageTypes.CLIENT_CONNECT, "Nieuwe client maakt verbinding met de server"},
        {MessageTypes.CLIENT_DISCONNECT, "Een client heeft de verbinding met de server verbroken"},
        {MessageTypes.CLIENT_MESSAGE, "Ontvangt bericht van het type v1 van een client"},
        {MessageTypes.CLIENT_LOGIN_REQUEST, "Een client probeert in te loggen met de gebruikersnaam 'v1' en het wachtwoord 'v2'"},
        {MessageTypes.GAME_MAZE_NEXT_FLOOR, "Ga naar het volgende doolhof" },
        {MessageTypes.GAME_BATTLE_START, "Komt een monster tegen!" },
        {MessageTypes.UNKNOWN_MESSAGE_TYPE, "Onbekend berichttype"},
    };

    public static void PrintDebugMessage(MessageTypes t, params string[] vars)
    {
        //if (!printDebugMessages) return;
        string txt = "Er klopt iets niet als je dit kunt lezen";
        if (debugMessages.ContainsKey(t))
        {
            txt = debugMessages[t];
            int i = 1;
            foreach (var s in vars)
            {
                string searchTerm = "v" + i;
                int nextOccurance = txt.IndexOf(searchTerm);
                if (nextOccurance == -1) break;
                else
                {
                    string temp = txt.Substring(0, nextOccurance);
                    string temp2 = txt.Substring(nextOccurance + searchTerm.Length);
                    txt = temp + s + temp2;
                }
                i++;
            }
        }
        Debug.Log(txt);
    }
}
