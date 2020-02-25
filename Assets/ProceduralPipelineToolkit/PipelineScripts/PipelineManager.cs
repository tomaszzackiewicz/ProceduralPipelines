using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

namespace PipelineToolkit {

    public class PipelineManager : MonoBehaviour {

        public GameObject pipelinePrefab;
        public List<GameObject> pipelines = new List<GameObject>();
        public int segments;
        
		private PipeSpline pipeSpline;
        private PipeSplineMesh pipeSplineMesh;
		private int id;
		private float diameterX;
		private float diameterY;
		private int pipesCount = 1;
		private float offset = 8.0f;
		private bool isCreatingAllowed = true;
		
		public int Id { 
			get{
				return id;
			}
			set {
                if (value > 0) {
                    id = value;
                }
            }
		}
		public float DiameterX { 
			get{
				return diameterX = pipeSplineMesh.xyScale.x;
			} 
		}
		public float DiameterY { 
			get{
				return diameterY = pipeSplineMesh.xyScale.y;
			} 
		}
		public int PipesCount { 
			get{
				return pipesCount;
			} 
			set {
                if (value > 0) {
                    pipesCount = value;
                }
            }
		}
        public int Segments {
            get {
                return segments;
            }
            set {
                if (value > 0) {
                    segments = value;
                }
            }
        }
		
		public float Offset {
            get {
                return offset;
            }
            set {
                if (value > 0) {
                    offset = value;
                }
            }
        }
        
		private void LateUpdate() {
            if(AutomaticUpdater.Update()){
				if(pipelines.Count > 0){
					UpdateSupports();
				}
            }
        }
		
		public void PipelineCreator() {
			
			PipeSystemCreation();
        }
		
		private void PipeSystemCreation(){
			if(pipelines.Count > 0){
				isCreatingAllowed = false;
			}else{
				isCreatingAllowed = true;
			}
			if(isCreatingAllowed){
				
				int counter = 0;
				int number = 0;
				float posOffset = 0.0f;
				pipelines.RemoveAll(x => x == null);
				PipelineController pipelineController = null;
				while (pipesCount > number) {
					GameObject pipeline = Instantiate(pipelinePrefab, transform.localPosition, Quaternion.identity) as GameObject;
					pipeline.transform.SetParent(transform, true);
					
					if (PipelineGameManager.instance.XAxis == Axis.xAxis) {
					   pipeline.transform.localPosition = transform.localPosition + new Vector3(0.0f, 0.0f, posOffset);
					} else if (PipelineGameManager.instance.YAxis == Axis.yAxis) {
					   pipeline.transform.localPosition = transform.localPosition + new Vector3(0.0f, 0.0f, posOffset);
					} else if (PipelineGameManager.instance.ZAxis == Axis.zAxis) {
					   pipeline.transform.localPosition = transform.localPosition + new Vector3(posOffset, 0.0f, 0.0f);
					} else {
					   
					}
					
					pipelineController = pipeline.GetComponentInChildren<PipelineController>();
					pipelineController.pipelineIndex = number;
					
					pipeline.name = pipelinePrefab.name + "_" + number;
					pipelines.Add(pipeline);
					PipelineGameManager.instance.helpers.Add(pipeline);
					if (pipesCount > 1) {
						pipelineController.pipelineType = PipelineType.Multi;
					}else{
						pipelineController.pipelineType = PipelineType.Single;
					}

					posOffset += offset;
					number++;
				}
				GetPipelines();
				
				StartCreatingCoroutines();
			}
		}

        public void StartCreatingCoroutines(){
			
			StartCoroutine(CreatePipesAutomaticCor());
		}

        private void GetPipelines() {
            foreach (GameObject pipeline in pipelines) {
                PipeSpline pipeSplineItem = pipeline.GetComponent<PipeSpline>();
                pipeSpline = pipeSplineItem;
                pipeSplineMesh = pipeline.GetComponentInChildren<PipeSplineMesh>();
                pipeSplineMesh.segmentCount = segments;
            }
        }
		
		IEnumerator CreatePipesAutomaticCor(){
			yield return new WaitForSeconds(1.0f);
			yield return StartCoroutine(CreateMultiPipesCor());
			yield return StartCoroutine(ParentNodesToMasterCor());
		}

        IEnumerator CreateMultiPipesCor() {
            CreateNodes();
            UpdateSpline();
            GroupPipelines();
			yield return null;
		}
				
        private void CreateNodes() {
            foreach (GameObject pipeline in pipelines) {
                PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
                if (pipelineRoot) {
                    PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();
                    pipelineController.CreateNodesAuto();
                }
            }
        }

