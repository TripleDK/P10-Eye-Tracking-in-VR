﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealisticEyeMovements;

public class DisembodiedAvatarControls : MonoBehaviour
{

    public bool isLocalPlayer = false;
    public Transform headPos, leftHandPos, rightHandPos, lookAtPos;
    public Transform head, leftHand, rightHand, torso;
    public Transform leftEye, rightEye;
    [SerializeField] float neckHeight = 0.3f;
    [SerializeField] float torsoMoveSpeed = 1.0f;
    [SerializeField] float torsoRotateSpeed = 1.0f;
    [SerializeField] AnimationCurve torsoRotateSpeedOverDistance;
    [SerializeField] float headLength = 1.0f;
    [SerializeField] EyeGazeModel eyeGazeModel;
    [SerializeField] GameObject hmdObject;
    Transform leftHandTarget, rightHandTarget, headTarget, lookAtTarget;
    EyeAndHeadAnimator eyeModel;
    LookTargetController lookTargetController;

    enum EyeGazeModel { Hmd, Static, Modelled, Eyetracking };

    public void LocalIKSetup()
    {
        StartCoroutine(LocalIKSetupCo());
    }

    IEnumerator LocalIKSetupCo()
    {
        while (GameObject.Find("Camera (eye)") == null)
        {
            yield return new WaitForSeconds(1);
        }
        isLocalPlayer = true;
        rightHandTarget = GameObject.Find("Controller (right)").transform;
        leftHandTarget = GameObject.Find("Controller (left)").transform;
        headTarget = GameObject.Find("Camera (eye)").transform;
        lookTargetController = GetComponent<LookTargetController>();
        if (lookTargetController != null)
        {
            lookTargetController.pointsOfInterest = new Transform[1];
            TaskContext.singleton.lookTargetController = this.lookTargetController;
        }
        else
        {
            Debug.Log("No looktarget controller on player!");
        }

        //For main experiment!!
        //        eyeGazeModel = (EyeGazeModel)CalibrationContext.singleton.eyeModel;
        //Main experiment end
    }

    public void SetEyeModel()
    {
        int role;
        if (isLocalPlayer)
        {
            role = CalibrationContext.singleton.role;
        }
        else
        {
            if (CalibrationContext.singleton.role == 0)
            {
                role = 1;
            }
            else
            {
                role = 0;
            }
        }

        if (role == 1) //Fixer
        {
            Debug.Log("Setting eyeGazeModel...");
            eyeGazeModel = (EyeGazeModel)TaskContext.singleton.taskCondition;
            Debug.Log("Eyegazemodel now: " + eyeGazeModel + ", in ints: " + TaskContext.singleton.taskCondition);
            /*   if (CalibrationContext.singleton.taskCondition < 2)
               {
                   eyeGazeModel = EyeGazeModel.Eyetracking;
               }
               else
               {
                   eyeGazeModel = EyeGazeModel.Hmd;
               }*/
        }
        else if (role == 0)
        { //Fetcher
            eyeGazeModel = EyeGazeModel.Static;
        }



        if (eyeGazeModel == EyeGazeModel.Eyetracking)
        {
            if (headTarget != null) lookAtTarget = headTarget.gameObject.transform.Find("GazeDirection").GetComponent<GazeDirection>().calculatedLookAt;
        }
        else
        {
            lookAtTarget = null;
        }
        if (eyeGazeModel == EyeGazeModel.Modelled)
        {
            eyeModel = GetComponent<EyeAndHeadAnimator>();
            eyeModel.enabled = true;
        }
        else
        {
            eyeModel = GetComponent<EyeAndHeadAnimator>();
            eyeModel.enabled = false;
        }

        if (eyeGazeModel == EyeGazeModel.Hmd)
        {
            hmdObject.SetActive(true);
        }
        else
        {
            hmdObject.SetActive(false);
        }
    }


    public void ResetTorsoPosition()
    {
        torso.position = head.position - Vector3.up * neckHeight - head.forward * headLength;
    }


    void Update()
    {
        if (isLocalPlayer)
        {
            headPos.position = headTarget.position;
            headPos.rotation = headTarget.rotation;
            leftHandPos.position = leftHandTarget.position;
            leftHandPos.rotation = leftHandTarget.rotation;
            rightHandPos.position = rightHandTarget.position;
            rightHandPos.rotation = rightHandTarget.rotation;

            switch (eyeGazeModel)
            {
                case (EyeGazeModel.Static):
                    lookAtPos.position = headPos.position + headPos.forward * 10;
                    break;
                case (EyeGazeModel.Modelled):

                    break;
                case (EyeGazeModel.Eyetracking):
                    lookAtPos.position = lookAtTarget.position;
                    break;
                case (EyeGazeModel.Hmd):

                    break;
                default:
                    Debug.LogWarning("Eye model makes no sense :(");
                    break;
            }
        }
        head.position = headPos.position;
        head.rotation = headPos.rotation;
        leftHand.position = leftHandPos.position;
        leftHand.rotation = leftHandPos.rotation;
        rightHand.position = rightHandPos.position;
        rightHand.rotation = rightHandPos.rotation;
        leftEye.LookAt(lookAtPos);
        rightEye.LookAt(lookAtPos);

        //Torso Movement
        int upSideDown = 1;
        if (head.up.y < 0) upSideDown *= -1;
        torso.position = Vector3.MoveTowards(torso.position,
             head.position - Vector3.up * neckHeight - Vector3.ProjectOnPlane(head.forward * upSideDown, Vector3.up).normalized * headLength, torsoMoveSpeed * Time.deltaTime);

        torso.rotation = Quaternion.RotateTowards(torso.rotation, head.rotation,
        torsoRotateSpeed * torsoRotateSpeedOverDistance.Evaluate((Mathf.Abs(torso.rotation.eulerAngles.y - head.rotation.eulerAngles.y) / 180)));
        torso.eulerAngles = new Vector3(0, torso.rotation.eulerAngles.y, 0);
        //   torsoRotateSpeed * torsoRotateSpeedOverDistance.Evaluate((Mathf.Abs(torso.rotation.eulerAngles.y - head.rotation.eulerAngles.y) / 180))
        //   * Time.deltaTime * Mathf.Sign(torso.rotation.eulerAngles.y - head.rotation.eulerAngles.y));
        //   Debug.Log(Mathf.Abs(torso.rotation.eulerAngles.y - head.rotation.eulerAngles.y) / 180);
        // torso.RotateAround(torso.position, torso.forward,
        // torsoRotateSpeed * torsoRotateSpeedOverDistance.Evaluate((Mathf.Abs(torso.rotation.eulerAngles.y - head.rotation.eulerAngles.y) / 180)) * Time.deltaTime * Mathf.Sign(torso.rotation.eulerAngles.y - head.rotation.eulerAngles.y));
        //      torso.rotation.eulerAngles = new Vector3(0, Mathf.Lerp(torso.rotation.eulerAngles.y, head.rotation.eulerAngles.y, 0.1f), 0);
        // Quaternion.RotateTowards(torso.rotation, Quaternion.LookRotation(head.position - torso.position, -head.up), torsoRotateSpeed);
        //Vector3.RotateTowards(torso.eulerAngles, Quaternion.LookRotation(head.position).eulerAngles, torsoRotateSpeed).Quaternion;

    }
}
