using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelsController))]
public class LevelsControllerEditor : Editor
{
	private LevelsController levelsController;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		levelsController = (LevelsController) target;

		var providers = levelsController.GetComponents<ILevelProvider>();
		if (providers.Length == 0)
		{
			EditorGUILayout.HelpBox("Выберите профайдер", MessageType.Warning);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Префабы"))
			{
				foreach (var provider in providers)
					DestroyImmediate((Component) provider);

				levelsController.gameObject.AddComponent<PrefabLevelsProvider>();
			}

			if (GUILayout.Button("Сцены"))
			{
				foreach (var provider in providers)
					DestroyImmediate((Component) provider);

				levelsController.gameObject.AddComponent<SceneLevelsProvider>();
			}

			if (GUILayout.Button("Кастомный"))
			{
				foreach (var provider in providers)
					DestroyImmediate((Component) provider);

				levelsController.gameObject.AddComponent<CustomLevelsProvider>();
			}

			GUILayout.EndHorizontal();
		}
	}
}