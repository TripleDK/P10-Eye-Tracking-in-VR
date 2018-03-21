﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DisembodiedAvatarScaling : NetworkBehaviour
{
    [SerializeField] DisembodiedAvatarControls disembodiedControls;
    [SerializeField] Transform rHandContainer;
    [SerializeField] Transform lHandContainer;
    [SerializeField] Transform torsoContainer;
    [SerializeField] Transform headContainer;

    private Transform leftController, rightController;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }


    void Resize()
    {

        Transform cameraRig = GameObject.Find("[CameraRig]").transform;
        cameraRig.position = new Vector3(transform.position.x, 0, transform.position.z);
        cameraRig.eulerAngles = new Vector3(0, transform.rotation.y, 0);
        CalibrationContext.singleton.playerTransform = transform;
        leftController = GameObject.Find("Controller (left)").transform;
        rightController = GameObject.Find("Controller (right)").transform;

        float headHeight = mainCamera.transform.position.y;
        float yScale = headHeight / disembodiedControls.leftEye.position.y;
        Debug.Log("yScale: " + yScale);
        float armLength = Vector2.Distance(new Vector2(leftController.position.x, leftController.position.z), new Vector2(rightController.position.x, rightController.position.z));
        float xScale = armLength / Vector2.Distance(new Vector2(disembodiedControls.leftHand.position.x, disembodiedControls.leftHand.position.z), new Vector2(disembodiedControls.rightHand.position.x, disembodiedControls.rightHand.position.z)); ;
        Vector3 tempScale = Vector3.one;
        tempScale = new Vector3(tempScale.x * xScale, tempScale.y * yScale, tempScale.z);
        headContainer.localScale = new Vector3(tempScale.x, tempScale.y, tempScale.x);
        headContainer.localPosition = new Vector3(0, headContainer.localPosition.y * yScale, 0);
        torsoContainer.localScale = new Vector3(tempScale.x, tempScale.y, tempScale.x);
        torsoContainer.localPosition = new Vector3(0, torsoContainer.localPosition.y * yScale, 0);
        lHandContainer.localScale = new Vector3(tempScale.x, tempScale.y, tempScale.x);
        lHandContainer.localPosition = new Vector3(lHandContainer.localPosition.x * xScale, lHandContainer.localPosition.y, lHandContainer.localPosition.z);
        rHandContainer.localScale = new Vector3(tempScale.x, tempScale.y, tempScale.x);
        rHandContainer.localPosition = new Vector3(rHandContainer.localPosition.x * xScale, rHandContainer.localPosition.y, rHandContainer.localPosition.z);
        headContainer.gameObject.SetActive(false);
        torsoContainer.gameObject.SetActive(false);
    }



    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer && CalibrationContext.singleton.calibrationProgress > 0)
        {
            disembodiedControls.LocalIKSetup();
            Resize();
            gameObject.tag = "LocalPlayer";
        }
    }


}
