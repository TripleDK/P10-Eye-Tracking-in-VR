using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class SinglePlayerTask : MonoBehaviour
{

    [SerializeField]
    List<GameObject> shelfObjects = new List<GameObject>();
    [SerializeField]
    List<Transform> startPositions = new List<Transform>();
    [SerializeField]
    GazeDirection gazeDirection;
    [SerializeField] GameObject pupilManager;
    List<float> angleOffSets = new List<float>();
    List<string> objectNames = new List<string>();
    List<GameObject> instantiatedObjects = new List<GameObject>();
    List<GameObject> currrentRoundObjects = new List<GameObject>();
    Material objectMat;
    Color originColor;
    [SerializeField]
    int listRounds = 3;
    [SerializeField]
    AudioClip prelimWinSound;
    [SerializeField]
    AudioClip objectLogged;
    [SerializeField] Transform cameraRig;
    Vector3 cameraRigStartPos;
    [SerializeField] Color highLightColor = Color.black;

    // Use this for initialization
    void Start()
    {
        cameraRigStartPos = cameraRig.position;
        cameraRig.position = new Vector3(300, 300, 300);
        pupilManager.SetActive(true);
        PupilTools.OnCalibrationEnded += StartTask;
    }


    void StartTask()
    {
        cameraRig.position = cameraRigStartPos;
        foreach (GameObject go in shelfObjects)
        {
            int startIndex = Random.Range(0, startPositions.Count);
            instantiatedObjects.Add((GameObject)Instantiate(go, startPositions[startIndex].position, startPositions[startIndex].rotation));
            instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<Rigidbody>().isKinematic = true;
            instantiatedObjects[instantiatedObjects.Count - 1].name = startPositions[startIndex].name;
            startPositions.RemoveAt(startIndex);

        }
        currrentRoundObjects = new List<GameObject>(instantiatedObjects);
        StartCoroutine(AccuracyOffSet(currrentRoundObjects[Random.Range(0, currrentRoundObjects.Count - 1)]));
    }

    IEnumerator AccuracyOffSet(GameObject targetObject)
    {
        //highlight object
        objectMat = targetObject.transform.GetComponent<Renderer>().material;
        originColor = objectMat.color;
        objectMat.SetColor("_Color", highLightColor);

        //wait for object fixation
        while (true)
        {
            //press trigger buttons 
            if (Input.GetKeyUp("joystick button 14") || Input.GetKeyUp("joystick button 15"))
            {
                AudioSource.PlayClipAtPoint(objectLogged, Camera.main.transform.position);
                break;
            }
            yield return null;
        }

        //log results
        Vector3 objToHeadPos = targetObject.transform.position - gazeDirection.transform.position;
        Vector3 trackedLookAtPos = gazeDirection.calculatedLookAt.position - gazeDirection.transform.position;
        float positionAngle = Vector3.Angle(objToHeadPos, trackedLookAtPos);
        angleOffSets.Add(positionAngle);
        objectNames.Add(targetObject.name);
        Debug.Log(positionAngle);
        //        Debug.Log("Count before: " + instantiatedObjects.Count);
        currrentRoundObjects.Remove(targetObject);
        yield return null;
        //      Debug.Log("Count before: " + instantiatedObjects.Count);
        objectMat.SetColor("_Color", originColor);
        if (currrentRoundObjects.Count == 0)
        {
            listRounds -= 1;
            if (listRounds <= 0)
            {
                string data = "";
                //foreach (float f in angleOffSets)
                for (int i = 0; i < angleOffSets.Count; i++)
                {
                    data += angleOffSets[i].ToString("0.00") + " - " + objectNames[i] + "\n";
                }
                File.WriteAllText("Assets/Resources/Logs/AccuracyTest/" + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt",
                    data + "\n" + "Average: " + angleOffSets.Average().ToString("0.000"));
                if (File.Exists("Assets/Resources/Logs/AccuracyTest/Averages.txt"))
                {
                    File.AppendAllText("Assets/Resources/Logs/AccuracyTest/Averages.txt", angleOffSets.Average().ToString("0.000") + "\n");
                }
                else
                {
                    File.WriteAllText("Assets/Resources/Logs/AccuracyTest/Averages.txt", "Experiment averages: \n" + angleOffSets.Average().ToString("0.000") + "\n");
                }
                AudioSource.PlayClipAtPoint(prelimWinSound, Camera.main.transform.position);
            }
            else
            {
                currrentRoundObjects = new List<GameObject>(instantiatedObjects);
                objectMat.SetColor("_Color", originColor);
                StartCoroutine(AccuracyOffSet(currrentRoundObjects[Random.Range(0, currrentRoundObjects.Count - 1)]));
            }
        }
        else
        {
            objectMat.SetColor("_Color", originColor);
            StartCoroutine(AccuracyOffSet(currrentRoundObjects[Random.Range(0, currrentRoundObjects.Count - 1)]));
        }
    }
}
