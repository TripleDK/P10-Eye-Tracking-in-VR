﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminatorVision : MonoBehaviour
{

    public Transform target;
    [SerializeField] Image targetMarker;
    Camera cam;
    RectTransform rTransform;
    Vector2 uiOffset;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
        rTransform = GetComponent<RectTransform>();
        uiOffset = new Vector2(rTransform.sizeDelta.x / 2f, rTransform.sizeDelta.y / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 viewPortPos = cam.WorldToViewportPoint(target.position);
        Vector2 proportionalPosition = new Vector2(viewPortPos.x * rTransform.sizeDelta.x, viewPortPos.y * rTransform.sizeDelta.y);
        targetMarker.rectTransform.localPosition = proportionalPosition - uiOffset;
        //    targetMarker.transform.localEulerAngles = new Vector3(0, 0, transform.localRotation.z);
    }
}
