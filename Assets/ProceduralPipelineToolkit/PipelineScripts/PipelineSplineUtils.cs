using UnityEngine;

namespace PipelineToolkit {
	public static class PipelineSplineUtils {

		public static float WrapValue(float v, float start, float end, WrapMode wMode){
			switch(wMode){
				case WrapMode.Clamp:
				case WrapMode.ClampForever:
					return Mathf.Clamp( v, start, end );
				case WrapMode.Default:
				case WrapMode.Loop:
					return Mathf.Repeat( v, end - start ) + start;
				case WrapMode.PingPong:
					return Mathf.PingPong( v, end - start ) + start;
				default:
					return v;
			}
		}
	}
}
