using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CalibrationContext : MonoBehaviour
{
    static public CalibrationContext singleton;
    public IKControlTest localPlayerIKControls;
    public Transform playerTransform;
    public int networkFunction = 0; //0 = Host, 1 = Client, 2 = Server
    public int gender = 0; //0 = Male, 1 = Female
    public int style = 1; //0 = Realistic, 1 = Disembodied cartoon
    public int role = 0; //0 = Fetcher, 1 = Fixer
    public int eyeModel = 0; //0 = static, 1 = Hmd, 2 = modelled, 3 = eye tracking
    public int taskCondition = 0; //0 = static, 1 = Hmd, 2 = modelled, 3 = eye tracking
    public int calibrationProgress = 0;
    public LikertManager likertManager;

    [SerializeField] PupilManager pupilManager;
    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject genderSelect;
    [SerializeField] GameObject networkButtons;
    [SerializeField] GameObject styleButtons;
    [SerializeField] GameObject roleSelect;
    [SerializeField] Transform pupilLabCalibratePos;
    [SerializeField] Transform[] mirrorPos = new Transform[2];
    [SerializeField] MirrorMovement[] mirrorMovement = new MirrorMovement[2];

    [SerializeField] BlackHole blackHole;
    [SerializeField] FetcherTutorialContext fetcherTutorial;
    [SerializeField] FixerTutorialContext fixerTutorial;
    [SerializeField] Transform offsetCenterPosition;
    Transform fixerPos, fetcherPos;
    Vector3 positionalOffset;
    Transform leftHand, rightHand, head;
    Transform[] startPos = new Transform[2];
    Transform cameraRig;
    bool steamVRActive = false;
    bool readyUp = false;
    DisembodiedAvatarScaling avatarScaling;


    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("Two calibrationcontexts in scene!", this);
        }
        startPos[0] = GameObject.Find("Player1StartPos").transform;
        startPos[1] = GameObject.Find("Player2StartPos").transform;
        taskCondition = Random.Range(0, 4);
    }

    void OnDrawGizmos()
    {
        if (offsetCenterPosition) Gizmos.DrawCube(offsetCenterPosition.position, new Vector3(.1f, 2, .1f));
    }

    void Start()
    {
        networkButtons.SetActive(false);
        genderSelect.SetActive(false);
        styleButtons.SetActive(false);
        string ip = NetworkManager.singleton.networkAddress;
        if (ip == "192.168.0.1")
        {
            networkFunction = 0;
        }
        else
        {
            networkFunction = 1;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        StartCoroutine(FindSteamVR());
    }

    void OnGUI()
    {
        if (readyUp == false)
        {
            GUI.color = Color.yellow;
            GUI.Label(new Rect(10, 10, 200, 100), "SteamVR active: " + steamVRActive);
            if (GUI.Button(new Rect(10, 40, 100, 30), "Male"))
            {
                gender = 0;
            }
            if (GUI.Button(new Rect(120, 40, 100, 30), "Female"))
            {
                gender = 1;
            }
            if (GUI.Button(new Rect(10, 80, 100, 30), "Cartoony"))
            {
                style = 0;
            }
            if (GUI.Button(new Rect(120, 80, 100, 30), "Realistic"))
            {
                style = 1;
            }
            if (GUI.Button(new Rect(10, 120, 100, 30), "Fetcher"))
            {
                role = 0;
            }
            if (GUI.Button(new Rect(120, 120, 100, 30), "Fixer"))
            {
                role = 1;
            }

            //Only for main experiment
            /*  if (GUI.Button(new Rect(10, 160, 100, 30), "Static"))
              {
                  eyeModel = 0;
              }
              if (GUI.Button(new Rect(120, 160, 100, 30), "Hmd"))
              {
                  eyeModel = 1;
              }
              if (GUI.Button(new Rect(240, 160, 100, 30), "Model"))
              {
                  eyeModel = 2;
              }
              if (GUI.Button(new Rect(360, 160, 100, 30), "Real tracking"))
              {
                  eyeModel = 3;
              }

            if (GUI.Button(new Rect(480, 160, 100, 30), "Random"))
            {
                eyeModel = Random.Range(0, 4);
            }
            */
            //Main experiment end
            if (GUI.Button(new Rect(10, 200, 100, 30), "Hosting (0)"))
            {
                networkFunction = 0;
            }
            if (GUI.Button(new Rect(120, 200, 100, 30), "Client (1)"))
            {
                networkFunction = 1;
            }

            //Only for prelim!!!
            if (GUI.Button(new Rect(10, 160, 100, 30), "Hmd"))
            {
                taskCondition = 0;
            }
            if (GUI.Button(new Rect(120, 160, 100, 30), "Static"))
            {
                taskCondition = 1;
            }
            if (GUI.Button(new Rect(240, 160, 100, 30), "Modelled"))
            {
                taskCondition = 2;
            }
            if (GUI.Button(new Rect(360, 160, 100, 30), "Tracked"))
            {
                taskCondition = 3;
            }
            //Prelim end

            if (steamVRActive == false) GUI.color = Color.black; else GUI.color = Color.green;
            GUI.Label(new Rect(10, 240, 350, 60), "Status:\nGender: " + gender + ", Style: " + style + " Role: " + role + " Eye Models: " + eyeModel + " Hosting: " + networkFunction + ", TaskCondition: " + taskCondition);
            if (GUI.Button(new Rect(10, 290, 100, 30), "Ready"))
            {
                readyUp = true;
                StartCoroutine(WaitForCalibrate());
            }
        }
    }



    IEnumerator FindSteamVR()
    {

        while (leftHand == null || rightHand == null || head == null)
        {
            if (GameObject.Find("Controller (left)"))
                leftHand = GameObject.Find("Controller (left)").transform;
            if (GameObject.Find("Controller (right)"))
                rightHand = GameObject.Find("Controller (right)").transform;
            if (GameObject.Find("Camera (eye)"))
                head = GameObject.Find("Camera (eye)").transform;
            yield return null;
        }
        steamVRActive = true;
        //  genderSelect.SetActive(true);
        calibrationProgress = 1;
    }

    public void ChooseGender(int gender)
    {
        AvatarScaling.gender = gender;
        this.gender = gender;
        styleButtons.SetActive(true);
        genderSelect.SetActive(false);
    }

    public void ChooseStyle(int style)
    {
        this.style = style;
        styleButtons.SetActive(false);
        networkButtons.SetActive(true);
    }

    public void ChooseNetwork(int networkType)
    {
        networkFunction = networkType;
        networkButtons.SetActive(false);
        StartCoroutine(WaitForCalibrate());
    }

    IEnumerator WaitForCalibrate()
    {
        yield return null; //To not call on same fram when choosing network!
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        while (true)
        {
            if (Input.GetKeyDown("joystick button 14"))
            {
                FinishCalibration();
                yield break;
            }
            if (Input.GetKeyDown("joystick button 15"))
            {
                FinishCalibration();    //Trigger right
                yield break;
            }
            yield return null;
        }
    }

    public void FinishCalibration()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        switch (networkFunction)
        {
            case 0:
                Debug.Log("Trying to start host!");
                networkManager.StartHost();
                break;
            case 1:
                Debug.Log("Trying to start client!");
                networkManager.StartClient();
                break;
            default:
                break;
        }
        TaskContext.singleton.SetTaskCondition();

        leftHand.GetChild(0).gameObject.SetActive(false);
        rightHand.GetChild(0).gameObject.SetActive(false);
        positionalOffset = head.localPosition;
        offsetCenterPosition.localPosition = positionalOffset;
        positionalOffset.y = 0;
        likertManager.transform.localPosition += positionalOffset;
        StartCoroutine(ChooseRole(role));
        //        roleSelect.SetActive(true);
    }

    public IEnumerator ChooseRole(int role)
    {
        yield return new WaitForSeconds(1);
        this.role = role;
        cameraRig = GameObject.Find("[CameraRig]").transform;
        Debug.Log("Starting mirrors or Pupil with condition: " + TaskContext.singleton.taskCondition);
        if (role == 1)
        {
            yield return null;
            pupilManager.gameObject.SetActive(true);
            cameraRig.rotation = pupilLabCalibratePos.rotation;
            cameraRig.position = pupilLabCalibratePos.position - pupilLabCalibratePos.rotation * positionalOffset;

            PupilTools.OnCalibrationEnded += PupilCalibrateDone;

        }
        yield return new WaitForSeconds(1);
        if (role == 0)
        {
            cameraRig.rotation = mirrorPos[role].rotation;
            cameraRig.position = mirrorPos[role].position - mirrorPos[role].rotation * positionalOffset;
            likertManager.EnableContinueButton();
            StartCoroutine(WaitForMirroring());
        }
        playerTransform.position = cameraRig.position;
    }
    void PupilCalibrateDone()
    {
        PupilTools.OnCalibrationEnded -= PupilCalibrateDone;
        positionalOffset = head.localPosition;
        offsetCenterPosition.localPosition = positionalOffset;
        positionalOffset.y = 0;

        cameraRig.rotation = mirrorPos[role].rotation;
        cameraRig.position = mirrorPos[role].position - mirrorPos[role].rotation * positionalOffset;
        playerTransform.position = cameraRig.position;

        blackHole.TakeAuthority();
        likertManager.EnableContinueButton();
        StartCoroutine(WaitForMirroring());
    }


    IEnumerator WaitForMirroring()
    {
        yield return null;
        mirrorMovement[role].enabled = true;
        mirrorMovement[role].transformsToMirror.Clear();
        avatarScaling = playerTransform.GetComponent<DisembodiedAvatarScaling>();

        mirrorMovement[role].transformsToMirror.Add(avatarScaling.lHandContainer);
        mirrorMovement[role].transformsToMirror.Add(avatarScaling.rHandContainer);
        mirrorMovement[role].transformsToMirror.Add(avatarScaling.headContainer);
        mirrorMovement[role].transformsToMirror.Add(avatarScaling.torsoContainer);
        mirrorMovement[role].transformsToMirror.Add(avatarScaling.lEyeContainer);
        mirrorMovement[role].transformsToMirror.Add(avatarScaling.rEyeContainer);
        mirrorMovement[role].Intialize();
        // mirrorMovement[role].transform.eulerAngles = new Vector3(0, -90, 0);
        mirrorMovement[role].transform.localPosition = Vector3.zero - mirrorMovement[role].transform.parent.rotation * (new Vector3(positionalOffset.x, 0, -positionalOffset.z));
    }

    public void EndMirroring()
    {
        positionalOffset = head.localPosition;
        offsetCenterPosition.localPosition = positionalOffset;
        positionalOffset.y = 0;
        mirrorMovement[role].enabled = false;
        cameraRig.rotation = startPos[this.role].rotation;
        cameraRig.position = startPos[this.role].position - startPos[this.role].rotation * positionalOffset;
        playerTransform.position = cameraRig.position;
        if (TaskContext.conditionsCompleted.Count == 0)
        {
            StartTutorials();
        }
        else
        {
            if (role == 0)
            {
                TaskContext.singleton.FetcherTutDone();
            }
            else if (role == 1)
            {
                TaskContext.singleton.FixerTutDone();
            }
        }
    }

    void StartTutorials()
    {

        if (role == 0)
        {
            fetcherTutorial.StartTutorial();
        }
        if (role == 1)
        {
            fixerTutorial.StartTutorial();
        }
    }

    public void ResetToMirror()
    {

        StartCoroutine(ResetToMirrorCo());
    }

    IEnumerator ResetToMirrorCo()
    {
        cameraRig.rotation = mirrorPos[role].rotation;
        cameraRig.position = mirrorPos[role].position - mirrorPos[role].rotation * positionalOffset;
        playerTransform.position = cameraRig.position;
        mirrorMovement[role].Intialize();
        avatarScaling.disembodiedControls.LocalIKSetup();
        yield return null;
    }
}
