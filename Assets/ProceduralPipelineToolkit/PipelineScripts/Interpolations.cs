using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipelineToolkit {

    public static class Interpolations {

        private static InterpolationMode interpolationMode = PipelineToolkit.InterpolationMode.Hermite;

        public static InterpolationMode InterpolationMode {
            get {
                return interpolationMode;
            }
            set {
                interpolationMode = value;
            }
        }

        public static Vector3 InterpolatePosition(double t, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3) {
            double t2 = t * t;
            double t3 = t2 * t;

            double b1; double b2;
            double b3; double b4;

            double[] matrix = GetMatrix(InterpolationMode);

            b1 = matrix[0] * t3 + matrix[1] * t2 + matrix[2] * t + matrix[3];
            b2 = matrix[4] * t3 + matrix[5] * t2 + matrix[6] * t + matrix[7];
            b3 = matrix[8] * t3 + matrix[9] * t2 + matrix[10] * t + matrix[11];
            b4 = matrix[12] * t3 + matrix[13] * t2 + matrix[14] * t + matrix[15];

            return new Vector3((float)(b1 * v0.x + b2 * v1.x + b3 * v2.x + b4 * v3.x),
                               (float)(b1 * v0.y + b2 * v1.y + b3 * v2.y + b4 * v3.y),
                               (float)(b1 * v0.z + b2 * v1.z + b3 * v2.z + b4 * v3.z));
        }

        public static Vector3 InterpolateTangent(double t, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3) {
            double t2 = t * t;

            double b1; double b2;
            double b3; double b4;

            double[] matrix = GetMatrix(InterpolationMode);

            t = t * 2.0;
            t2 = t2 * 3.0;

            b1 = matrix[0] * t2 + matrix[1] * t + matrix[2];
            b2 = matrix[4] * t2 + matrix[5] * t + matrix[6];
            b3 = matrix[8] * t2 + matrix[9] * t + matrix[10];
            b4 = matrix[12] * t2 + matrix[13] * t + matrix[14];

            return new Vector3((float)(b1 * v0.x + b2 * v1.x + b3 * v2.x + b4 * v3.x),
                               (float)(b1 * v0.y + b2 * v1.y + b3 * v2.y + b4 * v3.y),
                               (float)(b1 * v0.z + b2 * v1.z + b3 * v2.z + b4 * v3.z));
        }

        public static double InterpolateValue(double t, double v0, double v1, double v2, double v3) {
            double t2 = t * t;
            double t3 = t2 * t;

            double b1; double b2;
            double b3; double b4;

            double[] matrix = GetMatrix(InterpolationMode);

            b1 = matrix[0] * t3 + matrix[1] * t2 + matrix[2] * t + matrix[3];
            b2 = matrix[4] * t3 + matrix[5] * t2 + matrix[6] * t + matrix[7];
            b3 = matrix[8] * t3 + matrix[9] * t2 + matrix[10] * t + matrix[11];
            b4 = matrix[12] * t3 + matrix[13] * t2 + matrix[14] * t + matrix[15];

            return b1 * v0 + b2 * v1 + b3 * v2 + b4 * v3;
        }

        private static double[] GetMatrix(InterpolationMode iMode) {
            switch (iMode) {
                case PipelineToolkit.InterpolationMode.Bezier:
                    return Matrices.BezierMatrix;
                case PipelineToolkit.InterpolationMode.BSpline:
                    return Matrices.BSplineMatrix;
                case PipelineToolkit.InterpolationMode.Hermite:
                    return Matrices.HermiteMatrix;
                case PipelineToolkit.InterpolationMode.Linear:
                    return Matrices.LinearMatrix;

                default:
                    return Matrices.LinearMatrix;
            }
        }
    }
}
