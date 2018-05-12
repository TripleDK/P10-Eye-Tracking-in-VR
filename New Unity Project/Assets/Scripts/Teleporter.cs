using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class Teleporter : NetworkBehaviour
{

    [SerializeField] Transform teleportTarget;
    [SerializeField] float repulsionForce = 2f;
    [SerializeField] AudioClip beep;
    [SerializeField] AudioClip errorBeep;
    [SerializeField] Transform floatTarget;
    [SerializeField] float floatForce = 50;
    [SerializeField] float floatDrag = 10;
    [SerializeField] float teleportTime = 1.0f;
    [SerializeField] AnimationCurve dissolveEffect;
    [SerializeField] AnimationCurve particleMovement;
    [SerializeField] AnimationCurve outlineMovement;
    [SerializeField] Transform standbyParticles;
    [SerializeField] GameObject teleportParticles;
    [SerializeField] Transform receiverParticles;
    [SerializeField] AnimationCurve receiverPMovement;
    [SerializeField] AudioClip teleportSound;

    private ObjectInteractions objectToTeleport = null;
    private Rigidbody objectRigidbody = null;
    Vector3 particleStartScale, receiverStartScale;
    bool teleporting = false;

    public UnityEvent OnAcceptItem = new UnityEvent();
    public UnityEvent OnTeleportItem = new UnityEvent();

    void Awake()
    {
        particleStartScale = standbyParticles.localScale;
        receiverStartScale = receiverParticles.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ObjectInteractions>())
        {
            //     Debug.Log("Trigger hit by " + other.gameObject.name);
            if (objectToTeleport != null)
            {
                if (other.gameObject == objectToTeleport.gameObject)
                {
                    Debug.Log("Wat");
                }
                Debug.Log("Already have " + objectToTeleport.gameObject.name + ", so repulsing " + other.gameObject.name);
                other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), 1, Random.Range(-1, 1)) * repulsionForce, ForceMode.Impulse);
                other.gameObject.GetComponent<ObjectInteractions>().ResetPosition(5);
                return;

            }
            OnAcceptItem.Invoke();
            objectToTeleport = other.gameObject.GetComponent<ObjectInteractions>();
            objectRigidbody = other.gameObject.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (objectToTeleport != null)
        {
            if (other.gameObject == objectToTeleport.gameObject)
            {
                objectRigidbody.AddForce((floatTarget.position - objectRigidbody.transform.position) * floatForce, ForceMode.Force);
                objectRigidbody.AddForce(-objectRigidbody.velocity * floatDrag, ForceMode.Force);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //        Debug.Log("Trigger exit by " + other.gameObject.name);
        if (other.gameObject.GetComponent<ObjectInteractions>() == objectToTeleport)
        {
            objectToTeleport = null;
        }
    }
    public void Activate() //Button grants authority before calling this
    {
        Debug.Log("Trying to activate teleporter!");
        if (objectToTeleport != null && teleporting == false)
        {
            teleporting = true;
            if (objectToTeleport.gameObject == TaskContext.singleton.objectToFind)
            {
                CmdActivate(objectToTeleport.gameObject);
                //  objectToTeleport.startPos = teleportTarget.position;
            }
            else if (objectToTeleport.gameObject.name == "Sphere(Clone)")
            {
                AudioSource.PlayClipAtPoint(beep, transform.position);
                Rigidbody teleportRigid = objectToTeleport.gameObject.GetComponent<Rigidbody>();
                teleportRigid.velocity = Vector3.zero;
                Destroy(objectToTeleport.gameObject, 3);
                StartCoroutine(TeleportEffect(objectToTeleport.gameObject));
            }
            else
            {
                CmdErrorBeep();
                objectToTeleport.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), 1, Random.Range(-1, 1)) * repulsionForce, ForceMode.Impulse);
            }
        }
    }

    [Command]
    public void CmdActivate(GameObject gameObject)
    {
        //    NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        //  playerId.GetComponent<Player>().CmdSetAuth(TaskContext.singleton.netId, playerId);
        RpcActivate(gameObject);
    }


    [ClientRpc]
    public void RpcActivate(GameObject go)
    {
        //     Debug.Log("RPC ACTIVAtea!");
        AudioSource.PlayClipAtPoint(beep, transform.position);
        teleporting = true;
        Rigidbody teleportRigid = go.GetComponent<Rigidbody>();
        teleportRigid.velocity = Vector3.zero;
        StartCoroutine(TeleportEffect(go));
    }

    IEnumerator TeleportEffect(GameObject go)
    {
        Rigidbody teleportRigid = go.GetComponent<Rigidbody>();
        Material mat;
        if (go.GetComponent<MeshRenderer>().enabled)
        {
            mat = go.GetComponent<MeshRenderer>().material;
        }
        else
        {
            mat = go.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
        }
        float outlineSize = mat.GetFloat("_OutlineWidth");
        AudioSource.PlayClipAtPoint(teleportSound, transform.position);

        float startTime = Time.time;
        while (Time.time - startTime < teleportTime)
        {
            float animCurveTime = (Time.time - startTime) / teleportTime;
            float dissolveValue = dissolveEffect.Evaluate(animCurveTime);
            float scaleValue = particleMovement.Evaluate(animCurveTime);
            float outlineValue = outlineMovement.Evaluate(animCurveTime) * outlineSize;
            mat.SetFloat("_DissolveSize", dissolveValue);
            mat.SetFloat("_OutlineWidth", outlineValue);
            standbyParticles.localScale = particleStartScale * scaleValue;
            yield return null;
        }
        mat.SetFloat("_DissolveSize", 0);
        mat.SetFloat("_OutlineWidth", outlineSize);
        Destroy(Instantiate(teleportParticles, objectToTeleport.transform.position, Quaternion.identity), 2);
        objectToTeleport = null;
        if (go.GetComponent<NetworkIdentity>().hasAuthority)
        {
            teleportRigid.MovePosition(teleportTarget.position);
        }
        go.GetComponent<ObjectInteractions>().startPos = teleportTarget.position;

        startTime = Time.time;
        OnTeleportItem.Invoke();
        while (Time.time - startTime < teleportTime)
        {
            float scaleValue = particleMovement.Evaluate((Time.time - startTime) / teleportTime);
            receiverParticles.localScale = receiverStartScale * scaleValue;
            yield return null;
        }
        teleporting = false;

    }

    [Command]
    public void CmdErrorBeep()
    {
        RpcErrorBeep();
    }

    [ClientRpc]
    public void RpcErrorBeep()
    {
        AudioSource.PlayClipAtPoint(errorBeep, transform.position);
    }


}
