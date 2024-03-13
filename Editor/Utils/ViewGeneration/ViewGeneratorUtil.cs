using System.IO;
using UnityEditor;
using UnityEngine;

namespace ArchCore.Utils.ViewGeneration
{
	public static class ViewGeneratorUtil
	{
		private const string viewTemplatePath = "ArchCore/Editor/Utils/ViewGeneration/vgt00.txt";
		private const string presenterTemplatePath = "ArchCore/Editor/Utils/ViewGeneration/pgt00.txt";
		
		private const string popupTemplatePath = "ArchCore/Editor/Utils/ViewGeneration/pvgt00.txt";
		private const string popupPresenterTemplatePath = "ArchCore/Editor/Utils/ViewGeneration/ppgt00.txt";


		private const string nameVariable = "NAME";
		private const string namespaceVariable = "NAMESPACE";
		
		static string Var(string var) => $"#{var}#";

		static void ReplaceVariable(ref string source, string variableName, string value)
		{
			source = source.Replace(Var(variableName), value);
		}
		
		public static void Generate(ViewType type, string name, string namespaceName)
		{
			var localPath = "Scripts/Views";

			string view, presenter;
			if (type == ViewType.View)
			{
				view = GenerateFile(name, namespaceName, localPath, viewTemplatePath, "View");
				presenter = GenerateFile(name, namespaceName, localPath, presenterTemplatePath, "Presenter");
			}
			else
			{
				view = GenerateFile(name, namespaceName, localPath, popupTemplatePath, "Popup");
				presenter = GenerateFile(name, namespaceName, localPath, popupPresenterTemplatePath, "Presenter");
			}
			
			
			AssetDatabase.Refresh();
			AssetDatabase.ImportAsset(view, ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.ImportAsset(presenter, ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.Refresh();
			
		}


		static string GenerateFile(string name, string namespaceName, string localPath, string templatePath, string postFix)
		{
			var directory = Path.Combine(Application.dataPath, localPath);
			
			var path = Path.Combine(directory, name + postFix + ".cs");
			
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			string template = File.ReadAllText(Path.Combine(Application.dataPath, templatePath));

			ReplaceVariable(ref template, nameVariable, name);
			ReplaceVariable(ref template, namespaceVariable, namespaceName);

			File.WriteAllText(path, template);

			return path;
		}

	}
}