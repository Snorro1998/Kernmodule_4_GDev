using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoginFunctions
{

    public static void OnLoginResultAlreadyLoggedIn(object sender)
    {
        Debug.Log("Kan niet inloggen. De opgegeven gebruikersnaam is al ingelogd of jij bent al ingelogd.");
    }
    public static void OnLoginResultInvalidPassword(object sender)
    {
        Debug.Log("Kan niet inloggen. Het opgegeven wachtwoord klopt niet.");
    }
    public static void OnLoginResultSucces(object sender)
    {
        var client = sender as ClientBehaviour;
        Debug.Log("Succesvol ingelogd!");
        ScreenTransitioner.Instance.ChangeScreen(ActiveScreen.GAME_WAIT_SCREEN, 0.5f, 0.5f);
    }
    public static void OnLoginResultUnknownUsername(object sender)
    {
        Debug.Log("Kan niet inloggen. De opgegeven gebruikersnaam bestaat niet!");
    }
}
