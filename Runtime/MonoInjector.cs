using System.Collections.Generic;
using Kryz.DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kryz.MonoDI
{
	public static class MonoInjector
	{
		public static Container DefaultParent;

		public static readonly IReadOnlyDictionary<Scene, Container> Containers;
		public static readonly IReadOnlyDictionary<Scene, Container> ParentContainers;

		private static readonly Dictionary<Scene, Container> containers;
		private static readonly Dictionary<Scene, Container> parentContainers;

		static MonoInjector()
		{
			DefaultParent = DependencyInjector.RootContainer;

			int sceneCount = SceneManager.sceneCountInBuildSettings;
			Containers = containers = new Dictionary<Scene, Container>(sceneCount);
			ParentContainers = parentContainers = new Dictionary<Scene, Container>(sceneCount);
		}

		public static void Inject<T>(T obj) where T : MonoBehaviour
		{
			Scene scene = obj.gameObject.scene;
			containers[scene].Inject(obj);
		}

		public static void SetParent(Scene scene, Container parent)
		{
			if (containers[scene] != null)
			{
				Debug.LogError($"Can't change parent {nameof(Container)} of a loaded Scene ({scene.name}). Please unload it first.");
				return;
			}
			parentContainers[scene] = parent;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void Init()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			Application.quitting += Quit;
		}

		private static void Quit()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
			Application.quitting -= Quit;

			containers.Clear();
			parentContainers.Clear();
		}

		private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (!parentContainers.TryGetValue(scene, out Container parent))
			{
				parent = DefaultParent;
			}
			containers[scene] = parent?.CreateChild() ?? new Container();
		}

		private static void OnSceneUnloaded(Scene scene)
		{
			Container container = containers[scene];
			container.Parent?.RemoveChild(container);
			containers.Remove(scene);
			parentContainers.Remove(scene);
		}
	}
}