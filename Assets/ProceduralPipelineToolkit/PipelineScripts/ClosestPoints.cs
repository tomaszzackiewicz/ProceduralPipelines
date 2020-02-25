using UnityEngine;

namespace PipelineToolkit {

    public static class ClosestPoints {

        private delegate float DistanceFunction(Vector3 splinePos);
        private static PipeSpline pSplineStatic;

        static ClosestPoints() {
            PipeSpline pSpline = new PipeSpline();
            pSplineStatic = pSpline;
        }

        public static float GetClosestPointParam(Vector3 point, int iterations, float start = 0, float end = 1, float step = .01f) {
            return GetClosestPointParamIntern((splinePos) => (point - splinePos).sqrMagnitude, iterations, start, end, step);
        }

        public static float GetClosestPointParamToRay(Ray ray, int iterations, float start = 0, float end = 1, float step = .01f) {
            return GetClosestPointParamIntern((splinePos) => Vector3.Cross(ray.direction, splinePos - ray.origin).sqrMagnitude, iterations, start, end, step);
        }

        public static float GetClosestPointParamToPlane(Plane plane, int iterations, float start = 0, float end = 1, float step = .01f) {
            return GetClosestPointParamIntern((splinePos) => plane.GetDistanceToPoint(splinePos), iterations, start, end, step);
        }

        private static float GetClosestPointParamIntern(DistanceFunction distFnc, int iterations, float start, float end, float step) {
            iterations = Mathf.Clamp(iterations, 0, 5);

            float minParam = GetClosestPointParamOnSegmentIntern(distFnc, start, end, step);

            for (int i = 0; i < iterations; i++) {
                float searchOffset = Mathf.Pow(10.0f, -(i + 2.0f));

                start = Mathf.Clamp01(minParam - searchOffset);
                end = Mathf.Clamp01(minParam + searchOffset);
                step = searchOffset * 0.1f;

                minParam = GetClosestPointParamOnSegmentIntern(distFnc, start, end, step);
            }

            return minParam;
        }

        private static float GetClosestPointParamOnSegmentIntern(DistanceFunction distFnc, float start, float end, float step) {
            float minDistance = Mathf.Infinity;
            float minParam = 0f;

            for (float param = start; param <= end; param += step) {
                float distance = distFnc(pSplineStatic.GetPositionOnSpline(param));

                if (minDistance > distance) {
                    minDistance = distance;
                    minParam = param;
                }
            }

            return minParam;
        }

    }
}
