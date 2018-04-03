using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recording", menuName = "Custom/Recording", order = 1)]
public class MotionRecording : ScriptableObject
{

    [System.Serializable]
    public class MotionData
    {
        public List<Vector3> position = new List<Vector3>();
        public List<Vector3> rotation = new List<Vector3>();
    }
    public List<MotionData> data = new List<MotionData>();
    public List<string> transformNames;
    public int fps;
}
