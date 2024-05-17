using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using System;

public class LobbyManager : MonoBehaviour
{
    public TMP_InputField lobbyCodeInput;
    public GameObject introLobby, lobbyPanel;
    public TMP_Text[] playerNameText;
    public TMP_Text lobbyCodeText;
    Lobby hostLobby, joinnedLobby;


    [Header("Login")]
    public CloudSaveManager cloudSaveManager;
    public TMP_InputField usersName;
    public TMP_InputField password;
    public TMP_InputField playerNameInput;
    public Button loginBtn;
    public Button signInBtn;
    public Button loginSectionBtn;
    public Button signInSectionBtn;
    public GameObject selectSection;
    public GameObject loginSection;


    public string playerId;

    // Start is called before the first frame update
    async void Start()
    {
        cloudSaveManager.lobbyManager = this;
        await UnityServices.InitializeAsync();
        loginBtn.onClick.AddListener(OnClickLogin);
        signInBtn.onClick.AddListener(OnClickSingIn);
        loginSectionBtn.onClick.AddListener(OnClickSectionloginBtn);
        signInSectionBtn.onClick.AddListener(OnClickSectionSignInBtn);

        await Authenticate();
    }

    private void OnClickSectionSignInBtn()
    {
        selectSection.gameObject.SetActive(false);
        loginSection.gameObject.SetActive(true);
        signInBtn.gameObject.SetActive(true);
        playerNameInput.gameObject.SetActive(true);
    }
    private void OnClickSectionloginBtn()
    {
        selectSection.gameObject.SetActive(false);
        loginSection.gameObject.SetActive(true);
        loginBtn.gameObject.SetActive(true);
    }
    private void OnClickSingIn()
    {
        var respond = cloudSaveManager.SignIn(usersName.text, password.text, playerNameInput.text, true);
        OpenIntroLobby();
    }
    private void OnClickLogin()
    {
        var respond = cloudSaveManager.SignIn(usersName.text, password.text, "", false);
        OpenIntroLobby();
    }
    void OpenIntroLobby()
    {
        introLobby.gameObject.SetActive(true);
        loginSection.SetActive(false);
    }
    async Task Authenticate()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            return;
        }
        // AuthenticationService.Instance.ClearSessionToken();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        playerId = AuthenticationService.Instance.PlayerId;
        Debug.Log("User login : " + AuthenticationService.Instance.PlayerId);
    }

    async public void CreateLobby()
    {

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
