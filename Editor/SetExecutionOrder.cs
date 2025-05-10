using Kryz.UnityDI;
using UnityEditor;

public static class SetExecutionOrder
{
	[InitializeOnLoadMethod]
	private static void HandleExecutionOrder()
	{
		MonoScript? injectable = null;
		MonoScript? compositionRoot = null;

		foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
		{
			if (monoScript.GetClass() == typeof(MonoBehaviourInjectable))
			{
				injectable = monoScript;
			}
			else if (monoScript.GetClass() == typeof(SceneCompositionRoot))
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