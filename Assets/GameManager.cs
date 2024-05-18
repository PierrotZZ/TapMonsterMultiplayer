using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] internal List<PlayerTest> players = new List<PlayerTest>();
    [SerializeField] GameObject[] playerSprite;
    [SerializeField] internal MonsterScript monster;
    // Start is called before the first frame update

    private void Awake()
    {

    }

    public void AddMoney(int amount)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null)
            {
                players[i].playerData.Money += amount;
            }
        }
        TrackMoneyClientRpc(players[0].playerData.Money);
    }

    [ClientRpc]
    public void TrackMoneyClientRpc(int moneyAmount)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null)
            {
                players[i].playerData.Money = moneyAmount;
            }
        }
    }


    private void Update()
    {
        UpdateSprite();
    }


    void UpdateSprite()
    {
        switch (players.Count)
        {
            case 0:
                playerSprite[0].SetActive(false);
                playerSprite[1].SetActive(false);
                playerSprite[2].SetActive(false);
                break;

            case 1:
                playerSprite[0].SetActive(true);
                playerSprite[1].SetActive(false);
                playerSprite[2].SetActive(false);
                break;

            case 2:
                playerSprite[0].SetActive(true);
                playerSprite[1].SetActive(true);
                playerSprite[2].SetActive(false);
                break;
            case 3:
                playerSprite[0].SetActive(true);
                playerSprite[1].SetActive(true);
                playerSprite[2].SetActive(true);
                break;

        }
    }

}
