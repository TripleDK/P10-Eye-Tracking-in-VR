using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IKControlTest : MonoBehaviour
{
    [HideInInspector]
    public bool isLocalPlayer = false;
    public Transform lookAtPos;
    public Transform headPos;
    public Transform rightHandPos;
    public Transform leftHandPos;
    public Transform head, hip;
    public float height, width;
    Transform lookAtTarget, headTarget, rightHandTarget, leftHandTarget;
    public Transform leftEye;
    [SerializeField] Transform rightEye;
    [SerializeField, Range(0, 1)] float eyeWeight;
    [SerializeField, Range(0, 1)] float headWeight;
    [SerializeField, Range(0, 1)] float bodyWeight;
    [SerializeField] float bodyRotateSpeed = 5f;
    Animator anim;
    Vector3 eyeOffset, eyeOffsetLocal;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void LocalIKSetup()
    {
        Debug.Log("Setting IK stuff");

        isLocalPlayer = true; ;

        rightHandTarget = GameObject.Find("Controller (right)").transform;
        leftHandTarget = GameObject.Find("Controller (left)").transform;
        headTarget = GameObject.Find("Camera (eye)").transform;
        lookAtTarget = GameObject.Find("Sphere (2)").transform;

        //Debug.Break();
        eyeOffset = ((leftEye.position - hip.position) + (rightEye.position - hip.position)) / 2;
        eyeOffsetLocal = transform.InverseTransformVector(eyeOffset);
        Debug.DrawLine(new Vector3(hip.position.x, leftEye.position.y, hip.position.z),
        new Vector3(hip.position.x + eyeOffset.x, leftEye.position.y, hip.position.z + eyeOffset.z), Color.green);
    }

    void OnAnimatorIK()
    {
        if (anim)
        {
            if (isLocalPlayer)
            {
                rightHandPos.position = rightHandTarget.position;
                rightHandPos.rotation = rightHandTarget.rotation;
                leftHandPos.position = leftHandTarget.position;
                leftHandPos.rotation = leftHandTarget.rotation;
                headPos.position = headTarget.position;
                headPos.rotation = headTarget.rotation;
                lookAtPos.position = lookAtTarget.position;
                anim.bodyPosition = new Vector3(headTarget.position.x - eyeOffset.x, anim.bodyPosition.y, headTarget.position.z - eyeOffset.z);
                eyeOffset = transform.TransformVector(eyeOffsetLocal);
                anim.SetBoneLocalRotation(HumanBodyBones.Head, Quaternion.Inverse(anim.bodyRotation) * headTarget.rotation);

                anim.SetBoneLocalRotation(HumanBodyBones.Hips, Quaternion.Euler(hip.rotation.eulerAngles.x, headTarget.rotation.eulerAngles.y, hip.rotation.eulerAngles.z));
            }

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            anim.SetLookAtWeight(1, bodyWeight, 0, eyeWeight, 0.5f);

            anim.SetLookAtPosition(lookAtPos.position);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandPos.rotation);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPos.rotation);


            //head.position = headPos.position;
            //head.rotation = headPos.rotation;
        }
    }
}
