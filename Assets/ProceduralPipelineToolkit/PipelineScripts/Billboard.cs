using UnityEngine;
using System.Collections;

namespace PipelineToolkit {

    public class Billboard : MonoBehaviour {
		
        private Camera camera;

        private void Start() {
            camera = Camera.main;
        }

        private void LateUpdate() {
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,camera.transform.rotation * Vector3.up);
        }
    }
}
