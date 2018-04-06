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
    public int style = 0; //0 = Realistic, 1 = Disembodied cartoon
    public int role = 0; //0 = Fetcher, 1 = Fixer
    public int calibrationProgress = 0;

    [SerializeField] PupilManager pupilManager;
    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject genderSelect;
    [SerializeField] GameObject networkButtons;
    [SerializeField] GameObject styleButtons;
    [SerializeField] GameObject roleSelect;
    [SerializeField] Transform pupilLabCalibratePos;
    [SerializeField] BlackHole blackHole;
    Transform fixerPos, fetcherPos;

    Transform leftHand, rightHand, head;
    Transform[] startPos = new Transform[2];

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
    }

    void Start()
    {
        networkButtons.SetActive(false);
        genderSelect.SetActive(false);
        styleButtons.SetActive(false);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        StartCoroutine(FindSteamVR());
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
        genderSelect.SetActive(true);
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
        leftHand.GetChild(0).gameObject.SetActive(false);
        rightHand.GetChild(0).gameObject.SetActive(false);

        roleSelect.SetActive(true);

    }

    public void ChooseRole(int role)
    {
        this.role = role;
        Transform cameraRig = GameObject.Find("[CameraRig]").transform;
        cameraRig.rotation = startPos[this.role].rotation;
        cameraRig.position = startPos[this.role].position;

        if (role == 1)
        {
            pupilManager.gameObject.SetActive(true);
            cameraRig.position = pupilLabCalibratePos.position;
            cameraRig.rotation = pupilLabCalibratePos.rotation;
            PupilTools.OnCalibrationEnded += PupilCalibrateDone;
            //Give blackhole authority
            blackHole.TakeAuthority();
        }
        playerTransform.position = cameraRig.position;
    }

    void PupilCalibrateDone()
    {
        Transform cameraRig = GameObject.Find("[CameraRig]").transform;
        cameraRig.rotation = startPos[this.role].rotation;
        cameraRig.position = startPos[this.role].position;
        playerTransform.position = cameraRig.position;
    }
}
