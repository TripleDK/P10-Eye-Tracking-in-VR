using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class VRButton : MonoBehaviour
{
    Material material;
    bool leftController, rightController = false;

    void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }
    void OnTriggerEnter(Collider collision)
    {
        material.SetColor("g_vOutlineColor", Color.green);
        if (collision.gameObject.name == "Controller (left)")
        {
            leftController = true;
        }
        else if (collision.gameObject.name == "Controller (right)")
        {
            rightController = true;
        }
        StartCoroutine("WaitingForInput");
    }
    void OnTriggerExit(Collider collision)
    {

        if (collision.gameObject.name == "Controller (left)")
        {
            leftController = false;
        }
        else if (collision.gameObject.name == "Controller (right)")
        {
            rightController = false;
        }
        if (!leftController && !rightController)
        {
            material.SetColor("g_vOutlineColor", Color.yellow);
        }
        StopCoroutine("WaitingForInput");
    }

    IEnumerator WaitingForInput()
    {
        while (true)
        {
            if (leftController && Input.GetKeyDown("joystick button 14"))
            {
                Action();
            }
            if (rightController && Input.GetKeyDown("joystick button 15"))
            {
                Action();
            }
            yield return null;

        }
    }

    public virtual void Action() { }
}
