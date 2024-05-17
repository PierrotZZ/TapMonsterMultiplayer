using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;

public class CloudSaveManager : MonoBehaviour
{
    [HideInInspector] public LobbyManager lobbyManager;
    public string username;
    public string password;


    void Update()
    {

    }
    public async Task<string> SignIn(string _users, string _password, string name, bool newUsers)
    {
        if (newUsers)
        {
            Debug.Log("New Users");
            Debug.Log(_users + " : " + _password + " : " + name);
        }
        else
        {
            Debug.Log("Login");
        }
        var respond = await CloudCodeService.Instance.CallModuleEndpointAsync("SaveModule"
        , "LoadPlayerData"
        , new Dictionary<string, object> { { "PlayerId", lobbyManager.playerId } });
        Debug.Log(respond);
        // ResultContainer resultContainer = JsonUtility.FromJson<ResultContainer>(respond);
        // Debug.Log(resultContainer.results[0].value.Class);
        return respond;
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
    public string Class;
    public int Level;
    public string PlayerName;
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
