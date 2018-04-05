using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveObjects : MonoBehaviour
{


    [SerializeField]
    Material[] material = new Material[8];
    [SerializeField]
    AnimationCurve DissolveAnimCurve;




    void DissolveObject()
    {
        //LerpFromTo();
    }

    void LerpFromTo(Material objectMaterial, float fromValue, float toValue)
    {
        //Mathf.Lerp (objectMaterial.SetFloat("_DissolveColor", fromValue), objectMaterial.SetFloat("_DissolveColor", toValue), DissolveAnimCurve.Evaluate(Time.deltaTime));
    }
}
