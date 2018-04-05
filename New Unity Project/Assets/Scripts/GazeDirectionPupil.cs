using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeDirectionPupil : MonoBehaviour
{
    public Transform calculatedLookAt;
    public float leftEyeDiameter, rightEyeDiameter;
    [SerializeField] float markerDistance;
    [SerializeField] Transform marker;

    Vector3 leftEyeDirRaw, rightEyeDirRaw;
    Vector3 leftEyeDirRotated, rightEyeDirRotated;
    Camera cam;

    // Use this for initialization
    void Start()
    {
        PupilTools.OnConnected += StartPupilSubscription;
        //   PupilTools.OnCalibrationEnded += CalibrationFinish;
        PupilTools.OnDisconnecting += StopPupilSubscription;
        PupilTools.OnReceiveData += CustomReceiveData;

        cam = transform.parent.GetComponent<Camera>();
    }
    void StartPupilSubscription()
    {
        Debug.Log("Starting pupil subscription!!");
        PupilTools.CalibrationMode = Calibration.Mode._3D;


        PupilTools.SubscribeTo("pupil.");
    }

    public void SubscribeToData()
    {
        PupilTools.OnReceiveData += CustomReceiveData;
    }

    void CalibrationFinish()
    {
        Debug.Log("Calibration has finished!");
        PupilTools.OnReceiveData += CustomReceiveData;
    }

    void StopPupilSubscription()
    {
        Debug.Log("Stopping Pupil subscription :(");
        PupilTools.UnSubscribeFrom("pupil.");
    }


    void CustomReceiveData(string topic, Dictionary<string, object> dictionary, byte[] thirdFrame = null)
    {
        if (topic.StartsWith("pupil"))
        {
            var idValue = PupilTools.StringFromDictionary(dictionary, "id");
            Debug.Log("id: " + idValue);
            foreach (var item in dictionary)
            {
                switch (item.Key)
                {

                    case "diameter_3d":
                        var valueForKey = PupilTools.FloatFromDictionary(dictionary, item.Key);
                        if (idValue == "0")
                        {
                            rightEyeDiameter = valueForKey;
                        }
                        else
                        {
                            leftEyeDiameter = valueForKey;
                        }

                        break;
                    case "circle_3d":
                        var dictionaryForKey = PupilTools.DictionaryFromDictionary(dictionary, item.Key);
                        foreach (var pupilEllipse in dictionaryForKey)
                        {
                            switch (pupilEllipse.Key.ToString())
                            {
                                case "radius":
                                    var radius = (float)(double)pupilEllipse.Value;
                                    break;
                                case "center":
                                case "normal":
                                    var vector = PupilTools.ObjectToVector(pupilEllipse.Value);
                                    Debug.Log(vector);
                                    if (idValue == "0")
                                    {
                                        rightEyeDirRaw = vector;
                                        rightEyeDirRotated = Quaternion.Euler(cam.transform.eulerAngles) * rightEyeDirRaw;
                                    }
                                    else
                                    {
                                        leftEyeDirRaw = vector;
                                        leftEyeDirRotated = Quaternion.Euler(cam.transform.eulerAngles) * leftEyeDirRaw;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

    }

    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + rightEyeDirRaw * markerDistance, Color.blue);

        Debug.DrawLine(transform.position, transform.position + leftEyeDirRaw * markerDistance, Color.magenta);

        Vector3 averageDir = ((leftEyeDirRotated + rightEyeDirRotated) / 2).normalized;
        Debug.DrawLine(transform.position - (transform.right * cam.stereoSeparation / 2), transform.position - (transform.right * cam.stereoSeparation / 2)
        + leftEyeDirRotated * markerDistance, Color.yellow);
        Debug.DrawLine(transform.position + (transform.right * cam.stereoSeparation / 2), transform.position + (transform.right * cam.stereoSeparation / 2)
        + rightEyeDirRotated * markerDistance, Color.green);
        Debug.DrawLine(transform.position, transform.position + averageDir * markerDistance, Color.red);
        RaycastHit hit;
        Physics.Raycast(transform.position, averageDir, out hit, markerDistance);
        calculatedLookAt.position = hit.point;
        if (hit.collider == null) { calculatedLookAt.position = transform.position + averageDir * markerDistance; }

    }

}
