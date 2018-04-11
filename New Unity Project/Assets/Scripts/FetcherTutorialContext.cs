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
    ObjectInteractions spawnedTestObject;

    public void StartTutorial()
    {
        textField.text = "Welcome, your role is -Fetcher- Your task is to communicate with your partner about which object you have to fetch. Let’s try this out! Grab the big RED object; reach out and then hold the trigger button on the back of the controller.";
        spawnedTestObject = Instantiate(testObject, testObjectStartPos.position, testObjectStartPos.rotation);
        spawnedTestObject.transform.GetComponent<MeshRenderer>().material.color = Color.red;
        spawnedTestObject.OnBallGrabbed.AddListener(MoveObjectToTeleporter);

    }

    void MoveObjectToTeleporter()
    {
        spawnedTestObject.OnBallGrabbed.RemoveListener(MoveObjectToTeleporter);
        textField.text = "Now move the object over to the teleporter and let go of it by releasing the trigger.";
        teleporter.OnAcceptItem.AddListener(StartTheTeleporter);
    }

    void StartTheTeleporter()
    {
        teleporter.OnAcceptItem.RemoveListener(StartTheTeleporter);
        textField.text = "Great, now you just have to smack that big button on the front of the machine, and there you have it.";
        teleporter.OnTeleportItem.AddListener(CompleteTutorial);
    }

    void CompleteTutorial()
    {
        teleporter.OnTeleportItem.RemoveListener(CompleteTutorial);
        textField.text = "Good jobbo!";
        TaskContext.singleton.FetcherTutDone();
        StartCoroutine(TutorialFiveSecDelay());
    }

    IEnumerator TutorialFiveSecDelay()
    {
        yield return new WaitForSeconds(5);
        Destroy(textField.gameObject);
    }
}
