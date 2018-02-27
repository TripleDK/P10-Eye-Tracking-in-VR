using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IKControlTest : MonoBehaviour
{
	[HideInInspector] 
	public bool isLocalPlayer = false;
	public Transform lookAtPos;
	public Transform rightHandPos;
	public Transform leftHandPos;
	Transform lookAtTarget, rightHandTarget, leftHandTarget;
	[SerializeField, Range(0, 1)] float eyeWeight;
	[SerializeField, Range(0, 1)] float headWeight;
	[SerializeField, Range(0, 1)] float bodyWeight;
	Animator anim;

	// Use this for initialization
	void Awake()
	{
		anim = GetComponent<Animator>();

		if (isLocalPlayer)
		{
			return;
		}
		Debug.Log("Setting IK stuff");
		rightHandTarget = GameObject.Find("Sphere").transform;
		leftHandTarget = GameObject.Find("Sphere (1)").transform;
		lookAtTarget = GameObject.Find("Sphere (2)").transform;
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
				lookAtPos.position = lookAtTarget.position;
			}

			anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
			anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
			anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);	
			anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
			anim.SetLookAtWeight(1, bodyWeight, headWeight, eyeWeight, 0.5f);

			anim.SetLookAtPosition(lookAtPos.position);
			anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos.position);
			anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandPos.rotation);
			anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos.position);
			anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPos.rotation);
		}
	}
}
