using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.IO;
using System;

public class TaskContext : NetworkBehaviour
{
    public static TaskContext singleton;
    [SyncVar]
    public int errorGrabs = 0;
    [SyncVar]
    public float timeGazeAtFace = 0;
    public GameObject previewObject;
    public List<GameObject> objects = new List<GameObject>();
    [SerializeField] List<Transform> spawnPos = new List<Transform>();
    [SerializeField] float previewRotationSpeed = 180f;
    [SerializeField] TextMeshPro nameField;
    [SerializeField] TextMeshPro debugNameField;
    [SerializeField] AudioClip winSound;
    [SyncVar] bool fetcherTutDone = false;
    [SyncVar] bool fixerTutDone = false;

    [SerializeField] SyncListInt SyncListShuffledObjects = new SyncListInt();
    [SyncVar] public string previewObjectName = "Namerino";
    float timeStart;

    UnityEvent OnHasAuthority = new UnityEvent();

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogWarning("Two TaskContexts in scene!");
        }

    }

    public override void OnStartClient()
    {
        string tempName = previewObjectName.Replace("(Clone)", string.Empty);
        nameField.text = tempName;
        debugNameField.text = tempName;
        /*   if (NetworkServer.active)
           {
               StartCoroutine(WaitABit());
           }*/
    }

    /* IEnumerator WaitABit()
      {
          yield return new WaitForSeconds(1);
          //  CmdSetup();
      }*/

    [Command]
    void CmdSetup()
    {
        SyncListShuffledObjects.Clear();
        timeStart = Time.time;
        int objectCount = objects.Count;
        List<int> usedPos = new List<int>();
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        for (int i = 0; i < objectCount; i++)
        {
            //Task order list
            int y = UnityEngine.Random.Range(0, objects.Count);
            while (SyncListShuffledObjects.Contains(y))
            {
                y++;
                if (y > objectCount - 1) y = 0;
            }
            SyncListShuffledObjects.Add(y);

            //Spawn Objects
            int posIndex = UnityEngine.Random.Range(0, objects.Count);
            while (usedPos.Contains(posIndex))
            {
                posIndex--;
                if (posIndex < 0) posIndex = objects.Count - 1;
            }
            usedPos.Add(posIndex);
            var go = (GameObject)Instantiate(objects[y], spawnPos[posIndex].position, Quaternion.identity);
            NetworkServer.SpawnWithClientAuthority(go, playerId.connectionToClient);

        }
        CmdNextObject();
    }

    [Command]
    public void CmdNextObject()
    {
        if (SyncListShuffledObjects.Count == 0)
        {
            RpcWin();
            return;
        }
        GameObject tempObject = previewObject;
        previewObject = Instantiate(objects[SyncListShuffledObjects[0]], previewObject.transform.position, Quaternion.identity);
        previewObject.GetComponent<Rigidbody>().useGravity = false;
        previewObjectName = previewObject.name;
        NetworkServer.Spawn(previewObject);
        NetworkServer.Destroy(tempObject);
        SyncListShuffledObjects.Remove(SyncListShuffledObjects[0]);
        previewObject.name = previewObject.name.Replace("(Clone)", string.Empty);
        RpcNameChange(previewObject.name);
    }


    [ClientRpc]
    void RpcNameChange(string name)
    {
        Debug.Log("Changing name!");
        nameField.text = name;
        debugNameField.text = name;
    }

    void Update()
    {
        previewObject.transform.RotateAround(previewObject.transform.position, Vector3.up, previewRotationSpeed * Time.deltaTime);
    }

    [ClientRpc]
    void RpcWin()
    {
        nameField.text = "You did it! gz mang";
        Debug.Log("Chicken dinner!");
        AudioSource.PlayClipAtPoint(winSound, transform.position);
        File.WriteAllText("Assets/Resources/Logs/" + DateTime.Now.ToString("h-mm-ss tt") + ".txt",
            "Scene: " + SceneManager.GetActiveScene().name + "\nErrors: " + errorGrabs.ToString("0") + "\nTime stared at a face: " + timeGazeAtFace + "\nTime taken: " + (Time.time - timeStart).ToString("0.00"));
    }

    public void FetcherTutDone()
    {
        OnHasAuthority.AddListener(CmdFetcherTutDone);
        StartCoroutine(WaitForAuthority());
    }

    [Command]
    void CmdFetcherTutDone()
    {
        fetcherTutDone = true;
        if (fixerTutDone)
        {
            CmdSetup();
        }
    }


    public void FixerTutDone()
    {
        OnHasAuthority.AddListener(CmdFixerTutDone);
        StartCoroutine(WaitForAuthority());
    }

    [Command]
    void CmdFixerTutDone()
    {
        fixerTutDone = true;
        if (fetcherTutDone)
        {
            CmdSetup();
        }
    }


    IEnumerator WaitForAuthority()
    {
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        playerId.GetComponent<Player>().CmdSetAuth(netId, playerId);
        while (!hasAuthority)
        {
            yield return null;
        }
        OnHasAuthority.Invoke();
        OnHasAuthority.RemoveAllListeners();
    }
}
