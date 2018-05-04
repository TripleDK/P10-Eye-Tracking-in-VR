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
    [SyncVar]
    public int taskCondition = -1;
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

    public GameObject objectToFind = null;
    [SerializeField] SyncListInt SyncListShuffledObjects = new SyncListInt();
    [SyncVar] public string previewObjectName = "Namerino";
    [SyncVar] public float timeStart;
    public SyncListInt conditionsCompleted = new SyncListInt();
    string data = "";

    public UnityEvent OnWindowOpen = new UnityEvent();
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
            conditionsCompletedString += cond.ToString("0") + " ";
        }
        Debug.Log("Starting game! Current condition: " + TaskContext.singleton.taskCondition + ", Conditions completed: " + conditionsCompletedString);
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
        OnWindowOpen.Invoke();

    }

    [ClientRpc]
    void RpcCloseWindow()
    {
        windowAnimator[0].SetTrigger("Close");
        windowAnimator[1].SetTrigger("Close");
    }

    [Command]
    public void CmdNextObject()
    {
        if (objects.Count - SyncListShuffledObjects.Count >= numOfObjects)
        {
            conditionsCompleted.Add(TaskContext.singleton.taskCondition);

            /*      if (conditionsCompleted.Count == 4)
                  {
                      RpcWin();
                  }
                  else
                  {*/

            int newCondition = UnityEngine.Random.Range(0, 4);
            if (conditionsCompleted.Count < 4)
            {
                while (conditionsCompleted.Contains(newCondition))
                {
                    newCondition++;
                    if (newCondition > 3) newCondition = 0;
                }
            }
            foreach (GameObject ob in spawnedObjects)
            {
                NetworkServer.Destroy(ob);
            }
            spawnedObjects.Clear();

            RpcConditionDone(newCondition, (Time.time - timeStart));
            RpcCloseWindow();
            //  }
            return;
        }
        GameObject tempObject = previewObject;
        previewObject = Instantiate(objects[SyncListShuffledObjects[0]], previewObject.transform.position, Quaternion.identity);
        previewObject.GetComponent<Rigidbody>().useGravity = false;
        previewObjectName = previewObject.name;
        NetworkServer.Spawn(previewObject);
        NetworkServer.Destroy(tempObject);
        objectToFind = spawnedObjects[0];
        //        Debug.Log("Objects left: " + (SyncListShuffledObjects.Count - objects.Count + numOfObjects) + ", new object: " + previewObject.name);
        SyncListShuffledObjects.Remove(SyncListShuffledObjects[0]);
        spawnedObjects.Remove(spawnedObjects[0]);
        previewObject.name = previewObject.name.Replace("(Clone)", string.Empty);

        RpcNameChange(previewObject.name, objectToFind.GetComponent<NetworkIdentity>());

    }


    [ClientRpc]
    void RpcConditionDone(int value, float timeTaken)
    {
        Debug.Log("One condition is done!! Good job!");
        CalibrationContext.singleton.likertManager.gameObject.SetActive(true);
        CalibrationContext.singleton.likertManager.Initialize(TaskContext.singleton.taskCondition);
        if (File.Exists("Assets/Resources/Logs/Prelim3Test/Highscores.txt"))
        {
            File.AppendAllText("Assets/Resources/Logs/Prelim3Test/Highscores.txt", "Condition: " + TaskContext.singleton.taskCondition + ", Time: " + (timeTaken + errorGrabs * 10).ToString("0.000") + "\n");
        }
        else
        {
            File.WriteAllText("Assets/Resources/Logs/Prelim3Test/Highscores.txt", "High scores: \n" + "Condition: " + TaskContext.singleton.taskCondition + ", Time: " + (timeTaken + errorGrabs * 10).ToString("0.000") + "\n");
        }
        data += "\n\nCondition: " + TaskContext.singleton.taskCondition + ", Errors: " + errorGrabs.ToString("0") + ", Time stared at a face: " + timeGazeAtFace +
                   ", Time taken: " + timeTaken.ToString("0.00") + ", Average FPS: " + (Time.frameCount / Time.realtimeSinceStartup);


        CalibrationContext.singleton.ResetToMirror();

        AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);

        //Prelim experiment only:
        CalibrationContext.singleton.taskCondition = value;
        TaskContext.singleton.taskCondition = value;

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
    void RpcNameChange(string name, NetworkIdentity target)
    {
        //        Debug.Log("Changing name: " + name + ", targetName: " + target.name, target);
        objectToFind = target.gameObject;
        lookTargetController.pointsOfInterest[0] = target.transform;
        lookTargetController.pointsOfInterest[1] = target.transform;
        terminatorVision.target = target.transform;
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
        File.WriteAllText("Assets/Resources/Logs/Prelim3Test/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt",
            "Scene: " + SceneManager.GetActiveScene().name + "\nRole: " + role + data + "\n\n" + likertAnswers);
        File.AppendAllText("Assets/Resources/Logs/Prelim3Test/Highscores.txt", "\n\n");

    }

    #region FetcherTutDone
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
    #endregion


    #region FixerTutDoneRegion
    public void FixerTutDone()
    {
        OnHasAuthority.AddListener(FixerTutDoneCall);
        StartCoroutine(WaitForAuthority());
    }

    void FixerTutDoneCall()
    {
        CmdFixerTutDone();
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
    #endregion



    public void SetTaskCondition()
    {
        if (isServer)
        {
            Debug.Log("You decide taskcondition!");
            CmdSetTaskCondition();
        }
        else
        {
            Debug.Log("You are not worthy!");
        }
    }

    [Command]
    void CmdSetTaskCondition()
    {
        taskCondition = CalibrationContext.singleton.taskCondition;
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
