using UnityEditor;
using UnityEngine;

namespace LowPolySurvival.Game.EditorTools
{
	public static class CreateSafehouseScene
	{
		[MenuItem("Tools/Create Safehouse Scene")] 
		public static void Create()
		{
			var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
			var boot = new GameObject("AutoBootstrap").AddComponent<LowPolySurvival.Game.Bootstrap.AutoBootstrap>();
			boot.transform.position = Vector3.zero;
			UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/Safehouse.unity");
			EditorUtility.DisplayDialog("Scene Created", "Assets/Scenes/Safehouse.unity", "OK");
		}
	}
}


