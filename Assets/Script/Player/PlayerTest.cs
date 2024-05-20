using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Netcode;

public class PlayerTest : NetworkBehaviour
{
    internal PlayerData playerData = new PlayerData();
    
    //Player data stat
    [SerializeField] internal int moneyPlayer;
    [SerializeField] internal int playerDamage;
    [SerializeField] internal int playerDamageLevel;
    [SerializeField] internal int playerDamagePrice;
    [SerializeField] internal float playerFirerate;
    [SerializeField] internal int playerFireRateLevel;
    [SerializeField] internal int playerFireRatePrice;
    [SerializeField] Animator animator;
    internal float time;
    int num;

    private void Start()
    {
        GameManager.Instance.players.Add(this);
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;
        if (time >= playerData.fireRate && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!IsOwner) return;
            PlayAnimation();
            AttackMonsterServerRpc();
            time = 0;
        }

        //Get data form "playerData"
        //Money
        moneyPlayer = playerData.Money;
        //Damage
        playerDamage = playerData.Damage;
        playerDamageLevel = playerData.DamageLevel;
        playerDamagePrice = playerData.DamagePrice;
        //Fire rate
        playerFirerate = playerData.fireRate;
        playerFireRateLevel = playerData.FireRateLevel;
        playerFireRatePrice = playerData.FireRatePrice;

    }

    void PlayAnimation()
    {
        // if (num == 0)
        // {
        //     animator.SetTrigger("SwingLeft");
        //     num++;
        // }
        // else
        // {
        //     animator.SetTrigger("SwingRight");
        //     num = 0;
        // }
        if (!IsOwner) return;
        PlayAnimationServerRpc();
    }

    [ServerRpc]
    void PlayAnimationServerRpc()
    {
        if (num == 0)
        {
            animator.SetTrigger("SwingRight");
            num++;
        }
        else
        {
            animator.SetTrigger("SwingLeft");
            num = 0;
        }
    }

    [ServerRpc]
    void AttackMonsterServerRpc()
    {
        GameManager.Instance.monster.TakeDamage(playerDamage);
    }





}
