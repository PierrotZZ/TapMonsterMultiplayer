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
    int damageLevel = 1;
    float damagePriceValue = 50;
    int valueIntDmg;

    [Header("FireRate")]
    [SerializeField] Button fireRateUpgradeButton;
    float fireRatePriceValue = 50;
    int fireRateLevel = 1;
    int valueIntFireRate;

    [Header("Ui interface")]
    [SerializeField] TextMeshProUGUI fireRateValue;
    [SerializeField] TextMeshProUGUI fireRatelevelText;
    [SerializeField] TextMeshProUGUI fireRatePriceText;
    public Slider sliderCoolDownAtk;

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
        if (player == null)
        {
            StartCoroutine(GetPlayerInLobbyServerRpc());
        }
        else
        {
            moneyValue.text = player.playerData.Money.ToString();
            UpdateSliderAttack();

        }



    }

    void UpgradeFireRate()
    {
        if (player.playerData.Money >= fireRatePriceValue && fireRateLevel < 20)
        {
            fireRateLevel++;
            player.playerData.FireRateLevel++;
            player.playerData.fireRate -= 0.05f;

            if (fireRateLevel <= 10)
            {
                fireRatePriceValue = (fireRatePriceValue * 2) / 1.5f;
            }
            else if (fireRateLevel > 10)
            {
                fireRatePriceValue = (fireRatePriceValue * 2) / 1.7f;
            }

            valueIntFireRate = (int)fireRatePriceValue;

            player.playerData.Money -= valueIntFireRate;

            UpdateStat();
        }

    }

    void UpgradeDamage()
    {
        if (player.playerData.Money >= damagePriceValue && damageLevel < 20)
        {
            damageLevel++;
            player.playerData.DamageLevel++;
            player.playerData.Damage += 3;

            if (damageLevel <= 10)
            {
                damagePriceValue = (damagePriceValue * 2) / 1.5f;
            }
            else if (damageLevel > 10)
            {
                damagePriceValue = (damagePriceValue * 2) / 1.7f;
            }

            valueIntDmg = (int)damagePriceValue;

            player.playerData.Money -= valueIntDmg;

            UpdateStat();
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
        //Get stat player
        if (player != null)
        {
            valueIntDmg = player.playerData.DamagePrice;
            damageLevel = player.playerData.DamageLevel;
            valueIntFireRate = player.playerData.FireRatePrice;
            fireRateLevel = player.playerData.FireRateLevel;
            UpdateStat();
        }
    }

    public void UpdateSliderAttack()
    {
        sliderCoolDownAtk.value = player.time;
        sliderCoolDownAtk.maxValue = player.playerData.fireRate;
    }

    public void UpdateStat()
    {
        //Fire rate interface
        fireRatePriceText.text = "Price : " + valueIntFireRate.ToString();
        fireRatelevelText.text = "Lv. : " + fireRateLevel.ToString();
        fireRateValue.text = player.playerData.fireRate.ToString();

        //Damage interface
        damagePriceText.text = "Price : " + valueIntDmg.ToString();
        damageLevelText.text = "Lv. : " + damageLevel.ToString();
        damageValue.text = player.playerData.Damage.ToString();
    }
}
