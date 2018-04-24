using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class RecordPrelim2 : MonoBehaviour
{

    [SerializeField] MotionCaptureRecorder recorder;
    public PreliminaryTestContext testContext;
    int objectIndex = -1;

    // Use this for initialization
    void Start()
    {
        NextObject();
    }

    public void NextObject()
    {
        if (objectIndex >= 0) testContext.instantiatedObjects[objectIndex].GetComponent<ObjectInteractions>().FeedbackColor(Color.yellow, 1.1f);
        objectIndex++;
        if (objectIndex >= testContext.instantiatedObjects.Count) objectIndex = 0;
        testContext.instantiatedObjects[objectIndex].GetComponent<ObjectInteractions>().FeedbackColor(Color.red, 1.3f);

    }

    public void SaveTestMotion(MotionRecording recording)
    {
        AssetDatabase.CreateAsset(recording, "Assets/Resources/Recordings/" + testContext.instantiatedObjects[objectIndex].name + " " + testContext.scaleLevel + " " + testContext.depthLevel + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        NextObject();
    }
}
#endif