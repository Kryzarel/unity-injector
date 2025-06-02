using System;
using Kryz.UnityDI;
using UnityEditor;

public static class SetExecutionOrder
{
	[InitializeOnLoadMethod]
	private static void HandleExecutionOrder()
	{
		MonoScript? injectable = null;
		MonoScript? compositionRoot = null;

		MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
		for (int i = 0; i < scripts.Length; i++)
		{
			MonoScript monoScript = scripts[i];
			Type scriptType = monoScript.GetClass();

			if (scriptType == typeof(MonoBehaviourInjectable))
			{
				injectable = monoScript;
			}
			else if (scriptType == typeof(SceneCompositionRoot))
			{
				compositionRoot = monoScript;
			}
		}

		int injectableOrder = MonoImporter.GetExecutionOrder(injectable);
		if (injectableOrder >= 0)
		{
			injectableOrder = -10;
			MonoImporter.SetExecutionOrder(injectable, injectableOrder);
		}

		int compositionRootOrder = MonoImporter.GetExecutionOrder(compositionRoot);
		if (compositionRootOrder >= injectableOrder)
		{
			compositionRootOrder = injectableOrder - 10;
			MonoImporter.SetExecutionOrder(compositionRoot, compositionRootOrder);
		}
	}
}