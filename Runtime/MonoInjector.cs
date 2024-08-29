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
		public static void Clear()
		{
			DefaultParent = DependencyInjector.RootContainer;
			containers.Clear();
			parentContainers.Clear();
		}

		/// <summary>
		/// Attempts to get the <see cref="Container"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns>The corresponding <see cref="Container"/>, or <see cref="null"/> if the <see cref="Scene"/> is not loaded.</returns>
		public static Container? GetContainer(Scene scene)
		{
			TryGetContainer(scene, out Container? container);
			return container;
		}

		/// <summary>
		/// Attempts to get the <see cref="Container"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns><see cref="true"/> if <see cref="Scene.isLoaded"/>, <see cref="false"/> otherwise.</returns>
		public static bool TryGetContainer(Scene scene, out Container? container)
		{
			if (!scene.isLoaded)
			{
				container = null;
				return false;
			}
			if (containers.TryGetValue(scene, out container))
			{
				return true;
			}
			if (!parentContainers.TryGetValue(scene, out Container? parent))
			{
				parent = DefaultParent;
			}
			container = containers[scene] = parent?.CreateChild() ?? new Container();
			return true;
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