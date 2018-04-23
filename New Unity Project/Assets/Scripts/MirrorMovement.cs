using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MirrorMovement : MonoBehaviour
{

    [SerializeField] List<Transform> transformsToMirror = new List<Transform>();
    List<Transform> movingTransforms = new List<Transform>();

    // Use this for initialization
    void Start()
    {
        List<Transform> myTransforms = gameObject.GetComponentsInChildren<Transform>().ToList();
        foreach (Transform tran in transformsToMirror)
        {
            movingTransforms.Add(myTransforms.Where(obj => obj.name == tran.name).SingleOrDefault());
        }
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transformsToMirror.Count; i++)
        {
            movingTransforms[i].localPosition = transformsToMirror[i].localPosition;
            movingTransforms[i].localRotation = transformsToMirror[i].localRotation;
        }
    }
}
