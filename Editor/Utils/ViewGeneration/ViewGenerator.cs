using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArchCore.Utils.ViewGeneration
{
	public enum ViewType
	{
		View,
		Popup
	}
	public class ViewGeneratorWindow : EditorWindow
	{
		[MenuItem("Tools/View Generator")]
		public static void ShowWindow()
		{
			var w = GetWindow<ViewGeneratorWindow>( true, "View Generator");
		}

		
		private const string subNamespace = "Views";

		private ViewGeneratorSettings settings;

		private AfterCompileAction afterCompileAction;



		private ViewType currentViewType;
		
		void OnGUI()
		{
			EditorGUI.BeginChangeCheck();
			settings.rootNamespace = EditorGUILayout.TextField("Root Namespace", settings.rootNamespace);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Save Path", GUILayout.MaxWidth(100));
			GUI.enabled = false;
			EditorGUILayout.TextField(Application.dataPath);
			GUI.enabled = true;
			EditorGUILayout.LabelField("/", GUILayout.MaxWidth(8));
			settings.savePath = EditorGUILayout.TextField(settings.savePath);
			EditorGUILayout.LabelField("/", GUILayout.MaxWidth(8));
			settings.scriptName = EditorGUILayout.TextField(settings.scriptName);
			currentViewType = (ViewType)EditorGUILayout.EnumPopup(currentViewType);
			EditorGUILayout.LabelField(".cs", GUILayout.MaxWidth(18));
			EditorGUILayout.EndHorizontal();
			settings.openAfterGenerating = EditorGUILayout.Toggle("Open After Creation",settings.openAfterGenerating);
			
			if(GUILayout.Button("Generate"))
			{
				if (Check())
				{
					afterCompileAction = new AfterCompileAction()
					{
						scriptName = $"{settings.scriptName}{currentViewType.ToString()}",
						nameSpace = $"{settings.rootNamespace}.{subNamespace}"
					};
					ViewGeneratorUtil.Generate(currentViewType, settings.scriptName, settings.rootNamespace);
				}
				
			}

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(settings);
			}
		}

		private bool Check()
		{
			File.Exists(Path.Combine(Application.dataPath, settings.savePath, settings.scriptName,
				currentViewType + ".cs"));
			
			return true;
		}

		[Serializable]
		class AfterCompileAction
		{
			public string scriptName;
			public string nameSpace;
		}

		private void OnEnable()
		{
			LoadSettings();

			
			CreatePrefabIfNeeded();

		}

		void CreatePrefabIfNeeded()
		{
			if (afterCompileAction == null || afterCompileAction.scriptName == null || afterCompileAction.scriptName == "")
				return;
			
			
			
			var viewType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())//.Distinct()
				.FirstOrDefault(t => t.Name == afterCompileAction.scriptName && t.Namespace == afterCompileAction.nameSpace);
				
			
			GameObject viewTemplate = Resources.Load<GameObject>("v00");
			var copy = Instantiate(viewTemplate);
			copy.AddComponent(viewType);

			var dir = Path.Combine(Application.dataPath, "Resources/Views");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			var path = Path.Combine("Assets/Resources/Views", $"{afterCompileAction.scriptName}.prefab");


			PrefabUtility.SaveAsPrefabAsset(copy, path);
			DestroyImmediate(copy);
			
			if(settings.openAfterGenerating) AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(path));
			
			afterCompileAction.scriptName = "";
			afterCompileAction = null;
		}
		
		void LoadSettings()
		{
			if (!settings)
			{
				settings = Resources.Load<ViewGeneratorSettings>("ViewGeneratorSettings");

				if (!settings)
				{
					settings = ScriptableObject.CreateInstance<ViewGeneratorSettings>();
			
					AssetDatabase.CreateAsset (settings, 
						Path.Combine("Assets", "Resources", "ViewGeneratorSettings.asset"));
 
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
			}
		}
	}
}