        private void UpdateSpline() {
            foreach (GameObject pipeline in pipelines) {
                PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
                if (pipelineRoot) {
                    PipeSpline pipeSpline = pipelineRoot.child.GetComponent<PipeSpline>();
                    pipeSpline.UpdateSpline();
                }
            }
        }
		
		private void GroupPipelines() {
            GameObject parent = null;
            for (int i = 0; i < pipelines.Count; i++) {
				PipelineRoot pipelineRoot = pipelines[i].GetComponent<PipelineRoot>();
				if (pipelineRoot) {
					
					PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();
					if (pipelineController.isMaster && pipelineController.pipelineType == PipelineType.Multi) {
						pipelineRoot.IsDragabble = true;
						parent = pipelines[i];
						break;
					}
				}
            }
			if(parent){
				for (int i = 0; i < pipelines.Count; i++) {
					PipelineRoot pipelineRoot = pipelines[i].GetComponent<PipelineRoot>();
					if (pipelineRoot) {
						
						PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();
						if(!pipelineController.isMaster && pipelineController.pipelineType == PipelineType.Multi){
							pipelineRoot.IsDragabble = false;
							pipelines[i].transform.SetParent(parent.transform, true);
						}
					}
				}
			}
        }
		
		IEnumerator ParentNodesToMasterCor(){
			if(pipesCount > 1){
				ParentNodesInMultiPipes();
			}
			UpdateSupports();
			yield return null;
		}
		
		private void ParentNodesInMultiPipes(){
			List<PipeSplineNode> parentNodes = new List<PipeSplineNode>();
			parentNodes = GetParentNodes();
			for (int i = 0; i < pipelines.Count; i++) {
				if (pipelines[i] == pipelines[0]) {
                    continue;
                }
				List<PipeSplineNode> childNodes = new List<PipeSplineNode>();
				childNodes = GetChildNodes(pipelines[i]);
				
				for (int j = 0; j < childNodes.Count; j++) {
					childNodes[j].gameObject.transform.SetParent(parentNodes[j].gameObject.transform,true);	
				}
			}
		}
		
		private List<PipeSplineNode> GetParentNodes(){
			List<PipeSplineNode> parentNodes = new List<PipeSplineNode>();
			GameObject parent = GetMultiPipeParent();
			
			PipelineRoot pipelineRoot = parent.GetComponent<PipelineRoot>();
			if(pipelineRoot){
				PipeSpline pipeSpline = pipelineRoot.child.GetComponent<PipeSpline>();PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();
				if(pipelineController.pipelineType == PipelineType.Multi){
					foreach(PipeSplineNode node in pipeSpline.pipeSplineNodesArray){
						parentNodes.Add(node);
					}
				}
			}
			return parentNodes;
		}
		
		private GameObject GetMultiPipeParent(){
			GameObject parent = null;
			foreach(GameObject pipeline in pipelines){
				PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
				if(pipelineRoot){
					PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();

					if(pipelineController.isMaster == true){
						parent = pipeline;
					}
				}
			}
			return parent;
		}
		
		private List<PipeSplineNode> GetChildNodes(GameObject child){
			List<PipeSplineNode> childNodes = new List<PipeSplineNode>();
			PipelineRoot pipelineRoot = child.GetComponent<PipelineRoot>();
			if(pipelineRoot){
				PipeSpline pipeSpline = pipelineRoot.child.GetComponent<PipeSpline>();
				PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();
				foreach(PipeSplineNode node in pipeSpline.pipeSplineNodesArray){
					if(pipelineController.pipelineType == PipelineType.Multi){
						childNodes.Add(node);
					}
				}
			}
			return childNodes;
		}
		
		public void UpdateSupports(){
			if(pipelines.Count > 0){
				RemoveObject();
				foreach(GameObject pipeline in pipelines){
					PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
					if (pipelineRoot) {
						PipelineController pipelineController = pipelineRoot.child.GetComponent<PipelineController>();
						pipelineController.SupportSpawner();
					}
				}
			}
		}
		
		public void UpdateScale(float scale){
			foreach(GameObject pipeline in pipelines){
				PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
                if (pipelineRoot) {
                    PipeSplineMesh pipeSplineMesh = pipelineRoot.child.GetComponent<PipeSplineMesh>();
                    pipeSplineMesh.xyScale = new Vector2(scale, scale);
                }
			}
		}
		
		public void UpdateNodeScale(float scale){
			foreach(GameObject pipeline in pipelines){
				PipelineRoot pipelineRoot = pipeline.GetComponent<PipelineRoot>();
                if (pipelineRoot) {
                    PipelineNodes pipelineNodes = pipelineRoot.child.GetComponent<PipelineNodes>();
                    pipelineNodes.UpdateNodeScale(scale);
                }
			}
		}
		
		private void RemoveObject(){
			pipelines.RemoveAll(item => item == null);
		}

    }
}
