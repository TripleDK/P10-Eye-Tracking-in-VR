using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MotionCaptureRecorder : MonoBehaviour {

	[SerializeField] int fps = 24;
	[SerializeField] bool allowInfiniteRecording = false;
	[SerializeField] int recordingTime = 60;
	[SerializeField] List<Transform> recordingObjects = new List<Transform> ();

	string buttonText = "Start recording!";
	List<List<Vector3>> recordings = new List<List<Vector3>> ();
	bool isRecording = false;
	MotionRecording currentRecording = null;

	// Use this for initialization
	void Start () {
		
	}

	#if UNITY_EDITOR
	void OnGUI () {
		if (GUI.Button (new Rect (10, 10, 250, 50), buttonText)) {
			if (!isRecording) {
				StartRecording ();
			} else if (isRecording) {
				EndRecording ();
			}
		}

	}

	void StartRecording () {
		buttonText = "Recording!!";
		isRecording = true;
	
		recordings.Clear ();
		foreach (Transform ob in recordingObjects) {
			recordings.Add (new List<Vector3> ());//Position
			recordings.Add (new List<Vector3> ());//Rotation
		}
		foreach (List<Vector3> list in recordings) {
			list.Capacity = recordingTime * fps;
			Debug.Log ("List!");
		}

		StartCoroutine (Recording ());	
	}

	IEnumerator Recording () {
		float startTime = Time.time;
		int index = 0;

		//	while (Time.time - startTime < recordingTime && isRecording) {
		while (index <= recordingTime * fps && isRecording) {
			int j = 0;
			for (int i = 0; j < recordingObjects.Count; i += 2,j++) {
				//	Debug.Log ("i: " + i + ", j: " + j + ", index: " + index);
				recordings [i].Add (recordingObjects [j].position);
				recordings [i + 1].Add (recordingObjects [j].eulerAngles);
			}
			yield return new WaitForSeconds (1.0f / fps);
			index++;
		}
	}


	void EndRecording () {
		buttonText = "Start recording!";
		isRecording = false;
		MotionRecording recording = ScriptableObject.CreateInstance<MotionRecording> ();


		AssetDatabase.CreateAsset (recording, "Assets/Resources/Recordings/MotionCapture - " + Time.time + ".asset");
	}
	#endif
}
