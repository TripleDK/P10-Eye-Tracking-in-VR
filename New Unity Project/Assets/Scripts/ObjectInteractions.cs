using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class ObjectInteractions : VRButton
{

    public bool attached = false;
    public Vector3 startPos;
    [HideInInspector] public NetworkIdentity playerId = null;
    [SerializeField] float minHeight = 0.3f;
    [SerializeField] GameObject teleportParticles;
    FixedJoint tempJoint = null;
    Vector3 velocity;
    Quaternion angVelocity;
    Vector3 prevPosition;
    Quaternion prevAng;
    Rigidbody rigid;

    public UnityEvent OnBallGrabbed = new UnityEvent();

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
            OnBallGrabbed.Invoke();
            attached = true;
            //  transform.position = controller.transform.position;
            tempJoint = gameObject.AddComponent<FixedJoint>();
            tempJoint.connectedBody = controller.GetComponent<Rigidbody>();
            rigid.velocity = Vector3.zero;
            networkIdentity.localPlayerAuthority = true;
            playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
            playerId.GetComponent<Player>().CmdSetAuth(netId, playerId);
            CmdAttach(playerId);
        }
    }

    [Command]
    void CmdAttach(NetworkIdentity playerId)
    {
        if (gameObject.name != TaskContext.singleton.previewObjectName)
        {
            TaskContext.singleton.errorGrabs++;
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
        CmdDetach();
    }

    [Command]
    void CmdDetach()
    {
        // networkIdentity.localPlayerAuthority = false;

    }

    public override void FeedbackColor(Color color)
    {
        material.SetColor("_OutlineColor", color);
    }

    public override void OnTriggerEnter(Collider collision)
    {
        //  color = material.color;
        base.OnTriggerEnter(collision);
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
        if (transform.position.y < minHeight)
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
        Vector3 ogPos = transform.position;
        CmdResetPosition(ogPos);
    }

    [Command]
    public void CmdResetPosition(Vector3 ogPos)
    {
        RpcResetPosition(ogPos);
    }

    [ClientRpc]
    public void RpcResetPosition(Vector3 ogPos)
    {
        Destroy(Instantiate(teleportParticles, ogPos, Quaternion.identity), 2);
        transform.position = startPos;
        rigid.velocity = Vector3.zero;
    }
}
