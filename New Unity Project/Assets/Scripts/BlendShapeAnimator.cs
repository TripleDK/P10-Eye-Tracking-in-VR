using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeAnimator : MonoBehaviour
{

	[SerializeField] SkinnedMeshRenderer meshRenderer;
	[SerializeField] float transitionSpeed = 100f;
	int numOfBlendShapes;
	int blendShapeIndex = 1;

	void Awake()
	{
		numOfBlendShapes = meshRenderer.sharedMesh.blendShapeCount;
		meshRenderer.SetBlendShapeWeight(0, 100);
	}

	void Update()
	{
		float currentWeight = meshRenderer.GetBlendShapeWeight(blendShapeIndex - 1) - transitionSpeed * Time.deltaTime;
		if (currentWeight <= 0)
		{
			meshRenderer.SetBlendShapeWeight(blendShapeIndex - 1, 0);
			meshRenderer.SetBlendShapeWeight(blendShapeIndex, 100);
			blendShapeIndex++;
			if (blendShapeIndex > numOfBlendShapes)
				blendShapeIndex = 0;
		}
		else
		{
			meshRenderer.SetBlendShapeWeight(blendShapeIndex - 1, currentWeight);
			meshRenderer.SetBlendShapeWeight(blendShapeIndex, 100 - currentWeight);
		}
	}
}
