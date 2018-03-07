using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{

    [SerializeField] Transform teleportTarget;
    [SerializeField] float repulsionForce = 2f;

    private ObjectInteractions objectToTeleport = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ObjectInteractions>())
        {
            if (other.gameObject.GetComponent<ObjectInteractions>().attached == false)
            {
                if (objectToTeleport != null)
                {
                    other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * repulsionForce, ForceMode.Impulse);
                    other.gameObject.GetComponent<ObjectInteractions>().ResetPosition(5);
                    return;

                }
                objectToTeleport = other.gameObject.GetComponent<ObjectInteractions>();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<ObjectInteractions>() == objectToTeleport)
        {
            objectToTeleport = null;
        }
    }

    public void Activate()
    {
        if (objectToTeleport != null)
        {
            if (objectToTeleport.gameObject.name == TaskContext.singleton.previewObject.name)
            {
                objectToTeleport.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                objectToTeleport.transform.position = teleportTarget.position;
                objectToTeleport = null;
                TaskContext.singleton.NextObject();
            }
            else
            {
                objectToTeleport.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * repulsionForce, ForceMode.Impulse);
                objectToTeleport.ResetPosition(5);
            }
        }
    }


}
