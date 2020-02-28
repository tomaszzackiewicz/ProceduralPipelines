using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PipelineToolkit {

    public class PipelineGameManager : MonoBehaviour {

        public static PipelineGameManager instance = null;
        public GameObject pipelineSystemPrefab;
		public Text targetText;
        public List<GameObject> pipeSystems = new List<GameObject>();
        public List<GameObject> helpers = new List<GameObject>();
		
        private bool isHelpers = false;
		private int id = 0;
		private int numberOfNodes = 10;
		private float distanceOfNodes = 10.0f;
		private string targetName;
        private Vector3 pipelineSystemPos;
        private GameObject clickedObject;
        private GameObject clickedPipelineSystem;
        private GameObject clickedPipeline;
        private int layerMask;
        private bool isFollow = false;
        private float lastClickTime = 0.0f;
        private float catchTime = 0.25f;
        private int pipelineSystemNumber = 0;
		private Axis xAxis;
        private Axis yAxis;
        private Axis zAxis;
        private Material helperWhite;
        private Material helperBlack;
		
		public Axis XAxis{
			get{
				return xAxis;
			}
			set{
				xAxis = value;
			}
		}
		
		public Axis YAxis{
			get{
				return yAxis;
			}
			set{
				yAxis = value;
			}
		}
		public Axis ZAxis{
			get{
				return zAxis;
			}
			set{
				zAxis = value;
			}
		}
		public GameObject ClickedObject{
			get{
				return clickedObject;
			}
		}
		public int NumberOfNodes {
            get { 
				return numberOfNodes; 
			}
			set {
				if(value > 0){
					numberOfNodes = value;
				}
			}
        }
		public float DistanceOfNodes {
            get { 
				return distanceOfNodes; 
			}
			set {
				if(value > 0){
					distanceOfNodes = value;
				}
			}
        }

        void Awake() {
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Destroy(gameObject);
            }
            //DontDestroyOnLoad(gameObject);

            layerMask = LayerMask.GetMask("Pipeline");
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                RemoveNullValues();
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layerMask)) {
					
					if(hit.collider.gameObject.tag.Contains("Build")){
						clickedObject = hit.collider.gameObject;
						targetText.text = clickedObject.name;
					}
                    if (clickedObject.tag == "BuildPipelineSystem") {
                        if (helpers.Count > 0) {
                            foreach (GameObject helper in helpers) {
                                if (helper.GetComponent<Renderer>()) {
                                    Renderer rend = helper.GetComponent<Renderer>();
                                    rend.material = Resources.Load("Materials/HelperWhite") as Material;
                                }
                            }
                        }
                        clickedPipelineSystem = hit.collider.gameObject;
                        if (clickedPipelineSystem.GetComponent<Renderer>()) {
                            Renderer ren = clickedPipelineSystem.GetComponent<Renderer>();
                            ren.material = Resources.Load("Materials/HelperBlack") as Material;
                        }
                    } else if (clickedObject.tag == "BuildPipeline") {
                        if (helpers.Count > 0) {
                            foreach (GameObject helper in helpers) {
                                if (helper.GetComponent<Renderer>()) {
                                    Renderer rend = helper.GetComponent<Renderer>();
                                    rend.material = Resources.Load("Materials/HelperWhite") as Material;
                                }
                            }
                        }
                        clickedPipeline = hit.collider.gameObject;
                        if (clickedPipeline.GetComponent<Renderer>()) {
                            Renderer ren = clickedPipeline.GetComponent<Renderer>();
                            ren.material = Resources.Load("Materials/HelperBlack") as Material;
                        }
                    }
                }
            }
		
            if (isFollow) {
                FollowMode();
            }
        }

        private void RemoveNullValues() {
            helpers.RemoveAll(x => x == null);
            pipeSystems.RemoveAll(x => x == null);
        }

        public void CreatePipelineSystem(bool param) {
            isFollow = param;
        }

        public PipelineManager GetPipelineSystemManager() {
            PipelineManager pipelineManager = null;
            if (clickedPipelineSystem) {
                pipelineManager = clickedPipelineSystem.GetComponentInChildren<PipelineManager>();
            }
            return pipelineManager;
        }

        public PipeSpline GetPipeSpline() {
            PipeSpline pipeSpline = null;
            if (clickedPipeline) {
                pipeSpline = clickedPipeline.GetComponentInChildren<PipeSpline>();
            }
            return pipeSpline;
        }
		
		public PipelineNodes GetPipelineNodes() {
            PipelineNodes pipelineNodes = null;
            if (clickedPipeline) {
                pipelineNodes = clickedPipeline.GetComponentInChildren<PipelineNodes>();
            }
            return pipelineNodes;
        }

        public PipeSplineMesh GetPipeSplineMesh() {
            PipeSplineMesh pipeSplineMesh = null;
            if (clickedPipeline) {
                pipeSplineMesh = clickedPipeline.GetComponentInChildren<PipeSplineMesh>();
            }
            return pipeSplineMesh;
        }

        public PipelineController GetPipelineController() {
            PipelineController pipelineController = null;
            if (clickedPipeline) {
                pipelineController = clickedPipeline.GetComponentInChildren<PipelineController>();
            }
            return pipelineController;
        }

        public PipelineRoot GetPipelineRoot() {
            PipelineRoot pipelineRoot = null;
            if (clickedPipeline) {
                pipelineRoot = clickedPipeline.GetComponent<PipelineRoot>();
            }
            return pipelineRoot;
        }

        public void RemoveSelectedObject() {
			if(clickedObject){
				if (clickedObject.tag == "BuildPipelineSystem") {
					GameObject.Destroy(clickedObject);
				}
				if(clickedObject.tag == "BuildPipeline"){
					PipelineRoot pipelineRoot = clickedObject.GetComponent<PipelineRoot>();
					if (pipelineRoot) {
						PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();
						if(pipelineController.isMaster){
							GameObject.Destroy(clickedObject);
						}
					}
				}	
			}
        }

        void FollowMode() {
            Vector3 temp = Input.mousePosition;
            temp.z = 10f;
            Camera camera = Camera.main.GetComponent<Camera>();
            pipelineSystemPrefab.transform.position = camera.ScreenToWorldPoint(temp);

            if (pipelineSystemPrefab) {
                if (Input.GetMouseButtonDown(0)) {
                    if (Time.time - lastClickTime < catchTime) {
						PipeSystemCreation();
                    }
                    lastClickTime = Time.time;
                }
            }
        }
		
		void PipeSystemCreation(){
			
			GameObject pipelineSystem = Instantiate(pipelineSystemPrefab, pipelineSystemPrefab.transform.position, Quaternion.identity) as GameObject;
			pipelineSystemNumber++;
			pipelineSystem.name = pipelineSystemPrefab.name + pipelineSystemNumber;
			PipelineRoot pipelineRoot = pipelineSystem.GetComponent<PipelineRoot>();
			PipelineManager pipelineManager = pipelineRoot.child.GetComponent<PipelineManager>();
			pipelineManager.Id = id++;
			pipelineSystem.name = pipelineSystemPrefab.name + "_" + id;
			pipeSystems.Add(pipelineSystem);
			helpers.Add(pipelineSystem);
		}
		
        public void ToggleHelpers() {
            isHelpers = !isHelpers;
            if (isHelpers) {
                foreach (GameObject helper in helpers) {
                    Renderer ren = helper.GetComponent<Renderer>();
                    ren.enabled = true;
                }
            } else {
                foreach (GameObject helper in helpers) {
                    Renderer ren = helper.GetComponent<Renderer>();
                    ren.enabled = false;
                }
            }
        }
    }
}