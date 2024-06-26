using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string PlayerName;
    public int Damage = 3;
    public int DamageLevel = 1;
    public int DamagePrice = 50;
    public int Money = 0;
    public float fireRate = 1;
    public int FireRatePrice = 50;
    public int FireRateLevel = 1;

}
