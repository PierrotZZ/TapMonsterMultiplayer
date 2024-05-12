using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public MonsterScript monster;

    internal PlayerData playerData = new PlayerData();
    [SerializeField] int moneyPlayer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            monster.TakeDamage();
        }
        moneyPlayer = playerData.Money;
    }
}
