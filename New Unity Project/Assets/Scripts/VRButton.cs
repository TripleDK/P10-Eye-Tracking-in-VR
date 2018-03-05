using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class VRButton : MonoBehaviour
{
    public bool leftController, rightController = false;
    public Rigidbody prevConnected;
    public Material material;

    public virtual void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }
    void OnTriggerEnter(Collider collision)
    {
        FeedbackColor(Color.green);
        if (collision.gameObject.name == "Controller (left)")
        {
            leftController = true;
        }
        else if (collision.gameObject.name == "Controller (right)")
        {
            rightController = true;
        }
        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            prevConnected = collision.gameObject.GetComponent<Rigidbody>();
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
            FeedbackColor(Color.yellow);
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
            if (leftController && Input.GetKeyUp("joystick button 14"))
            {
                ActionUp();
            }
            if (rightController && Input.GetKeyUp("joystick button 15"))
            {
                ActionUp();
            }
            yield return null;

        }
    }

    public virtual void FeedbackColor(Color color)
    {
        material.SetColor("g_vOutlineColor", color);
    }

    public virtual void Action() { }

    public virtual void ActionUp() { }
}
