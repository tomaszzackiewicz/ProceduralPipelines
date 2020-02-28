using UnityEngine;

namespace PipelineToolkit {
	
	public static class AutomaticUpdater {
		
		public static UpdateMode mode = UpdateMode.EveryFrame;	
		public static float deltaSeconds = 0.1f; 				
		public static int deltaFrames = 2;
		
		private static float passedTime;
		
		public static bool Update(){
			switch(mode){
				case UpdateMode.EveryFrame:
					return true;
					
				case UpdateMode.EveryXFrames:
					if(Time.frameCount % deltaFrames == 0)
						return true;
					
					return false;
					
				case UpdateMode.EveryXSeconds:
					passedTime += Time.deltaTime;
					
					if(passedTime >= deltaSeconds){
						passedTime = 0f;
						return true;
					}
					
					return false;
			}
			
			return false;
		}

	}
}
