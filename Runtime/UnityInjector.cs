using System.Collections.Generic;
using Kryz.DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kryz.UnityDI
{
	public static class UnityInjector
	{
		public static IContainer? DefaultContainer;

		public static readonly IReadOnlyDictionary<Scene, IContainer> Containers;
		public static readonly IReadOnlyDictionary<Scene, IContainer> ParentContainers;

		private static readonly Dictionary<Scene, IContainer> containers;
		private static readonly Dictionary<Scene, IContainer> parentContainers;

		static UnityInjector()
		{
			int sceneCount = SceneManager.sceneCountInBuildSettings;
			Containers = containers = new Dictionary<Scene, IContainer>(sceneCount);
			ParentContainers = parentContainers = new Dictionary<Scene, IContainer>(sceneCount);

			// Don't use this. No need to register scenes that don't have any Injectable objects.
			// SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			Application.quitting += Clear;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		public static void Clear()
		{
			DefaultContainer?.Dispose();
			DefaultContainer = null;
			containers.Clear();
			parentContainers.Clear();
		}

		/// <summary>
		/// Attempts to get the <see cref="IContainer"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns>The corresponding <see cref="IContainer"/>, or <see cref="null"/> if the <see cref="Scene"/> is not loaded.</returns>
		public static IContainer? GetContainer(Scene scene)
		{
			TryGetContainer(scene, out IContainer? container);
			return container;
		}

		/// <summary>
		/// Attempts to get the <see cref="IContainer"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns><see cref="true"/> if <see cref="Scene.isLoaded"/>, <see cref="false"/> otherwise.</returns>
		public static bool TryGetContainer(Scene scene, out IContainer? container)
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
			if (!parentContainers.TryGetValue(scene, out IContainer? parent))
			{
				parent = DefaultContainer;
			}
			container = containers[scene] = parent?.CreateScope() ?? new Builder().Build();
			return true;
		}

		public static void SetParent(Scene scene, IContainer parent)
		{
			if (containers.ContainsKey(scene))
			{
				Debug.LogWarning($"Changing the parent {nameof(IContainer)} of a loaded Scene ({scene.name}) will only have an effect the next time it's loaded.");
			}
			parentContainers[scene] = parent;
		}

		public static bool RemoveParent(Scene scene)
		{
			if (containers.ContainsKey(scene))
			{
				Debug.LogWarning($"Changing the parent {nameof(IContainer)} of a loaded Scene ({scene.name}) will only have an effect the next time it's loaded.");
			}
			return parentContainers.Remove(scene);
		}

		public static bool RemoveParent(IContainer container)
		{
			bool success = false;
			foreach (KeyValuePair<Scene, IContainer> item in parentContainers)
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
			if (containers.TryGetValue(scene, out IContainer container))
			{
				container.Dispose();
				containers.Remove(scene);
			}
		}
	}
}