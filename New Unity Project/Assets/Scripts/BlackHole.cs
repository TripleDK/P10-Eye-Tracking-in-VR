using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class BlackHole : NetworkBehaviour
{
    public UnityEvent OnEatObject = new UnityEvent();
    [SerializeField] float eatTime = 0.8f;
    [SerializeField] AnimationCurve eatMovement;

    Vector3 startScale;

    void Awake()
    {
        startScale = transform.localScale;
    }

    public void TakeAuthority()
    {
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        playerId.GetComponent<Player>().CmdSetAuth(netId, playerId);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ObjectInteractions>())
        {
            OnEatObject.Invoke();
            Destroy(col.gameObject);
            StartCoroutine(Eat());
            if (col.gameObject.name == "Sphere(Clone)")
            {
                return;
            }
            NetworkIdentity playerId = col.gameObject.GetComponent<ObjectInteractions>().playerId;
            if (playerId == null)
                Debug.Log("That's weird!");
            else
                playerId.GetComponent<Player>().CmdSetAuth(TaskContext.singleton.netId, playerId);
            StartCoroutine(WaitForAuthorTaskContext());
        }
    }

    IEnumerator WaitForAuthorTaskContext()
    {
        while (!TaskContext.singleton.hasAuthority)
        {
            yield return null;
        }
        TaskContext.singleton.CmdNextObject();
    }

    IEnumerator Eat()
    {
        float startTime = Time.time;
        while (Time.time - startTime < eatTime)
        {
            float scaleValue = eatMovement.Evaluate((Time.time - startTime) / eatTime);
            transform.localScale = startScale * scaleValue;
            yield return null;
        }
        transform.localScale = startScale;
    }

}
