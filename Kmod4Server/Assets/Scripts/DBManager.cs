using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class DBManager
{
    public static readonly string dbBaseUrl = "https://studenthome.hku.nl/~tjaard.vanverseveld/content/vakken/jaar2/kernmodule4gdev/";
    public static readonly string dbCreds = "servername=localhost&username=tjaardvanverseveld&password=waiQuoh0Th&database=tjaardvanverseveld";
    public static string response;
    public static Texture text = null;
    public static byte[] imgData;

    public static IEnumerator OpenImage(string page)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(page);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            imgData = request.downloadHandler.data;
            text = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    public static IEnumerator OpenURL(string page, string vars)
    {
        Debug.Log(dbBaseUrl + page + ".php?" + dbCreds + "&" + vars);
        var request = UnityWebRequest.Get(dbBaseUrl + page + ".php?" + dbCreds + "&" + vars);
        {
            yield return request.SendWebRequest();
            if (request.isDone && !request.isHttpError)
            {
                response = request.downloadHandler.text;
            }
        }
    }

    public static IEnumerator OpenURL(string page)
    {
        Debug.Log(page);
        var request = UnityWebRequest.Get(page);
        {
            yield return request.SendWebRequest();
            if (request.isDone && !request.isHttpError)
            {
                response = request.downloadHandler.text;
            }
        }
    }
}
