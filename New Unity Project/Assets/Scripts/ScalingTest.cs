using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingTest : MonoBehaviour
{

    [SerializeField] SkinnedMeshRenderer mesh;
    float startSizeY, startSizeX;
    Transform leftHand, rightHand, head;
    [SerializeField] Transform leftEye;
    Camera mainCamera;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(FindSteamVR());
        mainCamera = Camera.main;
        //     startSizeY = mesh.bounds.size.y;
        startSizeY = leftEye.position.y;
        startSizeX = mesh.bounds.size.x;
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
        StartCoroutine(ScaleTest());
    }


    // Update is called once per frame
    IEnumerator ScaleTest()
    {
        while (true)
        {
            float headHeight = mainCamera.transform.localPosition.y;
            float yScale = headHeight / startSizeY;
            float armLength = Vector2.Distance(new Vector2(leftHand.position.x, leftHand.position.z), new Vector2(rightHand.position.x, rightHand.position.z));
            float xScale = armLength / startSizeX;
            Vector3 tempScale = Vector3.one;
            tempScale = new Vector3(tempScale.x * xScale, tempScale.y * yScale, tempScale.z);
            transform.localScale = tempScale;
            yield return null;
        }
    }
}
