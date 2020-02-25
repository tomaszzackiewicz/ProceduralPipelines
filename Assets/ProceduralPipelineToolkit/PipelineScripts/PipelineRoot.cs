using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipelineToolkit {

    public class PipelineRoot : MonoBehaviour {

        public Transform child;
        private bool isDragabble = true;

        public Transform Child {
            get {
                return child;
            }
        }

        public bool IsDragabble {
            get {
                return isDragabble;
            }
            set {
                isDragabble = value;
            }
        }

        private void Awake() {
            child = gameObject.transform.GetChild(0);
        }
		
		private void OnEnable(){
			InputManager.toggleMouseDragging += ToggleDraggable;
		}

        private void OnMouseDrag() {
            if (isDragabble) {
                float distanceToScreen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
                transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen));
            }
        }

        public void ToggleDraggable(bool param){
            isDragabble = param;
        }
		
		private void OnDisable(){
			InputManager.toggleMouseDragging -= ToggleDraggable;
		}
    }
}
