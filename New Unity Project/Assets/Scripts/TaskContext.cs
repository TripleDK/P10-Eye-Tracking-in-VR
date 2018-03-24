using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
public class TaskContext : NetworkBehaviour
{
    public static TaskContext singleton;
    public GameObject previewObject;
    public List<GameObject> objects = new List<GameObject>();
    [SerializeField] List<Transform> spawnPos = new List<Transform>();
    [SerializeField] float previewRotationSpeed = 180f;
    [SerializeField] TextMeshPro nameField;
    [SerializeField] TextMeshPro debugNameField;


    [SerializeField] SyncListInt SyncListShuffledObjects = new SyncListInt();
    [SyncVar] public string previewObjectName = "Namerino";

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
        if (NetworkServer.active)
        {
            StartCoroutine(WaitABit());
        }
    }

    IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(1);
        CmdSetup();
    }

    [Server]
    void CmdSetup()
    {
        Debug.Log("Commanding!");
        SyncListShuffledObjects.Clear();
        int objectCount = objects.Count;
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        for (int i = 0; i < objectCount; i++)
        {
            //Task order list
            int y = Random.Range(0, objects.Count);
            while (SyncListShuffledObjects.Contains(y))
            {
                y++;
                if (y > objectCount - 1) y = 0;
            }
            SyncListShuffledObjects.Add(y);
            Debug.Log(SyncListShuffledObjects[i]);

            //Spawn Objects
            List<int> usedPos = new List<int>();
            int posIndex = Random.Range(0, objects.Count);
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
            Win();
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
    void Win()
    {
        nameField.text = "You did it! gz maen";
    }
}
