using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class URLLoader : V2Singleton<URLLoader>
{
    public Image img;
    byte[] imgData;
    string imgDownloadURL;

    bool updateBool = false;
    int currentIndex = 0;

    System.Random rand = new System.Random();

    private static string baseUrl = "https://e621.net/posts?tags=solo+order%3Arandom+rating%3Asafe";
    private static string normalURL = "https://e621.net/posts?tags=solo+rating%3Asafe+order%3Arandom";
    private static string nsfwURL =   "https://e621.net/posts?tags=solo+order%3Arandom";
    public Queue<int> postNumbers = new Queue<int>();

    public enum SearchMode
    {
        all,
        sfwOnly,
        nsfwOnly
    }

    private SearchMode searchMode = SearchMode.nsfwOnly;

    protected override void Awake()
    {
        base.Awake();
    }

    public void UpdateTexture(byte[] imgDat)
    {
        imgData = imgDat;
        TestServerBehaviour.Instance.responder.SendImageToAll(imgDat);
        //TestServerBehaviour.Instance.responder.SendImageToAll(imgDat);
        
        imgData = imgDat;
        //Debug.Log("OpenImageRoutine");
        Texture2D txt = new Texture2D(2, 2);
        txt.LoadImage(imgData);

        Sprite s = Sprite.Create(txt, new Rect(0.0f, 0.0f, txt.width, txt.height), new Vector2(0.5f, 0.5f), 100.0f);
        img.sprite = s;
    }

    public IEnumerator OpenImage(string url)
    {
        yield return StartCoroutine(DBManager.OpenImage(url));
        if (DBManager.text != null)
        {
            UpdateTexture(DBManager.imgData);
        }
    }

    private void GetPostNumbersFromE621Search(ref string fileContent)
    {
        Regex regex = new Regex(@"ID: *[0-9]*");
        MatchCollection matches = regex.Matches(fileContent);

        if (matches.Count > 0)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                // het fucking werkt gewoon
                string ss = matches[i].Value.Substring(4, matches[i].Value.Length - 4);
                int j = -1;
                int.TryParse(ss, out j);
                postNumbers.Enqueue(j);
            }
        }
    }

    public IEnumerator GetSearchResults()
    {
        yield return StartCoroutine(DBManager.OpenURL(normalURL));
        if (DBManager.response != "")
        {
            GetPostNumbersFromE621Search(ref DBManager.response);
        }
    }

    /// <summary>
    /// Zoekt de downloadlink van de afbeelding op en opent hem
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private IEnumerator GetContentFromRandomPost(string url)
    {
        imgDownloadURL = "";
        yield return StartCoroutine(DBManager.OpenURL(url));
        if (DBManager.response != "")
        {
            string s = DBManager.response;
            DBManager.response = "";

            int downloadTagStartIndex = s.IndexOf("<div id=\"image-download-link\">");
            if (downloadTagStartIndex != -1)
            {
                int linkStartIndex = s.IndexOf("href=", downloadTagStartIndex);
                if (linkStartIndex != -1)
                {
                    int nQuotes = 0;
                    int endPos = -1;
                    for (int i = linkStartIndex; i < linkStartIndex + 200; i++)
                    {
                        if (s[i] == '"')
                        {
                            nQuotes++;
                        }
                        if (nQuotes == 2)
                        {
                            endPos = i;
                            break;
                        }
                    }
                    if (endPos != -1)
                    {
                        imgDownloadURL = s.Substring(linkStartIndex + 6, endPos - linkStartIndex - 6);
                        StartCoroutine(OpenImage(imgDownloadURL));
                        //Debug.Log("Downloadlink = " + imgDownloadURL);
                    }
                }
            }
        }
    }


    private void OpenRandomPost()
    {
        int postID = rand.Next(10, 2000000);
        if (postNumbers.Count > 0)
        {
            postID = postNumbers.Dequeue();
            postNumbers.Enqueue(postID);
        }
        string s = "https://e621.net/posts/" + postID.ToString();
        StartCoroutine(GetContentFromRandomPost(s));
    }

    public void GetImage()
    {
        OpenRandomPost();
        //StartCoroutine(OpenImage("https://studenthome.hku.nl/~tjaard.vanverseveld/content/images/rollercoaster_tycoon.jpg"));
    }

    private void Start()
    {
        StartCoroutine(GetSearchResults());
    }
}
