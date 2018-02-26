using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControlTest : MonoBehaviour
{

	[SerializeField] Transform lookAtPos;
	[SerializeField] Transform rightHandPos;
	[SerializeField] Transform leftHandPos;
	[SerializeField, Range(0, 1)] float eyeWeight;
	[SerializeField, Range(0, 1)] float headWeight;
	[SerializeField, Range(0, 1)] float bodyWeight;
	Animator anim;

	// Use this for initialization
	void Awake()
	{
		anim = GetComponent<Animator>();
	
	}

	void OnAnimatorIK()
	{
		if (anim)
		{
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
