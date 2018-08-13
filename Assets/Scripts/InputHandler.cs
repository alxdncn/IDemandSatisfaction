using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputState{
	DOWN,
	UP,
	HELD,
	NONE
}

public class InputHandler : MonoBehaviour {

	public InputState playerState = InputState.NONE;
	
	[SerializeField] string[] axesNames;

	[SerializeField] KeyCode releaseKey;

	bool axisDownLastFrame = false;

	float threshold;

	string platformName;

	bool joystickConnected = false;

	[SerializeField] string instructionPrefix;

	[System.NonSerialized] public string instructionText;

	void Awake(){
		if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor){
			platformName = "Win";
			threshold = 0.5f;
		} else if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor){
			platformName = "Mac";
			threshold = 0f;
		} else{
			Debug.LogError("INVALID RUNTIME PLATFORM!");
		}

		if(Input.GetJoystickNames().Length > 0){
			joystickConnected = true;
			instructionText = instructionPrefix + " Trigger"; 
		} else{
			instructionText = "Hold " + releaseKey.ToString();
		}
	}


	// Update is called once per frame
	void Update () {
		if(joystickConnected){
			playerState = HandleJoystickInput();
		} else{
			playerState = HandleKeyboard();
		}
	}

	InputState HandleJoystickInput(){
		bool axisDown = AxisIsDown(axesNames);

		if(axisDown && axisDownLastFrame){
			return InputState.HELD;
		} else if(axisDown && !axisDownLastFrame){
			return InputState.DOWN;
		} else if(!axisDown && axisDownLastFrame){
			return InputState.UP;
		} else{
			return InputState.NONE;
		}

		axisDownLastFrame = axisDown;
	}

	InputState HandleKeyboard(){
		if(Input.GetKeyDown(releaseKey)){
			return InputState.DOWN;
		} else if(Input.GetKeyUp(releaseKey)){
			return InputState.UP;
		} else if(Input.GetKey(releaseKey)){
			return InputState.HELD;
		} else{
			return InputState.NONE;
		}
	}

	bool AxisIsDown(string[] playerAxisNames){
		for(int i = 0; i < playerAxisNames.Length; i++){
			if(Input.GetAxisRaw(playerAxisNames[i] + platformName) > threshold){
				return true;
			}
		}
		return false;
	}


}
