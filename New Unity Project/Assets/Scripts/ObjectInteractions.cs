using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectInteractions : VRButton
{

    bool attached = false;
    FixedJoint tempJoint = null;
    Vector3 velocity;
    Quaternion angVelocity;
    Vector3 prevPosition;
    Quaternion prevAng;
    Rigidbody rigid;

    public override void Awake()
    {
        base.Awake();
        rigid = GetComponent<Rigidbody>();
    }

    public override void Action()
    {
        attached = !attached;
        if (attached)
        {

            if (rightController)
            {
            }
            else if (leftController)
            {
            }
            else
            {
                Debug.Log("Wat");
            }
            tempJoint = gameObject.AddComponent<FixedJoint>();
            tempJoint.connectedBody = prevConnected;
            StartCoroutine("TrackVelocity");
        }
        else
        {
        }
    }

    public override void FeedbackColor(Color color)
    {
        material.color = color;
    }


}
