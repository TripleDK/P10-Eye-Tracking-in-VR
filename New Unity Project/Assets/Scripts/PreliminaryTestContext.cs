using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using RealisticEyeMovements;

public class PreliminaryTestContext : MonoBehaviour
{
    public float depth;
    public float objectDistance;
    public float maxDistanceObjectAndFixer;
    public float maxAngle;
    [SerializeField] GameObject pupilManager;
    [SerializeField] GameObject pupilManagerRecording;
    [SerializeField] GameObject window;
    [SerializeField] float shrinkExpandFactor = 1.1f;
    [SerializeField] float moveStep = 0.2f;
    [SerializeField]
    List<GameObject> shelfObjects = new List<GameObject>();
    [SerializeField]
    List<Transform> startPositions = new List<Transform>();
    [SerializeField] Transform cameraRig;
    [SerializeField] Transform fixerHead;
    [SerializeField] MotionCapturePlayback motionCapturePlayback;
    [SerializeField] int rounds = 3;
    [SerializeField] AudioClip correctSound;
    [SerializeField] AudioClip failSound;
    [SerializeField] AudioClip moveSound;
    [SerializeField] AudioClip winSound;

    public List<GameObject> instantiatedObjects = new List<GameObject>();
    List<GameObject> currentRoundObjects = new List<GameObject>();
    Vector3 startScale;
    Vector3 startPos;
    Vector3 cameraRigStartPos;
    Vector3 objStartScale;

    public int depthLevel = 0;
    public int scaleLevel = 0;

    ObjectInteractions currentObject;

    public const float MINIMUM_ANGLE = 7.2f;

    string data = "";

    // Use this for initialization
    void Start()
    {
        cameraRigStartPos = cameraRig.position;
        startScale = transform.localScale;
        startPos = transform.position;
        objStartScale = shelfObjects[0].transform.localScale;
        //Next two lines if eyetracker!
        // cameraRig.position = new Vector3(300, 300, 300);
        //   pupilManager.SetActive(true);
        pupilManagerRecording.SetActive(true);
        PupilTools.OnCalibrationEnded += StartTask;
    }

    void StartTask()
    {
        motionCapturePlayback.gameObject.SetActive(true);
        motionCapturePlayback.StartPlayback();
        cameraRig.position = cameraRigStartPos;
        window.GetComponent<AudioSource>().Play();
        window.GetComponent<Animator>().SetTrigger("Open");

        for (int i = 0; i < shelfObjects.Count; i++)
        {
            // int startIndex = Random.Range(0, startPositions.Count);
            int startIndex = i;
            instantiatedObjects.Add((GameObject)Instantiate(shelfObjects[i], startPositions[startIndex].position, startPositions[startIndex].rotation));
            instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<Rigidbody>().isKinematic = true;
            instantiatedObjects[instantiatedObjects.Count - 1].name = startPositions[startIndex].name;
            instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<ObjectInteractions>().OnBallGrabbed.AddListener(BallGrabbed);
            // startPositions.RemoveAt(startIndex);
            //   instantiatedObjects[instantiatedObjects.Count - 1].transform.parent = this.transform;
        }
        currentRoundObjects = new List<GameObject>(instantiatedObjects);
        StartCoroutine(StartDelay());
    }

    IEnumerator StartDelay()
    {
        motionCapturePlayback.StartRecording(0);
        yield return new WaitForSeconds(5);
        GuessObject(currentRoundObjects[Random.Range(0, currentRoundObjects.Count - 1)]);
    }

    void BallGrabbed(GameObject go)
    {
        Debug.Log(go.name);
        if (go.GetComponent<ObjectInteractions>() == currentObject)
        {
            Debug.Log("You did it!");
            data += "Correct: ";
            AudioSource.PlayClipAtPoint(correctSound, go.transform.position);
            Shrink();
        }
        else
        {
            Debug.Log("Wrong :(");
            data += "Wrong: ";
            AudioSource.PlayClipAtPoint(failSound, go.transform.position);
            Expand();
        }
        data += go.name + ", Depth: " + depth + ", Object distance: " + objectDistance + "\n";
        currentRoundObjects.Remove(currentObject.gameObject);
        Debug.Log("Objects left: " + currentRoundObjects.Count);
        if (currentRoundObjects.Count == 0)
        {
            rounds--;
            if (rounds <= 0)
            {
                AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
                Debug.Log("Winner!");
                motionCapturePlayback.StartRecording(0);
                File.WriteAllText("Assets/Resources/Logs/DistanceTest/" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt",
                   data);

            }
            else
            {
                Debug.Log("Next round!");
                AudioSource.PlayClipAtPoint(moveSound, Camera.main.transform.position);
                currentRoundObjects = new List<GameObject>(instantiatedObjects);
                MoveBack();
                ResetScale();
                GuessObject(currentRoundObjects[Random.Range(0, currentRoundObjects.Count - 1)]);
            }

        }
        else
        {
            GuessObject(currentRoundObjects[Random.Range(0, currentRoundObjects.Count - 1)]);
        }
    }

