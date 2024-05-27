using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactFollowPlayer : MonoBehaviour
{
    Vector3 pos;
    [SerializeField] ParticleSystem impact;

    void Start()
    {
        impact = GetComponent<ParticleSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            pos = Input.mousePosition;
            transform.position = Camera.main.ScreenToWorldPoint(pos);
            impact.Play();
        }
    }
}
