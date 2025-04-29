using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Kryz.DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kryz.UnityDI
{
	public static class UnityInjector
	{
		public static IContainer? CurrentContainer => containers.Count > 0 ? containers[^1] : null;

		public static readonly IReadOnlyList<IContainer> Containers;
		public static readonly IReadOnlyDictionary<Scene, IContainer> SceneContainers;

		private static readonly List<IContainer> containers;
		private static readonly Dictionary<Scene, IContainer> sceneContainers;

		static UnityInjector()
		{
			int sceneCount = SceneManager.sceneCountInBuildSettings;
			Containers = containers = new List<IContainer>();
			SceneContainers = sceneContainers = new Dictionary<Scene, IContainer>(sceneCount);

			// Don't use this. No need to register scenes that don't have any Injectable objects.
			// SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			Application.quitting += Clear;
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
			containers.Clear();

			foreach (KeyValuePair<Scene, IContainer> item in sceneContainers)
			{
				item.Value.Dispose();
			}
			sceneContainers.Clear();
		}

		/// <summary>
		/// Attempts to get the <see cref="IContainer"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns><see cref="true"/> if <see cref="Scene.isLoaded"/>, <see cref="false"/> otherwise.</returns>
		public static bool TryGetContainer(Scene scene, [MaybeNullWhen(returnValue: false)] out IContainer container)
		{
			if (!scene.isLoaded)
			{
				container = null;
				return false;
			}
			if (sceneContainers.TryGetValue(scene, out container))
			{
				return true;
			}
			container = sceneContainers[scene] = CurrentContainer?.CreateScope() ?? new Builder().Build();
			return true;
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

		public static void PushContainer(Action<IScopeBuilder> builderAction)
		{
			Builder builder = new();
			builderAction(builder);
			containers.Add(builder.Build());
		}

		public static void PushContainer(IContainer parent)
		{
			containers.Add(parent.CreateScope());
		}

		public static void PushContainer(IContainer parent, Action<IScopeBuilder> builderAction)
		{
			containers.Add(parent.CreateScope(builderAction));
		}

		public static void PopContainer()
		{
			int last = containers.Count - 1;
			if (last < 0) return;

			IContainer container = containers[last];
			containers.RemoveAt(last);
			container.Dispose();
		}
	}
}