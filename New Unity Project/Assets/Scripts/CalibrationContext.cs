using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CalibrationContext : MonoBehaviour
{
    static public CalibrationContext singleton;
    public IKControlTest localPlayerIKControls;
    static int calibrationProgress = 0;

    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject genderSelect;
    [SerializeField] GameObject networkButtons;

    Transform leftHand, rightHand, head;
    int networkFunction = 0; //0 = Host, 1 = Client, 2 = Server

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("Two calibrationcontexts in scene!");
        }
    }

    void Start()
    {
        networkButtons.SetActive(false);
        genderSelect.SetActive(false);
        StartCoroutine(FindSteamVR());
    }

    IEnumerator FindSteamVR()
    {
        while (leftHand == null && rightHand == null && head == null)
        {
            leftHand = GameObject.Find("Controller (left)").transform;
            rightHand = GameObject.Find("Controller (right)").transform;
            head = GameObject.Find("Camera (eye)").transform;
            yield return null;
        }
        genderSelect.SetActive(true);
    }

    public void ChooseGender(int gender)
    {

        networkButtons.SetActive(true);
        genderSelect.SetActive(false);
    }

    public void ChooseNetwork(int networkType)
    {
        networkFunction = networkType;
        StartCoroutine(WaitForCalibrate());
    }

    IEnumerator WaitForCalibrate()
    {
        while (true)
        {
            if (Input.GetKeyDown("joystick button 14"))
            {
                Calibrate();
                yield break;
            }
            if (Input.GetKeyDown("joystick button 15"))
            {
                Calibrate();    //Trigger right
                yield break;
            }
            yield return null;
        }
    }

    public void Calibrate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }
        networkManager.StartHost();
        //Find left and right controller
        //Scale
        //Set right model (male/female)
        //Hide Vive controllers
        //Hide local head
        //Transparent head mat + transparent eyelash mat + disable hair
    }
}
