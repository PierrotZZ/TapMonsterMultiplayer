using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;
using Unity.Services.Friends.Model;
using static HelloWorld.MyModule;

namespace HelloWorld;

public class MyModule
{
    public class ModoleConfig : ICloudCodeSetup
    {
        public void Setup(ICloudCodeConfig config)
        {
            config.Dependencies.AddSingleton(GameApiClient.Create());
        }
    }
    [CloudCodeFunction("SavePlayerData")]
    public async Task<string> SavePlayerData(IExecutionContext ctx, IGameApiClient gameApiClient, string PlayerId,PlayerData _PlayerData)
    {
        //PlayerData a = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerData>(_PlayerData);

        PlayerData savePlayerData = new PlayerData();
        savePlayerData.PlayerName = _PlayerData.PlayerName;
        savePlayerData.Damage = _PlayerData.Damage;
        savePlayerData.Money = _PlayerData.Money;
        savePlayerData.FireRate = _PlayerData.FireRate;
        var respond = await gameApiClient.CloudSaveData.SetItemAsync(ctx, ctx.AccessToken
            , "518daf08-ebfa-43b7-ba32-7d0be30f82fa", PlayerId, new SetItemBody("PlayerData",savePlayerData));
        string jsonDefaultData = JsonConvert.SerializeObject(savePlayerData);
        //return jsonDefaultData;
        return jsonDefaultData;
    }
    [CloudCodeFunction("LoadPlayerData")]
    public async Task<string> LoadPlayerData(IExecutionContext ctx, IGameApiClient gameApiClient, string PlayerId)
    {
        var respond = await gameApiClient.CloudSaveData.GetItemsAsync(ctx, ctx.AccessToken,
            "518daf08-ebfa-43b7-ba32-7d0be30f82fa", PlayerId, new List<string> { "PlayerData" });
        if (respond.Data.Results.Count == 0)
        {
            var defaultRespond = await GeneateDeafaultPlayerData(ctx, ctx.AccessToken, gameApiClient, PlayerId);
            return defaultRespond.ToString();
        }
        else
        {
            string palyerDataJson = respond.Data.ToJson();
            //string palyerDataJson = respond.Data.ToJson();
            return palyerDataJson;
        }
    }

    async Task<string> GeneateDeafaultPlayerData(IExecutionContext ctx, string accessToken,
        IGameApiClient gameApiClient, string PlayerId)
    {
        PlayerData newPlayerData = new PlayerData();
        newPlayerData.PlayerName = "";
        UsersPassword usersAndPassword = new UsersPassword();
        usersAndPassword.Users = "users1";
        usersAndPassword.Password = "users2";
        UsersData usersData = new UsersData();
        usersData._UsersData.Add(usersAndPassword, PlayerId);
        
        var respond = gameApiClient.CloudSaveData.SetItemAsync(ctx, ctx.AccessToken,
            "518daf08-ebfa-43b7-ba32-7d0be30f82fa", PlayerId, new SetItemBody("PlayerData", newPlayerData));
        string jsonDefaultData = JsonConvert.SerializeObject(newPlayerData);
        return jsonDefaultData;
    }
    [CloudCodeFunction("Login")]
    public async Task Login(IExecutionContext ctx, IGameApiClient gameApiClient, string UsersPassword)
    {
          
    }
    [CloudCodeFunction("CreateAccount")]
    public async Task<string> CreateAccount(IExecutionContext ctx, IGameApiClient gameApiClient,string PlayerId, UsersPassword UsersPassword)
    {
        /*var respond = await gameApiClient.CloudSaveData.GetItemsAsync(ctx, ctx.AccessToken,
            "518daf08-ebfa-43b7-ba32-7d0be30f82fa", "R4DThHjctqOJWw08YQE2KJQmwZw6", new List<string> { "UsersData" });*/
        UsersPassword usersData1 = new UsersPassword();
        usersData1.Users = "Admin1";
        usersData1.Password = "Admin1";
        UsersPassword usersData2 = new UsersPassword();
        usersData2.Users = "Admin2";
        usersData2.Password = "Admin2";
        UsersData _usersData = new UsersData();
        _usersData._UsersData = new Dictionary<UsersPassword, string>();
        _usersData._UsersData.Add(usersData1,"Admin1");
        _usersData._UsersData.Add(usersData2,"Admin2");

        var respond = await gameApiClient.CloudSaveData.SetItemAsync(ctx, ctx.AccessToken
            , "518daf08-ebfa-43b7-ba32-7d0be30f82fa", "R4DThHjctqOJWw08YQE2KJQmwZw6", new SetItemBody("UsersData", _usersData));
        // UsersData usersData = 

        return respond.Data.ToJson();
    }
    public class PlayerData
    {
        public string PlayerName;
        public int Level;
        public int Damage;
        public int Money;
        public float FireRate;
    }   
    public class UsersData
    {
        public Dictionary<UsersPassword, string> _UsersData;
    }
    public class UsersPassword
    {
        public string Users;
        public string Password;
    }

}


