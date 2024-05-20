using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StatUpgradeManager : MonoBehaviour
{
    [SerializeField] PlayerTest player;
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI moneyValue;



    [Header("Damage")]
    [SerializeField] Button damageUpgradeButton;
    [SerializeField] TextMeshProUGUI damageValue;
    [SerializeField] TextMeshProUGUI damageLevelText;
    [SerializeField] TextMeshProUGUI damagePriceText;
    int damageLevel = 0;
    float damagePriceValue = 50;


    [Header("FireRate")]
    [SerializeField] Button fireRateUpgradeButton;
    float fireRatePriceValue = 50;
    int fireRateLevel = 0;
    [SerializeField] TextMeshProUGUI fireRateValue;
    [SerializeField] TextMeshProUGUI fireRatelevelText;
    [SerializeField] TextMeshProUGUI fireRatePriceText;

    // Start is called before the first frame update
    void Start()
    {
        // button.onClick.AddListener(GetplayerinLobbyServerRpc);
        // fireRatePriceText.text = fireRatePriceValue.ToString();
        // fireRatelevelText.text = fireRateLevel.ToString();
        // fireRateValue.text = player.playerData.fireRate.ToString();

        fireRateUpgradeButton.onClick.AddListener(UpgradeFireRate);
        damageUpgradeButton.onClick.AddListener(UpgradeDamage);
    }



    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        moneyValue.text = player.moneyPlayer.ToString();
    }

    void UpgradeFireRate()
    {
        
        int valueInt;
        if (player.moneyPlayer >= fireRatePriceValue && fireRateLevel < 20)
        {
            fireRateLevel++;
            player.playerFirerate -= 0.05f;

            if (fireRateLevel <= 10)
            {
                fireRatePriceValue = (fireRatePriceValue * 2) / 1.5f;
            }
            else if (fireRateLevel > 10)
            {
                fireRatePriceValue = (fireRatePriceValue * 2) / 1.7f;
            }

            valueInt = (int)fireRatePriceValue;

            player.moneyPlayer -= valueInt;

            fireRatePriceText.text = valueInt.ToString();
            fireRatelevelText.text = fireRateLevel.ToString();
            fireRateValue.text = player.playerFirerate.ToString();
        }

    }

    void UpgradeDamage()
    {
        int valueInt;
        if (player.moneyPlayer >= fireRatePriceValue && damageLevel < 20)
        {
            damageLevel++;
            player.playerDamage += 3;

            if (damageLevel <= 10)
            {
                damagePriceValue = (damagePriceValue * 2) / 1.5f;
            }
            else if (damageLevel > 10)
            {
                damagePriceValue = (damagePriceValue * 2) / 1.7f;
            }

            valueInt = (int)damagePriceValue;

            player.moneyPlayer -= valueInt;

            damagePriceText.text = valueInt.ToString();
            damageLevelText.text = damageLevel.ToString();
            damageValue.text = player.playerDamage.ToString();
        }

    }


    // public void GetplayerinLobbyServerRpc()
    // {
    //     playerTest = GameManager.Instance.players.Find(x => x.IsOwner);
    //     print("Asd");
    // }

    [ServerRpc]
    public IEnumerator GetPlayerInLobbyServerRpc()
    {
        yield return new WaitForSeconds(1);
        player = GameManager.Instance.players.Find(x => x.IsOwner);
    }
}
