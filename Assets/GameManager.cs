using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] internal List<PlayerTest> players = new List<PlayerTest>();
    [SerializeField] Transform[] spawnTransform;
    [SerializeField] internal MonsterScript monster;
    [SerializeField] StatUpgradeManager statUpgradeManager;
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
        //TrackMoneyClientRpc(players[0].playerData.Money);
    }

    public void AddMoney02(int amount)
    {
        statUpgradeManager.player.playerData.Money += amount;
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
        UpdatePosition();
        //HostOutClientRpc();
    }


    void UpdatePosition()
    {

        if (players != null)
        {
            switch (players.Count)
            {
                case 1:
                    players[0].transform.position = spawnTransform[0].position;
                    break;
                case 2:
                    players[1].transform.position = spawnTransform[1].position;
                    break;
                case 3:
                    players[2].transform.position = spawnTransform[2].position;
                    break;
                default:
                    return;

            }
        }




        // if (players[0] != null)
        // {
        //     players[0].transform.position = spawnTransform[0].position;
        // }

        // if (players[1] != null)
        // {
        //     players[1].transform.position = spawnTransform[1].position;
        // }

        // if (players[2] != null)
        // {
        //     players[2].transform.position = spawnTransform[2].position;
        // }


    }

    [ClientRpc]
    internal void HostOutClientRpc()
    {
        NetworkManager.Singleton.Shutdown();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        Debug.Log("Host shut down the server.");
    }


}
