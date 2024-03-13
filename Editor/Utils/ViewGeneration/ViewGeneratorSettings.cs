using UnityEngine;

namespace ArchCore.Utils.ViewGeneration
{
	public class ViewGeneratorSettings : ScriptableObject
	{
		public string scriptName = "MyScript";
		public string rootNamespace = "DefaultNamespace";
		public string savePath = "Scripts/Views";
		public bool openAfterGenerating = true;
	}
}