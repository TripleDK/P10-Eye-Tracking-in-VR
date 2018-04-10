using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class TeleporterButton : NetworkBehaviour
{
    [SerializeField] float maxPushPos = 1f;
    [SerializeField] Teleporter teleporter;
    [SerializeField] float coolDownTime = 1f;
    [SerializeField] float pushOffset = 0.5f;

    Material material;
    Color startCol;
    float startX;
    bool coolingDown = false;

    void Awake()
    {
        startX = transform.localPosition.x;
        Physics.IgnoreLayerCollision(8, 8, true);
        material = GetComponent<MeshRenderer>().material;
        startCol = material.color;
    }


    private void OnTriggerStay(Collider other)
    {
        if (coolingDown == false)
        {
            transform.localPosition = new Vector3(Mathf.Max(startX, transform.InverseTransformPoint(other.transform.position).x + pushOffset), transform.localPosition.y, transform.localPosition.z);
            other.gameObject.GetComponent<VRGrab>().Vibrate(Time.deltaTime, (ushort)1000);
            if (transform.localPosition.x >= maxPushPos + startX)
            {
                Debug.Log(other.gameObject.name);
                Debug.Log("Pushed the button!");
                NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
                playerId.GetComponent<Player>().CmdSetAuth(teleporter.netId, playerId);
                StartCoroutine(WaitForAuth());

                StartCoroutine(ButtonCooldown());
            }
        }
    }

    IEnumerator WaitForAuth()
    {
        while (!teleporter.hasAuthority)
        {
            yield return null;
        }
        teleporter.Activate();
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
            transform.localPosition = new Vector3(Mathf.Lerp(startX + maxPushPos, startX, (Time.time - startTime) / coolDownTime), transform.localPosition.y, transform.localPosition.z);
            yield return null;
        }
        coolingDown = false;
    }


}
