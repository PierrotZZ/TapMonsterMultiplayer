using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Netcode;

public class PlayerTest : NetworkBehaviour
{

    internal PlayerData playerData = new PlayerData();
    [SerializeField] internal int moneyPlayer;
    [SerializeField] internal int playerDamage;
    [SerializeField] internal float playerFirerate;
    [SerializeField] Animator animator;
    float time;
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

        playerData.Damage = playerDamage;
        playerData.fireRate = playerFirerate;
        playerData.Money = moneyPlayer;

    }

    void PlayAnimation()
    {
        if (num == 0)
        {
            animator.SetTrigger("SwingLeft");
            num++;
        }
        else
        {
            animator.SetTrigger("SwingRight");
            num = 0;
        }
        if (!IsOwner) return;
        PlayAnimationServerRpc();
    }

    [ServerRpc]
    void PlayAnimationServerRpc()
    {
        if (num == 0)
        {
            animator.SetTrigger("SwingLeft");
            num++;
        }
        else
        {
            animator.SetTrigger("SwingRight");
            num = 0;
        }
    }

    [ServerRpc]
    void AttackMonsterServerRpc()
    {
        GameManager.Instance.monster.TakeDamage(playerDamage);
    }





}
