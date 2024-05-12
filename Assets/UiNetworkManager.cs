using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UiNetworkManager : MonoBehaviour
{

    [SerializeField] Button HostBotton;
    [SerializeField] Button ClientBotton;
    // Start is called before the first frame update
    void Start()
    {
        HostBotton.onClick.AddListener(OnClickHostBotton);
        ClientBotton.onClick.AddListener(OnClickClientBotton);

    }

    private void OnClickClientBotton()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void OnClickHostBotton()
    {
        NetworkManager.Singleton.StartHost();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        HostBotton.onClick.RemoveListener(OnClickHostBotton);
        ClientBotton.onClick.RemoveListener(OnClickClientBotton);

    }
}
