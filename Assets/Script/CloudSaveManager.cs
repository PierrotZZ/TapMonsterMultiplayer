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

public class CloudSaveManager : Singleton<CloudSaveManager>
{
    [HideInInspector] public LobbyManager lobbyManager;
    public string _lobbyId;
    public string _playerId;
    internal PlayerData _playerData = new PlayerData();

    [Header("Get Login 1 time")]
    public string username;
    public bool statusLoginCheck;
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Return))
        // {
        //     PlayerData test = new PlayerData();
        //     test.Damage = 20;
        //     var _Save = Save(test);
        // }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     var _Load = Load();
        //     // Debug.Log(_playerData.Damage);
        // }
    }

    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            lobbyManager.OpenIntroLobby();
            Debug.Log("SignUp is successful.");
            Debug.Log("User login : " + AuthenticationService.Instance.PlayerId);
            _playerId = AuthenticationService.Instance.PlayerId;
            var _Load = Load();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            // Debug.LogException(ex);
            // Debug.Log("Username Has Already");
            lobbyManager.error.text = "Username Has Already";
            lobbyManager.usersName.text = "";
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            // Debug.LogException(ex);
            // Debug.Log("Insert at least 1 uppercase, 1 lowercase, 1 digit and 1 symbol. With minimum 8 characters and a maximum of 30");
            lobbyManager.error.text = "Insert at least 1 uppercase, 1 lowercase, 1 digit and 1 symbol. With minimum 8 characters and a maximum of 30";
            lobbyManager.password.text = "";
        }
    }
    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            lobbyManager.OpenIntroLobby();
            Debug.Log("User login : " + AuthenticationService.Instance.PlayerId);
            _playerId = AuthenticationService.Instance.PlayerId;
            var _Load = Load();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            // Debug.LogException(ex);
            lobbyManager.usersName.text = "";
            lobbyManager.password.text = "";
            lobbyManager.error.text = "Invalid username or password";
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            // Debug.LogException(ex);
            lobbyManager.error.text = "Invalid username or password";
            lobbyManager.usersName.text = "";
            lobbyManager.password.text = "";
        }
    }
    public async Task SignUp(string _users, string _password, string name, bool newUsers)
    {
        if (newUsers)
        {
            Debug.Log("New Users");
            Debug.Log(_users + " : " + _password + " : " + name);
            var SignUp = SignUpWithUsernamePasswordAsync(_users, _password);
        }
        else
        {
            var SignIn = SignInWithUsernamePasswordAsync(_users, _password);
        }
    }
    public async Task<PlayerData> Load()
    {
        PlayerData loadPlayerdata = new PlayerData();
        try
        {
            var respond = await CloudCodeService.Instance.CallModuleEndpointAsync("SaveModule"
                    , "LoadPlayerData"
                    , new Dictionary<string, object> { { "PlayerId", _playerId } });
            // Debug.Log(respond + " : respond");
            ResultContainer resultContainer = JsonUtility.FromJson<ResultContainer>(respond);
            // Debug.Log(resultContainer.results[0].value.Damage);
            _playerData.PlayerName = resultContainer.results[0].value.PlayerName;
            _playerData.Damage = resultContainer.results[0].value.Damage;
            _playerData.DamageLevel = resultContainer.results[0].value.DamageLevel;
            _playerData.DamagePrice = resultContainer.results[0].value.DamagePrice;
            _playerData.Money = resultContainer.results[0].value.Money;
            _playerData.fireRate = resultContainer.results[0].value.fireRate;
            _playerData.FireRatePrice = resultContainer.results[0].value.FireRatePrice;
            _playerData.FireRateLevel = resultContainer.results[0].value.FireRateLevel;
        }
        catch (AuthenticationException ex)
        {
            Debug.Log(ex);
        }
        return loadPlayerdata;

    }
    //Save
    public async Task Save(PlayerData savePlayerData)
    {
        Debug.Log("SaveData");
        var respond = await CloudCodeService.Instance.CallModuleEndpointAsync("SaveModule"
        , "SavePlayerData"
        , new Dictionary<string, object> { { "PlayerId", _playerId }, { "_PlayerData", savePlayerData } });
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
    public int DamageLevel;
    public int DamagePrice;
    public int Money;
    public float fireRate;
    public int FireRatePrice;
    public int FireRateLevel;
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