    void GuessObject(GameObject ob)
    {
        currentObject = ob.GetComponent<ObjectInteractions>();
        int recordingIndex = 0;
        if (motionCapturePlayback.recordings.FirstOrDefault(obj => obj.name == ob.name + " " + scaleLevel + " " + depthLevel) != null)
        {
            MotionRecording clipToPlay = motionCapturePlayback.recordings.Single(obj => obj.name == ob.name + " " + scaleLevel + " " + depthLevel);
            recordingIndex = motionCapturePlayback.recordings.IndexOf(clipToPlay);
        }

        motionCapturePlayback.StartRecording(recordingIndex);
        Debug.Log("Current object to find is: " + currentObject.name);

    }

    void Shrink()
    {
        transform.localScale /= shrinkExpandFactor;
        scaleLevel--;
        UpdateObjectPositions();
        CalculateData();
        if (maxAngle < MINIMUM_ANGLE)
        {
            Expand();
        }
    }
    void Expand()
    {
        transform.localScale *= shrinkExpandFactor;
        scaleLevel++;
        UpdateObjectPositions();
    }

    void MoveBack()
    {
        transform.position -= new Vector3(0, 0, moveStep);
        cameraRig.position -= new Vector3(0, 0, moveStep);
        AudioSource.PlayClipAtPoint(moveSound, transform.position);
        depthLevel--;
        UpdateObjectPositions();
    }
    void MoveForward()
    {
        transform.position += new Vector3(0, 0, moveStep);
        cameraRig.position += new Vector3(0, 0, moveStep);
        AudioSource.PlayClipAtPoint(moveSound, transform.position);
        depthLevel++;
        UpdateObjectPositions();
    }

    void UpdateObjectPositions()
    {
        for (int i = 0; i < instantiatedObjects.Count; i++)
        {
            instantiatedObjects[i].transform.position = startPositions[i].position; //This will not work if objects placement are randomized in Start
        }
    }

    void ResetScale()
    {
        transform.localScale = startScale;
        scaleLevel = 0;
        UpdateObjectPositions();
    }

    void ResetAll()
    {
        transform.localScale = startScale;
        transform.position = startPos;
        cameraRig.position = cameraRigStartPos;
        scaleLevel = 0;
        depthLevel = 0;
        UpdateObjectPositions();
    }

    void OnGUI()
    {
        GUI.color = Color.yellow;

        if (GUI.Button(new Rect(120, 80, 100, 30), "Start task!"))
        {
            StartTask();
        }
        GUI.Label(new Rect(120, 40, 100, 30), "Scale Level: " + scaleLevel);
        if (GUI.Button(new Rect(10, 40, 100, 30), "Shrink"))
        {
            Shrink();
        }
        if (GUI.Button(new Rect(10, 80, 100, 30), "Expand"))
        {
            Expand();
        }
        GUI.Label(new Rect(120, 120, 100, 30), "Depth Level: " + depthLevel);
        if (GUI.Button(new Rect(10, 120, 100, 30), "Move Forward"))
        {
            MoveForward();
        }
        if (GUI.Button(new Rect(10, 160, 100, 30), "Move Back"))
        {
            MoveBack();
        }
        if (GUI.Button(new Rect(10, 200, 100, 30), "Reset Scale"))
        {
            ResetScale();
        }
        if (GUI.Button(new Rect(10, 240, 100, 30), "Reset All"))
        {
            ResetAll();
        }
        CalculateData();
        if (maxAngle <= MINIMUM_ANGLE) GUI.color = Color.red;
        GUI.Label(new Rect(10, 290, 250, 90), "Depth: " + depth + "\nObject separation: " + objectDistance + "\nDistance fixer-corner:" + maxDistanceObjectAndFixer + "\nmax angle = " + maxAngle);
    }

    void CalculateData()
    {
        depth = (new Vector2(transform.position.x, transform.position.z) - new Vector2(fixerHead.transform.position.x, fixerHead.transform.position.z)).magnitude;
        objectDistance = (startPositions[0].position - startPositions[1].position).magnitude;
        maxDistanceObjectAndFixer = (fixerHead.transform.position - startPositions[0].position).magnitude;
        maxAngle = Mathf.Rad2Deg * Mathf.Atan((objectDistance / maxDistanceObjectAndFixer));
    }
}


