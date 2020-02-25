using UnityEngine;
using System;
using System.Collections.Generic;

namespace PipelineToolkit {

    public class PipeSpline : MonoBehaviour, ISpline {

        public List<PipeSplineNode> pipeSplineNodesArray = new List<PipeSplineNode>();
        public List<PipeSplineNode> pipeSplineNodesInternal = new List<PipeSplineNode>();
        public RotationMode rotationMode = RotationMode.Tangent;
        public bool autoClose = false;
        public float tension = 0.5f;
        public Vector3 tanUpVector = Vector3.up;
        private int interpolationAccuracy = 5;
        private LengthData lengthData = new LengthData();
		private PipelineNodes pipelineNodes;
		private int step;
		
		public List<PipeSplineNode> PipeSplineNodesArray {
            get { 
				return pipeSplineNodesArray; 
			}
        }
		public List<PipeSplineNode> PipeSplineNodesInternal {
            get { 
				return pipeSplineNodesInternal; 
			}
        }
        public float Length {
            get { 
				return (float)lengthData.length; 
			}
        }
		public bool AutoClose {
            get {
                return autoClose && !IsBezier;
            }
        }
		public float Tension {
            get { 
				return tension; 
			}
        }
        public int Step {
            get { 
				return IsBezier ? 3 : 1; 
			}
        }
        public int SegmentCount {
            get { 
				return (pipelineNodes.ControlNodeCount - 1) / Step; 
			}
        }
        public bool IsBezier {
            get {
                return Interpolations.InterpolationMode == InterpolationMode.Bezier;
            }
        }
		public int InterpolationAccuracy {
            get { 
				return interpolationAccuracy; 
			}
			set{
				interpolationAccuracy = value;
			}
        }
        public double InvertedAccuracy {
            get { 
				return 1.0 / interpolationAccuracy; 
			}
        }
        public PipeSplineNode[] PipeSplineNodes {
            get {
                if (pipeSplineNodesInternal == null){
                    pipeSplineNodesInternal = new List<PipeSplineNode>();
				}

                return pipeSplineNodesInternal.ToArray();
            }
        }
        public PipeSplineSegment[] PipeSplineSegments {
            get {
                PipeSplineSegment[] sSegments = new PipeSplineSegment[SegmentCount];

                for (int i = 0; i < sSegments.Length; i++){
                    sSegments[i] = new PipeSplineSegment(this, pipelineNodes.GetNode(i * Step, 0), pipelineNodes.GetNode(i * Step, 1));
				}

                return sSegments;
            }
        }
		private bool HasNodes {
            get { 
				return pipeSplineNodesInternal == null ? false : pipeSplineNodesInternal.Count > 0; }
        }
		
		public void Awake(){
			pipelineNodes = gameObject.GetComponent<PipelineNodes>();
		}

        public void LateUpdate() {
            if(AutomaticUpdater.Update()){
                UpdateSpline();
			}
        }

        public void UpdateSpline() {
            //PipeSplineNodesArray.Remove(null);

            for (int i = 0; i < pipeSplineNodesArray.Count; i++){
                pipeSplineNodesArray[i].Spline = this;
			}

            int relevantNodeCount = pipelineNodes.GetRelevantNodeCount(pipeSplineNodesArray.Count);

            if (pipeSplineNodesInternal == null){
                pipeSplineNodesInternal = new List<PipeSplineNode>();
			}

            pipeSplineNodesInternal.Clear();

            if (!pipelineNodes.EnoughNodes(relevantNodeCount)){
                return;
			}

            pipeSplineNodesInternal.AddRange(pipeSplineNodesArray.GetRange(0, relevantNodeCount));

            ReparameterizeCurve();
        }
		
		private void ReparameterizeCurve() {
            if (lengthData == null){
                lengthData = new LengthData();
			}

            lengthData.Calculate(this);
        }

        public Vector3 GetPositionOnSpline(float param) {
			Vector3 posOnSpline = Vector3.zero;
            if (HasNodes){
                posOnSpline = pipelineNodes.GetPositionInternal(RecalculateParameter(param));
			}
            return posOnSpline;
        }

        private Vector3 GetTangentToSpline(float param) {
			Vector3 tanOnSpline = Vector3.zero;
            if (HasNodes){
				tanOnSpline = pipelineNodes.GetTangentInternal(RecalculateParameter(param));
			}
            return tanOnSpline;
        }
		
