using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Kryz.DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kryz.UnityDI
{
	public static class UnityInjector
	{
		public static IContainer CurrentContainer => containers[^1];

		public static readonly IReadOnlyList<IContainer> Containers;
		public static readonly IReadOnlyDictionary<Scene, IContainer> SceneContainers;

		private static readonly List<IContainer> containers;
		private static readonly Dictionary<Scene, IContainer> sceneContainers;
		private static readonly Dictionary<Scene, IBuilder> sceneBuilders;

		static UnityInjector()
		{
			int sceneCount = SceneManager.sceneCountInBuildSettings;
			Containers = containers = new List<IContainer>();
			SceneContainers = sceneContainers = new Dictionary<Scene, IContainer>(sceneCount);
			sceneBuilders = new Dictionary<Scene, IBuilder>(sceneCount);

			containers.Add(new Builder().Build());

			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			Application.quitting += Clear;
		}

		private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (!sceneBuilders.Remove(scene, out IBuilder builder))
			{
				builder = CurrentContainer.CreateScopeBuilder();
			}
			sceneContainers[scene] = builder.Build();
		}

		private static void OnSceneUnloaded(Scene scene)
		{
			if (sceneContainers.TryGetValue(scene, out IContainer container))
			{
				container.Dispose();
				sceneContainers.Remove(scene);
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		public static void Clear()
		{
			foreach (IContainer container in containers)
			{
				container.Dispose();
			}

			foreach (KeyValuePair<Scene, IContainer> item in sceneContainers)
			{
				item.Value.Dispose();
			}

			containers.Clear();
			containers.Add(new Builder().Build());
			sceneContainers.Clear();
		}

		/// <summary>
		/// Attempts to get the <see cref="IContainer"/> for a given <see cref="Scene"/>.
		/// </summary>
		public static bool TryGetSceneContainer(Scene scene, [MaybeNullWhen(returnValue: false)] out IContainer container)
		{
			return sceneContainers.TryGetValue(scene, out container);
		}

		/// <summary>
		/// Attempts to get the <see cref="IScopeBuilder"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns><see cref="true"/> if <see cref="Scene.IsValid"/>, <see cref="false"/> otherwise.</returns>
		public static bool TryGetSceneBuilder(Scene scene, [MaybeNullWhen(returnValue: false)] out IScopeBuilder register)
		{
			if (sceneContainers.ContainsKey(scene) || scene.isLoaded || !scene.IsValid())
			{
				register = null;
				return false;
			}
			if (!sceneBuilders.TryGetValue(scene, out IBuilder builder))
			{
				builder = sceneBuilders[scene] = CurrentContainer.CreateScopeBuilder();
			}
			register = builder;
			return true;
		}

		/// <summary>
		/// Attempts to get the <see cref="IContainer"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns>The corresponding <see cref="IContainer"/>, or <see cref="null"/> if the <see cref="Scene"/> is not valid.</returns>
		public static IContainer? GetSceneContainer(Scene scene)
		{
			TryGetSceneContainer(scene, out IContainer? container);
			return container;
		}

		/// <summary>
		/// Pushes a new <see cref="IContainer"/> to the <see cref="Containers"/> list. The last pushed container will the parent for newly loaded scenes.
		/// </summary>
		/// <param name="container"></param>
		public static void PushContainer(IContainer container)
		{
			containers.Add(container);
		}

		/// <summary>
		/// Removes the last pushed <see cref="IContainer"/> from the <see cref="Containers"/> list. The default container (at index 0) will always remain in the list and cannot be removed.
		/// </summary>
		/// <returns></returns>
		public static bool PopContainer()
		{
			int last = containers.Count - 1;
			if (last <= 0) return false; // Always keep the first container in the list. That one is the root and cannot be removed.

			IContainer container = containers[last];
			containers.RemoveAt(last);
			container.Dispose();
			return true;
		}
	}
}