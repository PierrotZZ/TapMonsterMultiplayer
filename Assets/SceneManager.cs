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

    // Start is called before the first frame update

    private void Awake()
    {

    }
    void Start()
    {
        quitButton.onClick.AddListener(BackToLobby); //Add function to the button
    }

    // Update is called once per frame
    void Update()
    {
        if (hostHasQuit)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        }
    }

    void BackToLobby()
    {
        ShutdownServerRpc();
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

}
