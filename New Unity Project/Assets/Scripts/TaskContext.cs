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
    [SerializeField] float previewRotationSpeed = 180f;
    [SerializeField] TextMeshPro nameField;
    [SyncVar]
    SyncListInt SyncListShuffledObjects = new SyncListInt();


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
        if (NetworkServer.active) StartCoroutine(WaitABit());
    }

    IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(1);
        CmdSetup();
    }

    [Command]
    void CmdSetup()
    {
        Debug.Log("Commanding!");
        SyncListShuffledObjects.Clear();
        int objectCount = objects.Count;
        for (int i = 0; i < objectCount; i++)
        {
            int y = Random.Range(0, objects.Count);
            while (SyncListShuffledObjects.Contains(y))
            {
                y++;
                if (y > objectCount) y = 0;
            }
            SyncListShuffledObjects.Add(y);
            Debug.Log(SyncListShuffledObjects[i]);
        }

        NextObject();
    }

    public void NextObject()
    {
        if (SyncListShuffledObjects.Count == 0)
        {
            Win();
            return;
        }
        GameObject tempObject = previewObject;
        previewObject = Instantiate(objects[SyncListShuffledObjects[0]], previewObject.transform.position, Quaternion.identity);
        previewObject.GetComponent<Rigidbody>().useGravity = false;
        Destroy(tempObject);
        SyncListShuffledObjects.Remove(SyncListShuffledObjects[0]);
        previewObject.name = previewObject.name.Replace("(Clone)", string.Empty);
        nameField.text = previewObject.name;
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
