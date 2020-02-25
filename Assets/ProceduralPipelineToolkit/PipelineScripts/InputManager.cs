using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipelineToolkit {

	public class InputManager : MonoBehaviour {
		
		public delegate void ToggleMouseDragging(bool isDraggable);
		public static event ToggleMouseDragging toggleMouseDragging;
		public static InputManager instance = null;
		
		private GameObject cameraObject;
		private bool isRuntimeGizmo = false;
		private bool isAddPipelineSystemAllowed = false;
		private bool isDraggable = true;
		
		private void Awake() {
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Destroy(gameObject);
            }

        }

		private void Update () {
			if(Input.GetKeyDown(KeyCode.Delete)){
				PipelineGameManager.instance.RemoveSelectedObject();
			}
			
			if(Input.GetKeyDown(KeyCode.P)){
				isAddPipelineSystemAllowed = !isAddPipelineSystemAllowed;
				PipelineGameManager.instance.CreatePipelineSystem(isAddPipelineSystemAllowed);
			}
			
			if(Input.GetKeyDown(KeyCode.Space)){
				PipelineManager pipelineManager = PipelineGameManager.instance.GetPipelineSystemManager();
				if (pipelineManager != null) {
					pipelineManager.PipelineCreator();
				}
			}
			
		}
	}
}
