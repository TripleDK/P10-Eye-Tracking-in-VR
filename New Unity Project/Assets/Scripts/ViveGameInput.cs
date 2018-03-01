using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveGameInput : MonoBehaviour
{

	
	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown("joystick button 14"))
		{
			//Trigger left
		}
		if(Input.GetKeyDown("joystick button 15"))
		{
			//Trigger right
		}

		if(Input.GetKeyDown("joystick button 8"))
		{
		//Touch pad left
		}
		if(Input.GetKeyDown("joystick button 9"))
		{
				//Touch pad right
		}

		float leftPull = -Input.GetAxis("LeftTrackPad");

		float rightPull = -Input.GetAxis("RightTrackPad");

	}
}
