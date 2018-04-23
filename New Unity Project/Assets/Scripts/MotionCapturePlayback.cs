using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionCapturePlayback : MonoBehaviour
{

    [SerializeField] MotionRecording recording; //Make it a list 
    [SerializeField] List<Transform> playbackObjects;
    bool isPlaying = false;
    string buttonText = "Start playback!";

    // Use this for initialization
    void Start()
    {

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 250, 50), buttonText))
        {
            if (!isPlaying)
            {
                StartPlayback();
            }
            else if (isPlaying) //Make a pause function
            {
                EndPlayback();
            }
        }

    }

    void StartPlayback()
    {
        isPlaying = true;
        buttonText = "Stop playing...";
        if (recording == null)
        {
            Debug.LogWarning("No recording inserted!");
            EndPlayback();
            return;
        }
        StartCoroutine(Playback());
    }

    IEnumerator Playback() // Make a transition into another playback , use animation curve for weigths then average two positions
    {
        int index = 0;
        Debug.Log(recording.data.Count);
        while (index < recording.data[0].position.Count && isPlaying)
        {
            int j = 0;

            for (int i = 0; j < recording.transformNames.Count; i++, j++)
            {
                playbackObjects[j].position = recording.data[i].position[index];
                playbackObjects[j].eulerAngles = recording.data[i].rotation[index];
            }
            yield return new WaitForSeconds(1.0f / recording.fps);
            index++;
        }
        EndPlayback();
    }

    void EndPlayback()
    {
        buttonText = "Start playback!";
        isPlaying = false;
    }


}
