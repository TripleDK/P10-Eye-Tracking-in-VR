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

	void Awake()
	{
		textField.text = "";
	}

	public void StartTutorial()
	{
		textField.text = "Welcome \n-Fetcher- \n\nOverall goal: \nClose the black hole \n\nImportant tool: \nCommunicating";
		AudioSource.PlayClipAtPoint(dialog[0], textField.transform.position);
	}

	IEnumerator GrabObject()
	{
		yield return new WaitForSeconds(dialog[0].length);
		textField.text = "Current goal: \nGrab red object \n\nImportant tool: \nOpposable thumb";
		AudioSource.PlayClipAtPoint(dialog[1], textField.transform.position);
		spawnedTestObject = Instantiate(testObject, testObjectStartPos.position, testObjectStartPos.rotation);
		spawnedTestObject.transform.GetComponent<MeshRenderer>().material.color = Color.red;
		spawnedTestObject.OnBallGrabbed.AddListener(MoveObjectToTeleporter);
	}

	void MoveObjectToTeleporter(GameObject sender)
	{
		spawnedTestObject.OnBallGrabbed.RemoveListener(MoveObjectToTeleporter);
		textField.text = "Current goal: \nPlace object in teleporter \n\nImportant tool: \nStrong throwing arm and good aim";
		AudioSource.PlayClipAtPoint(dialog[2], textField.transform.position);
		teleporter.OnAcceptItem.AddListener(StartTheTeleporter);
	}

	void StartTheTeleporter()
	{
		teleporter.OnAcceptItem.RemoveListener(StartTheTeleporter);
		textField.text = "Current goal: \nActivate teleporter \n\nImportant tool: \nCuriosity of big red buttons";
		AudioSource.PlayClipAtPoint(dialog[3], textField.transform.position);
		teleporter.OnTeleportItem.AddListener(CompleteTutorial);
	}

	void CompleteTutorial()
	{
		teleporter.OnTeleportItem.RemoveListener(CompleteTutorial);
		AudioSource.PlayClipAtPoint(dialog[4], textField.transform.position);
		textField.text = "Test complete \n\nCurrent goal: \nWait for partner to be ready \n\nImportant tool: \nPatience";
		TaskContext.singleton.FetcherTutDone();
		StartCoroutine(TutorialFiveSecDelay());
	}

	IEnumerator TutorialFiveSecDelay()
	{
		yield return new WaitForSeconds(5);
		Destroy(textField.gameObject);
	}
}
