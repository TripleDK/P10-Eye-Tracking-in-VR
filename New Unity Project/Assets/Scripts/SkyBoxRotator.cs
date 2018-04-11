using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxRotator : MonoBehaviour
{

    [SerializeField] Material skyBoxMat;
    [SerializeField] float speed = 0.1f;


    // Update is called once per frame
    void Update()
    {
        skyBoxMat.SetFloat("_Rotation", Time.time * speed);
    }

    void OnDisable()
    {
        skyBoxMat.SetFloat("_Rotation", 0);
    }
}
