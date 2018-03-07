using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class VRButton : MonoBehaviour
{
    public bool leftController, rightController = false;
    public Rigidbody prevConnected;
    public Material material;

    public VRGrab controllerGrab = null;
    public enum Controller
    {
        left, right
    };


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
