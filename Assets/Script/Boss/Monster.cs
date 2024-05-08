using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    BossStat bossStat = new BossStat();

    int rndHp;
    public Slider sliderHp;

    void OnEnable()
    {
        int rndHp = UnityEngine.Random.Range(30, 80);

        bossStat.Health = rndHp;
        sliderHp.maxValue = rndHp;
        sliderHp.value = rndHp;
        Debug.Log(bossStat.Health);
    }

    void Update()
    {
        
    }

    public void takeDamage()
    {
        bossStat.Health -= 5;
        sliderHp.value = bossStat.Health;
    }
}
