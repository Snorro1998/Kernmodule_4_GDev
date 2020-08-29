using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRoom : MonoBehaviour
{
    public string MonsterName = "Grote Boze Luiaard";
    public int monsterHealth = 150;

    public int playerMaxHealth = 100;

    public ActiveRoom()
    {
        Debug.Log("Een " + MonsterName +  " verschijnt uit het niets!");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
