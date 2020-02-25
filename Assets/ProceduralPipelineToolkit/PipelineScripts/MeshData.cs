using UnityEngine;

namespace PipelineToolkit {

    public class MeshData {

        private Vector3[] vertices;
        private Vector2[] uvCoord;
        private Vector3[] normals;
        private Vector4[] tangents;

        private int[] triangles;

        private Bounds bounds;

        public bool HasNormals {
            get { return normals.Length > 0; }
        }
        public bool HasTangents {
            get { return tangents.Length > 0; }
        }
        public int VertexCount {
            get { return vertices.Length; }
        }
        public int TriangleCount {
            get { return triangles.Length; }
        }
		public Vector3[] Vertices {
            get { return vertices; }
        }
		public Vector2[] UvCoord {
            get { return uvCoord; }
        }
		public Vector3[] Normals {
            get { return normals; }
        }
		public Vector4[] Tangents {
            get { return tangents; }
        }
		public int[] Triangles {
            get { return triangles; }
        }
		public Bounds Bounds {
            get { return bounds; }
        }

        public MeshData(Mesh mesh) {
            vertices = mesh.vertices;
            normals = mesh.normals;
            tangents = mesh.tangents;
            uvCoord = mesh.uv;

            triangles = mesh.triangles;

            bounds = mesh.bounds;
        }

        public MeshData(MeshData mData, int segmentCount) {
            vertices = new Vector3[mData.vertices.Length * segmentCount];
            uvCoord = new Vector2[mData.uvCoord.Length * segmentCount];
            normals = new Vector3[mData.normals.Length * segmentCount];
            tangents = new Vector4[mData.tangents.Length * segmentCount];
            triangles = new int[mData.triangles.Length * segmentCount];
        }

        public void AssignToMesh(Mesh mesh) {
            mesh.vertices = vertices;
            mesh.uv = uvCoord;

            if (HasNormals)
                mesh.normals = normals;

            if (HasTangents)
                mesh.tangents = tangents;

            mesh.triangles = triangles;
        }
    }
}
