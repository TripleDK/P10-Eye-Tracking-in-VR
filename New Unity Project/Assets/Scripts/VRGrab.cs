using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGrab : MonoBehaviour
{
    enum Controller { left, right };
    public Animator handAnim;
    Controller side;
    public List<VRButton> grabbedObject = new List<VRButton>();
    public float leftTrigger, rightTrigger;

    void Awake()
    {
        if (gameObject.name == "Controller (left)") side = Controller.left;
        if (gameObject.name == "Controller (right)") side = Controller.right;
    }

    void Update()
    {
        Debug.Log("Left: " + Input.GetAxis("LeftTrigger").ToString("00.00") + ", Right: " + (float)Input.GetAxis("RightTrigger"));
        if (side == Controller.left)
        {
            if (handAnim) handAnim.SetFloat("GrabValue", Input.GetAxis("Mouse ScrollWheel"));
            if (Input.GetKeyDown("joystick button 14"))
            {
                if (handAnim) handAnim.SetBool("Grabbing", true);
                if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.left, this);
                if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.left);

            }
            if (Input.GetKeyUp("joystick button 14"))
            {
                if (handAnim) handAnim.SetBool("Grabbing", false);
                if (grabbedObject.Count > 0)
                {
                    grabbedObject[0].ActionUp(VRButton.Controller.left);
                }
            }
        }
        if (side == Controller.right)
        {
            if (handAnim) handAnim.SetFloat("GrabValue", Input.GetAxis("Horizontal"));
            if (Input.GetKeyDown("joystick button 15"))
            {
                if (handAnim) handAnim.SetBool("Grabbing", true);
                if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.right, this);
                if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.right);

            }

            if (Input.GetKeyUp("joystick button 15"))
            {
                if (handAnim) handAnim.SetBool("Grabbing", false);
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
