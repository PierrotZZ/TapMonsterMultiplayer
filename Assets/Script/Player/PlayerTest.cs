using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies;

public class PlayerTest : NetworkBehaviour
{
    [SerializeField] public PlayerData playerData;
    [SerializeField] Animator animator;
    internal float time;
    int num;

    private void OnApplicationQuit()
    {

    }
    public override void OnDestroy()
    {
        var _Save = CloudSaveManager.Instance.Save(playerData);
        //NetworkManager.Singleton.Shutdown();
    }
    private void Start()
    {
        playerData = CloudSaveManager.Instance._playerData;
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
        GameManager.Instance.monster.TakeDamage(playerData.Damage);
    }





}
