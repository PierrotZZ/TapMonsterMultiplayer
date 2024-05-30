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
using Unity.VisualScripting;

public class LobbyManager : MonoBehaviour
{
    public static event Action CallUpdateLobby;
    float heartbeatTimer = 0;

    public TMP_InputField lobbyCodeInput;
    public GameObject introLobby, lobbyPanel, singInPanel;
    public TMP_Text[] playerNameText;
    public TMP_Text lobbyCodeText;
    Lobby hostLobby, joinnedLobby;

    public Button startGameBtn;
    public Button leaveLobbyBtn;


    [Header("Login")]
    public CloudSaveManager cloudSaveManager;
    public TMP_InputField usersName;
    public TMP_InputField password;
    // public TMP_InputField playerNameInput;
    public Button loginBtn;
    public Button signUpBtn;
    public Button loginSectionBtn;
    public Button signUpSectionBtn;
    public GameObject selectSection;
    public GameObject loginSection;

    public Button backToManu;
    public TMP_Text error;


    public string hostId;
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
        leaveLobbyBtn.onClick.AddListener(OnClickLeaveLobby);

        // await Authenticate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateLobby();
            ShowPlayer();
        }
        CheckStartGame();
    }

    private void OnClickBackToManu()
    {
        loginSection.SetActive(false);
        loginBtn.gameObject.SetActive(false);
        signUpBtn.gameObject.SetActive(false);
        //playerNameInput.gameObject.SetActive(false);
        usersName.text = "";
        password.text = "";
        //playerNameInput.text = "";
        backToManu.gameObject.SetActive(false);
        selectSection.SetActive(true);
    }

    private void OnClickSectionSignInBtn()
    {
        selectSection.gameObject.SetActive(false);
        loginSection.gameObject.SetActive(true);
        signUpBtn.gameObject.SetActive(true);
        //playerNameInput.gameObject.SetActive(true);
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
        var respond = cloudSaveManager.SignUp(usersName.text, password.text, "nu", true);
        CloudSaveManager.Instance.username = usersName.text;
        CloudSaveManager.Instance.statusLoginCheck = true;
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

        Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
        lobbyData.Add("IsStart", new DataObject(DataObject.VisibilityOptions.Member, "false"));
        options.Data = lobbyData;

        LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += OnLobbyChanged;

        hostLobby = await Lobbies.Instance.CreateLobbyAsync("lobby", 3, options);
        joinnedLobby = hostLobby;
        Debug.Log("Create lobby " + hostLobby.LobbyCode);
        Debug.Log(hostLobby.Data["IsStart"].Value);

        try
        {
            var addedCallback = LobbyService.Instance.SubscribeToLobbyEventsAsync(hostLobby.Id, callbacks);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Reason);
        }

        InvokeRepeating("SendLobbyHeartBeat", 10, 10);
        UpdateLobby();
        ShowPlayer();
        Debug.Log("Host lobby id" + hostLobby.Id);

        hostId = cloudSaveManager._playerId;
        Debug.Log(hostLobby.Id);
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
        Debug.Log(joinnedLobby.Data["IsStart"].Value);

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
        UpdateLobby();
        ShowPlayer();

        SendLobbyHeartBeat();
        Debug.Log(joinnedLobby.Id);
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

    //HeartBeat
    async void SendLobbyHeartBeat()
    {
        if (hostLobby == null)
        {
            return;
        }

        await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        Debug.Log("HeartBeatUpdate");
        UpdateLobby();
        ShowPlayer();
    }

    private async void SendHeartbeatTimer()
    {
        if (hostLobby == null) return;
        heartbeatTimer += Time.deltaTime;
        if (heartbeatTimer > 15)
        {
            heartbeatTimer = 0;
            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
    }

    void ShowPlayer()
    {
        for (int i = 0; i < joinnedLobby.Players.Count; i++)
        {
            playerNameText[i].text = joinnedLobby.Players[i].Data["name"].Value;
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
            Debug.Log(onChange.PlayerJoined.Value[0].Player.Data["name"].Value);
            SendLobbyHeartBeat();
            UpdateLobby();
            ShowPlayer();
        }
        if (onChange.PlayerLeft.Value != null)
        {
            Debug.Log(onChange.PlayerLeft.Value[0]);
        }
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

    void CheckStartGame()
    {
        if (hostId != null)
        {
            if (hostId == cloudSaveManager._playerId)
            {
                startGameBtn.interactable = true;
            }
            else
            {
                startGameBtn.interactable = false;
            }
        }
    }

    public void OnClickLeaveLobby()
    {
        introLobby.SetActive(true);
        lobbyPanel.SetActive(false);
        UpdateLobby();
        ShowPlayer();
        OnClientDisconnected();
    }
    private void OnClientDisconnected()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Client disconnected from server.");

        }
        else if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Host shut down the server.");
        }
    }
}
