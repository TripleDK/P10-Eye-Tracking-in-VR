using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingTest : MonoBehaviour
{

    [SerializeField] SkinnedMeshRenderer mesh;
    float startSizeY, startSizeX;
    Transform leftController, rightController, head;
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftEye;
    Camera mainCamera;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(FindSteamVR());
        mainCamera = Camera.main;
        //     startSizeY = mesh.bounds.size.y;
        startSizeY = leftEye.position.y;
        if (mesh != null) startSizeX = mesh.bounds.size.x;
        else startSizeX = Vector3.Distance(leftHand.position, rightHand.position);
    }

    IEnumerator FindSteamVR()
    {
        while (leftController == null || rightController == null || head == null)
        {
            if (GameObject.Find("Controller (left)"))
                leftController = GameObject.Find("Controller (left)").transform;
            if (GameObject.Find("Controller (right)"))
                rightController = GameObject.Find("Controller (right)").transform;
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
            float armLength = Vector2.Distance(new Vector2(leftController.position.x, leftController.position.z), new Vector2(rightController.position.x, rightController.position.z));
            float xScale = armLength / startSizeX;
            Vector3 tempScale = Vector3.one;
            tempScale = new Vector3(tempScale.x * xScale, tempScale.y * yScale, tempScale.z);
            transform.localScale = tempScale;
            yield return null;
        }
    }
}
