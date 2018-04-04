using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketWith3DCalibration : MonoBehaviour
{
    public Transform marker;
    public Transform closeMarker;
    public float markerDistance = 2.0f;
    public Transform eye3D;
    LineRenderer laserEyes;

    // Use this for initialization
    void Start()
    {
        laserEyes = GetComponent<LineRenderer>();
        PupilData.calculateMovingAverage = true;
    }

    void OnEnable()
    {
        if (PupilTools.IsConnected)
        {
            Debug.Log("Starting gaze in 3d!");
            PupilGazeTracker.Instance.StartVisualizingGaze();
        }
        else
        {
            StartCoroutine(KeepConnected());
        }
    }

    IEnumerator KeepConnected()
    {
        while (true)
        {
            if (PupilTools.IsConnected)
            {
                Debug.Log("Starting gaze late in 3d!");
                PupilGazeTracker.Instance.StartVisualizingGaze();
                yield break;
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PupilTools.IsConnected && PupilTools.IsGazing)
        {
            marker.localPosition = PupilData._3D.GazePosition;
            closeMarker.position = (PupilData._3D.GazePosition - transform.position).normalized * markerDistance;
            Debug.Log("World Space: " + PupilData._3D.GazePosition + ", LeftNorma: " + PupilData._3D.LeftGazeNormal + ", RightNormal: " + PupilData._3D.RightGazeNormal);
            Debug.Log("Left Eye Norm X" + PupilData._3D.LeftGazeNormal.x + "  " + "Left Eye Norm Y" + PupilData._3D.LeftGazeNormal.y
                    + "  " + "Left Eye Norm Z" + PupilData._3D.LeftGazeNormal.z + ", normalized: " + PupilData._3D.GazePosition.normalized);
            Vector3 leftEyeDir, rightEyeDir;
            leftEyeDir = (PupilData._3D.LeftGazeNormal).normalized;
            Debug.DrawLine(Vector3.zero, leftEyeDir * markerDistance, Color.black);
            leftEyeDir = Quaternion.Euler(transform.eulerAngles) * leftEyeDir;
            rightEyeDir = (PupilData._3D.RightGazeNormal).normalized;
            Debug.DrawLine(Vector3.zero, rightEyeDir * markerDistance, Color.white);
            rightEyeDir = Quaternion.Euler(transform.eulerAngles) * rightEyeDir;
            Debug.DrawLine(transform.position, transform.position + leftEyeDir * markerDistance, Color.yellow);
            Debug.DrawLine(transform.position, transform.position + rightEyeDir * markerDistance, Color.green);
            Debug.Log((transform.position + leftEyeDir));
            Debug.Log((transform.position + rightEyeDir));
            laserEyes.SetPosition(0, transform.position);
            laserEyes.SetPosition(1, transform.position + (leftEyeDir + rightEyeDir).normalized * markerDistance);
            Debug.DrawLine(transform.position, transform.position + PupilData._3D.LeftGazeNormal * markerDistance, Color.red);
            Debug.DrawLine(transform.position, transform.position + PupilData._3D.RightGazeNormal * markerDistance, Color.blue);
            //      Debug.DrawLine(marker.position, marker.position + PupilData._3D.GazePosition);
        }
    }
}
