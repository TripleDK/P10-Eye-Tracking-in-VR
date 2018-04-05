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
    public VRGrab[] controllerGrab;

    public enum Controller
    {
        left, right
    };
    public NetworkIdentity networkIdentity;

    public virtual void Awake()
    {
        if (GetComponent<MeshRenderer>().enabled)
        {
            material = GetComponent<MeshRenderer>().material;
        }
        else
        {
            material = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
        }
        networkIdentity = GetComponent<NetworkIdentity>();
        controllerGrab = new VRGrab[] { null, null };
    }
    public virtual void OnTriggerEnter(Collider collision)
    {
        VRGrab controller = collision.gameObject.GetComponent<VRGrab>();
        FeedbackColor(Color.green);
        if (collision.gameObject.name == "Controller (left)")
        {
            leftController = true;
            controllerGrab[0] = controller;
        }
        else if (collision.gameObject.name == "Controller (right)")
        {
            rightController = true;
            controllerGrab[1] = controller;
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
            controllerGrab[0].grabbedObject.Remove(this);
        }
        else if (collision.gameObject.name == "Controller (right)")
        {
            rightController = false;
            controllerGrab[1].grabbedObject.Remove(this);
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

    public virtual void Action(Controller side, VRGrab controller)
    {

    }

    public virtual void ActionUp(Controller side, VRGrab controller) { controllerGrab = new VRGrab[] { null, null }; }

    public void OnDisable()
    {
        if (controllerGrab[0] != null)
        {
            controllerGrab[0].grabbedObject.Remove(this);
        }
        if (controllerGrab[1] != null)
        {
            controllerGrab[1].grabbedObject.Remove(this);
        }
    }

}
