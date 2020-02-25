using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipelineToolkit {

    public class NodeRoot : MonoBehaviour {

        private Transform child;

        public Transform Child {
            get {
                return child;
            }
        }
		
        void Awake() {
            child = gameObject.transform.GetChild(0);
        }

    }
}
