using System;
using System.Collections.Generic;
using Kryz.DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kryz.MonoDI
{
	public static class MonoInjector
	{
		public static readonly IReadOnlyList<Container?> Containers;
		public static readonly IReadOnlyList<Container?> ParentContainers;

		private static readonly Container?[] containers;
		private static readonly Container[] parentContainers;

		static MonoInjector()
		{
			Containers = containers = new Container[SceneManager.sceneCountInBuildSettings];
			ParentContainers = parentContainers = new Container[SceneManager.sceneCountInBuildSettings];
			Array.Fill(parentContainers, DependencyInjector.RootContainer);
		}

		public static void Inject<T>(T obj) where T : MonoBehaviour
		{
			Scene scene = obj.gameObject.scene;
			containers[scene.buildIndex]?.Inject(obj);
		}

		public static void SetParent(int sceneIndex, Container container)
		{
			if (containers[sceneIndex] != null)
			{
				Debug.LogError("Can't change parent Container of a loaded Scene. Please unload it first.");
				return;
			}
			parentContainers[sceneIndex] = container;
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

			Array.Clear(containers, 0, containers.Length);
			Array.Clear(parentContainers, 0, parentContainers.Length);
		}

		private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			int index = scene.buildIndex;
			containers[index] = parentContainers[index].CreateChild();
		}

		private static void OnSceneUnloaded(Scene scene)
		{
			ref Container? container = ref containers[scene.buildIndex];
			container?.Parent?.RemoveChild(container);
			container = null;
		}
	}
}