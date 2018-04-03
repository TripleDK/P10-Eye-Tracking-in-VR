using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeDirectionPupil : MonoBehaviour
{
    public Transform calculatedLookAt;
    public float leftEyeDiameter, rightEyeDiameter;
    [SerializeField] float markerDistance;
    [SerializeField] Transform marker;

    Vector3 leftEyeDir, rightEyeDir;
    Camera camera;

    // Use this for initialization
    void Start()
    {
        PupilTools.OnConnected += StartPupilSubscription;
        PupilTools.OnDisconnecting += StopPupilSubscription;

        PupilTools.OnReceiveData += CustomReceiveData;
        camera = GetComponent<Camera>();
    }
    void StartPupilSubscription()
    {
        PupilTools.CalibrationMode = Calibration.Mode._3D;

        PupilTools.SubscribeTo("pupil.");
    }

    void StopPupilSubscription()
    {
        PupilTools.UnSubscribeFrom("pupil.");
    }
    void OnDisable()
    {
        PupilTools.OnConnected -= StartPupilSubscription;
        PupilTools.OnDisconnecting -= StopPupilSubscription;

        PupilTools.OnReceiveData -= CustomReceiveData;
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
                                    if (idValue == "0")
                                    {
                                        rightEyeDir = vector;

                                    }
                                    else
                                    {
                                        leftEyeDir = vector;

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
        Debug.DrawLine(transform.position, transform.position + rightEyeDir * markerDistance, Color.blue);
        rightEyeDir = Quaternion.Euler(transform.eulerAngles) * rightEyeDir;
        Debug.DrawLine(transform.position, transform.position + leftEyeDir * markerDistance, Color.red);
        leftEyeDir = Quaternion.Euler(transform.eulerAngles) * leftEyeDir;
        Vector3 averageDir = ((leftEyeDir + rightEyeDir) / 2).normalized;
        Debug.DrawLine(transform.position - (transform.right * camera.stereoSeparation / 2), transform.position - (transform.right * camera.stereoSeparation / 2) + leftEyeDir * markerDistance, Color.yellow);
        Debug.DrawLine(transform.position + (transform.right * camera.stereoSeparation / 2), transform.position + (transform.right * camera.stereoSeparation / 2) + rightEyeDir * markerDistance, Color.green);
        Debug.DrawLine(transform.position, transform.position + averageDir * markerDistance, Color.red);
        RaycastHit hit;
        Physics.Raycast(transform.position, averageDir, out hit, markerDistance);
        calculatedLookAt.position = hit.point;
        if (hit.collider == null) { calculatedLookAt.position = transform.position + averageDir * markerDistance; }

    }

}
