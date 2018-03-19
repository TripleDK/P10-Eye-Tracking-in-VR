using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterButton : MonoBehaviour
{
    [SerializeField] float maxPushPos = 1f;
    [SerializeField] Teleporter teleporter;
    [SerializeField] float coolDownTime = 1f;
    [SerializeField] float pushOffset = 0.5f;

    Material material;
    Color startCol;
    float startZ;
    bool coolingDown = false;
    void Awake()
    {
        startZ = transform.position.z;
        Physics.IgnoreLayerCollision(8, 8, true);
        material = GetComponent<MeshRenderer>().material;
        startCol = material.color;
    }

    private void OnTriggerStay(Collider other)
    {
        if (coolingDown == false)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Max(startZ, other.transform.position.z + pushOffset));
            if (transform.position.z >= maxPushPos + startZ)
            {
                teleporter.CmdActivate();

                StartCoroutine(ButtonCooldown());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (coolingDown == false) StartCoroutine(ButtonCooldown());
    }
    IEnumerator ButtonCooldown()
    {
        coolingDown = true;
        float startTime = Time.time;
        while (Time.time - startTime < coolDownTime)
        {
            material.color = startCol * (Time.time - startTime) / coolDownTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(startZ + maxPushPos, startZ, (Time.time - startTime) / coolDownTime));
            yield return null;
        }
        coolingDown = false;
    }

}
