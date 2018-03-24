using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class VRButton : NetworkBehaviour
{
    public bool leftController, rightController = false;
    public Rigidbody prevConnected;
    public Material material;
    public Color color;

    public VRGrab controllerGrab = null;
    public enum Controller
    {
        left, right
    };
    public NetworkIdentity networkIdentity;

    public virtual void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        networkIdentity = GetComponent<NetworkIdentity>();
    }
    void OnTriggerEnter(Collider collision)
    {
        color = material.color;
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
            FeedbackColor(color);
        }
    }

    public virtual void FeedbackColor(Color color)
    {
        material.SetColor("g_vOutlineColor", color);
    }

    public virtual void Action(Controller side) { }

    public virtual void ActionUp(Controller side) { }

    public virtual void Action(Controller side, VRGrab controller) { controllerGrab = controller; }

    public virtual void ActionUp(Controller side, VRGrab controller) { controllerGrab = null; }

    public void OnDisable()
    {
        if (controllerGrab != null)
        {
            controllerGrab.grabbedObject.Remove(this);
        }
    }

}
