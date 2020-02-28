using UnityEngine;
using System.Collections.Generic;

namespace PipelineToolkit {

	public class NodeController: MonoBehaviour {
		
		public List<GameObject> nodes = new List<GameObject>();
		public GameObject nodeMenu;
		public bool isParent = false;
		public static GameObject parentNode;
		public static bool isParenting = false;
		public Transform originalParent;
		
		private bool isNodeMenu = false;
		private bool isNodeMenuUp = true;
		private Vector3 upPosition = new Vector3(0,2,0);
		private Vector3 downPosition = new Vector3(0,-2,0);
		private bool isDraggable = true;
		
		public GameObject NodeMenu {
			get {
				return nodeMenu;
			}
		}
		
		private void OnEnable(){
			InputManager.toggleMouseDragging += ToggleDraggable;
		}
			
		private void OnMouseDrag(){
			 float distanceToScreen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
			 transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen ));
		}
		
		private void Start(){
			originalParent = this.gameObject.transform.parent;
			nodeMenu.SetActive(false);
		}
		
		public void SetParentButton(){
			isParent = !isParent;
			if(isParent){
				parentNode = this.gameObject;
				isParenting = true;
			}
		}
			
		public void AddToParentButton(){
			if(isParenting){
				this.gameObject.transform.SetParent(parentNode.transform,true);
			}else{
				this.gameObject.transform.parent = originalParent;
			}	
		}
		
		public void RemoveFromParentButton(){
			isParenting = false;
		}
			
		private void ToggleNodeMenu(){
			isNodeMenu = !isNodeMenu;
			if(isNodeMenu){
				nodeMenu.SetActive(true);
			}else{
				nodeMenu.SetActive(false);
			}	
		}
		
		private void OnMouseOver() {
			if (Input.GetMouseButtonDown(1)) {
				ToggleNodeMenu();
			}
		}
		
		public void ToggleNodeMenuPosition(){
			isNodeMenuUp = !isNodeMenuUp;
			if(isNodeMenuUp){
				nodeMenu.transform.localPosition = upPosition;
			}else{
				nodeMenu.transform.localPosition = downPosition;
			}
		}
		
		private void ToggleDraggable(bool param){
			isDraggable = param;
		}
		
		private void OnDisable(){
			InputManager.toggleMouseDragging -= ToggleDraggable;
		}
		
	 }
}
