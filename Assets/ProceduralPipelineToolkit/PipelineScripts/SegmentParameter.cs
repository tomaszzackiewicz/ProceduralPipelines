namespace PipelineToolkit {

	public sealed class SegmentParameter {
		
		private double normalizedParam;
		private int normalizedIndex;
		
		public double NormalizedParam{
			get{
				return normalizedParam;
			}
		}
		
		public int NormalizedIndex{
			get{
				return normalizedIndex;
			}
		}

		public SegmentParameter() {
			normalizedParam = 0;
			normalizedIndex = 0;
		}

		public SegmentParameter(int maxIndex, double param) {
			normalizedParam = param;
			normalizedIndex = maxIndex;
		}
	}
}
