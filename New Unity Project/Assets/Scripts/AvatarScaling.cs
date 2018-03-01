using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarScaling : MonoBehaviour
{


    private Camera mainCamera;
    [SerializeField] private MeshRenderer playerMesh;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Resize()
    {
        float headHeight = mainCamera.transform.localPosition.y;
        float scale = headHeight / playerMesh.bounds.size.y;
        transform.localScale *= scale;
    }

    void onEnable()
    {
        Resize();
    }
}
