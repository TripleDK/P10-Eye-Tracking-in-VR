using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealisticEyeMovements;

public class PreliminaryTestContext : MonoBehaviour
{
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
    [SerializeField] int rounds = 3;
    [SerializeField] AudioClip correctSound;
    [SerializeField] AudioClip failSound;
    [SerializeField] AudioClip winSound;

    List<GameObject> instantiatedObjects = new List<GameObject>();
    List<GameObject> currentRoundObjects = new List<GameObject>();
    Vector3 startScale;
    Vector3 startPos;
    Vector3 cameraRigStartPos;
    Vector3 objStartScale;

    ObjectInteractions currentObject;

    // Use this for initialization
    void Start()
    {
        cameraRigStartPos = cameraRig.position;
        cameraRig.position = new Vector3(300, 300, 300);
        startScale = transform.localScale;
        startPos = transform.position;
        objStartScale = shelfObjects[0].transform.localScale;
        pupilManager.SetActive(true);
        pupilManagerRecording.SetActive(true);
        PupilTools.OnCalibrationEnded += StartTask;
    }

    void StartTask()
    {
        cameraRig.position = cameraRigStartPos;
        window.SetActive(true);

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
        GuessObject(currentRoundObjects[Random.Range(0, currentRoundObjects.Count - 1)]);
    }

    void BallGrabbed(GameObject go)
    {
        Debug.Log(go.name);
        if (go.GetComponent<ObjectInteractions>() == currentObject)
        {
            Debug.Log("You did it!");
            AudioSource.PlayClipAtPoint(correctSound, go.transform.position);
            Shrink();
        }
        else
        {
            Debug.Log("Wrong :(");
            AudioSource.PlayClipAtPoint(failSound, go.transform.position);
            Expand();
        }
        currentRoundObjects.Remove(go);
        Debug.Log("Objects left: " + currentRoundObjects.Count);
        if (currentRoundObjects.Count == 0)
        {
            rounds--;
            if (rounds <= 0)
            {
                AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
                Debug.Log("Winner!");
            }
            else
            {
                Debug.Log("Next round!");
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

    }

    void Shrink()
    {
        transform.localScale /= shrinkExpandFactor;
        UpdateObjectPositions();
    }
    void Expand()
    {
        transform.localScale *= shrinkExpandFactor;
        UpdateObjectPositions();
    }

    void MoveBack()
    {
        transform.position -= new Vector3(0, 0, moveStep);
        cameraRig.position -= new Vector3(0, 0, moveStep);
        UpdateObjectPositions();
    }
    void MoveForward()
    {
        transform.position += new Vector3(0, 0, moveStep);
        cameraRig.position += new Vector3(0, 0, moveStep);
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
        UpdateObjectPositions();
    }

    void ResetAll()
    {
        transform.localScale = startScale;
        transform.position = startPos;
        cameraRig.position = cameraRigStartPos;
        UpdateObjectPositions();
    }

    void OnGUI()
    {
        GUI.color = Color.yellow;

        if (GUI.Button(new Rect(10, 40, 100, 30), "Shrink"))
        {
            Shrink();
        }
        if (GUI.Button(new Rect(10, 80, 100, 30), "Expand"))
        {
            Expand();
        }
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
    }
}


