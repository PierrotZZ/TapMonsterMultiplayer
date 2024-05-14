using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public TMP_InputField playerNameInput, lobbyCodeInput;
    public GameObject introLobby, lobbyPanel;
    public TMP_Text[] playerNameText;
    public TMP_Text lobbyCodeText;
    Lobby hostLobby, joinnedLobby;
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    async Task Authenticate()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            return;
        }

        AuthenticationService.Instance.ClearSessionToken();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("User login " + AuthenticationService.Instance.PlayerId);
    }

    async public void CreateLobby()
    {
        await Authenticate();

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = GetPlayer()
        };

        hostLobby = await Lobbies.Instance.CreateLobbyAsync("lobby", 3, options);
        joinnedLobby = hostLobby;
        Debug.Log("Create lobby " + hostLobby.LobbyCode);
        InvokeRepeating("SendLobbyHeartBeat", 2, 2);
        ShowPlayer();

        //Interface
        introLobby.SetActive(false);
        lobbyPanel.SetActive(true);
        lobbyCodeText.text = joinnedLobby.LobbyCode;
    }

    async public void JoinLobbyByCode()
    {
        await Authenticate();

        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
        {
            Player = GetPlayer()
        };

        joinnedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCodeInput.text, options);
        Debug.Log("Enter lobby " + joinnedLobby.LobbyCode);
        ShowPlayer();

        //Interface
        introLobby.SetActive(false);
        lobbyPanel.SetActive(true);
        lobbyCodeText.text = joinnedLobby.LobbyCode;
    }

    Player GetPlayer()
    {
        Player player = new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {"name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerNameInput.text)}
            }
        };

        return player;
    }

    async void SendLobbyHeartBeat()
    {
        if (hostLobby == null)
        {
            return;
        }

        await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        Debug.Log("Update lobby");
        UpdateLobby();
        ShowPlayer();
    }

    void ShowPlayer()
    {
        for (int i = 0; i < joinnedLobby.Players.Count; i++)
        {
                playerNameText[i].text = joinnedLobby.Players[i].Data["name"].Value;
                Debug.Log(joinnedLobby.Players[i].Data["name"].Value);
        }
    }

    async void UpdateLobby()
    {
        if (joinnedLobby == null)
        {
            return;
        }

        joinnedLobby = await LobbyService.Instance.GetLobbyAsync(joinnedLobby.Id);
    }
}
