namespace PipelineToolkit {

    public enum Axis {
        xAxis,
        yAxis,
        zAxis,
        none
    }

    public enum TangentMode {
        UseNormalizedTangents,
        UseTangents,
        UseNodeForwardVector
    }

    public enum RotationMode {
        None,
        Node,
        Tangent
    }

    public enum InterpolationMode {
        Hermite,
        Bezier,
        BSpline,
        Linear
    }

    public enum UVMode {
        Normal,
        Swap,
        DontInterpolate
    }
	
	public enum UpdateMode{
		DontUpdate,
		EveryFrame,
		EveryXFrames,
		EveryXSeconds 
	}
	
	public enum PipelineType{
		Single,
		Multi
	}
}



