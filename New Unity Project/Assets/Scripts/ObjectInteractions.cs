﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectInteractions : VRButton
{

    public bool attached = false;
    FixedJoint tempJoint = null;
    Vector3 velocity;
    Quaternion angVelocity;
    Vector3 prevPosition;
    Quaternion prevAng;
    Rigidbody rigid;
    Vector3 startPos;

    public override void Awake()
    {
        base.Awake();
        rigid = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    public override void Action(Controller side, VRGrab controller)
    {
        if (!attached)
        {
            attached = true;
            transform.position = controller.transform.position;
            tempJoint = gameObject.AddComponent<FixedJoint>();
            tempJoint.connectedBody = controller.GetComponent<Rigidbody>();
            rigid.velocity = Vector3.zero;
        }
    }

    public override void ActionUp(Controller side)
    {
        if (attached)
        {
            attached = false;
            SteamVR_TrackedObject trackedObj = tempJoint.connectedBody.GetComponent<SteamVR_TrackedObject>();
            var device = SteamVR_Controller.Input((int)trackedObj.index);
            Destroy(tempJoint);
            var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
            if (origin != null)
            {
                rigid.velocity = origin.TransformVector(device.velocity);
                rigid.angularVelocity = origin.TransformVector(device.angularVelocity);
            }
            else
            {
                rigid.velocity = device.velocity;
                rigid.angularVelocity = device.angularVelocity;
            }

            rigid.maxAngularVelocity = rigid.angularVelocity.magnitude;
        }
    }
    public override void FeedbackColor(Color color)
    {
        material.color = color;
    }

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(CheckForFall());
    }

    IEnumerator CheckForFall()
    {
        while (rigid.velocity != Vector3.zero)
        {
            yield return null;
        }
        if (transform.position.y != startPos.y)
        {
            ResetPosition(0);
        }
    }

    public void ResetPosition(int delay)
    {
        StartCoroutine(ResetPositionCo(delay));
    }
    IEnumerator ResetPositionCo(int delay)
    {
        yield return new WaitForSeconds(delay);
        transform.position = startPos;
        rigid.velocity = Vector3.zero;
    }
}
