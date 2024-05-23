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
        PlayerData savePlayerData = new PlayerData();
        savePlayerData.PlayerName = _PlayerData.PlayerName;
        savePlayerData.Damage = _PlayerData.Damage;
        savePlayerData.DamageLevel = _PlayerData.DamageLevel;
        savePlayerData.DamagePrice = _PlayerData.DamagePrice;
        savePlayerData.Money = _PlayerData.Money;
        savePlayerData.fireRate = _PlayerData.fireRate;
        savePlayerData.FireRatePrice = _PlayerData.FireRatePrice;
        savePlayerData.FireRateLevel = _PlayerData.FireRateLevel;
        var respond = await gameApiClient.CloudSaveData.SetItemAsync(ctx, ctx.AccessToken
            , "518daf08-ebfa-43b7-ba32-7d0be30f82fa", PlayerId, new SetItemBody("PlayerData", _PlayerData));
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
            return palyerDataJson;
        }
    }

    async Task<string> GeneateDeafaultPlayerData(IExecutionContext ctx, string accessToken,
        IGameApiClient gameApiClient, string PlayerId)
    {
        PlayerData newPlayerData = new PlayerData();
        newPlayerData.PlayerName = "";
        newPlayerData.Damage = 3;
        newPlayerData.DamageLevel  = 1;
        newPlayerData.DamagePrice = 50;
        newPlayerData.Money  = 0;
        newPlayerData.fireRate = 1;
        newPlayerData.FireRatePrice = 50;
        newPlayerData.FireRateLevel = 1;

    var respond = gameApiClient.CloudSaveData.SetItemAsync(ctx, ctx.AccessToken,
            "518daf08-ebfa-43b7-ba32-7d0be30f82fa", PlayerId, new SetItemBody("PlayerData", newPlayerData));
        string jsonDefaultData = JsonConvert.SerializeObject(newPlayerData);
        return jsonDefaultData;
    }
    public class PlayerData
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


