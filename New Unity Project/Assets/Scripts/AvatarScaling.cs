using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarScaling : NetworkBehaviour
{

    public static int gender = 0; //0 = Male, 1 = Female
    private Camera mainCamera;
    private Transform leftController, rightController;
    [SerializeField] IKControlTest ikControls;
    [SerializeField] private SkinnedMeshRenderer maleMesh;
    [SerializeField] private SkinnedMeshRenderer femaleMesh;
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
        }
        if (gender == 1)
        {
            playerMesh = femaleMesh;
        }
        leftController = GameObject.Find("Controller (left)").transform;
        rightController = GameObject.Find("Controller (right)").transform;
        float headHeight = mainCamera.transform.localPosition.y;
        float yScale = headHeight / playerMesh.bounds.size.y;
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
        if (isLocalPlayer)
        {
            Resize();
            transform.GetChild(0).GetComponent<IKControlTest>().isLocalPlayer = true;
        }
    }
}
