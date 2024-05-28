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
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static event Action CallUpdateLobby;

    public TMP_InputField lobbyCodeInput;
    public GameObject introLobby, lobbyPanel, singInPanel;
    public TMP_Text[] playerNameText;
    public TMP_Text lobbyCodeText;
    Lobby hostLobby, joinnedLobby;

    public Button startGameBtn;


    [Header("Login")]
    public CloudSaveManager cloudSaveManager;
    public TMP_InputField usersName;
    public TMP_InputField password;
    public TMP_InputField playerNameInput;
    public Button loginBtn;
    public Button signUpBtn;
    public Button loginSectionBtn;
    public Button signUpSectionBtn;
    public GameObject selectSection;
    public GameObject loginSection;

    public Button backToManu;
    public TMP_Text error;


    public string playerId;
    string currentJoinCode = "";
    string currentLobbyId;

    void OnEnable()
    {
        LobbyManager.CallUpdateLobby += UpdateLobby;
        LobbyManager.CallUpdateLobby += ShowPlayer;
    }

    void OnDisable()
    {
        LobbyManager.CallUpdateLobby -= UpdateLobby;
        LobbyManager.CallUpdateLobby -= ShowPlayer;
    }

    // Start is called before the first frame update
    async void Start()
    {
        Screen.SetResolution(1280, 720, false);

        CheckLogin();

        cloudSaveManager.lobbyManager = this;
        await UnityServices.InitializeAsync();
        loginBtn.onClick.AddListener(OnClickLogin);
        signUpBtn.onClick.AddListener(OnClickSingUp);
        loginSectionBtn.onClick.AddListener(OnClickSectionloginBtn);
        signUpSectionBtn.onClick.AddListener(OnClickSectionSignInBtn);
        backToManu.onClick.AddListener(OnClickBackToManu);
        startGameBtn.onClick.AddListener(OnClickStartGame);

        // await Authenticate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateLobby();
            ShowPlayer();
        }
    }

    private void OnClickBackToManu()
    {
        loginSection.SetActive(false);
        loginBtn.gameObject.SetActive(false);
        signUpBtn.gameObject.SetActive(false);
        playerNameInput.gameObject.SetActive(false);
        usersName.text = "";
        password.text = "";
        playerNameInput.text = "";
        backToManu.gameObject.SetActive(false);
        selectSection.SetActive(true);
    }

    private void OnClickSectionSignInBtn()
    {
        selectSection.gameObject.SetActive(false);
        loginSection.gameObject.SetActive(true);
        signUpBtn.gameObject.SetActive(true);
        playerNameInput.gameObject.SetActive(true);
        backToManu.gameObject.SetActive(true);
    }
    private void OnClickSectionloginBtn()
    {
        selectSection.gameObject.SetActive(false);
        loginSection.gameObject.SetActive(true);
        loginBtn.gameObject.SetActive(true);
        backToManu.gameObject.SetActive(true);
    }
    private void OnClickSingUp()
    {
        var respond = cloudSaveManager.SignUp(usersName.text, password.text, playerNameInput.text, true);

    }
    private void OnClickLogin()
    {
        var respond = cloudSaveManager.SignUp(usersName.text, password.text, "", false);
        CloudSaveManager.Instance.username = usersName.text;
        CloudSaveManager.Instance.statusLoginCheck = true;
    }
    public void OpenIntroLobby()
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
        InvokeRepeating("SendLobbyHeartBeat", 10, 10);
        ShowPlayer();
        Debug.Log("Host lobby id" + hostLobby.Id);
        //Interface
        introLobby.SetActive(false);
        lobbyPanel.SetActive(true);
        lobbyCodeText.text = joinnedLobby.LobbyCode;
        currentLobbyId = hostLobby.Id;
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

        LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += OnLobbyChanged;
        try
        {
            var addedCallback = LobbyService.Instance.SubscribeToLobbyEventsAsync
                    (joinnedLobby.Id, callbacks);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Reason);
        }
        
        CallUpdateLobby?.Invoke();

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
                {"name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, CloudSaveManager.Instance.username)}
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
        Debug.Log("ShowPlayer");
    }

    async void UpdateLobby()
    {
        if (joinnedLobby == null)
        {
            return;
        }
        Debug.Log("UpdateLobby");
        joinnedLobby = await LobbyService.Instance.GetLobbyAsync(joinnedLobby.Id);
    }

    void OnClickStartGame()
    {
        StartGame(currentLobbyId);
    }

    //Relay
    //Client
    private async void SetClientRelayConnection(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void StartGameClient(string joinCode)
    {
        lobbyPanel.SetActive(false);
        SetClientRelayConnection(joinCode);
    }

    //Host
    private async void SetHostRelaytConnection(string lobbyId)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        currentJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log(currentJoinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();

        try
        {
            UpdateLobbyOptions updateLobbyOption = new UpdateLobbyOptions();
            updateLobbyOption.Data = new Dictionary<string, DataObject>();
            updateLobbyOption.Data.Add("IsStart", new DataObject(DataObject.VisibilityOptions.Member, currentJoinCode));

            hostLobby = await LobbyService.Instance.UpdateLobbyAsync(lobbyId, updateLobbyOption);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        Debug.Log(hostLobby.Data["IsStart"].Value);
    }

    public async void StartGame(string lobbyId)
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        lobbyPanel.SetActive(false);
        SetHostRelaytConnection(lobbyId);
        CloudSaveManager.Instance._lobbyId = lobbyId;
    }

    //lobby change
    private void OnLobbyChanged(ILobbyChanges onChange)
    {
        if (onChange.PlayerJoined.Value != null)
        {
            Debug.Log(onChange.PlayerJoined.Value[0].Player.Data["PlayerName"].Value);
            UpdateLobby();
            ShowPlayer();
        }

        if (onChange.PlayerLeft.Value != null)
        {
            Debug.Log(onChange.PlayerLeft.Value[0]);
        }

        // Debug.Log(onChange.Data.Changed);
        // Debug.Log(onChange.Data.ChangeType);
        // Debug.Log(onChange.Data.Value);
        if (onChange.Data.Changed == true)
        {
            Debug.Log(onChange.Data.Value["IsStart"].Value.Value);
            StartGameClient(onChange.Data.Value["IsStart"].Value.Value);
        }
    }

    void CheckLogin()
    {
        if (CloudSaveManager.Instance.statusLoginCheck)
        {
            singInPanel.SetActive(false);
            introLobby.SetActive(true);
        }
        else
        {
            singInPanel.SetActive(true);
            introLobby.SetActive(false);
        }
    }
}
