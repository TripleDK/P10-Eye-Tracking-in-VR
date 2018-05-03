using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.IO;
using System;
using RealisticEyeMovements;

public class TaskContext : NetworkBehaviour
{
    public static TaskContext singleton;
    [SyncVar]
    public int errorGrabs = 0;
    [SyncVar]
    public float timeGazeAtFace = 0;
    [SyncVar]
    public float highScoreTime = 0;
    public GameObject previewObject;
    public List<GameObject> objects = new List<GameObject>();
    public Transform realEyeTarget;
    public LookTargetController lookTargetController;
    public TerminatorVision terminatorVision;
    [HideInInspector] public string likertAnswers;

    [SerializeField] List<Transform> spawnPos = new List<Transform>();
    [SerializeField] int numOfObjects = 5;
    [SerializeField] float previewRotationSpeed = 180f;
    [SerializeField] TextMeshPro nameField;
    [SerializeField] TextMeshPro debugNameField;
    [SerializeField] Animator[] windowAnimator = new Animator[2];
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip windowSound;
    [SyncVar] bool fetcherTutDone = false;
    [SyncVar] bool fixerTutDone = false;
    List<GameObject> spawnedObjects = new List<GameObject>();
    float averageFPS;

    GameObject objectToFind = null;
    [SerializeField] SyncListInt SyncListShuffledObjects = new SyncListInt();
    [SyncVar] public string previewObjectName = "Namerino";
    float timeStart;
    public static List<int> conditionsCompleted = new List<int>();
    string data = "";


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

