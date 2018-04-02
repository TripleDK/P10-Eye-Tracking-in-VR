using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Recording", menuName = "Custom/Recording", order = 1)]
public class MotionRecording : ScriptableObject {

	public List<List<Vector3>> data;
	public List<string> transformNames;
	public int fps;
}
