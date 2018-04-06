using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeDirection : MonoBehaviour
{
    public Transform calculatedLookAt;
    [SerializeField] bool searchingForPupil = false;
    [SerializeField] float markerDistance;
    [SerializeField] Transform marker;
    Camera cam;
    Coroutine connecting;

    // Use this for initialization
    void Start()
    {
        PupilData.calculateMovingAverage = true;
        if (GetComponent<Camera>()) cam = GetComponent<Camera>(); else cam = transform.parent.GetComponent<Camera>();
    }


    void OnEnable()
    {
        if (PupilTools.IsConnected)
        {
            Debug.Log("Starting gaze in 3d!");
            PupilGazeTracker.Instance.StartVisualizingGaze();
            searchingForPupil = false;
        }
        else
        {
            Debug.Log("can't find Pupil labs!", this);
            connecting = StartCoroutine(KeepConnected());
        }
    }

    IEnumerator KeepConnected()
    {
        searchingForPupil = true;
        while (true)
        {
            if (PupilTools.IsConnected)
            {
                Debug.Log("Starting gaze late in 3d!");
                PupilGazeTracker.Instance.StartVisualizingGaze();
                searchingForPupil = false;
                connecting = null;
                this.enabled = true;
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
            //      Debug.Log(PupilData._3D.Circle.Radius(0));
            //    Debug.Log("World Space: " + PupilData._3D.GazePosition + ", LeftNorma: " + PupilData._3D.LeftGazeNormal + ", RightNormal: " + PupilData._3D.RightGazeNormal);
            //  Debug.Log("Left Eye Norm X " + PupilData._3D.LeftGazeNormal.x + "  " + "Left Eye Norm Y " + PupilData._3D.LeftGazeNormal.y
            //     + "  " + "Left Eye Norm Z " + PupilData._3D.LeftGazeNormal.z + ", normalized: " + PupilData._3D.GazePosition.normalized);
            Vector3 leftEyeDir, rightEyeDir;
            leftEyeDir = (PupilData._3D.LeftGazeNormal).normalized;
            leftEyeDir = Quaternion.Euler(transform.eulerAngles) * leftEyeDir;
            rightEyeDir = (PupilData._3D.RightGazeNormal).normalized;
            rightEyeDir = Quaternion.Euler(transform.eulerAngles) * rightEyeDir;
            Vector3 averageDir = ((leftEyeDir + rightEyeDir) / 2).normalized;
            Debug.DrawLine(transform.position - (transform.right * cam.stereoSeparation / 2), transform.position - (transform.right * cam.stereoSeparation / 2) + leftEyeDir * markerDistance, Color.yellow);
            Debug.DrawLine(transform.position + (transform.right * cam.stereoSeparation / 2), transform.position + (transform.right * cam.stereoSeparation / 2) + rightEyeDir * markerDistance, Color.green);
            Debug.DrawLine(transform.position, transform.position + averageDir * markerDistance, Color.red);
            RaycastHit hit;
            Physics.Raycast(transform.position, averageDir, out hit, markerDistance);
            calculatedLookAt.position = hit.point;
            if (hit.collider == null) { calculatedLookAt.position = transform.position + averageDir * markerDistance; }
            else if (hit.collider.gameObject.tag == "PlayerHead")
            {
                Debug.Log("I see you!");
                TaskContext.singleton.timeGazeAtFace += Time.deltaTime;
            }
            /*   laserEyes.SetPosition(0, transform.position);
               laserEyes.SetPosition(1, transform.position + (leftEyeDir + rightEyeDir).normalized * markerDistance);*/
            Debug.DrawLine(transform.position, transform.position + PupilData._3D.LeftGazeNormal * markerDistance, Color.red);
            Debug.DrawLine(transform.position, transform.position + PupilData._3D.RightGazeNormal * markerDistance, Color.blue);
            //      Debug.DrawLine(marker.position, marker.position + PupilData._3D.GazePosition);
        }
        else
        {
            if (connecting == null) connecting = StartCoroutine(KeepConnected());
        }
    }
}
