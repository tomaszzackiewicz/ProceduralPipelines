using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipelineToolkit{

	public class QuitApp : MonoBehaviour {
		
		private bool isInputDisabled;
		private bool isCursor;

		private void OnEnable () {
			isInputDisabled = false;
			isCursor = true;
		}
		
		private void Update () {
			if(Input.GetKeyDown(KeyCode.Q) && !isInputDisabled){
				Application.Quit();
				Debug.Log(Application.streamingAssetsPath);
			}
			
			if(Input.GetKeyDown(KeyCode.Escape) && !isInputDisabled){
				isCursor = !isCursor;
				if(isCursor){
					OnCursorVisible();
				}else{
					OnCursorInvisible();
				}	
			}
		}
		
		public void OnCursorVisible(){
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		public void OnCursorInvisible(){
			Cursor.visible = false;
			  Cursor.lockState = CursorLockMode.Locked;
		}
		
		private void DisableInput(bool isInput){
			isInputDisabled = isInput;
		}
		
		
	}
}

