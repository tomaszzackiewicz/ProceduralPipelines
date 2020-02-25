namespace PipelineToolkit {

    public class LengthData {
		
        public double[] subSegmentLength;
        public double[] subSegmentPosition;
        public double length;
		
		public double[] SubSegmentLength{
			get{
				return subSegmentLength;
			}
		}
		public double[] SubSegmentPosition{
			get{
				return subSegmentPosition;
			}
		}
		public double Length{
			get{
				return length;
			}
		}

        public void Calculate(PipeSpline spline) {
            int subsegmentCount = spline.SegmentCount * spline.InterpolationAccuracy;
            double invertedAccuracy = 1.0 / spline.InterpolationAccuracy;

            subSegmentLength = new double[subsegmentCount];
            subSegmentPosition = new double[subsegmentCount];

            length = 0.0;

            for (int i = 0; i < subsegmentCount; i++) {
                subSegmentLength[i] = 0.0;
                subSegmentPosition[i] = 0.0;
            }

            for (int i = 0; i < spline.SegmentCount; i++) {
                for (int j = 0; j < spline.InterpolationAccuracy; j++) {
                    int index = i * spline.InterpolationAccuracy + j;

                    subSegmentLength[index] = spline.GetSegmentLengthInternal(i * spline.Step, j * invertedAccuracy, (j + 1) * invertedAccuracy, 0.2 * invertedAccuracy);

                    length += subSegmentLength[index];
                }
            }

            for (int i = 0; i < spline.SegmentCount; i++) {
                for (int j = 0; j < spline.InterpolationAccuracy; j++) {
                    int index = i * spline.InterpolationAccuracy + j;

                    subSegmentLength[index] /= length;

                    if (index < subSegmentPosition.Length - 1)
                        subSegmentPosition[index + 1] = subSegmentPosition[index] + subSegmentLength[index];
                }
            }

            SetupSplinePositions(spline);
        }

        private void SetupSplinePositions(PipeSpline spline) {
            foreach (PipeSplineNode node in spline.pipeSplineNodesInternal)
                node.ResetNode();

            for (int i = 0; i < subSegmentLength.Length; i++)
                spline.pipeSplineNodesInternal[((i - (i % spline.InterpolationAccuracy)) / spline.InterpolationAccuracy) * spline.Step].LengthNode += subSegmentLength[i];

            for (int i = 0; i < spline.pipeSplineNodesInternal.Count - spline.Step; i += spline.Step)
                spline.pipeSplineNodesInternal[i + spline.Step].PosInSpline = spline.pipeSplineNodesInternal[i].PosInSpline + spline.pipeSplineNodesInternal[i].LengthNode;

            if (spline.IsBezier) {
                for (int i = 0; i < spline.pipeSplineNodesInternal.Count - spline.Step; i += spline.Step) {
                    spline.pipeSplineNodesInternal[i + 1].PosInSpline = spline.pipeSplineNodesInternal[i].PosInSpline;
                    spline.pipeSplineNodesInternal[i + 2].PosInSpline = spline.pipeSplineNodesInternal[i].PosInSpline;
                    spline.pipeSplineNodesInternal[i + 1].LengthNode = 0.0;
                    spline.pipeSplineNodesInternal[i + 2].LengthNode = 0.0;
                }
            }

            if (!spline.AutoClose){
                spline.pipeSplineNodesInternal[spline.pipeSplineNodesInternal.Count - 1].PosInSpline = 1.0;
			}
        }
    }
}
