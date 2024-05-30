using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine.SceneManagement;

public class PlayerTest : NetworkBehaviour
{
    [SerializeField] public PlayerData playerData;
    [SerializeField] Animator animator;

    SceneManagerScript sceneManager;
    internal float time;
    int num;

    private void OnApplicationQuit()
    {
        var _Save = CloudSaveManager.Instance.Save(playerData);
    }
    public override void OnDestroy()
    {
        var _Save = CloudSaveManager.Instance.Save(playerData);
        //NetworkManager.Singleton.Shutdown();
    }
    private void Start()
    {
        sceneManager = FindObjectOfType<SceneManagerScript>();
        playerData = CloudSaveManager.Instance._playerData;
        GameManager.Instance.players.Add(this);
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneManager.canStart)
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
        else
        {
            
        }
    }

    void PlayAnimation()
    {
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
