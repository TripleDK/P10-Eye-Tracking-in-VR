using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class BlendShapeAnimator : MonoBehaviour
{

    SkinnedMeshRenderer meshRenderer;
    [SerializeField] float transitionSpeed = 100f;
    int numOfBlendShapes;
    int blendShapeIndex = 0;

    void Awake()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        numOfBlendShapes = meshRenderer.sharedMesh.blendShapeCount;
        if (numOfBlendShapes == 0)
        {
            Destroy(this);
            return;
        }
        meshRenderer.SetBlendShapeWeight(0, 100);
    }

    void Update()
    {
        float currentWeight = meshRenderer.GetBlendShapeWeight(blendShapeIndex) - transitionSpeed * Time.deltaTime;

        if (currentWeight <= 0)
        {
            meshRenderer.SetBlendShapeWeight(blendShapeIndex, 0);
            if (blendShapeIndex == numOfBlendShapes - 1)
            {
                meshRenderer.SetBlendShapeWeight(0, 100);
            }
            else
            {
                meshRenderer.SetBlendShapeWeight(blendShapeIndex + 1, 100);
            }
            blendShapeIndex++;
            if (blendShapeIndex >= numOfBlendShapes)
                blendShapeIndex = 0;
        }
        else
        {
            if (blendShapeIndex == numOfBlendShapes - 1)
            {
                meshRenderer.SetBlendShapeWeight(blendShapeIndex, currentWeight);
                meshRenderer.SetBlendShapeWeight(0, 100 - currentWeight);
            }
            else
            {
                meshRenderer.SetBlendShapeWeight(blendShapeIndex, currentWeight);
                meshRenderer.SetBlendShapeWeight(blendShapeIndex + 1, 100 - currentWeight);
            }
        }
    }
}
