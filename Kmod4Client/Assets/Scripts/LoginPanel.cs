using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public Text userNameField;
    public Text passwordField;

    public void Login()
    {
        if (userNameField.text != "")
        {
            TestClientBehaviour.Instance.LoginToServer((NativeString64)userNameField.text, (NativeString64)passwordField.text);
        }
        else
        {
            //voer een naam in!
        }
    }

    public void Register()
    {
        if (userNameField.text != "")
        {
            TestClientBehaviour.Instance.RegisterToServer((NativeString64)userNameField.text, (NativeString64)passwordField.text);
        }
    }
}
