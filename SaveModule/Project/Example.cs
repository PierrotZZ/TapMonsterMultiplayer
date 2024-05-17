using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

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
    public async Task<string> SavePlayerData(IExecutionContext ctx, IGameApiClient gameApiClient, string PlayerId)
    {
        var respond = await gameApiClient.CloudSaveData.SetItemAsync(ctx, ctx.AccessToken
            , "518daf08-ebfa-43b7-ba32-7d0be30f82fa", PlayerId, new SetItemBody(/* ใส่ที่หลัง */));
        return respond.ToString();
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
        IGameApiClient gameApiClient, string playerId)
    {
        PlayerData newPlayerData = new PlayerData();
        newPlayerData.PlayerName = "";
        newPlayerData.Class = "Novice";
        newPlayerData.Lavel = 1;
        var respond = gameApiClient.CloudSaveData.SetItemAsync(ctx, ctx.AccessToken,
            "518daf08-ebfa-43b7-ba32-7d0be30f82fa", playerId, new SetItemBody("PlayerData", newPlayerData));
        string jsonDefaultData = JsonConvert.SerializeObject(newPlayerData);
        return jsonDefaultData;

    }
    public class PlayerData
    {
        public string PlayerName;
        public string Class;
        public int Lavel;
        /*
        Damage
        Fire Rate
        Money
         * */
    }
}


