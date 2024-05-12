using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] internal PlayerTest[] players;
    [SerializeField] internal MonsterScript monster;
    // Start is called before the first frame update

    private void Awake()
    {
        players = FindObjectsOfType<PlayerTest>();
    }

    public void AddMoney(int amount)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                players[i].playerData.Money += amount;
            }
        }
    }

}
