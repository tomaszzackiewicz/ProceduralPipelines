﻿using UnityEngine;

namespace PipelineToolkit {

    public class Billboard : MonoBehaviour {
		
        private Camera mainCamera;

        private void Start() {
            mainCamera = Camera.main;
        }

        private void LateUpdate() {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,mainCamera.transform.rotation * Vector3.up);
        }
    }
}
