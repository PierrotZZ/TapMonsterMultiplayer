using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnerScript : Singleton<MonsterSpawnerScript>
{
    [SerializeField] GameObject monsterGo;

    
    [SerializeField] float spawnTime;
    float delay;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RespawnMonster();
    }

    void RespawnMonster()
    {
        if (monsterGo.activeInHierarchy == false)
        {
            delay += Time.deltaTime;
            if (delay >= spawnTime)
            {
                delay = 0;
                monsterGo.SetActive(true);
            }
        }
    }




}



