using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarScaling : NetworkBehaviour
{

    public static int gender = 0; //0 = Male, 1 = Female
    [SerializeField] IKControlTest ikControls;
    [SerializeField] private SkinnedMeshRenderer maleMesh;
    [SerializeField] private GameObject maleHair;
    [SerializeField] private SkinnedMeshRenderer femaleMesh;
    [SerializeField] private GameObject femaleHair;
    [SerializeField] private Material seeThroughMaterial;
    private Camera mainCamera;
    private Transform leftController, rightController;
    private SkinnedMeshRenderer playerMesh;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Resize()
    {
        if (gender == 0)
        {
            playerMesh = maleMesh;
            maleHair.SetActive(false);
        }
        if (gender == 1)
        {
            femaleHair.SetActive(false);
            playerMesh = femaleMesh;
        }

        Debug.Log("Change material!");
        playerMesh.materials = new Material[3] { seeThroughMaterial, playerMesh.materials[1], seeThroughMaterial };
        leftController = GameObject.Find("Controller (left)").transform;
        rightController = GameObject.Find("Controller (right)").transform;
        float headHeight = mainCamera.transform.localPosition.y;
        float yScale = headHeight / ikControls.leftEye.position.y;
        float armLength = Vector2.Distance(new Vector2(leftController.position.x, leftController.position.z), new Vector2(rightController.position.x, rightController.position.z));
        float xScale = armLength / playerMesh.bounds.size.x;
        Vector3 tempScale = transform.localScale;
        tempScale = new Vector3(tempScale.x * xScale, tempScale.y * yScale, tempScale.z);
        transform.localScale = tempScale;
        transform.position = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
        transform.eulerAngles = new Vector3(0, mainCamera.transform.rotation.y, 0);
        ikControls.height = headHeight;
        ikControls.width = armLength;
    }


    void Start()
    {
        if (CalibrationContext.singleton != null && CalibrationContext.singleton.calibrationProgress == 0)
        {
            if (isLocalPlayer) gameObject.tag = "LocalPlayer";
            //  gameObject.SetActive(false);
            return;
        }
        if (isLocalPlayer && CalibrationContext.singleton.style == 0)
        {
            gameObject.tag = "LocalPlayer";
            Resize();
            ikControls.LocalIKSetup();
        }
    }
}
