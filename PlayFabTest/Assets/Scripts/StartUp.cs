using System.Collections;
using System.Collections.Generic;
using PlayFabCustom;
using UnityEngine;

public class StartUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayFabAuthService.Instance.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
