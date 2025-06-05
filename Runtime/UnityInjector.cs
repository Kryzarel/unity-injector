using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Kryz.DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kryz.UnityDI
{
	public class UnityInjector
	{
		public static readonly UnityInjector Instance = new();

		public IContainer CurrentParent => parentContainers[^1];

		public readonly IReadOnlyList<IContainer> ParentContainers;
		public readonly IReadOnlyDictionary<Scene, IBuilder> SceneBuilders;
		public readonly IReadOnlyDictionary<Scene, IContainer> SceneContainers;

		private readonly List<IContainer> parentContainers;
		private readonly Dictionary<Scene, IBuilder> sceneBuilders;
		private readonly Dictionary<Scene, IContainer> sceneContainers;

		public UnityInjector()
		{
			ParentContainers = parentContainers = new List<IContainer>();
			parentContainers.Add(new Builder().Build());

			int sceneCount = SceneManager.sceneCountInBuildSettings;
			SceneBuilders = sceneBuilders = new Dictionary<Scene, IBuilder>(sceneCount);
			SceneContainers = sceneContainers = new Dictionary<Scene, IContainer>(sceneCount);

			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			Application.quitting += Clear;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (!sceneBuilders.Remove(scene, out IBuilder builder))
			{
				builder = CurrentParent.CreateScopeBuilder();
			}
			sceneContainers[scene] = builder.Build();
		}

		private void OnSceneUnloaded(Scene scene)
		{
			if (sceneContainers.TryGetValue(scene, out IContainer container))
			{
				container.Dispose();
				sceneContainers.Remove(scene);
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Reset()
		{
			Instance.Clear();
		}

		public void Clear()
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
			sceneBuilders.Clear();
			sceneContainers.Clear();
		}

		/// <summary>
		/// Attempts to get the <see cref="IContainer"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns><see cref="true"/> if the <see cref="Scene"/> is loaded, <see cref="false"/> otherwise.</returns>
		public bool TryGetSceneContainer(Scene scene, [MaybeNullWhen(returnValue: false)] out IContainer container)
		{
			return sceneContainers.TryGetValue(scene, out container);
		}

		/// <summary>
		/// Attempts to get the <see cref="IScopeBuilder"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns><see cref="true"/> while the <see cref="Scene"/> is being loaded, <see cref="false"/> otherwise.</returns>
		public bool TryGetSceneBuilder(Scene scene, [MaybeNullWhen(returnValue: false)] out IScopeBuilder register)
		{
			if (sceneContainers.ContainsKey(scene) || scene.isLoaded || !scene.IsValid())
			{
				register = null;
				return false;
			}
			if (!sceneBuilders.TryGetValue(scene, out IBuilder builder))
			{
				builder = sceneBuilders[scene] = CurrentParent.CreateScopeBuilder();
			}
			register = builder;
			return true;
		}

		/// <summary>
		/// Attempts to get the <see cref="IContainer"/> for a given <see cref="Scene"/>.
		/// </summary>
		/// <returns>The corresponding <see cref="IContainer"/>, or <see cref="null"/> if the <see cref="Scene"/> is not loaded.</returns>
		public IContainer? GetSceneContainer(Scene scene)
		{
			TryGetSceneContainer(scene, out IContainer? container);
			return container;
		}

		/// <summary>
		/// Pushes a new <see cref="IContainer"/> to the <see cref="ParentContainers"/> list, as a child (aka scope) of the <see cref="CurrentParent"/>. The last pushed container will be the parent for newly loaded scenes.
		/// </summary>
		public void PushContainer()
		{
			parentContainers.Add(CurrentParent.CreateScope());
		}

		/// <summary>
		/// Pushes a new <see cref="IContainer"/> to the <see cref="ParentContainers"/> list, as a child (aka scope) of the <see cref="CurrentParent"/>. The last pushed container will be the parent for newly loaded scenes.
		/// </summary>
		/// <param name="builderAction">Additional registrations.</param>
		public void PushContainer(Action<IScopeBuilder> builderAction)
		{
			parentContainers.Add(CurrentParent.CreateScope(builderAction));
		}

		/// <summary>
		/// Pushes a new <see cref="IContainer"/> to the <see cref="ParentContainers"/> list. The last pushed container will be the parent for newly loaded scenes.
		/// </summary>
		/// <param name="container">The <see cref="IContainer"/> to push.</param>
		public void PushContainer(IContainer container)
		{
			parentContainers.Add(container);
		}

		/// <summary>
		/// Removes the last pushed <see cref="IContainer"/> from the <see cref="ParentContainers"/> list. The default container (at index 0) will always remain in the list and cannot be removed.
		/// </summary>
		public bool PopContainer()
		{
			int last = parentContainers.Count - 1;
			if (last <= 0) return false; // Always keep the first container in the list. That one is the root and cannot be removed.

			IContainer container = parentContainers[last];
			parentContainers.RemoveAt(last);
			container.Dispose();
			return true;
		}

		/// <summary>
		/// Removes the specified <see cref="IContainer"/> from the <see cref="ParentContainers"/> list. The default container (at index 0) will always remain in the list and cannot be removed.
		/// </summary>
		/// <param name="container">The <see cref="IContainer"/> to remove.</param>
		/// <param name="dispose">If true, Dispose() will be called on the container after removal.</param>
		public bool RemoveContainer(IContainer container, bool dispose = true)
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