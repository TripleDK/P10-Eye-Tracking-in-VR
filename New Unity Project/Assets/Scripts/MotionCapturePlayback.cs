using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionCapturePlayback : MonoBehaviour
{

    public List<MotionRecording> recordings = new List<MotionRecording>();
    public int currentRecording = 0;
    [SerializeField] List<Transform> playbackObjects;
    [SerializeField] int transitionFrames = 48;
    [SerializeField] int loopTransitionFrames = 24;
    [SerializeField] AnimationCurve transition;
    [SerializeField] bool loopPlayBack = true;
    int nextRecording = 1;
    bool isPlaying = false;
    bool isPaused = false;
    string buttonText = "Start playback!";
    string pauseText = "Pause";
    int index = 0;
    bool transitioning = false;
    int transitionIndex = 0;


    void OnGUI()
    {
        /*    if (GUI.Button(new Rect(130, 150, 250, 50), buttonText))
            {
                if (!isPlaying)
                {
                    StartPlayback();
                }
                else if (isPlaying)
                {
                    EndPlayback();

                }
            }
            if (isPlaying)
            {
                if (GUI.Button(new Rect(130, 210, 250, 50), pauseText))
                {
                    if (!isPaused) PausePlayback();
                    else if (isPaused) ResumePlayback();
                }
                if (loopPlayBack)
                {
                    if (GUI.Button(new Rect(130, 270, 250, 50), "Next animation"))
                    {
                        StartRecording(currentRecording + 1);
                    }
                }
            }*/
    }
    public void StartRecording(int number)
    {
        //        Debug.Log("Starting recording " + number);
        if (isPlaying == false) StartPlayback();
        transitioning = true;
        transitionIndex = 0;
        nextRecording = number;
        if (nextRecording >= recordings.Count) nextRecording = 0;
        if (recordings[nextRecording] == null) nextRecording = 0;
    }


    public void StartPlayback()
    {
        isPlaying = true;
        buttonText = "Stop playing...";
        if (recordings.Count == 0 || recordings[0] == null)
        {
            Debug.LogWarning("No recording inserted!");
            EndPlayback();
            return;
        }
        if (loopPlayBack)
        {
            StartCoroutine(PlaybackLoop());
        }
        else
        {
            StartCoroutine(Playback());
        }
    }

    IEnumerator Playback() // Make a transition into another playback , use animation curve for weigths then average two positions
    {
        index = 0;
        for (int r = 0; r < recordings.Count; r++)
        {
            while (index < recordings[r].data[0].position.Count && isPlaying)
            {
                int j = 0;
                if (isPaused == false)
                {
                    //Transitioning -> if frames are within transition range, and is not the last recording
                    if (recordings[r].data[0].position.Count - index < transitionFrames && r < recordings.Count - 1)
                    {
                        float transitionWeight = transition.Evaluate(index / recordings[r].data[0].position.Count);
                        int transitionFrame = index - recordings[r].data[0].position.Count + transitionFrames;
                        for (int i = 0; j < recordings[r].transformNames.Count; i++, j++)
                        {
                            playbackObjects[j].position = recordings[r].data[i].position[index] * (1 - transitionWeight) + recordings[r + 1].data[i].position[transitionFrame] * transitionWeight;
                            playbackObjects[j].eulerAngles = recordings[r].data[i].rotation[index] * (1 - transitionWeight) + recordings[r + 1].data[i].rotation[transitionFrame] * transitionWeight; ;
                        }
                    }
                    //Normal playback
                    else
                    {
                        for (int i = 0; j < recordings[r].transformNames.Count; i++, j++)
                        {
                            playbackObjects[j].position = recordings[r].data[i].position[index];
                            playbackObjects[j].eulerAngles = recordings[r].data[i].rotation[index];
                        }
                    }

                    index++;
                }
                yield return new WaitForSeconds(1.0f / recordings[r].fps);
            }
            index = transitionFrames;
        }
        EndPlayback();
    }

    IEnumerator PlaybackLoop() // Make a transition into another playback , use animation curve for weigths then average two positions
    {
        index = 0;
        currentRecording = 0;
        while (isPlaying)
        {
            if (isPaused == false)
            {
                //Transitioning -> if frames are within transition range, and is not the last recording
                if (recordings[currentRecording].data[0].position.Count - index < loopTransitionFrames)
                {
                    int transitionFrame = index - recordings[currentRecording].data[0].position.Count + loopTransitionFrames;
                    float transitionWeight = transition.Evaluate((float)transitionFrame / (float)loopTransitionFrames);
                    //  Debug.Log("Transitioning: weight: " + transitionWeight + ", frame: " + transitionFrame);
                    int j = 0;
                    for (int i = 0; j < recordings[currentRecording].transformNames.Count; i++, j++)
                    {
                        playbackObjects[j].position = recordings[currentRecording].data[i].position[index] * (1 - transitionWeight)
                        + recordings[currentRecording].data[i].position[transitionFrame] * transitionWeight;
                        playbackObjects[j].eulerAngles = Quaternion.Lerp(Quaternion.Euler(recordings[currentRecording].data[i].rotation[index]),
                        Quaternion.Euler(recordings[currentRecording].data[i].rotation[transitionFrame]), transitionWeight).eulerAngles;
                    }
                }
                //Normal playback
                else
                {
                    int j = 0;
                    for (int i = 0; j < recordings[currentRecording].transformNames.Count; i++, j++)
                    {
                        playbackObjects[j].position = recordings[currentRecording].data[i].position[index];
                        playbackObjects[j].eulerAngles = recordings[currentRecording].data[i].rotation[index];
                    }
                }

                //Nextanimation transition
                if (transitioning == true)
                {
                    if (recordings[nextRecording] == null) StartRecording(nextRecording + 1);

                    float transitionWeight = transition.Evaluate((float)transitionIndex / (float)transitionFrames);



                    //                    Debug.Log("Transitioning: weight: " + transitionWeight + ", frame: " + transitionIndex);
                    int j = 0;
                    for (int i = 0; j < recordings[currentRecording].transformNames.Count; i++, j++)
                    {
                        playbackObjects[j].position = playbackObjects[j].position * (1 - transitionWeight)
                        + recordings[nextRecording].data[i].position[transitionIndex] * transitionWeight;
                        playbackObjects[j].eulerAngles = Quaternion.Lerp(Quaternion.Euler(playbackObjects[j].eulerAngles),
                        Quaternion.Euler(recordings[nextRecording].data[i].rotation[transitionIndex]), transitionWeight).eulerAngles;
                    }

                    transitionIndex++;
                    //Transition done
                    if (transitionIndex >= transitionFrames)
                    {
                        index = transitionFrames;
                        currentRecording = nextRecording;
                        if (currentRecording >= recordings.Count) currentRecording = 0;
                        transitioning = false;
                    }
                }

                //Next frame, check if loop is done
                index++;
                if (index >= recordings[currentRecording].data[0].position.Count)
                {
                    index = loopTransitionFrames;
                }
            }
            yield return new WaitForSeconds(1.0f / recordings[currentRecording].fps);
        }

        EndPlayback();
    }

    void PausePlayback()
    {
        isPaused = true;
        pauseText = "Resume";
    }

    void ResumePlayback()
    {
        isPaused = false;
        pauseText = "Pause";
    }

    void EndPlayback()
    {
        buttonText = "Start playback!";
        currentRecording = 0;
        isPlaying = false;
    }
}
