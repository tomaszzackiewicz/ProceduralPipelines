using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PipelineToolkit {

    public class PipelineNodes : MonoBehaviour {
        
        
        private TangentMode tangentMode = TangentMode.UseTangents;
        private ISpline pipeSpline;
		
		public TangentMode TangentMode{
			get{
				return tangentMode;
			}
			set{
				tangentMode = value;
			}
		}

        private void Awake() {
            pipeSpline = gameObject.GetComponent<ISpline>();
           
        }
        public int ControlNodeCount {
            get {
                return pipeSpline.AutoClose ? pipeSpline.PipeSplineNodesInternal.Count + 1 : pipeSpline.PipeSplineNodesInternal.Count;
            }
        }

        public PipeSplineNode GetNode(int idxNode, int idxOffset) {
            idxNode += idxOffset;
			if(pipeSpline != null){
				if (pipeSpline.AutoClose) {
					return pipeSpline.PipeSplineNodesInternal[(idxNode % pipeSpline.PipeSplineNodesInternal.Count + pipeSpline.PipeSplineNodesInternal.Count) % pipeSpline.PipeSplineNodesInternal.Count];
				} else {
					return pipeSpline.PipeSplineNodesInternal[Mathf.Clamp(idxNode, 0, pipeSpline.PipeSplineNodesInternal.Count - 1)];
				}
			}
            return null;
        }

        public int MaxNodeIndex() {
            return ControlNodeCount - pipeSpline.Step - 1;
        }

        public void UpdateNodeScale(float scale) {
            foreach (PipeSplineNode psNode in pipeSpline.PipeSplineNodesArray) {
                psNode.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        public int GetRelevantNodeCount(int nodeCount) {
            int relevantNodeCount = nodeCount;

            if (pipeSpline != null && pipeSpline.IsBezier) {
                if (nodeCount < 7) {
                    relevantNodeCount -= (nodeCount) % 4;
                } else {
                    relevantNodeCount -= (nodeCount - 4) % 3;
                }
            }

            return relevantNodeCount;
        }

        public bool EnoughNodes(int nodeCount) {
            if (pipeSpline != null && pipeSpline.IsBezier)
                return (nodeCount < 4) ? false : true;
            else
                return (nodeCount < 2) ? false : true;
        }

        public Vector3 GetPositionInternal(SegmentParameter sParam) {
            PipeSplineNode n0;
            PipeSplineNode n1;
            PipeSplineNode n2;
            PipeSplineNode n3;

            GetAdjacentNodes(sParam, out n0, out n1, out n2, out n3);

            Vector3 P0 = n0.Position;
            Vector3 P1 = n1.Position;
            Vector3 P2 = n2.Position;
            Vector3 P3 = n3.Position;

            RecalcVectors(n0, n1, ref P2, ref P3);

            return Interpolations.InterpolatePosition(sParam.NormalizedParam, P0, P1, P2, P3);
        }

        public Vector3 GetTangentInternal(SegmentParameter sParam) {
            PipeSplineNode n0;
            PipeSplineNode n1;
            PipeSplineNode n2;
            PipeSplineNode n3;

            GetAdjacentNodes(sParam, out n0, out n1, out n2, out n3);

            Vector3 P0 = n0.Position;
            Vector3 P1 = n1.Position;
            Vector3 P2 = n2.Position;
            Vector3 P3 = n3.Position;

            RecalcVectors(n0, n1, ref P2, ref P3);

            return Interpolations.InterpolateTangent(sParam.NormalizedParam, P0, P1, P2, P3);
        }

        public double GetValueInternal(SegmentParameter sParam) {
            PipeSplineNode n0;
            PipeSplineNode n1;
            PipeSplineNode n2;
            PipeSplineNode n3;

            GetAdjacentNodes(sParam, out n0, out n1, out n2, out n3);

            double P0 = n0.CustomValue;
            double P1 = n1.CustomValue;
            double P2 = n2.CustomValue;
            double P3 = n3.CustomValue;

            RecalcScalars(n0, n1, ref P2, ref P3);

            return Interpolations.InterpolateValue(sParam.NormalizedParam, P0, P1, P2, P3);
        }

        public Quaternion GetRotationInternal(SegmentParameter sParam) {
            Quaternion Q0 = GetNode(sParam.NormalizedIndex, -1).Transform.rotation;
            Quaternion Q1 = GetNode(sParam.NormalizedIndex, 0).Transform.rotation;
            Quaternion Q2 = GetNode(sParam.NormalizedIndex, 1).Transform.rotation;
            Quaternion Q3 = GetNode(sParam.NormalizedIndex, 2).Transform.rotation;

            Quaternion T1 = Quaternions.GetSquadIntermediate(Q0, Q1, Q2);
            Quaternion T2 = Quaternions.GetSquadIntermediate(Q1, Q2, Q3);

            return Quaternions.GetQuatSquad(sParam.NormalizedParam, Q1, Q2, T1, T2);
        }

        public void GetAdjacentNodes(SegmentParameter sParam, out PipeSplineNode node0, out PipeSplineNode node1, out PipeSplineNode node2, out PipeSplineNode node3) {
            
			switch (Interpolations.InterpolationMode) {
                case InterpolationMode.BSpline:
                    node0 = GetNode(sParam.NormalizedIndex, -1);
                    node1 = GetNode(sParam.NormalizedIndex, 0);
                    node2 = GetNode(sParam.NormalizedIndex, 1);
                    node3 = GetNode(sParam.NormalizedIndex, 2);

                    return;

                case InterpolationMode.Hermite:
                    node0 = GetNode(sParam.NormalizedIndex, 0);
                    node1 = GetNode(sParam.NormalizedIndex, 1);
                    node2 = GetNode(sParam.NormalizedIndex, -1);
                    node3 = GetNode(sParam.NormalizedIndex, 2);

                    return;

                case InterpolationMode.Linear:
                case InterpolationMode.Bezier:
                default:
                    node0 = GetNode(sParam.NormalizedIndex, 0);
                    node1 = GetNode(sParam.NormalizedIndex, 1);
                    node2 = GetNode(sParam.NormalizedIndex, 2);
                    node3 = GetNode(sParam.NormalizedIndex, 3);

                    return;
            }
        }

        public void RecalcVectors(PipeSplineNode node0, PipeSplineNode node1, ref Vector3 P2, ref Vector3 P3) {
            if (Interpolations.InterpolationMode != InterpolationMode.Hermite){
                return;
			}

            if (tangentMode == TangentMode.UseNodeForwardVector) {
                P2 = node0.Transform.forward * pipeSpline.Tension;
                P3 = node1.Transform.forward * pipeSpline.Tension;
            } else {
                P2 = node1.Position - P2;
                P3 = P3 - node0.Position;

                if (tangentMode != TangentMode.UseTangents) {
                    P2.Normalize();
                    P3.Normalize();
                }

                P2 = P2 * pipeSpline.Tension;
                P3 = P3 * pipeSpline.Tension;
            }
        }

        public void RecalcScalars(PipeSplineNode node0, PipeSplineNode node1, ref double P2, ref double P3) {
            if (Interpolations.InterpolationMode != InterpolationMode.Hermite){
                return;
			}

            P2 = node1.CustomValue - P2;
            P3 = P3 - node0.CustomValue;

            P2 = P2 * pipeSpline.Tension;
            P3 = P3 * pipeSpline.Tension;
        }

        
    }
}
