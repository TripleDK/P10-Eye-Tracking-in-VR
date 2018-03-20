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

    private ObjectInteractions objectToTeleport = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ObjectInteractions>())
        {
            Debug.Log("Trigger hit by " + other.gameObject.name);
            if (objectToTeleport != null)
            {
                if (other.gameObject == objectToTeleport.gameObject)
                {
                    Debug.Log("Wat");
                }
                Debug.Log("Already have " + objectToTeleport.gameObject.name + ", so repulsing " + other.gameObject.name);
                other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * repulsionForce, ForceMode.Impulse);
                other.gameObject.GetComponent<ObjectInteractions>().ResetPosition(5);
                return;

            }
            objectToTeleport = other.gameObject.GetComponent<ObjectInteractions>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exit by " + other.gameObject.name);
        if (other.gameObject.GetComponent<ObjectInteractions>() == objectToTeleport)
        {
            objectToTeleport = null;
        }
    }
    [Command]
    public void CmdActivate()
    {
        if (objectToTeleport != null)
        {
            if (objectToTeleport.gameObject.name == TaskContext.singleton.previewObject.name)
            {
                Rigidbody teleportRigid = objectToTeleport.gameObject.GetComponent<Rigidbody>();
                AudioSource.PlayClipAtPoint(beep, transform.position);
                teleportRigid.velocity = Vector3.zero;
                teleportRigid.MovePosition(teleportTarget.position);
                //  objectToTeleport.startPos = teleportTarget.position;
                objectToTeleport = null;
                TaskContext.singleton.NextObject();
            }
            else
            {
                AudioSource.PlayClipAtPoint(errorBeep, transform.position);
                objectToTeleport.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * repulsionForce, ForceMode.Impulse);
            }
        }
    }


}
