using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PipelineToolkit {

    public class UIManager : MonoBehaviour {

        public GameObject addPipelineSystemButtonRef;

        public GameObject interpolationMode;
        public GameObject rotationMode;
        public GameObject tangentMode;
		public GameObject updateMode;
        public GameObject otherSettings;
        public GameObject settingsPanel;
		public GameObject helpPanel;
		
        private bool isCheckedXAxis = false;
        private bool isCheckedYAxis = false;
        private bool isCheckedZAxis = false;

        private Axis axisX;
        private Axis axisY;
        private Axis axisZ;

        private int tuVectorX;
        private int tuVectorY;
        private int tuVectorZ;

        private int segments;
        private int pipes;
        private int xScale;
        private int yScale;
        private int uTile;
        private int vTile;

        private bool isDraggable = false;
		private bool isAddPipelineSystemAllowed = false;

        private void Start() {
            interpolationMode.SetActive(true);
            rotationMode.SetActive(false);
            tangentMode.SetActive(false);
			updateMode.SetActive(false);
            otherSettings.SetActive(false);
            settingsPanel.SetActive(false);
			helpPanel.SetActive(false);
        }

        public void AddPipelineSystemButton() {
			isAddPipelineSystemAllowed = !isAddPipelineSystemAllowed;
			PipelineGameManager.instance.CreatePipelineSystem(isAddPipelineSystemAllowed);
			if(isAddPipelineSystemAllowed){
				Image img = addPipelineSystemButtonRef.GetComponent<Image>();
				img.color = Color.red;
			}else{
				Image img = addPipelineSystemButtonRef.GetComponent<Image>();
				img.color = Color.black;
			}
            
        }

        public void AddPipeButton() {
            PipelineManager pipelineManager = PipelineGameManager.instance.GetPipelineSystemManager();
            if (pipelineManager != null) {
                pipelineManager.PipelineCreator();
            }
        }

        public void AddNodeButton() {
            PipelineController pipelineController = PipelineGameManager.instance.GetPipelineController();
            if (pipelineController != null) {
                pipelineController.CreateNodes();
            }
        }
		
		public void SetNumberOfNodesIF(string param){
			int numberOfNodes = 0;
            if (param != "") {
                numberOfNodes = int.Parse(param);
            }
			PipelineGameManager.instance.NumberOfNodes = numberOfNodes;	
            Debug.Log(numberOfNodes);
		}
		
		public void SetDistanceOfNodesIF(string param){
			float distanceOfNodes = 0.0f;
            if (param != "") {
				int temp = int.Parse(param);
                distanceOfNodes = (float)temp;
            }
			PipelineGameManager.instance.DistanceOfNodes = distanceOfNodes;
			Debug.Log(distanceOfNodes);
		}
		
		public void SetNodeMenuPositionButton(){
			PipelineManager pipelineManager = PipelineGameManager.instance.GetPipelineSystemManager();
			if(pipelineManager){
				foreach(GameObject pipeline in pipelineManager.pipelines){
					PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
					if (pipelineRoot) {
						PipeSpline pipeSpline = pipelineRoot.transform.GetChild(0).GetComponent<PipeSpline>();
						foreach(PipeSplineNode node in pipeSpline.pipeSplineNodesArray){
							NodeController nodeController = node.gameObject.GetComponent<NodeController>();
							nodeController.ToggleNodeMenuPosition();
						}
					}
				}
			}
		}

        public void ToggleNodesButton() {
            PipelineController pipelineController = PipelineGameManager.instance.GetPipelineController();
            if (pipelineController != null) {
                pipelineController.ToggleNodes();
            }
        }

        public void SmoothNodesXButton() {
            PipelineController pipelineController = PipelineGameManager.instance.GetPipelineController();
            if (pipelineController != null) {
                pipelineController.SmoothNodesX();
            }
        }

        public void SmoothNodesZButton() {
            PipelineController pipelineController = PipelineGameManager.instance.GetPipelineController();
            if (pipelineController != null) {
                pipelineController.SmoothNodesZ();
            }
        }

        public void ToggleSupportsButton() {
			
			PipelineManager pipelineManager = PipelineGameManager.instance.GetPipelineSystemManager();
			if(pipelineManager){
				foreach(GameObject pipeline in pipelineManager.pipelines){
					PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
					if (pipelineRoot) {
						PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();
						if (pipelineController != null) {
							pipelineController.ToggleSupports();
						}
					}
				}
			}  
        }

        public void RemoveObjectButton() {
            PipelineGameManager.instance.RemoveSelectedObject();
        }
		
        public void ToggleBlockingObjectButton() {
            isDraggable = !isDraggable;
            PipelineRoot pipelineRoot = PipelineGameManager.instance.GetPipelineRoot();
            if (pipelineRoot != null) {
                pipelineRoot.IsDragabble = isDraggable;
            }
        }

        public void SetNumberOfSegments(string param) {
            segments = int.Parse(param);
        }

        public void SetNumberOfSegmentsButton() {
            PipelineManager pipelineManager = PipelineGameManager.instance.GetPipelineSystemManager();
            if (pipelineManager != null) {
                pipelineManager.Segments = segments;
            }
        }

        public void SetNumberOfPipes(string param) {
            pipes = int.Parse(param);
        }

        public void SetNumberOfPipesButton() {
            PipelineManager pipelineManager = PipelineGameManager.instance.GetPipelineSystemManager();
            if (pipelineManager != null) {
                pipelineManager.PipesCount = pipes;
            }
        }

        public void SetPipeScaleX(string param) {
            xScale = int.Parse(param);
        }

        public void SetPipeScaleY(string param) {
            yScale = int.Parse(param);
        }

        public void SetScaleButton() {
            if (xScale <= 0) {
                xScale = yScale;
            }
            if (yScale <= 0) {
                yScale = xScale;
            }
            PipeSplineMesh pipeSplineMesh = PipelineGameManager.instance.GetPipeSplineMesh();
            if (pipeSplineMesh != null) {
                pipeSplineMesh.xyScale = new Vector2(xScale, yScale);
            }
        }
		
		public void SetPipeScaleEvenlySlider(float scale) {
            PipelineManager pipelineManager = PipelineGameManager.instance.GetPipelineSystemManager();
            if (pipelineManager != null) {
                pipelineManager.UpdateScale(scale);
            }
			
			foreach(GameObject pipeline in pipelineManager.pipelines){
				PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
				PipeSpline pipeSpline = pipelineRoot.child.GetComponent<PipeSpline>();
				if(pipelineManager.pipelines[0]){
					continue;
				}
				
				foreach (PipeSplineNode psNode in pipeSpline.pipeSplineNodesArray) {
					psNode.gameObject.transform.localPosition = new Vector3(psNode.gameObject.transform.localPosition.x, psNode.gameObject.transform.localPosition.y, psNode.gameObject.transform.localPosition.z + scale);
				}
			}
						
			foreach(GameObject pipeline in pipelineManager.pipelines){
				PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
				if (pipelineRoot) {
					PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();
					if (pipelineController != null) {
						pipelineController.HighSupportPosOffset = 8.0f + scale/2;pipelineController.HighSupportScaleOffset = 0.9f + scale/2;
					}
				}
			}
        }
		
		public void SetNodeScaleSlider(float scale) {
            PipelineManager pipelineManager = PipelineGameManager.instance.GetPipelineSystemManager();
            if (pipelineManager != null) {
				pipelineManager.UpdateNodeScale(scale);
            }
        }
		
		public void SetPipeOffsetSlider(float scale) {
            PipelineManager pipelineManager = PipelineGameManager.instance.GetPipelineSystemManager();
            if (pipelineManager != null) {
				pipelineManager.Offset = scale;
            }
        }

        public void SetUTile(string param) {
            uTile = int.Parse(param);
        }

        public void SetVTile(string param) {
            vTile = int.Parse(param);
        }

        public void SetUVTileButton() {
            if (uTile <= 0) {
                uTile = vTile;
            }
            if (vTile <= 0) {
                vTile = uTile;
            }
            PipeSplineMesh pipeSplineMesh = PipelineGameManager.instance.GetPipeSplineMesh();
            if (pipeSplineMesh != null) {
                pipeSplineMesh.uvScale = new Vector2(uTile, vTile);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////

        public void ChangeXAxisToggle(bool isChecked) {
            isCheckedXAxis = isChecked;
            if (isCheckedXAxis) {
                axisX = Axis.xAxis;
            } else {
                axisX = Axis.none;
            }
            PipelineGameManager.instance.XAxis = axisX;
        }

        public void ChangeYAxisToggle(bool isChecked) {
            isCheckedYAxis = isChecked;
            if (isCheckedYAxis) {
                axisY = Axis.yAxis;
            } else {
                axisY = Axis.none;
            }
            PipelineGameManager.instance.YAxis = axisY;
        }

        public void ChangeZAxisToggle(bool isChecked) {
            isCheckedZAxis = isChecked;
            if (isCheckedZAxis) {
                axisZ = Axis.zAxis;
            } else {
                axisZ = Axis.none;
            }
            PipelineGameManager.instance.ZAxis = axisZ;
        }

        public void BaseMeshToggle(bool isChecked) {
            PipeSplineMesh pipeSplineMesh = PipelineGameManager.instance.GetPipeSplineMesh();
            if (pipeSplineMesh != null) {
                pipeSplineMesh.isBaseMesh = isChecked;
            }
        }

        public void CylinerToggle(bool isChecked) {
            PipeSplineMesh pipeSplineMesh = PipelineGameManager.instance.GetPipeSplineMesh();
            if (pipeSplineMesh != null) {
                pipeSplineMesh.isCylinder = isChecked;
            }
        }

        public void CubeToggle(bool isChecked) {
            PipeSplineMesh pipeSplineMesh = PipelineGameManager.instance.GetPipeSplineMesh();
            if (pipeSplineMesh != null) {
                pipeSplineMesh.isCube = isChecked;
            }
        }

        public void InterpolationModeButton() {
            interpolationMode.SetActive(true);
            rotationMode.SetActive(false);
            tangentMode.SetActive(false);
			updateMode.SetActive(false);
            otherSettings.SetActive(false);
        }

        public void RotationModeButton() {
            interpolationMode.SetActive(false);
            rotationMode.SetActive(true);
            tangentMode.SetActive(false);
			updateMode.SetActive(false);
            otherSettings.SetActive(false);
        }

        public void TangentModeButton() {
            interpolationMode.SetActive(false);
            rotationMode.SetActive(false);
            tangentMode.SetActive(true);
			updateMode.SetActive(false);
            otherSettings.SetActive(false);
        }
		
		public void UpdateModeButton() {
            interpolationMode.SetActive(false);
            rotationMode.SetActive(false);
            tangentMode.SetActive(false);
			updateMode.SetActive(true);
            otherSettings.SetActive(false);
        }
		
        public void OtherSettingsButton() {
            interpolationMode.SetActive(false);
            rotationMode.SetActive(false);
            tangentMode.SetActive(false);
			updateMode.SetActive(false);
            otherSettings.SetActive(true);
        }

        //////////////////////////////////////////////////////////////////////

        public void InterpolationModeHermiteToggle() {
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                Interpolations.InterpolationMode = InterpolationMode.Hermite;
            }

        }

        public void InterpolationModeBezierToggle() {
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                Interpolations.InterpolationMode = InterpolationMode.Bezier;
            }

        }

        public void InterpolationModeBSplineToggle() {
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                Interpolations.InterpolationMode = InterpolationMode.BSpline;
            }

        }

        public void InterpolationModeLinearToggle() {
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                Interpolations.InterpolationMode = InterpolationMode.Linear;
            }

        }
		
		/////////////////////////////////////////////////////////////////////////////////
       
		public void RotationModeNoneToggle() {
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                pipeSpline.rotationMode = RotationMode.None;
            }

        }

        public void RotationModeNodeToggle() {
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                pipeSpline.rotationMode = RotationMode.Node;
            }

        }

        public void RotationModeTangentToggle() {
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                pipeSpline.rotationMode = RotationMode.Tangent;
            }

        }

        public void TangentModeNormalizedToggle() {
            PipelineNodes pipelineNodes = PipelineGameManager.instance.GetPipelineNodes();
            if (pipelineNodes != null) {
                pipelineNodes.TangentMode = TangentMode.UseNormalizedTangents;
            }

        }

        public void TangentModeUseTangentsToggle() {
            PipelineNodes pipelineNodes = PipelineGameManager.instance.GetPipelineNodes();
            if (pipelineNodes != null) {
                pipelineNodes.TangentMode = TangentMode.UseTangents;
            }

        }

        public void TangentModeUseNodeForwardVectorToggle() {
            PipelineNodes pipelineNodes = PipelineGameManager.instance.GetPipelineNodes();
            if (pipelineNodes != null) {
                pipelineNodes.TangentMode = TangentMode.UseNodeForwardVector;
            }

        }
		
		//////////////////////////////////////////////////////////////////////
		
		public void UpdateEveryXFramesIF(string param){
			AutomaticUpdater.deltaFrames = int.Parse(param);
		}
		
		public void UpdateEveryXSecondsIF(string param){
			AutomaticUpdater.deltaSeconds = int.Parse(param);
		}

        public void UpdateModeDontUpdateToggle() {
            AutomaticUpdater.mode = UpdateMode.DontUpdate;
        }

        public void UpdateModeEveryFrameToggle() {
           AutomaticUpdater.mode = UpdateMode.EveryFrame;
        }

        public void UpdateModeEveryXFramesToggle() {
            AutomaticUpdater.mode = UpdateMode.EveryXFrames;
        }

        public void UpdateModeEveryXSecondsToggle() {
            AutomaticUpdater.mode = UpdateMode.EveryXSeconds;
        }

        ///////////////////////////////////////////////////////////////////////////

        public void TanUpVectorX(string param) {
            tuVectorX = int.Parse(param);
        }

        public void TanUpVectorY(string param) {
            tuVectorY = int.Parse(param);
        }

        public void TanUpVectorZ(string param) {
            tuVectorZ = int.Parse(param);
        }

        public void TanUpVectorButton() {
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                pipeSpline.tanUpVector = new Vector3(tuVectorX, tuVectorY, tuVectorZ);
            }
        }

        public void AutocloseToggle(bool isClosed) {
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                pipeSpline.autoClose = isClosed;
            }
        }

        public void TensionToggle(string param) {
            float tension = (float)int.Parse(param);
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                pipeSpline.tension = tension;
            }
        }

        public void InterpolationAccuracyToggle(string param) {
            int accuracy = int.Parse(param);
            PipeSpline pipeSpline = PipelineGameManager.instance.GetPipeSpline();
            if (pipeSpline != null) {
                pipeSpline.InterpolationAccuracy = accuracy;
            }
        }

        public void ToggleSettingsPanelButton() {
            if (!settingsPanel.activeSelf) {
				if(helpPanel.activeSelf){
					helpPanel.SetActive(false);
				}
                settingsPanel.SetActive(true);
            } else {
                settingsPanel.SetActive(false);
            }
        }
		public void ToggleHelpPanelButton() {
            if (!helpPanel.activeSelf) {
				if(settingsPanel.activeSelf){
					settingsPanel.SetActive(false);
				}
                helpPanel.SetActive(true);
            } else {
                helpPanel.SetActive(false);
            }
        }
        
		//////////////////////////////////////////////////////////////////////

        public void ToggleHelpersButton() {
            PipelineGameManager.instance.ToggleHelpers();
        }

    }
}
