using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using WebSocketSharp.Server;
using System.Linq;
using Unity.Collections;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] Button quitButton;

    [SerializeField] StatUpgradeManager statUpgradeManager;

    [SerializeField] bool hostHasQuit = false;

    [Header("Notifacation host quit")]
    [SerializeField] Button quitButtonClient;
    [SerializeField] GameObject notificationHostQuit;
    public bool canStart;
    public GameObject blackDrop;

    // Start is called before the first frame update

    private void Awake()
    {

    }
    void Start()
    {
        StartCoroutine(DelayGameStart());
        quitButton.onClick.AddListener(BackToLobby); //Add function to the button
        quitButtonClient.onClick.AddListener(BackToLobby); //Add function to the button
    }

    // Update is called once per frame
    void Update()
    {

    }

    void BackToLobby()
    {
        // ShutdownServerRpc();
        if (!NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            Debug.Log("Client disconnected from server.");

        }
        else if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            Debug.Log("Host shut down the server.");
            GameManager.Instance.HostOutClientRpc();
        }
        var _Save = CloudSaveManager.Instance.Save(statUpgradeManager.player.playerData);
        //NetworkManager.Singleton.Shutdown();
    }

    [ServerRpc]
    void ShutdownServerRpc()
    {
        hostHasQuit = true;
        ClientQuitClientRpc(hostHasQuit);
    }

    [ClientRpc]
    void ClientQuitClientRpc(bool quit)
    {
        hostHasQuit = quit;
    }

    [ServerRpc]
    void ShowNotificationServerRpc()
    {
        notificationHostQuit.SetActive(true);
    }

    IEnumerator DelayGameStart()
    {
        blackDrop.SetActive(true);
        canStart = false;
        yield return new WaitForSeconds(3f);
        blackDrop.SetActive(false);
        canStart = true;
    }

}
