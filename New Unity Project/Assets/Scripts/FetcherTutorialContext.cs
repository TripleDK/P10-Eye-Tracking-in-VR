using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FetcherTutorialContext : MonoBehaviour
{
    [SerializeField] TextMeshPro textField;
    [SerializeField] ObjectInteractions testObject;
    [SerializeField] Transform testObjectStartPos;
    [SerializeField] Teleporter teleporter;
    [SerializeField] List<AudioClip> dialog = new List<AudioClip>();
    ObjectInteractions spawnedTestObject;
    float dialogueStartTime1, dialogueStartTime2, dialogueStartTime3 = 0.0f;


    void Awake()
    {
        textField.text = "";
    }

    public void StartTutorial()
    {
        textField.text = "Welcome \n-Fetcher- \n\nOverall goal: \nClose the black hole \n\nImportant tool: \nCommunicating";
        AudioSource.PlayClipAtPoint(dialog[0], textField.transform.position);
        StartCoroutine(GrabObject());
    }

    IEnumerator GrabObject()
    {
        yield return new WaitForSeconds(dialog[0].length + 1);
        textField.text = "Current goal: \nGrab red object \n\nImportant tool: \nOpposable thumb";
        AudioSource.PlayClipAtPoint(dialog[1], textField.transform.position);
        dialogueStartTime1 = Time.time;
        spawnedTestObject = Instantiate(testObject, testObjectStartPos.position, testObjectStartPos.rotation);
        spawnedTestObject.transform.GetComponent<MeshRenderer>().material.color = Color.red;
        spawnedTestObject.OnBallGrabbed.AddListener(MoveObjectToTeleporter);
        teleporter.OnAcceptItem.AddListener(StartTheTeleporter);
        teleporter.OnTeleportItem.AddListener(CompleteTutorial);
    }

    void MoveObjectToTeleporter(GameObject sender)
    {
        spawnedTestObject.OnBallGrabbed.RemoveListener(MoveObjectToTeleporter);
        StartCoroutine(MoveObjectToTeleporterCo());
    }

    IEnumerator MoveObjectToTeleporterCo()
    {
        while (Time.time - dialogueStartTime1 < dialog[1].length + 1)
        {
            yield return null;
        }
        textField.text = "Current goal: \nPlace object in teleporter \n\nImportant tool: \nStrong throwing arm and good aim";
        AudioSource.PlayClipAtPoint(dialog[2], textField.transform.position);
        dialogueStartTime2 = Time.time;

    }

    void StartTheTeleporter()
    {
        teleporter.OnAcceptItem.RemoveListener(StartTheTeleporter);
        StartCoroutine(StartTheTeleporterCo());
    }

    IEnumerator StartTheTeleporterCo()
    {
        while (Time.time - dialogueStartTime2 < dialog[2].length + 1)
        {
            yield return null;
        }
        textField.text = "Current goal: \nActivate teleporter \n\nImportant tool: \nCuriosity of big red buttons";
        AudioSource.PlayClipAtPoint(dialog[3], textField.transform.position);
        dialogueStartTime3 = Time.time;

    }

    void CompleteTutorial()
    {
        teleporter.OnTeleportItem.RemoveListener(CompleteTutorial);
        StartCoroutine(CompleteTutorialCo());
    }

    IEnumerator CompleteTutorialCo()
    {
        while (Time.time - dialogueStartTime3 < dialog[3].length + 1)
        {
            yield return null;
        }
        AudioSource.PlayClipAtPoint(dialog[4], textField.transform.position);
        textField.text = "Test complete \n\nCurrent goal: \nWait for partner to be ready \n\nImportant tool: \nPatience";
        TaskContext.singleton.FetcherTutDone();
        StartCoroutine(TutorialFiveSecDelay());
    }

    IEnumerator TutorialFiveSecDelay()
    {
        yield return new WaitForSeconds(dialog[4].length + 1);
        Destroy(textField.gameObject);
    }
}
