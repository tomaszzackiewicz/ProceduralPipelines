using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipelineToolkit {

    public class PipelineController : MonoBehaviour {

        public string pipelineName;
        public int pipelineIndex;
		public PipelineType pipelineType = PipelineType.Single;
        public Material nodeMaterialBlue;
        public Material nodeMaterialGreen;
        public List<Vector3> positions = new List<Vector3>();
        public List<Vector3> rotations = new List<Vector3>();
        public Transform[] bones;
        public GameObject nodePrefab;
        public GameObject nodesParent;
		public bool isMaster = false;
        public List<GameObject> supports = new List<GameObject>();
        public int segments;
        public GameObject handgripHighPrefab;
		
        private PipeSpline pipeSpline;
        private float length;
        private bool isActivated = false;
        private float highSupportPosOffset = 8.5f;
		private float highSupportScaleOffset = 0.0f;
        private PipeSplineMesh pipeSplineMesh;
        private bool isSupport = true;
		private int layerMask;
		private bool isSupportsToggled = true;
		
		public float HighSupportPosOffset{
			get{
				return highSupportPosOffset;
			}
			set{
				highSupportPosOffset = value;
			}
		}
		
		public float HighSupportScaleOffset{
			get{
				return highSupportScaleOffset;
			}
			set{
				highSupportScaleOffset = value;
			}
		}
		
        private void Start() {
            if (pipelineIndex == 0) {
                isMaster = true;
            } else {
                isMaster = false;

            }

            pipeSpline = GetComponent<PipeSpline>();
            DeactivateHandleNodes();
            pipeSplineMesh = GetComponent<PipeSplineMesh>();
            segments = pipeSplineMesh.segmentCount;
			layerMask = LayerMask.GetMask("Terrain");
        }

        public void CreateNodes() {
			float nodeDistance = PipelineGameManager.instance.DistanceOfNodes;
			int nOfNodes = PipelineGameManager.instance.NumberOfNodes;
			GameObject root = gameObject.transform.root.gameObject;
            GameObject node = Instantiate(nodePrefab, transform.position, Quaternion.identity) as GameObject;
            node.transform.SetParent(nodesParent.transform, true);
			NodeRoot nodeRoot = node.GetComponent<NodeRoot>();
            PipeSplineNode pipeSplineNode = nodeRoot.Child.GetComponent<PipeSplineNode>();
            pipeSpline.pipeSplineNodesArray.Add(pipeSplineNode);
			PipeSplineNode lastNode = pipeSpline.pipeSplineNodesArray[pipeSpline.pipeSplineNodesArray.Count-1];
			Vector3 lastNodePos = lastNode.gameObject.transform.localPosition;
			if(PipelineGameManager.instance.XAxis == Axis.xAxis){
				lastNodePos.x += nodeDistance * nOfNodes;
			}else if(PipelineGameManager.instance.YAxis == Axis.yAxis){
				lastNodePos.y += nodeDistance * nOfNodes;
			}else if(PipelineGameManager.instance.ZAxis == Axis.zAxis){
				lastNodePos.z += nodeDistance * nOfNodes;
			}
			node.transform.localPosition = lastNodePos;
			PipelineManager pipelineManager = root.GetComponentInChildren<PipelineManager>();
			pipelineManager.StartCreatingCoroutines();
			PipelineGameManager.instance.NumberOfNodes++;
        }

        public void CreateNodesAuto() {
            if(pipeSpline.pipeSplineNodesArray.Count == 0){
				CreateNodesLoop();
            }
			if (isMaster) {
                foreach (PipeSplineNode node in pipeSpline.pipeSplineNodesArray) {
                    MeshRenderer ren = node.gameObject.GetComponent<MeshRenderer>();
                    ren.material = nodeMaterialBlue;
                }
			} else {
				foreach (PipeSplineNode node in pipeSpline.pipeSplineNodesArray) {
					MeshRenderer ren = node.gameObject.GetComponent<MeshRenderer>();
					ren.material = nodeMaterialGreen;
				}
			}
        }
		
		private void CreateNodesLoop(){
			int nodeCounter = 0;
			float distance = 0.0f;
			int nOfNodes = PipelineGameManager.instance.NumberOfNodes;
			float nodeDistance = PipelineGameManager.instance.DistanceOfNodes;
			while (nOfNodes > nodeCounter) {
				GameObject node = Instantiate(nodePrefab, transform.position, Quaternion.identity) as GameObject;
				node.transform.SetParent(nodesParent.transform, true);
				if (PipelineGameManager.instance.XAxis == Axis.xAxis) {
					node.transform.localPosition = new Vector3(distance, node.transform.localPosition.y, node.transform.localPosition.z);
				} else if (PipelineGameManager.instance.YAxis == Axis.yAxis) {
					node.transform.localPosition = new Vector3(node.transform.localPosition.x, distance, node.transform.localPosition.z);
					node.transform.localEulerAngles = new Vector3(0.0f,0.0f,90.0f);
				} else if (PipelineGameManager.instance.ZAxis == Axis.zAxis) {
					node.transform.localPosition = new Vector3(node.transform.localPosition.x, node.transform.localPosition.y, distance);
				} else {
					node.transform.localPosition = new Vector3(distance, node.transform.localPosition.y, node.transform.localPosition.z);
				}
				
				NodeRoot nodeRoot = node.GetComponent<NodeRoot>();
				PipeSplineNode pipeSplineNode = nodeRoot.Child.GetComponent<PipeSplineNode>();
				pipeSplineNode.gameObject.name = nodePrefab.name + "_" + nodeCounter;
				pipeSpline.pipeSplineNodesArray.Add(pipeSplineNode);
				SupportSpawner();
				distance += nodeDistance;
				nodeCounter++;
			}
		}

        public void SmoothNodesZ() {
			if (PipelineGameManager.instance.ZAxis == Axis.zAxis) {
				Vector3 nPos = pipeSpline.pipeSplineNodesArray[0].transform.position;
				foreach (PipeSplineNode psNode in pipeSpline.pipeSplineNodesArray) {
					psNode.gameObject.transform.position = new Vector3(nPos.x, nPos.y, psNode.gameObject.transform.position.z);
				}
			}
        }

        public void SmoothNodesX() {
			if (PipelineGameManager.instance.XAxis == Axis.xAxis) {
				Vector3 nPos = pipeSpline.pipeSplineNodesArray[0].transform.position;
				foreach (PipeSplineNode psNode in pipeSpline.pipeSplineNodesArray) {
					psNode.gameObject.transform.position = new Vector3(psNode.gameObject.transform.position.x, nPos.y, nPos.z);
				}
			}
        }

        public void ToggleNodes() {
            isActivated = !isActivated;
            if (isActivated) {
                ActivateHandleNodes();
            } else {
                DeactivateHandleNodes();
            }
        }

        private void ActivateHandleNodes() {
            foreach (PipeSplineNode psNode in pipeSpline.pipeSplineNodesArray) {
                psNode.gameObject.SetActive(true);
            }
        }
        private void DeactivateHandleNodes() {
            foreach (PipeSplineNode psNode in pipeSpline.pipeSplineNodesArray) {
                psNode.gameObject.SetActive(false);
            }
        }

        public void SupportSpawner() {
			if(isSupportsToggled){
				foreach (GameObject support in supports) {
					GameObject.Destroy(support);
				}
				supports.Clear();
				RaycastHit hit;
				float heightAboveGround = 0;
				
				for (int segmentIdx = 0; segmentIdx < segments; segmentIdx++) {
					float param = (float)(segmentIdx) / segments;
					float num = param * 10.0f;
					if (num % 2 == 0) {

						Vector3 pos = pipeSpline.transform.InverseTransformPoint(pipeSpline.GetPositionOnSpline(param));
						Quaternion rot = pipeSpline.GetOrientationOnSpline(param) * Quaternion.Inverse(pipeSpline.transform.rotation);
						rot.x = 0.0f;
						rot.z = 0.0f;

						if (Physics.Raycast(
						pipeSpline.GetPositionOnSpline(param), -Vector3.up, out hit, Mathf.Infinity,layerMask)) {
							heightAboveGround = hit.distance;
							
							GameObject handgrip = Instantiate(handgripHighPrefab, new Vector3(pos.x, pos.y - highSupportPosOffset, pos.z), rot) as GameObject;
							handgrip.transform.localScale = new Vector3(handgrip.transform.localScale.x - highSupportScaleOffset, handgrip.transform.localScale.y,handgrip.transform.localScale.z - highSupportScaleOffset);
							handgrip.transform.SetParent(transform, false);
							supports.Add(handgrip); 
						}
					}
				}
			}
        }

        public void ToggleSupports() {
            isSupport = !isSupport;
            if (isSupport) {
                ActivateSuports();
            } else {
                DeactivateSupports();
            }
        }

        private void ActivateSuports() {
			isSupportsToggled = true;
            foreach (GameObject support in supports) {
				support.SetActive(true);
            }
        }

        private void DeactivateSupports() {
			isSupportsToggled = false;
            foreach (GameObject support in supports) {
                support.SetActive(false);
            }
        }

    }
}
