#if UNITY_EDITOR
	using UnityEditor;
	using UnityEngine;
	using System.IO;

	public class MissingScriptsFinder
	{
		[MenuItem("Tools/Find Missing Scripts In Assets")]
		static void FindMissingScripts()
		{
			string[] paths = Directory.GetFiles("Assets","*.prefab",SearchOption.AllDirectories);
			foreach (string path in paths) {
				GameObject go = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
				if(go==null) continue;
				var transforms = go.GetComponentsInChildren<Transform>(true);
				foreach (var t in transforms) {
					var components = t.GetComponents<Component>();
					foreach (var c in components) {
						if(c==null) { UnityEngine.Debug.LogErrorFormat(t.gameObject,"Missing script in: {0} > {1}",path,GetHierarchyPath(t)); }
					}
				}
			}
			UnityEngine.Debug.Log("Missing script search complete.");
		}

		static string GetHierarchyPath(Transform t)
		{
			string path = t.name;
			while (t.parent!=null) {
				t = t.parent;
				path = t.name+"/"+path;
			}
			return path;
		}
	}
	[InitializeOnLoad]
	public static class HighlightMissingScripts
	{
		static HighlightMissingScripts()
		{
			EditorApplication.hierarchyWindowItemOnGUI += DrawMissingScriptHighlight;
		}

		private static void DrawMissingScriptHighlight(int instanceId,Rect selectionRect)
		{
			GameObject obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			if(obj==null) return;

			// Check for missing scripts
			Component[] components = obj.GetComponents<Component>();
			foreach (var c in components) {
				if(c==null) {
					// Draw red background
					EditorGUI.DrawRect(selectionRect,new Color(1f,0f,0f,0.2f)); // Light red
					break;                                                      // Only need to find one missing script per object
				}
			}
		}
	}
#endif