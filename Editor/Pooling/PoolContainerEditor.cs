using UnityEditor;
using UnityEngine;

namespace ArchCore.Pooling
{
	[CustomEditor(typeof(PoolContainer))]
	public class PoolContainerEditor : Editor
	{
		public SerializedProperty SP_list;
		public SerializedProperty subContainers;
    
		public void OnEnable()
		{
			SP_list = serializedObject.FindProperty("list");
			subContainers = serializedObject.FindProperty("subContainers");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			//DrawDefaultInspector();
			
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("initOnAwake"), new GUIContent("Init on awake"));
			
			EditorGUILayout.LabelField("SubContainers", EditorStyles.boldLabel);
			
			subContainers = serializedObject.FindProperty("subContainers");
			
			for (int i = 0; i < subContainers.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();
				var element = subContainers.GetArrayElementAtIndex(i);
				EditorGUILayout.ObjectField(element, GUIContent.none);
				if (GUILayout.Button("x", GUILayout.MaxWidth(24)))
				{
					subContainers.DeleteArrayElementAtIndex(i);
				}
				EditorGUILayout.EndHorizontal();
			}

			if (GUILayout.Button("+"))
			{
				subContainers.InsertArrayElementAtIndex(subContainers.arraySize);
			}
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);
			SP_list = serializedObject.FindProperty("list");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("Min"), GUILayout.MaxWidth(64));
			EditorGUILayout.LabelField(new GUIContent("Max"), GUILayout.MaxWidth(64));
			EditorGUILayout.LabelField(new GUIContent("Asset"), GUILayout.MinWidth(64));
			EditorGUILayout.EndHorizontal();


			for (int i = 0; i < SP_list.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();
				var element = SP_list.GetArrayElementAtIndex(i);
				var minSize = element.FindPropertyRelative("minSize");
				var maxSize = element.FindPropertyRelative("maxSize");
				var obj     = element.FindPropertyRelative("asset");
				EditorGUILayout.PropertyField(minSize,  GUIContent.none, GUILayout.MaxWidth(64));
				EditorGUILayout.PropertyField(maxSize, GUIContent.none, GUILayout.MaxWidth(64));
				float tempWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 0.1f;
				EditorGUILayout.PropertyField(obj, new GUIContent(""), GUILayout.MinWidth(64));
				EditorGUIUtility.labelWidth = tempWidth;
				if (GUILayout.Button("x", GUILayout.MaxWidth(24)))
				{
					SP_list.DeleteArrayElementAtIndex(i);
				}
				EditorGUILayout.EndHorizontal();
			}

			if (GUILayout.Button("+"))
			{
				SP_list.InsertArrayElementAtIndex(SP_list.arraySize);
			}
			
			

			serializedObject.ApplyModifiedProperties();
		}
	}
}