		private float GetCustomValueOnSpline(float param) {
			float customValueOnSpline = 0.0f;
            if (HasNodes){
				customValueOnSpline = (float)pipelineNodes.GetValueInternal(RecalculateParameter(param));
			}
            return customValueOnSpline;
        }

        public Quaternion GetOrientationOnSpline(float param) {
            if (!HasNodes){
                return Quaternion.identity;
			}else{

				switch (rotationMode) {
					case RotationMode.Tangent:
						Vector3 tangent = GetTangentToSpline(param);

						if (tangent.sqrMagnitude == 0f || tanUpVector.sqrMagnitude == 0f){
							return Quaternion.identity;
						}

						return Quaternion.LookRotation(tangent.normalized, tanUpVector.normalized);

					case RotationMode.Node:
						return pipelineNodes.GetRotationInternal(RecalculateParameter(param));

					default:
						return Quaternion.identity;
				}
			}
        }
		
        /* private PipeSplineSegment GetSplineSegment(float param) {
            param = Mathf.Clamp(param, 0, 1f);

            for (int i = 0; i < pipelineNodes.ControlNodeCount - 1; i += Step) {
                if (param - pipeSplineNodesInternal[i].PosInSpline < pipeSplineNodesInternal[i].LengthNode){
                    return new PipeSplineSegment(this, pipelineNodes.GetNode(i, 0), pipelineNodes.GetNode(i, Step));
				}
            }
			
			PipeSplineSegment pipeSplineSegment = new PipeSplineSegment(this, pipelineNodes.GetNode(pipelineNodes.MaxNodeIndex(), 0), pipelineNodes.GetNode(pipelineNodes.MaxNodeIndex(), Step));

            return pipeSplineSegment;
        } */

        private float ConvertNormalizedParameterToDistance(float param) {
            return Length * param;
        }

        private float ConvertDistanceToNormalizedParameter(float param) {
            return (Length <= 0f) ? 0f : param / Length;
        }

        private SegmentParameter RecalculateParameter(double param) {
            if (param <= 0){
                return new SegmentParameter(0, 0);
			}else if (param > 1){
                return new SegmentParameter(pipelineNodes.MaxNodeIndex(), 1);
			}

            double invertedAccuracy = InvertedAccuracy;

            for (int i = lengthData.subSegmentPosition.Length - 1; i >= 0; i--) {
                if (lengthData.subSegmentPosition[i] < param) {
                    int floorIndex = (i - (i % (interpolationAccuracy)));

                    int normalizedIndex = floorIndex * Step / interpolationAccuracy;
                    double normalizedParam = invertedAccuracy * (i - floorIndex + (param - lengthData.subSegmentPosition[i]) / lengthData.subSegmentLength[i]);

                    if (normalizedIndex >= pipelineNodes.ControlNodeCount - 1){
                        return new SegmentParameter(pipelineNodes.MaxNodeIndex(), 1.0);
					}

                    return new SegmentParameter(normalizedIndex, normalizedParam);
                }
            }

            return new SegmentParameter(pipelineNodes.MaxNodeIndex(), 1);
        }

        public double GetSegmentLengthInternal(int idxFirstPoint, double startValue, double endValue, double stepIA) {
            Vector3 currentPos;

            double pPosX = 0.0; 
			double pPosY = 0.0; 
			double pPosZ = 0.0;
            double cPosX = 0.0; 
			double cPosY = 0.0; 
			double cPosZ = 0.0;

            double length = 0;

            for (double i = startValue + stepIA; i < (endValue + stepIA * 0.5); i += stepIA) {
                SegmentParameter segmentParameter = new SegmentParameter(idxFirstPoint, i);
				currentPos = pipelineNodes.GetPositionInternal(segmentParameter);

                //cPosX = pPosX - currentPos.x;
                //cPosY = pPosY - currentPos.y;
                //cPosZ = pPosZ - currentPos.z;
				
				pPosX = currentPos.x;
                pPosY = currentPos.y;
                pPosZ = currentPos.z;

                //length += Math.Sqrt(cPosX * cPosX + cPosY * cPosY + cPosZ * cPosZ);
				length += Math.Sqrt(pPosX * pPosX + pPosY * pPosY + pPosZ * pPosZ);
                //pPosX = currentPos.x;
                //pPosY = currentPos.y;
                //pPosZ = currentPos.z;
            }

            return length;
        }

        

    }
}

