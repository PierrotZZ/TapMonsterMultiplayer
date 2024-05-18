using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Netcode;

public class PlayerTest : NetworkBehaviour
{

    internal PlayerData playerData = new PlayerData();
    [SerializeField] int moneyPlayer;
    [SerializeField] int playerDamage;
    [SerializeField] float playerFirerate;
    float time;

    private void Start()
    {
        GameManager.Instance.players.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= playerData.fireRate && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!IsOwner) return;
            AttackMonsterServerRpc();
            time = 0;
        }

        playerData.Damage = playerDamage;
        playerData.fireRate = playerFirerate;
        moneyPlayer = playerData.Money;

    }

    [ServerRpc]
    void AttackMonsterServerRpc()
    {
        GameManager.Instance.monster.TakeDamage(playerData.Damage);
    }



}
