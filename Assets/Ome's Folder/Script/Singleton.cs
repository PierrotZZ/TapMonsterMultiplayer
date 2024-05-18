using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Singleton<T> : NetworkBehaviour where T : Component
{
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject gameObject = new GameObject("Controller");
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }
}
