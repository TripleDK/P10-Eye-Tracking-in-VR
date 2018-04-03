using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupilDemo3D : MonoBehaviour
{
    void Start()
    {
        PupilTools.OnConnected += StartPupilSubscription;
        PupilTools.OnDisconnecting += StopPupilSubscription;

        PupilTools.OnReceiveData += CustomReceiveData;
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

    void CustomReceiveData(string topic, Dictionary<string, object> dictionary, byte[] thirdFrame = null)
    {
        if (topic.StartsWith("pupil"))
        {
            foreach (var item in dictionary)
            {
                switch (item.Key)
                {
                    case "topic":
                    case "method":
                    case "id":
                        var textForKey = PupilTools.StringFromDictionary(dictionary, item.Key);
                        // Do stuff
                        break;
                    case "confidence":
                    case "timestamp":
                    case "diameter":
                        var valueForKey = PupilTools.FloatFromDictionary(dictionary, item.Key);
                        Debug.Log("Diameter: " + valueForKey);
                        // Do stuff
                        break;
                    case "norm_pos":
                        var positionForKey = PupilTools.VectorFromDictionary(dictionary, item.Key);
                        // Do stuff
                        break;
                    case "ellipse":
                        var dictionaryForKey = PupilTools.DictionaryFromDictionary(dictionary, item.Key);
                        foreach (var pupilEllipse in dictionaryForKey)
                        {
                            switch (pupilEllipse.Key.ToString())
                            {
                                case "angle":
                                    var angle = (float)(double)pupilEllipse.Value;
                                    // Do stuff
                                    break;
                                case "center":
                                case "axes":
                                    var vector = PupilTools.ObjectToVector(pupilEllipse.Value);
                                    // Do stuff
                                    break;
                                default:
                                    break;
                            }
                        }
                        // Do stuff
                        break;
                    case "circle_3d":
                        dictionaryForKey = PupilTools.DictionaryFromDictionary(dictionary, item.Key);
                        foreach (var pupilEllipse in dictionaryForKey)
                        {
                            switch (pupilEllipse.Key.ToString())
                            {
                                case "radius":
                                    var angle = (float)(double)pupilEllipse.Value;
                                    // Do stuff
                                    Debug.Log("Circle radius: " + angle);
                                    break;
                                case "center":
                                case "normal":
                                    var vector = PupilTools.ObjectToVector(pupilEllipse.Value);
                                    // Do stuff
                                    Debug.Log("NormaL: " + vector);
                                    break;
                                default:
                                    break;
                            }
                        }
                        // Do stuff
                        Debug.Log("Circle exists!");
                        break;
                    case "circle_3d_radius":
                        valueForKey = PupilTools.FloatFromDictionary(dictionary, item.Key);
                        Debug.Log("Diameter3D: " + valueForKey);
                        break;
                    case "circle_3d_normal":
                        positionForKey = PupilTools.VectorFromDictionary(dictionary, item.Key);
                        Debug.Log("Eye normal: " + positionForKey);
                        break;
                    case "circle_3d_normal_x":
                        valueForKey = PupilTools.FloatFromDictionary(dictionary, item.Key);
                        Debug.Log("Eye normal x: " + valueForKey);
                        break;
                    case "model_confidence":
                        valueForKey = PupilTools.FloatFromDictionary(dictionary, item.Key);
                        Debug.Log("Confidence: " + valueForKey);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void OnDisable()
    {
        PupilTools.OnConnected -= StartPupilSubscription;
        PupilTools.OnDisconnecting -= StopPupilSubscription;

        PupilTools.OnReceiveData -= CustomReceiveData;
    }
}
