using UnityEngine;
using System;

namespace PipelineToolkit {

    public class PipeSplineSegment {

        private readonly PipeSpline parentSpline;
        private readonly PipeSplineNode startNode;
        private readonly PipeSplineNode endNode;

        public PipeSpline ParentSpline { 
			get { 
				return parentSpline; 
			} 
		}
        public PipeSplineNode StartNode { 
			get { 
				return startNode; 
				} 
			}
        public PipeSplineNode EndNode { 
			get { 
				return endNode; 
			} 
		}

        public float Length {
            get { 
				return (float)(startNode.LengthNode * parentSpline.Length); 
			}
        }
        public float NormalizedLength {
            get { 
				return (float)startNode.LengthNode; 
			}
        }
		
        public PipeSplineSegment(PipeSpline pSpline, PipeSplineNode sNode, PipeSplineNode eNode) {
            if (pSpline != null) {
                parentSpline = pSpline;

                startNode = sNode;
                endNode = eNode;
            } else {
                throw new ArgumentNullException("pSpline");
            }
        }

        public float ConvertSegmentToSplineParameter(float param) {
            return (float)(startNode.PosInSpline + param * startNode.LengthNode);
        }

        public float ConvertSplineToSegmentParamter(float param) {
            if (param < startNode.PosInSpline){
                return 0;
			}

            if (param >= endNode.PosInSpline){
                return 1;
			}

            return (float)((param - startNode.PosInSpline) / startNode.LengthNode);
        }

        public float ClampParameterToSegment(float param) {
            if (param < startNode.PosInSpline){
                return (float)startNode.PosInSpline;
			}

            if (param >= endNode.PosInSpline){
                return (float)endNode.PosInSpline;
			}

            return param;
        }
    }
}

