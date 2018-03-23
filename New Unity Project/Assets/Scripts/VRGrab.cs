using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGrab : MonoBehaviour
{
    enum Controller { left, right };
    public HandSyncher handAnim;
    Controller side;
    public List<VRButton> grabbedObject = new List<VRButton>();

    void Awake()
    {
        if (gameObject.name == "Controller (left)") side = Controller.left;
        if (gameObject.name == "Controller (right)") side = Controller.right;
    }

    void Update()
    {
        if (side == Controller.left)
        {
            if (handAnim) handAnim.ChangeGrabL(Input.GetAxis("LeftTrigger"));
            if (Input.GetKeyDown("joystick button 14"))
            {
                if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.left, this);
                if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.left);

            }
            if (Input.GetKeyUp("joystick button 14"))
            {
                if (grabbedObject.Count > 0)
                {
                    grabbedObject[0].ActionUp(VRButton.Controller.left);
                }
            }
        }
        if (side == Controller.right)
        {
            if (handAnim) handAnim.ChangeGrabR(Input.GetAxis("RightTrigger"));
            if (Input.GetKeyDown("joystick button 15"))
            {
                if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.right, this);
                if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.right);

            }

            if (Input.GetKeyUp("joystick button 15"))
            {
                if (grabbedObject.Count > 0)
                {
                    grabbedObject[0].ActionUp(VRButton.Controller.right);
                }
            }

        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<VRButton>() && !grabbedObject.Contains(collider.GetComponent<VRButton>()))
        {
            grabbedObject.Add(collider.GetComponent<VRButton>());
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent<VRButton>())
        {
            if (grabbedObject.Count == 1)
            {
                VRButton.Controller tempContr = VRButton.Controller.left;
                if (side == Controller.left) tempContr = VRButton.Controller.left;
                if (side == Controller.right) tempContr = VRButton.Controller.right;
                grabbedObject[0].ActionUp(tempContr);
            }
            grabbedObject.Remove(collider.gameObject.GetComponent<VRButton>());
        }
    }
}
