using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class DBManager
{
    public static readonly string dbBaseUrl = "https://studenthome.hku.nl/~tjaard.vanverseveld/content/vakken/jaar2/kernmodule4gdev/";
    public static readonly string dbCreds = "servername=localhost&username=tjaardvanverseveld&password=waiQuoh0Th&database=tjaardvanverseveld";
    public static string response;
    //public static Texture text = null;
    public static byte[] imgData;

    public static IEnumerator OpenImage(string page)
    {
        imgData = null;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(page);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            imgData = request.downloadHandler.data;
            //text = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    public static IEnumerator OpenURL(string page, params string[] vars)
    {
        string url = dbBaseUrl + page + ".php?";
        foreach (var i in vars)
        {
            url += i + "&";
        }
        url = url.Substring(0, url.Length - 1);

        Debug.Log(url);
        
        var request = UnityWebRequest.Get(url);
        {
            yield return request.SendWebRequest();
            if (request.isDone && !request.isHttpError)
            {
                response = request.downloadHandler.text;
            }
        }
        
        yield return null;
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
