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
		public static IContainer CurrentParent => parentContainers[^1];

		public static readonly IReadOnlyList<IContainer> ParentContainers;
		public static readonly IReadOnlyDictionary<Scene, IContainer> SceneContainers;

		private static readonly List<IContainer> parentContainers;
		private static readonly Dictionary<Scene, IContainer> sceneContainers;

		private static readonly List<GameObject> rootObjects = new();
		private static readonly List<SceneCompositionRoot> compositionRoots = new(1);
		private static readonly List<MonoBehaviourInjectable> injectables = new(100);

		static UnityInjector()
		{
			ParentContainers = parentContainers = new List<IContainer>();
			parentContainers.Add(new Builder().Build());

			int sceneCount = SceneManager.sceneCountInBuildSettings;
			// SceneBuilders = sceneBuilders = new Dictionary<Scene, IBuilder>(sceneCount);
			SceneContainers = sceneContainers = new Dictionary<Scene, IContainer>(sceneCount);

			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			Application.quitting += Clear;
		}

		private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			IBuilder builder = CurrentParent.CreateScopeBuilder();

			rootObjects.Clear();
			scene.GetRootGameObjects(rootObjects);

			compositionRoots.Clear();
			foreach (GameObject go in rootObjects)
			{
				go.GetComponentsInChildren(includeInactive: true, compositionRoots);

				foreach (SceneCompositionRoot compositionRoot in compositionRoots)
				{
					compositionRoot.Register_Internal(builder);
				}
			}
			compositionRoots.Clear();

			IContainer container = builder.Build();
			sceneContainers[scene] = container;

			injectables.Clear();
			foreach (GameObject go in rootObjects)
			{
				go.GetComponentsInChildren(includeInactive: true, injectables);

				foreach (MonoBehaviourInjectable injectable in injectables)
				{
					injectable.Init(container);
				}
			}
			injectables.Clear();

			rootObjects.Clear();
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
			foreach (IContainer container in parentContainers)
			{
				container.Dispose();
			}

			foreach (KeyValuePair<Scene, IContainer> item in sceneContainers)
			{
				item.Value.Dispose();
			}

			parentContainers.Clear();
			parentContainers.Add(new Builder().Build());
			sceneContainers.Clear();
		}

		/// <summary>
		/// Attempts to get the <see cref="IContainer"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns><see cref="true"/> if the <see cref="Scene"/> is loaded, <see cref="false"/> otherwise.</returns>
		public static bool TryGetSceneContainer(Scene scene, [MaybeNullWhen(returnValue: false)] out IContainer container)
		{
			return sceneContainers.TryGetValue(scene, out container);
		}

		/// <summary>
		/// Attempts to get the <see cref="IContainer"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns>The corresponding <see cref="IContainer"/>, or <see cref="null"/> if the <see cref="Scene"/> is not loaded.</returns>
		public static IContainer? GetSceneContainer(Scene scene)
		{
			TryGetSceneContainer(scene, out IContainer? container);
			return container;
		}

		/// <summary>
		/// Pushes the specified <see cref="IContainer"/> to the <see cref="ParentContainers"/> list. The last pushed container will be the parent for newly loaded scenes.
		/// </summary>
		/// <param name="container">The <see cref="IContainer"/> to push.</param>
		public static void PushContainer(IContainer container)
		{
			parentContainers.Add(container);
		}

		/// <summary>
		/// Pushes a new <see cref="IContainer"/> to the <see cref="ParentContainers"/> list, as a child (aka scope) of the <see cref="CurrentParent"/>. The last pushed container will be the parent for newly loaded scenes.
		/// </summary>
		/// <param name="scopedToCurrent">If true, the new container will be created as a child (aka scope) of <see cref="CurrentParent"/>.</param>
		/// <returns>The newly created container.</returns>
		public static IContainer PushNewContainer(bool scopedToCurrent = true)
		{
			IContainer container = scopedToCurrent ? CurrentParent.CreateScope() : new Builder().Build();
			parentContainers.Add(container);
			return container;
		}

		/// <summary>
		/// Pushes a new <see cref="IContainer"/> to the <see cref="ParentContainers"/> list. The last pushed container will be the parent for newly loaded scenes.
		/// </summary>
		/// <param name="builderAction">Additional registrations.</param>
		/// <param name="scopedToCurrent">If true, the new container will be created as a child (aka scope) of <see cref="CurrentParent"/>.</param>
		/// <returns>The newly created container.</returns>
		public static IContainer PushNewContainer(Action<IBuilder> builderAction, bool scopedToCurrent = true)
		{
			IBuilder builder = scopedToCurrent ? CurrentParent.CreateScopeBuilder() : new Builder();
			builderAction?.Invoke(builder);
			IContainer container = builder.Build();
			parentContainers.Add(container);
			return container;
		}

		/// <summary>
		/// Removes the last pushed <see cref="IContainer"/> from the <see cref="ParentContainers"/> list. The default container (at index 0) will always remain in the list and cannot be removed.
		/// </summary>
		/// <param name="container">The removed <see cref="IContainer"/> or null if no containers could be removed OR the container was diposed.</param>
		/// <param name="dispose">If true, Dispose() will be called on the container after removal. When true, the "out <see cref="IContainer"/> <paramref name="container"/>" param will always be null.</param>
		/// <returns><see cref="true"/> if the container was removed successfully.</returns>
		public static bool PopContainer(out IContainer? container, bool dispose = true)
		{
			int last = parentContainers.Count - 1;
			if (last <= 0) // Always keep the first container in the list. That one is the root and cannot be removed.
			{
				container = null;
				return false;
			}

			container = parentContainers[last];
			parentContainers.RemoveAt(last);
			if (dispose)
			{
				container.Dispose();
				container = null;
			}
			return true;
		}

		/// <summary>
		/// Removes the specified <see cref="IContainer"/> from the <see cref="ParentContainers"/> list. The default container (at index 0) will always remain in the list and cannot be removed.
		/// </summary>
		/// <param name="container">The <see cref="IContainer"/> to remove.</param>
		/// <param name="dispose">If true, Dispose() will be called on the container after removal.</param>
		/// <returns><see cref="true"/> if the <paramref name="container"/> was found and removed successfully.</returns>
		public static bool RemoveContainer(IContainer container, bool dispose = true)
		{
			int index = parentContainers.LastIndexOf(container); // Use LastIndexOf to search the list from end to start.
			if (index <= 0) return false; // Always keep the first container in the list. That one is the root and cannot be removed.

			parentContainers.RemoveAt(index);
			if (dispose)
			{
				container.Dispose();
			}
			return true;
		}
	}
}