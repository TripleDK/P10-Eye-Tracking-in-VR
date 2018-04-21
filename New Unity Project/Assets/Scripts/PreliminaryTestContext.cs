using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealisticEyeMovements;

public class PreliminaryTestContext : MonoBehaviour
{
    [SerializeField] GameObject pupilManager;
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

    ObjectInteractions currentObject;

    // Use this for initialization
    void Start()
    {
        cameraRigStartPos = cameraRig.position;
        cameraRig.position = new Vector3(300, 300, 300);
        startScale = transform.localScale;
        startPos = transform.position;
        pupilManager.SetActive(true);
        PupilTools.OnCalibrationEnded += StartTask;
    }

    void StartTask()
    {
        cameraRig.position = cameraRigStartPos;
        window.SetActive(true);

        foreach (GameObject go in shelfObjects)
        {
            int startIndex = Random.Range(0, startPositions.Count);
            instantiatedObjects.Add((GameObject)Instantiate(go, startPositions[startIndex].position, startPositions[startIndex].rotation));
            instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<Rigidbody>().isKinematic = true;
            instantiatedObjects[instantiatedObjects.Count - 1].name = startPositions[startIndex].name;
            startPositions.RemoveAt(startIndex);
            instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<ObjectInteractions>().OnBallGrabbed.AddListener(BallGrabbed);

        }
        currentRoundObjects = new List<GameObject>(instantiatedObjects);
        GuessObject(currentRoundObjects[Random.Range(0, currentRoundObjects.Count - 1)]);
    }

    void GuessObject(GameObject ob)
    {
        currentObject = ob.GetComponent<ObjectInteractions>();

    }

    void BallGrabbed(GameObject go)
    {
        Debug.Log(go.name);
        if (go.GetComponent<ObjectInteractions>() == currentObject)
        {
            Debug.Log("You did it!");
            AudioSource.PlayClipAtPoint(correctSound, go.transform.position);
        }
        else
        {
            Debug.Log("Wrong :(");
            AudioSource.PlayClipAtPoint(failSound, go.transform.position);
        }
        currentRoundObjects.Remove(go);
        if (currentRoundObjects.Count == 0)
        {
            AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
            Debug.Log("Winner!");

        }
        else
        {
            GuessObject(currentRoundObjects[Random.Range(0, currentRoundObjects.Count - 1)]);
        }
    }

    void Shrink()
    {
        transform.localScale /= shrinkExpandFactor;
    }
    void Expand()
    {
        transform.localScale *= shrinkExpandFactor;
    }

    void MoveBack()
    {
        transform.position -= new Vector3(0, 0, moveStep);
        cameraRig.position -= new Vector3(0, 0, moveStep);
    }
    void MoveForward()
    {
        transform.position += new Vector3(0, 0, moveStep);
        cameraRig.position += new Vector3(0, 0, moveStep);
    }

    void Reset()
    {
        transform.localScale = startScale;
        transform.position = startPos;
        cameraRig.position = cameraRigStartPos;
    }

}
