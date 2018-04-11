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
    List<GameObject> instantiatedObjects = new List<GameObject>();
    Material objectMat;
    Color originColor;

    // Use this for initialization
    void Start()
    {
        pupilManager.SetActive(true);
        PupilTools.OnCalibrationEnded += StartTask;
    }


    void StartTask()
    {
        foreach (GameObject go in shelfObjects)
        {
            int startIndex = Random.Range(0, startPositions.Count);
            instantiatedObjects.Add((GameObject)Instantiate(go, startPositions[startIndex].position, startPositions[startIndex].rotation));
            instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<Rigidbody>().isKinematic = true;
            startPositions.RemoveAt(startIndex);

        }
        StartCoroutine(AccuracyOffSet(instantiatedObjects[0]));
    }

    IEnumerator AccuracyOffSet(GameObject targetObject)
    {
        //highlight object
        objectMat = targetObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
        originColor = objectMat.color;
        objectMat.SetColor("_Color", new Color(1, 1, 1));

        //wait for object fixation
        while (true)
        {
            if (Input.GetKeyUp("joystick button 14") || Input.GetKeyUp("joystick button 15"))
            {
                break;
            }
            yield return null;
        }

        //log results
        Vector3 objToHeadPos = targetObject.transform.position - gazeDirection.transform.position;
        Vector3 trackedLookAtPos = gazeDirection.calculatedLookAt.position - gazeDirection.transform.position;
        float positionAngle = Vector3.Angle(objToHeadPos, trackedLookAtPos);
        angleOffSets.Add(positionAngle);
        //        Debug.Log("Count before: " + instantiatedObjects.Count);
        instantiatedObjects.RemoveAt(0);
        yield return null;
        //      Debug.Log("Count before: " + instantiatedObjects.Count);
        objectMat.SetColor("_Color", originColor);
        if (instantiatedObjects.Count == 0)
        {
            string data = "";
            foreach (float f in angleOffSets)
            {
                data += f.ToString("0.00") + "\n";
            }
            File.WriteAllText("Assets/Resources/Logs/AccuracyTest/" + System.DateTime.Now.ToString("hh-mm-ss tt") + ".txt",
                data + "\n" + "Average: " + angleOffSets.Average().ToString("0.000"));
        }
        else
        {
            objectMat.SetColor("_Color", originColor);
            StartCoroutine(AccuracyOffSet(instantiatedObjects[0]));
        }
    }
}
