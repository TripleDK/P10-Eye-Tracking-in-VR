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
    [SerializeField] BlackHole blackHole;
    ObjectInteractions spawnedTestObject;

    public void StartTutorial()
    {
        textField.text = "Welcome, your role is -Fixer-. You need to fix the black hole by throwing in the correct objects. To get the objects, you must look through the window and communicate to your partner which objects you need. To see what objects you need, you must look at the monitor slightly above you.";
        gaze.OnTVRealized.AddListener(GrabAnObject);
    }

    public void GrabAnObject()
    {
        gaze.OnTVRealized.RemoveListener(GrabAnObject);
        textField.text = "When you have communicated successfully with your partner and the object is transferred to you, it will appear in the machine to your left. Let me send you an example object. Once an object appears you can reach out with your hand and grab it by holding down the trigger on the back of the controller.";
        spawnedTestObject = Instantiate(testObject, testObjectStartPos.position, testObjectStartPos.rotation);
        spawnedTestObject.OnBallGrabbed.AddListener(ThrowInBlackHole);
    }

    void ThrowInBlackHole()
    {
        spawnedTestObject.OnBallGrabbed.RemoveListener(ThrowInBlackHole);
        textField.text = "Then to your right is the black hole where you must put the object. Throw the object by releasing the trigger.";
        blackHole.OnEatObject.AddListener(FinishTutorial);
    }

    void FinishTutorial()
    {
        blackHole.OnEatObject.RemoveListener(FinishTutorial);
        textField.text = "Well done! Soon the window will open and you will be able to communicate with your partner. Remember to look at the monitor above you to check what object you need next.";
    }

    //Check for other tutorial finish and call setup in taskcontext
}
