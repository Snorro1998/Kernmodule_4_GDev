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
    public byte[] imgData;

    public enum SearchMode
    {
        sfwOnly,
        nsfwOnly,
        allResults
    }

    public SearchMode searchMode = SearchMode.nsfwOnly;

    //System.Random rand = new System.Random();
    public MonsterImageWebsiteE621 FirstSite;// = new MonsterImageWebsiteE621(new List<string>);

    protected override void Awake()
    {
        base.Awake();
        FirstSite = new MonsterImageWebsiteE621 (
            new List<string>() {"solo", "looking_at_viewer"},
            new List<string>() {"solo", "looking_at_viewer", "knot" }
        );
    }

    /// <summary>
    /// Opent de eerstvolgende opgeslagen weblink uit een monsterimagewebsite-object en downloadt de bijbehorende afbeelding en tekst.
    /// </summary>
    /// <param name="site"></param>
    /// <returns></returns>
    public IEnumerator GetImage(MonsterImageWebsite site, MonsterImageWebsite.ImageMode mode)
    {
        string imagePage = site.GetImagePage(mode);
        Debug.Log("Opent de pagina: " + imagePage);
        yield return StartCoroutine(DBManager.OpenURL(imagePage));
        if (DBManager.response != null)
        {
            GetSpeciesTags(DBManager.response);
            string imgLink = site.FetchImageDownloadLink(DBManager.response);
            if (imgLink == "")
            {
                Debug.LogError("Kan geen downloadlink van de afbeelding vinden!");
            }
            else
            {
                Debug.Log("Downloadlink afbeelding = " + imgLink);
                yield return StartCoroutine(DBManager.OpenImage(imgLink));
                if (DBManager.imgData != null)
                {
                    imgData = DBManager.imgData;
                    TestServerBehaviour.Instance.responder.SendImageToAll(imgData);
                }
            }
        }
        else
        {
            Debug.LogError("Kan de pagina niet openen!");
        }
    }

    /// <summary>
    /// Voert een zoekopdracht uit voor een monsterimagewebsite-object en slaat de resulterende weblinks hierin op.
    /// </summary>
    /// <param name="site"></param>
    /// <returns></returns>
    IEnumerator initImageWebsite(MonsterImageWebsite site)
    {
        Debug.Log("initImageWebsite eerste start");
        string searchHTML = site.URL_SEARCH_SFW;
        yield return StartCoroutine(DBManager.OpenURL(searchHTML));
        if (DBManager.response != null)
        {
            site.FetchSearchResults(DBManager.response, false);
        }
        Debug.Log("initImageWebsite eerste klaar");
        
        yield return new WaitForSeconds(5);
        Debug.Log("initImageWebsite tweede start");
        searchHTML = site.URL_SEARCH_NSFW;
        yield return StartCoroutine(DBManager.OpenURL(searchHTML));
        if (DBManager.response != null)
        {
            site.FetchSearchResults(DBManager.response, true);
        }
        Debug.Log("initImageWebsite tweede klaar");

    }

    private void GetSpeciesTags(string html)
    {
        int j = 0;
        int findStringBeginIndex = html.IndexOf("<ul class=\"species-tag-list\">", j);
        
        if (findStringBeginIndex != -1)
        {
            int findStringEndIndex = html.IndexOf("</ul>", findStringBeginIndex);
            string s = html.Substring(findStringBeginIndex, findStringEndIndex - findStringBeginIndex);
            GetTagsFromURL(s);
        }
    }

    private void GetTagsFromURL(string html)
    {
        int j = 0;
        for (int i = 0; i < 1000; i++)
        {
            int findStringBeginIndex = html.IndexOf("<a rel=\"nofollow\" class=\"search-tag\"", j);
            if (findStringBeginIndex == -1)
            {
                break;
            }
            else
            {
                int findStringEndIndex = html.IndexOf("</a>", findStringBeginIndex);
                findStringBeginIndex = html.LastIndexOf(">", findStringEndIndex) + 1;
                string ss = html.Substring(findStringBeginIndex, findStringEndIndex - findStringBeginIndex);
                Debug.Log("tag" + i + ": " + ss);
                j = findStringBeginIndex;
            }
        }
    }

    private void Start()
    {   
        StartCoroutine(initImageWebsite(FirstSite));
    }
}
