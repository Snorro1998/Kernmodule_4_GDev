using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

[System.Serializable]
public class FileWithHash
{
    public string filename;
    public string filehash;

    public FileWithHash(string _filename, string _filehash)
    {
        filename = _filename;
        filehash = _filehash;
    }
}

[System.Serializable]
public class AllFileData
{
    public List<FileWithHash> allFiles = new List<FileWithHash>();

    public void LoadResources()
    {
        string[] paths = Directory.GetFiles(Application.dataPath + "/Resources/", "*", SearchOption.AllDirectories);

        foreach (var path in paths)
        {
            if (path.LastIndexOf(".meta") == -1)
            {
                string fileName = path.Substring(path.IndexOf("/Resources/") + "/Resources/".Length).Replace("\\", "/");
                string fileHash = Utils.CalculateMD5(path);
                allFiles.Add(new FileWithHash(fileName, fileHash));
            }
        }
        //PrintData();
    }

    public void PrintData()
    {
        foreach (var i in allFiles)
        {
            Debug.Log(i.filename + ", " + i.filehash);
        }
    }
}

public class ResourceLoader : MonoBehaviour
{
    public bool updateDataDetails = true;
    public AllFileData allFiles;

    //private AudioSource aud;
    //private AudioSource aud2;
    //public bool writeit = false;
    //public bool loadit = false;
    //byte[] output;

    //public int position = 0;
    //public int samplerate = 44100;
    //public float frequency = 440;

    //public string audiofile;

    

    // Start is called before the first frame update
    void Start()
    {
        /*
        float[] floats = { 2.0f };
        var bytes = Utils.FloatArrayToByteArray(floats);
        foreach(var b in bytes)
        {
            Debug.Log(b);
        }
        */
        /*
        aud = gameObject.AddComponent<AudioSource>();
        aud2 = gameObject.AddComponent<AudioSource>();
        */
        InitSettings();
    }

    private void InitSettings()
    {
        string path = Application.dataPath + "/datadetails.json";

        if (!File.Exists(path))
        {
            DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.SERVER_DATA_DETAILS_FILE_DOENST_EXIST);
            //Debug.Log("Datadetails bestaat niet! Wordt nu aangemaakt.");
            allFiles = new AllFileData();
            allFiles.LoadResources();
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(allFiles));
        }
        else
        {
            DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.SERVER_DATA_DETAILS_FILE_EXISTS);
            //Debug.Log("Datadetails bestaat.");
            
            if (updateDataDetails)
            {
                //Debug.Log("Wordt bijgewerkt...");
                allFiles = new AllFileData();
                allFiles.LoadResources();
                System.IO.File.WriteAllText(path, JsonUtility.ToJson(allFiles));
            }

            else
            {
                //Debug.Log("Wordt ingeladen...");
                var str = System.IO.File.ReadAllText(Application.dataPath + "/datadetails.json");
                allFiles = JsonUtility.FromJson<AllFileData>(str);
            }
        }        
    }
}
