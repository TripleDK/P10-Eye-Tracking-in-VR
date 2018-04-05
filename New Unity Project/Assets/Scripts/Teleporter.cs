using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    [SerializeField] Transform standbyParticles;
    [SerializeField] GameObject teleportParticles;

    private ObjectInteractions objectToTeleport = null;
    private Rigidbody objectRigidbody = null;
    Vector3 particleStartScale;

    void Awake()
    {
        particleStartScale = standbyParticles.localScale;
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
    public void Activate()
    {
        Debug.Log("Trying to activate teleporter!");
        if (objectToTeleport != null)
        {
            if (objectToTeleport.gameObject.name == TaskContext.singleton.previewObjectName)
            {

                CmdActivate(objectToTeleport.gameObject);
                //  objectToTeleport.startPos = teleportTarget.position;


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
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        playerId.GetComponent<Player>().CmdSetAuth(TaskContext.singleton.netId, playerId);
        RpcActivate(gameObject);
        StartCoroutine(WaitForAuthor());
    }

    IEnumerator WaitForAuthor()
    {
        while (!TaskContext.singleton.hasAuthority)
        {
            yield return null;
        }
        TaskContext.singleton.CmdNextObject();
    }


    [ClientRpc]
    public void RpcActivate(GameObject go)
    {
        Debug.Log("RPC ACTIVAtea!");
        AudioSource.PlayClipAtPoint(beep, transform.position);
        Rigidbody teleportRigid = go.gameObject.GetComponent<Rigidbody>();
        teleportRigid.velocity = Vector3.zero;
        StartCoroutine(TeleportEffect(go));
    }

    IEnumerator TeleportEffect(GameObject go)
    {
        Rigidbody teleportRigid = go.gameObject.GetComponent<Rigidbody>();
        Material mat = go.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;

        float startTime = Time.time;
        while (Time.time - startTime < teleportTime)
        {
            float dissolveValue = dissolveEffect.Evaluate((Time.time - startTime) / teleportTime);
            float scaleValue = particleMovement.Evaluate((Time.time - startTime) / teleportTime);
            mat.SetFloat("_DissolveSize", dissolveValue);
            standbyParticles.localScale = particleStartScale * scaleValue;
            yield return null;
        }
        mat.SetFloat("_DissolveSize", 0);
        Destroy(Instantiate(teleportParticles, objectToTeleport.transform.position, Quaternion.identity), 2);
        objectToTeleport = null;
        teleportRigid.MovePosition(teleportTarget.position);
        go.GetComponent<ObjectInteractions>().startPos = teleportTarget.position;

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
