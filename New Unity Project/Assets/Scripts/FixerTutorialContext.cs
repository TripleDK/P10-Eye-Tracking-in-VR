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

    float dialogueStartTime1 = 999999999f,
     dialogueStartTime2 = 99999999f,
     dialogueStartTime3 = 99999999f;

    void Awake()
    {
        textField.text = "";
    }

    public void StartTutorial()
    {
        previewText.text = "";
        testObjectPreviewPos.gameObject.SetActive(false);
        textField.text = "Welcome: -Fixer- \n\nOverall goal: \nClose the black hole \n\nImportant tool: \nCommuncating";
        AudioSource.PlayClipAtPoint(dialogue[0], textField.transform.position);
        StartCoroutine(LookAtMonitor());
    }

    IEnumerator LookAtMonitor()
    {
        yield return new WaitForSeconds(dialogue[0].length + 1);
        AudioSource.PlayClipAtPoint(dialogue[1], textField.transform.position);
        dialogueStartTime1 = Time.time;
        previewText.text = "Look here";
        textField.text = "Current goal: \nLocate monitor \n\nImportant tool: \nSpatial awareness";
        gaze.OnTVRealized.AddListener(GrabAnObject);
    }

    public void GrabAnObject()
    {
        gaze.OnTVRealized.RemoveListener(GrabAnObject);
        StartCoroutine(GrabAnObjectCo());

    }

    IEnumerator GrabAnObjectCo()
    {
        while (Time.time - dialogueStartTime1 < dialogue[1].length + 1)
        {
            yield return null;
        }
        spawnedTestObjectPreview = Instantiate(testObject, testObjectPreviewPos.position, testObjectPreviewPos.rotation);
        spawnedTestObjectPreview.name = "Testing preview object";
        spawnedTestObjectPreview.GetComponent<Rigidbody>().isKinematic = true;
        previewText.text = "Test object";
        AudioSource.PlayClipAtPoint(dialogue[2], textField.transform.position);
        dialogueStartTime2 = Time.time;
        textField.text = "Current goal: \nLocate and pick up test object \n\nImportant tool: \nGaze movement";
        spawnedTestObject = Instantiate(testObject, testObjectStartPos.position, testObjectStartPos.rotation);
        spawnedTestObject.OnBallGrabbed.AddListener(ThrowInBlackHole);
        blackHole.OnEatObject.AddListener(FinishTutorial);
    }

    void ThrowInBlackHole(GameObject sender)
    {
        spawnedTestObject.OnBallGrabbed.RemoveListener(ThrowInBlackHole);
        StartCoroutine(ThrowInBlackHoleCo());
    }

    IEnumerator ThrowInBlackHoleCo()
    {
        while (Time.time - dialogueStartTime2 < dialogue[2].length + 1)
        {
            yield return null;
        }
        AudioSource.PlayClipAtPoint(dialogue[3], textField.transform.position);
        dialogueStartTime3 = Time.time;
        textField.text = "Current goal: \nThrow test object into black hole \n\nImportant tool: \nArm strength";

    }

    void FinishTutorial()
    {
        blackHole.OnEatObject.RemoveListener(FinishTutorial);
        previewText.text = "";
        Destroy(spawnedTestObjectPreview);
        spawnedTestObjectPreview.transform.position = new Vector3(1000, 1000, 1000);
        StartCoroutine(FinishTutorialCo());
    }

    IEnumerator FinishTutorialCo()
    {
        while (Time.time - dialogueStartTime3 < dialogue[3].length + 1)
        {
            yield return null;
        }
        AudioSource.PlayClipAtPoint(dialogue[4], textField.transform.position);

        textField.text = "Test complete \n\nCurrent goal: \nWait for partner to be ready \n\nImportant tool: \nPatience";

        StartCoroutine(TutorialFiveSecDelay());
    }

    IEnumerator TutorialFiveSecDelay()
    {
        yield return new WaitForSeconds(dialogue[4].length + 1);
        TaskContext.singleton.FixerTutDone();
        Destroy(textField.gameObject);
    }
}