    [Server]
    void CmdSetup()
    {
        string conditionsCompletedString = "";
        foreach (int cond in conditionsCompleted)
        {
            conditionsCompletedString += cond.ToString() + " ";
        }
        Debug.Log("Starting game! Current condition: " + CalibrationContext.singleton.taskCondition + ", Conditions completed: " + conditionsCompletedString);
        if (conditionsCompleted.Count >= 4)
        {
            RpcWin();
            return;
        }
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
            spawnedObjects.Add(go);
        }
        RpcOpenWindow();
        CmdNextObject();
    }

    [ClientRpc]
    void RpcOpenWindow()
    {

        AudioSource.PlayClipAtPoint(windowSound, windowAnimator[0].transform.position);
        windowAnimator[0].SetTrigger("Open");
        windowAnimator[1].SetTrigger("Open");
    }

    [Command]
    public void CmdNextObject()
    {
        if (objects.Count - SyncListShuffledObjects.Count >= numOfObjects)
        {
            conditionsCompleted.Add(CalibrationContext.singleton.eyeModel);

            /*      if (conditionsCompleted.Count == 4)
                  {
                      RpcWin();
                  }
                  else
                  {*/

            int newCondition = UnityEngine.Random.Range(0, 4);
            while (conditionsCompleted.Contains(newCondition))
            {
                newCondition++;
                if (newCondition > 3) newCondition = 0;
            }
            RpcConditionDone(newCondition);
            //  }
            return;
        }
        GameObject tempObject = previewObject;
        previewObject = Instantiate(objects[SyncListShuffledObjects[0]], previewObject.transform.position, Quaternion.identity);
        previewObject.GetComponent<Rigidbody>().useGravity = false;
        previewObjectName = previewObject.name;
        NetworkServer.Spawn(previewObject);
        NetworkServer.Destroy(tempObject);
        objectToFind = spawnedObjects[SyncListShuffledObjects[0]];
        Debug.Log("Objects left: " + (objects.Count - SyncListShuffledObjects.Count - numOfObjects) + ", new object: " + previewObject.name);
        SyncListShuffledObjects.Remove(SyncListShuffledObjects[0]);
        previewObject.name = previewObject.name.Replace("(Clone)", string.Empty);

        RpcNameChange(previewObject.name, objectToFind.transform);

    }


    [ClientRpc]
    void RpcConditionDone(int value)
    {
        CalibrationContext.singleton.likertManager.gameObject.SetActive(true);
        CalibrationContext.singleton.likertManager.Initialize(CalibrationContext.singleton.taskCondition);
        if (File.Exists("Assets/Resources/Logs/Prelim3Test/Highscores.txt"))
        {
            File.AppendAllText("Assets/Resources/Logs/Prelim3Test/Highscores.txt", "Condition: " + CalibrationContext.singleton.taskCondition + ", Time: " + ((Time.time - timeStart) + errorGrabs * 10).ToString("0.000") + "\n");
        }
        else
        {
            File.WriteAllText("Assets/Resources/Logs/Prelim3Test/Highscores.txt", "High scores: \n" + "Condition: " + CalibrationContext.singleton.taskCondition + ", Time: " + ((Time.time - timeStart) + errorGrabs * 10).ToString("0.000") + "\n");
        }
        data += "\nCondition: " + CalibrationContext.singleton.taskCondition + "\nErrors: " + errorGrabs.ToString("0") + "\nTime stared at a face: " + timeGazeAtFace +
                   "\nTime taken: " + (Time.time - timeStart).ToString("0.00") + "\nAverage FPS: " + (Time.frameCount / Time.time);


        CalibrationContext.singleton.ResetToMirror();



        //Prelim experiment only:
        CalibrationContext.singleton.taskCondition = value;

        //Main experiment only:
        //  CalibrationContext.singleton.eyeModel = value;

        foreach (DisembodiedAvatarControls avatarControls in FindObjectsOfType<DisembodiedAvatarControls>())
        {
            avatarControls.SetEyeModel();
        }

        errorGrabs = 0;
        timeGazeAtFace = 0;
        fetcherTutDone = false;
        fixerTutDone = false;
    }

    [ClientRpc]
    void RpcNameChange(string name, Transform target)
    {
        //        Debug.Log("Changing name!");
        lookTargetController.pointsOfInterest[0] = objectToFind.transform;
        terminatorVision.target = objectToFind.transform;
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
        nameField.text = "Thank you for participating!";
        Debug.Log("Chicken dinner!");
        AudioSource.PlayClipAtPoint(winSound, transform.position);
        string role = CalibrationContext.singleton.role == 0 ? "Fetcher" : "Fixer";
        File.WriteAllText("Assets/Resources/Logs/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt",
            "Scene: " + SceneManager.GetActiveScene().name + "\nRole: " + role + data);
        File.AppendAllText("Assets/Resources/Logs/Prelim3Test/Averages.txt", "Condition: " + CalibrationContext.singleton.taskCondition + ", Time: " + ((Time.time - timeStart) + errorGrabs * 10).ToString("0.000") + "\n\n" + likertAnswers);

    }

    public void FetcherTutDone()
    {
        OnHasAuthority.AddListener(FetcherTutDoneCall);
        StartCoroutine(WaitForAuthority());
    }

    void FetcherTutDoneCall()
    {
        CmdFetcherTutDone();
    }

    [Command]
    void CmdFetcherTutDone()
    {
        fetcherTutDone = true;

        RpcFetcherTutDone();
    }

    [ClientRpc]
    void RpcFetcherTutDone()
    {
        Debug.Log("Fetcher tut is done!");
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
        if (isServer) RpcFixerTutDone();
    }

    [ClientRpc]
    void RpcFixerTutDone()
    {
        Debug.Log("Fixer tut done!");
        fixerTutDone = true;
        if (fetcherTutDone)
        {
            CmdSetup();
        }
    }

    IEnumerator WaitForAuthority()
    {
        //    Debug.Log("Trying to get auth over taskcontext! " + Time.time);
        NetworkIdentity playerId = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<NetworkIdentity>();
        playerId.GetComponent<Player>().CmdSetAuth(netId, playerId);
        while (!hasAuthority)
        {
            yield return null;
        }
        //    Debug.Log("Got authority! " + Time.time);
        OnHasAuthority.Invoke();
        OnHasAuthority.RemoveAllListeners();
    }
}
