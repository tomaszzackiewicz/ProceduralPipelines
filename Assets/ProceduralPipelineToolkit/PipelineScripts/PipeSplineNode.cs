using UnityEngine;
using System.Collections;

namespace PipelineToolkit {

    public class PipeSplineNode : MonoBehaviour {
		
		private PipeSpline spline;
        private double posInSpline = 0.0;
        private double lengthNode = 0.0;
        private float customValue = 0.0f;
        

        public Transform Transform {
            get { 
				return transform; 
			}
        }
        public Vector3 Position {
            get { 
				return Transform.position; 
			}
            set { 
				Transform.position = value; 
			}
        }
        public Quaternion Rotation {
            get { 
				return Transform.rotation; 
			}
            set { 
				Transform.rotation = value; 
			}
        }
        public double PosInSpline {
            get { 
				return posInSpline; 
			}
			set{
				posInSpline = value;
			}
        }
        public double LengthNode {
            get { 
				return lengthNode; 
			}
			set{
				lengthNode = value;
			}
        }
		public float CustomValue {
            get { 
				return customValue; 
			}
			set{
				customValue = value;
			}
        }
		public PipeSpline Spline {
            get { 
				return spline; 
			}
			set{
				spline = value;
			}
        }
		

        public void ResetNode() {
            posInSpline = 0f;
            lengthNode = 0f;
        }


    }
}

