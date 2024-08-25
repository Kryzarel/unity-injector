using System;
using System.Collections.Generic;
using Kryz.DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kryz.MonoDI
{
	public static class MonoInjector
	{
		public static Container? DefaultParent;

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

			Application.quitting += Clear;
			// Don't use this. No need to register scenes that don't have any Injectable objects.
			// SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			Clear();
		}

		public static void Clear()
		{
			DefaultParent = DependencyInjector.RootContainer;
			containers.Clear();
			parentContainers.Clear();
		}

		public static void Inject<T>(T obj) where T : MonoBehaviour
		{
			if (obj == null)
				return;

			Scene scene = obj.gameObject.scene;
			GetOrCreateContainer(scene).Inject(obj);
		}

		public static void SetParent(Scene scene, Container parent)
		{
			if (containers.ContainsKey(scene))
			{
				Debug.LogWarning($"Changing the parent {nameof(Container)} of a loaded Scene ({scene.name}) will only have an effect the next time it's loaded.");
			}
			parentContainers[scene] = parent;
		}

		public static bool RemoveParent(Scene scene)
		{
			if (containers.ContainsKey(scene))
			{
				Debug.LogWarning($"Changing the parent {nameof(Container)} of a loaded Scene ({scene.name}) will only have an effect the next time it's loaded.");
			}
			return parentContainers.Remove(scene);
		}

		public static bool RemoveParent(Container container)
		{
			bool success = false;
			foreach (KeyValuePair<Scene, Container> item in parentContainers)
			{
				if (item.Value == container)
				{
					success |= RemoveParent(item.Key);
				}
			}
			return success;
		}

		private static Container GetOrCreateContainer(Scene scene)
		{
			if (!scene.IsValid())
			{
				throw new ArgumentException($"Scene is invalid. Scene Name: {scene.name ?? "null"}", nameof(scene));
			}
			if (containers.TryGetValue(scene, out Container container))
			{
				return container;
			}
			if (!parentContainers.TryGetValue(scene, out Container? parent))
			{
				parent = DefaultParent;
			}
			return containers[scene] = parent?.CreateChild() ?? new Container();
		}

		private static void OnSceneUnloaded(Scene scene)
		{
			if (containers.TryGetValue(scene, out Container container))
			{
				container.Parent?.RemoveChild(container);
				containers.Remove(scene);
			}
		}
	}
}