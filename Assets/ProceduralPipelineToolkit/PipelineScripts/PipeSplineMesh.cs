using UnityEngine;
using System;
using System.Collections.Generic;

namespace PipelineToolkit {

    //[ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class PipeSplineMesh : MonoBehaviour {

        public PipeSpline spline;
        public Mesh baseMesh;
        public Mesh cylinder;
        public Mesh cube;
        [HideInInspector]
        public int segmentCount = 50;
        public UVMode uvMode = UVMode.Normal;
        public Vector2 uvScale = Vector2.one;
        public Vector2 xyScale = Vector2.one;
        public int splineSegment = -1;
		[HideInInspector]
        public bool isCylinder = false;
        [HideInInspector]
        public bool isCube = false;
        [HideInInspector]
        public bool isBaseMesh = true;
        private Mesh bentMesh = null;
		private bool isSubSegment = true;

        public Mesh BentMesh {
            get { 
				return ReturnMeshReference(); 
			}
        }
        public bool IsSubSegment {
            get { 
				return (splineSegment != -1); 
			}
        }

        private void Start() {
            if (spline != null)
                spline.UpdateSpline();

            UpdateMesh();
        }

        private void OnEnable() {
            if (spline != null){
                spline.UpdateSpline();
			}

            UpdateMesh();
        }
		
		private void Update(){
			if(Input.GetKeyDown(KeyCode.U)){
				if (spline != null){
                spline.UpdateSpline();
			}

				UpdateMesh();
			}
		}

        public void LateUpdate() {
            if( AutomaticUpdater.Update()){
                UpdateMesh();
			}
        }

        public void UpdateMesh() {
            SetupMeshFilter();

            bentMesh.Clear();
            Mesh mesh = baseMesh;
            if (isCylinder) {
                mesh = cylinder;
            } else if (isCube) {
                mesh = cube;
            } else if (isBaseMesh) {
                mesh = baseMesh;
            }
            if (mesh == null || spline == null || segmentCount <= 0){
                return;
			}

            MeshData meshDataBase = new MeshData(mesh);
            MeshData meshDataNew = new MeshData(meshDataBase, segmentCount);

            PipeSplineSegment currentSegment = null;

            if (IsSubSegment) {
                currentSegment = spline.PipeSplineSegments[splineSegment];
            }

            float segmentStart = 0f;
            float segmentEnd = 0f;

            for (int segmentIdx = 0; segmentIdx < segmentCount; segmentIdx++) {
                if (!IsSubSegment) {
                    segmentStart = (float)(segmentIdx) / segmentCount;
                    segmentEnd = (float)(segmentIdx + 1) / segmentCount;
                } else {
                    segmentStart = currentSegment.ConvertSegmentToSplineParameter((float)(segmentIdx) / segmentCount);
                    segmentEnd = currentSegment.ConvertSegmentToSplineParameter((float)(segmentIdx + 1) / segmentCount);
                }
				
				 
				GenerateBentMensh(segmentIdx, segmentStart, segmentEnd, meshDataBase, meshDataNew);
            }

            meshDataNew.AssignToMesh(bentMesh);
        }

        private void GenerateBentMensh(int segmentIdx, float segmentStart, float segmentEnd, MeshData meshDataBase, MeshData meshDataNew) {
		
			Vector3 pos0 = spline.transform.InverseTransformPoint(spline.GetPositionOnSpline(segmentStart));
			Vector3 pos1 = spline.transform.InverseTransformPoint(spline.GetPositionOnSpline(segmentEnd));

			Quaternion rot0 = spline.GetOrientationOnSpline(segmentStart) * Quaternion.Inverse(spline.transform.rotation);
			Quaternion rot1 = spline.GetOrientationOnSpline(segmentEnd) * Quaternion.Inverse(spline.transform.rotation);
			Quaternion targetRot = Quaternion.identity;

			Vector3 vertex = Vector3.zero;
			Vector2 uvCoord = Vector2.zero;

			float normalizedZPos = 0f;

			int newVertexIndex = meshDataBase.VertexCount * segmentIdx;

			for (int i = 0; i < meshDataBase.VertexCount; i++, newVertexIndex++) {
				vertex = meshDataBase.Vertices[i];
				uvCoord = meshDataBase.UvCoord[i];

				normalizedZPos = vertex.z + 0.5f;

				targetRot = Quaternion.Lerp(rot0, rot1, normalizedZPos);

				vertex.Scale(new Vector3(xyScale.x, xyScale.y, 0));

				vertex = targetRot * vertex;
				vertex = vertex + Vector3.Lerp(pos0, pos1, normalizedZPos);

				meshDataNew.Vertices[newVertexIndex] = vertex;

				if (meshDataBase.HasNormals)
					meshDataNew.Normals[newVertexIndex] = targetRot * meshDataBase.Normals[i];

				if (meshDataBase.HasTangents)
					meshDataNew.Tangents[newVertexIndex] = targetRot * meshDataBase.Tangents[i];

				if (uvMode == UVMode.Normal)
					uvCoord.y = Mathf.Lerp(segmentStart, segmentEnd, normalizedZPos);
				else if (uvMode == UVMode.Swap)
					uvCoord.x = Mathf.Lerp(segmentStart, segmentEnd, normalizedZPos);

				meshDataNew.UvCoord[newVertexIndex] = Vector2.Scale(uvCoord, uvScale);
			}

			for (int i = 0; i < meshDataBase.TriangleCount; i++){
				meshDataNew.Triangles[i + (segmentIdx * meshDataBase.TriangleCount)] = meshDataBase.Triangles[i] + (meshDataBase.VertexCount * segmentIdx);
			}
        }

        private void SetupMeshFilter() {
            if (bentMesh == null) {
                bentMesh = new Mesh();

                bentMesh.name = "BentMesh";
                bentMesh.hideFlags = HideFlags.HideAndDontSave;
            }

            MeshFilter meshFilter = GetComponent<MeshFilter>();

            if (meshFilter.sharedMesh != bentMesh)
                meshFilter.sharedMesh = bentMesh;


            MeshCollider meshCollider = GetComponent<MeshCollider>();

            if (meshCollider != null) {
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = bentMesh;
            }
        }

        private Mesh ReturnMeshReference() {
            return bentMesh;
        }


    }
}

