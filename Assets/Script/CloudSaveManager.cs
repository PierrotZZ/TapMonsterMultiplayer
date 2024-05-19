using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class CloudSaveManager : MonoBehaviour
{
    [HideInInspector] public LobbyManager lobbyManager;
    public string username;
    public string password;
    public TMP_Text textPlayer;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var testsave = Save();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var testLoad = Load();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            var testCreateAccount = CreateAccount("Admin", "Admin", "Admin");
        }
        textPlayer.text = AuthenticationService.Instance.PlayerId;
    }
    void Start()
    {
        Dictionary<string, List<string>> a = new Dictionary<string, List<string>>();
        List<string> playerInfo = new List<string>();
        UsersPassword usersData = new UsersPassword();
        usersData.Users = "username";
        usersData.Password = "password";
        playerInfo.Add("username");
        playerInfo.Add("password");
        a.Add("player1", playerInfo);
        List<string> responPlayerInfo = new List<string>();
        responPlayerInfo = a["player1"];
        // Debug.Log(a["player1"]);
        // Debug.Log(responPlayerInfo[0]);
        // Debug.Log(responPlayerInfo[1]);
        Dictionary<UsersPassword, string> b = new Dictionary<UsersPassword, string>();
        UsersPassword newUsersData1 = new UsersPassword();
        newUsersData1.Users = "username1";
        newUsersData1.Password = "password2";
        UsersPassword newUsersData2 = new UsersPassword();
        newUsersData1.Users = "username2";
        newUsersData1.Password = "password3";
        UsersPassword newUsersData3 = new UsersPassword();
        newUsersData1.Users = "username4";
        newUsersData1.Password = "password5";
        // b.Add(newUsersData1, "55265142582");
        // b.Add(newUsersData2, "43787868789");
        // b.Add(newUsersData3, "42824276787");
        // Debug.Log(b.Values.Count);
        foreach (KeyValuePair<UsersPassword, string> s in b)
        {
            if (s.Key == newUsersData1)
            {
                // Debug.Log("have user : " + s.Value);
            }
            else
            {
                // Debug.Log("not have user");
            }
        }

        // var testSignUp = SignUpWithUsernamePasswordAsync("Admin", "Admin");
    }
    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            // Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            // Debug.LogException(ex);
            Debug.Log(ex.Message);
            Debug.Log("Insert at least 1 uppercase, 1 lowercase, 1 digit and 1 symbol. With minimum 8 characters and a maximum of 30");
            Debug.Log("Usersname Has Already");
        }
        Debug.Log("User login : " + AuthenticationService.Instance.PlayerId);
    }
    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        Debug.Log("User login : " + AuthenticationService.Instance.PlayerId);
    }
    public async Task SignIn(string _users, string _password, string name, bool newUsers)
    {
        if (newUsers)
        {
            Debug.Log("New Users");
            Debug.Log(_users + " : " + _password + " : " + name);
            var SignUp = SignUpWithUsernamePasswordAsync(_users, _password);
        }
        else
        {
            Debug.Log("Login");
            var SignIn = SignInWithUsernamePasswordAsync(_users, _password);
        }
    }
    public async Task CreateAccount(string _username, string _password, string name)
    {
        UsersPassword newUsersPassword = new UsersPassword();
        newUsersPassword.Users = _username;
        newUsersPassword.Password = _password;
        var respond = await CloudCodeService.Instance.CallModuleEndpointAsync("SaveModule"
                , "CreateAccount"
                , new Dictionary<string, object> { { "PlayerId", "" }, { "UsersPassword", newUsersPassword } });
        Debug.Log(respond);
        ResultContainerUsers resultContainerUsers = JsonUtility.FromJson<ResultContainerUsers>(respond);
        Debug.Log(resultContainerUsers.results[0].value.UsersData._UsersData.Values);
        UsersData receiveUsersData = resultContainerUsers.results[0].value.UsersData;
        Debug.Log(receiveUsersData._UsersData.Values.Count);
        foreach (KeyValuePair<UsersPassword, string> w in receiveUsersData._UsersData)
        {
            Debug.Log(w.Value);
        }
    }
    public async Task Load()
    {
        Debug.Log("LoadData");
        var respond = await CloudCodeService.Instance.CallModuleEndpointAsync("SaveModule"
                , "LoadPlayerData"
                , new Dictionary<string, object> { { "PlayerId", lobbyManager.playerId } });
        // Debug.Log(respond + " : respond");
        ResultContainer resultContainer = JsonUtility.FromJson<ResultContainer>(respond);
        Debug.Log(resultContainer.results[0].value.Damage);
    }
    //Save
    public async Task Save()
    {
        Debug.Log("SaveData");
        PlayerData testPlayerData = new PlayerData();
        testPlayerData.PlayerName = "Test1";
        testPlayerData.Damage = 100;
        testPlayerData.Money = 500;
        testPlayerData.fireRate = 50f;
        testPlayerData.PlayerName = "Test1";
        string jsonDefaultData = JsonConvert.SerializeObject(testPlayerData);
        Debug.Log(jsonDefaultData.ToString());
        var respond = await CloudCodeService.Instance.CallModuleEndpointAsync("SaveModule"
        , "SavePlayerData"
        , new Dictionary<string, object> { { "PlayerId", lobbyManager.playerId }, { "_PlayerData", testPlayerData } });
        Debug.Log(respond);
    }
}

[System.Serializable]
public class ResultContainer
{
    public Result[] results;
    public Links links;
}

[System.Serializable]
public class Result
{
    public string key;
    public Value value;
    public string writeLock;
    public Modified modified;
    public Created created;
}
[System.Serializable]
public class Value
{
    public string PlayerName;
    public int Damage;
    public int Money;
    public float FireRate;
}
[System.Serializable]
public class Modified
{
    public string date;
}
[System.Serializable]
public class Created
{
    public string date;
}

[System.Serializable]
public class Links
{
    public string next;
}
