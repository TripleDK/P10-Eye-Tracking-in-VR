﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGrab : MonoBehaviour
{
    enum Controller { left, right };
    public Animator handAnim;
    Controller side;
    public List<VRButton> grabbedObject = new List<VRButton>();

    void Awake()
    {
        if (gameObject.name == "Controller (left)") side = Controller.left;
        if (gameObject.name == "Controller (right)") side = Controller.right;
    }

    void Update()
    {
        if (Input.GetKeyDown("joystick button 14") && side == Controller.left)
        {
            if (handAnim) handAnim.SetBool("Grabbing", true);
            if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.left, this);
            if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.left);

        }
        if (Input.GetKeyDown("joystick button 15") && side == Controller.right)
        {
            if (handAnim) handAnim.SetBool("Grabbing", true);
            if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.right, this);
            if (grabbedObject.Count > 0) grabbedObject[0].Action(VRButton.Controller.right);

        }
        if (Input.GetKeyUp("joystick button 14") && side == Controller.left)
        {
            if (handAnim) handAnim.SetBool("Grabbing", false);
            if (grabbedObject.Count > 0)
            {
                grabbedObject[0].ActionUp(VRButton.Controller.left);
            }
        }
        if (Input.GetKeyUp("joystick button 15") && side == Controller.right)
        {
            if (handAnim) handAnim.SetBool("Grabbing", false);
            if (grabbedObject.Count > 0)
            {
                grabbedObject[0].ActionUp(VRButton.Controller.right);
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
            grabbedObject.Remove(collider.gameObject.GetComponent<VRButton>());
        }
    }
}