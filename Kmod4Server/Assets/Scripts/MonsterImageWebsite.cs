using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class MonsterImageWebsite
{
    // DISCUSS misschien is het beter om nsfw-content er helemaal uit te laten
    public virtual string URL_SEARCH_SFW { get; set; }
    public virtual string URL_SEARCH_NSFW { get; set; }
    public virtual string URL_IMAGE_PAGE { get; set; }

    public Queue<string> imagePagesSFW = new Queue<string>();
    public Queue<string> imagePagesNSFW = new Queue<string>();

    public List<string> searchTagsSFW = new List<string>();
    public List<string> searchTagsNSFW = new List<string>();

    System.Random rand = new System.Random();

    public enum ImageMode
    {
        sfwOnly,
        nsfwOnly,
        all
    }

    public virtual void AddTags()
    {
        foreach (string s in searchTagsSFW)
        {
            URL_SEARCH_SFW += "+" + s;
        }
        foreach (string s in searchTagsNSFW)
        {
            URL_SEARCH_NSFW += "+" + s;
        }
        //Debug.Log(URL_SEARCH_SFW);
    }

    public virtual void FetchSearchResults(string html, bool nsfw)
    {
        
    }

    public virtual string FetchImageDownloadLink(string html)
    {
        return "";
    }

    public string GetImagePage(ImageMode mode)
    {
        Queue<string> imageList = new Queue<string>();

        switch (mode)
        {
            default:
                imageList = imagePagesSFW;
                break;
            case ImageMode.all:
                imageList = rand.Next(0, 1) == 0 ? imagePagesSFW : imagePagesNSFW;
                break;
            case ImageMode.nsfwOnly:
                imageList = imagePagesNSFW;
                break;
        }

        string s = "";
        if (imageList.Count > 0)
        {
            s = imageList.Dequeue();
            imageList.Enqueue(s);
        }
        return s;
    }
}

public class MonsterImageWebsiteE621 : MonsterImageWebsite
{
    public MonsterImageWebsiteE621(List<string> _searchTagsSFW, List<string> _searchTagsNSFW)
    {
        URL_SEARCH_SFW = "https://e621.net/posts?tags=order%3Arandom+rating%3Asafe";
        URL_SEARCH_NSFW ="https://e621.net/posts?tags=order%3Arandom+-rating%3Asafe";
        URL_IMAGE_PAGE = "https://e621.net/posts/"; // met een willekeurig getal erachter
        searchTagsSFW = _searchTagsSFW;
        searchTagsNSFW = _searchTagsNSFW;
        AddTags();
    }

    public override void FetchSearchResults(string html, bool nsfw)
    {
        // DISCUSS Misschien is het beter om een fatsoenlijke htmlparser hiervoor te gebruiken
        Queue<string> imgPages = new Queue<string>();
        imgPages = nsfw ? imagePagesNSFW : imagePagesSFW;
        Regex regex = new Regex(@"ID: *[0-9]*");
        MatchCollection matches = regex.Matches(html);

        if (matches.Count > 0)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                string ss = matches[i].Value.Substring(4, matches[i].Value.Length - 4);
                //int j = -1;
                //int.TryParse(ss, out j);
                imgPages.Enqueue(URL_IMAGE_PAGE + ss);
            }
        }
    }

    public override string FetchImageDownloadLink(string html)
    {
        // DISCUSS Misschien is het beter om een fatsoenlijke htmlparser hiervoor te gebruiken
        string imgDownloadURL = "";
        int downloadTagStartIndex = html.IndexOf("<div id=\"image-download-link\">");
        if (downloadTagStartIndex != -1)
        {
            int linkStartIndex = html.IndexOf("href=", downloadTagStartIndex);
            if (linkStartIndex != -1)
            {
                int nQuotes = 0;
                int endPos = -1;
                for (int i = linkStartIndex; i < linkStartIndex + 200; i++)
                {
                    if (html[i] == '"')
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
                    imgDownloadURL = html.Substring(linkStartIndex + 6, endPos - linkStartIndex - 6);
                }
            }
        }
        return imgDownloadURL;
    }
}
