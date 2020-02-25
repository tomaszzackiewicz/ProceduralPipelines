using UnityEngine;
using System.Collections.Generic;

namespace PipelineToolkit {

	public interface ISpline{
		
		List<PipeSplineNode> PipeSplineNodesArray {
            get;
        }
		List<PipeSplineNode> PipeSplineNodesInternal {
            get;
        }
		float Length {
			get;
		}
		bool AutoClose {
			get;
		}
		float Tension {
            get;
        }
		int Step {
			get;
		}
		int SegmentCount {
			get;
		}
		bool IsBezier {
			get;
		}
		int InterpolationAccuracy {
			get;
			set;
		}
		double InvertedAccuracy {
			get;
		}
		PipeSplineNode[] PipeSplineNodes {
			get;
		}
		PipeSplineSegment[] PipeSplineSegments {
			get;
		}	
		
		void UpdateSpline();
		
		Vector3 GetPositionOnSpline(float param);
		
		Quaternion GetOrientationOnSpline(float param);
		
		double GetSegmentLengthInternal(int idxFirstPoint, double startValue, double endValue, double stepIA);
		
	}
}
