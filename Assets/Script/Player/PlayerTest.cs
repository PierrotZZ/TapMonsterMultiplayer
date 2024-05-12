using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{

    internal PlayerData playerData = new PlayerData();
    [SerializeField] int moneyPlayer;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameManager.Instance.monster.TakeDamage();
        }
        moneyPlayer = playerData.Money;
    }
}
