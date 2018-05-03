﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MirrorMovement : MonoBehaviour
{

    public List<Transform> transformsToMirror = new List<Transform>();
    [SerializeField] Transform mirroredSpot;
    [SerializeField] GameObject hmd;
    List<Transform> movingTransforms = new List<Transform>();
    Vector3 mirrorOffset;

    // Use this for initialization
    public void Intialize()
    {
        movingTransforms.Clear();
        List<Transform> myTransforms = gameObject.GetComponentsInChildren<Transform>().ToList();
        foreach (Transform tran in transformsToMirror)
        {
            movingTransforms.Add(myTransforms.Where(obj => obj.name == tran.name).SingleOrDefault());
            movingTransforms[movingTransforms.Count - 1].localScale = tran.localScale;
        }
        mirrorOffset = mirroredSpot.position - transform.parent.position;



        if (TaskContext.singleton.taskCondition == 0 && CalibrationContext.singleton.role == 1)
        {
            hmd.SetActive(true);
        }
        else
        {
            hmd.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transformsToMirror.Count; i++)
        {
            movingTransforms[i].localPosition = transformsToMirror[i].localPosition;
            movingTransforms[i].localRotation = transformsToMirror[i].localRotation;
            // movingTransforms[i].position = transformsToMirror[i].position - mirrorOffset;
            // movingTransforms[i].rotation = Quaternion.Euler(transformsToMirror[i].eulerAngles + Vector3.up * 180);
        }
    }
}
