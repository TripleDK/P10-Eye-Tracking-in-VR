using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControlTest : MonoBehaviour
{

	[SerializeField] Transform lookAtPos;
	[SerializeField] Transform rightHandPos;
	[SerializeField] Transform leftHandPos;
	Animator anim;

	// Use this for initialization
	void Awake()
	{
		anim = GetComponent<Animator>();
	
	}
	
	// Update is called once per frame
	void OnAnimatorIK()
	{
		if (anim)
		{
			anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
			anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
			anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);	
			anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
			anim.SetLookAtWeight(1);

			anim.SetLookAtPosition(lookAtPos.position);
			anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos.position);
			anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandPos.rotation);
			anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos.position);
			anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPos.rotation);
		}
	}
}
