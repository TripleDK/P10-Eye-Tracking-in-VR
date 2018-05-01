using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FixerTutorialContext : MonoBehaviour
{
    [SerializeField] TextMeshPro textField;
    [SerializeField] GazeDirection gaze;
    [SerializeField] ObjectInteractions testObject;
    [SerializeField] Transform testObjectStartPos;
    [SerializeField] Transform testObjectPreviewPos;
    [SerializeField] TextMeshPro previewText;
    [SerializeField] BlackHole blackHole;
    [SerializeField] List<AudioClip> dialogue = new List<AudioClip>();
    ObjectInteractions spawnedTestObject;
    ObjectInteractions spawnedTestObjectPreview;

    void Awake()
    {
        textField.text = "";
    }

    public void StartTutorial()
    {
        previewText.text = "";
        textField.text = "Welcome: -Fixer- \n\nOverall goal: \nClose the black hole \n\nImportant tool: \nCommuncating";
        AudioSource.PlayClipAtPoint(dialogue[0], textField.transform.position);
        StartCoroutine(LookAtMonitor());
    }

    IEnumerator LookAtMonitor()
    {
        yield return new WaitForSeconds(dialogue[0].length + 1);
        AudioSource.PlayClipAtPoint(dialogue[1], textField.transform.position);
        textField.text = "Current goal: \nLocate monitor \n\nImportant tool: \nSpatial awareness";
        gaze.OnTVRealized.AddListener(GrabAnObject);
    }

    public void GrabAnObject()
    {
        gaze.OnTVRealized.RemoveListener(GrabAnObject);
        spawnedTestObjectPreview = Instantiate(testObject, testObjectPreviewPos.position, testObjectPreviewPos.rotation);
        spawnedTestObjectPreview.GetComponent<Rigidbody>().isKinematic = true;
        previewText.text = "Test object";
        AudioSource.PlayClipAtPoint(dialogue[2], textField.transform.position);
        textField.text = "Current goal: \nLocate and pick up test object \n\nImportant tool: \nGaze movement";
        spawnedTestObject = Instantiate(testObject, testObjectStartPos.position, testObjectStartPos.rotation);
        spawnedTestObject.OnBallGrabbed.AddListener(ThrowInBlackHole);
    }

    void ThrowInBlackHole(GameObject sender)
    {
        spawnedTestObject.OnBallGrabbed.RemoveListener(ThrowInBlackHole);
        AudioSource.PlayClipAtPoint(dialogue[3], textField.transform.position);
        textField.text = "Current goal: \nThrow test object into black hole \n\nImportant tool: \nArm strength";
        blackHole.OnEatObject.AddListener(FinishTutorial);
    }

    void FinishTutorial()
    {
        blackHole.OnEatObject.RemoveListener(FinishTutorial);
        previewText.text = "";
        AudioSource.PlayClipAtPoint(dialogue[4], textField.transform.position);
        Destroy(spawnedTestObjectPreview);
        textField.text = "Test complete \n\nCurrent goal: \nWait for partner to be ready \n\nImportant tool: \nPatience";
        TaskContext.singleton.FixerTutDone();
        StartCoroutine(TutorialFiveSecDelay());
    }

    IEnumerator TutorialFiveSecDelay()
    {
        yield return new WaitForSeconds(dialogue[4].length + 1);
        Destroy(textField.gameObject);
    }
}
