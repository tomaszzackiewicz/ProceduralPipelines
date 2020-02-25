namespace PipelineToolkit {

    public static class Matrices {

        public static readonly double[] HermiteMatrix = new double[] {
         2.0, -3.0,  0.0,  1.0,
        -2.0,  3.0,  0.0,  0.0,
         1.0, -2.0,  1.0,  0.0,
         1.0, -1.0,  0.0,  0.0
        };

        public static readonly double[] BezierMatrix = new double[] {
        -1.0,  3.0, -3.0,  1.0,
         3.0, -6.0,  3.0,  0.0,
        -3.0,  3.0,  0.0,  0.0,
         1.0,  0.0,  0.0,  0.0
        };

        public static readonly double[] BSplineMatrix = new double[] {
        -1.0/6.0,   3.0/6.0, - 3.0/6.0,  1.0/6.0,
         3.0/6.0, - 6.0/6.0,   0.0/6.0,  4.0/6.0,
        -3.0/6.0,   3.0/6.0,   3.0/6.0,  1.0/6.0,
         1.0/6.0,   0.0/6.0,   0.0/6.0,  0.0/6.0
        };

        public static readonly double[] LinearMatrix = new double[] {
        0.0,   0.0, - 1.0,  1.0,
        0.0,   0.0,   1.0,  0.0,
        0.0,   0.0,   0.0,  0.0,
        0.0,   0.0,   0.0,  0.0
        };
    }
}
