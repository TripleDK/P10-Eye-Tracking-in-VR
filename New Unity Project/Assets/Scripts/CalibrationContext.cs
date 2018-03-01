﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CalibrationContext : MonoBehaviour
{
    static public CalibrationContext singleton;
    public IKControlTest localPlayerIKControls;
    public int networkFunction = 0; //0 = Host, 1 = Client, 2 = Server
    int calibrationProgress = 0;

    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject genderSelect;
    [SerializeField] GameObject networkButtons;


    Transform leftHand, rightHand, head;

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
    }

    void Start()
    {
        networkButtons.SetActive(false);
        genderSelect.SetActive(false);
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
    }

    public void ChooseGender(int gender)
    {
        AvatarScaling.gender = gender;
        networkButtons.SetActive(true);
        genderSelect.SetActive(false);
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

        //Hide local head
        //Transparent head mat + transparent eyelash mat + disable hair
    }
}
