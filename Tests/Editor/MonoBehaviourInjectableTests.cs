using System.Collections;
using System.Linq;
using Kryz.DI;
using Kryz.DI.Tests;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Kryz.DI.Tests.ContainerTestHelper;

namespace Kryz.MonoDI.Tests
{
	public class MonoBehaviourInjectableTests
	{
		private const string Scene1 = "Packages/com.kryzarel.monoinjector/Tests/Shared/Test Scene MonoInjector 1.unity";
		private const string Scene2 = "Packages/com.kryzarel.monoinjector/Tests/Shared/Test Scene MonoInjector 2.unity";

		private static readonly string[] scenes = { Scene1, Scene2 };

		[UnityTearDown]
		public IEnumerator UnityTearDown()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				yield return new ExitPlayMode();
			}
		}

		[UnityTest]
		public IEnumerator TestDifferentParents([Values(true, false)] bool useDefaultParent)
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			yield return new EnterPlayMode();

			TestInjectableMonoBehaviour[] testInjectables = new TestInjectableMonoBehaviour[scenes.Length];

			for (int i = 0; i < scenes.Length; i++)
			{
				Container container = new();
				SetupContainer(container, RegisterType.Scoped);
				SetupScene(scenes[i], useDefaultParent, container, out Scene scene);
				while (!scene.isLoaded)
				{
					yield return null;
				}
				testInjectables[i] = scene.GetRootGameObjects().Single().GetComponent<TestInjectableMonoBehaviour>();
				ValidateInjectable(testInjectables[i], container);
			}

			for (int i = 0; i < testInjectables.Length; i++)
			{
				TestInjectableMonoBehaviour injectable1 = testInjectables[i];
				for (int j = i + 1; j < testInjectables.Length; j++)
				{
					TestInjectableMonoBehaviour injectable2 = testInjectables[j];

					// injectables must have different objects
					Assert.AreNotEqual(injectable1.A, injectable2.A);
					Assert.AreNotEqual(injectable1.B, injectable2.B);
					Assert.AreNotEqual(injectable1.C, injectable2.C);
				}
			}
		}

		[UnityTest]
		public IEnumerator TestSameParent([Values(true, false)] bool useDefaultParent)
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			yield return new EnterPlayMode();

			TestInjectableMonoBehaviour[] testInjectables = new TestInjectableMonoBehaviour[scenes.Length];

			Container container = new();
			SetupContainer(container, RegisterType.Scoped);

			for (int i = 0; i < scenes.Length; i++)
			{
				SetupScene(scenes[i], useDefaultParent, container, out Scene scene);
				while (!scene.isLoaded)
				{
					yield return null;
				}
				testInjectables[i] = scene.GetRootGameObjects().Single().GetComponent<TestInjectableMonoBehaviour>();
				ValidateInjectable(testInjectables[i], container);
			}

			for (int i = 0; i < testInjectables.Length; i++)
			{
				TestInjectableMonoBehaviour injectable1 = testInjectables[i];
				for (int j = i + 1; j < testInjectables.Length; j++)
				{
					TestInjectableMonoBehaviour injectable2 = testInjectables[j];

					// injectables must have the same objects
					Assert.AreEqual(injectable1.A, injectable2.A);
					Assert.AreEqual(injectable1.B, injectable2.B);
					Assert.AreEqual(injectable1.C, injectable2.C);
				}
			}
		}

		[UnityTest]
		public IEnumerator TestDirectContainer()
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			yield return new EnterPlayMode();

			TestInjectableMonoBehaviour[] testInjectables = new TestInjectableMonoBehaviour[scenes.Length];

			for (int i = 0; i < scenes.Length; i++)
			{
				SceneManager.sceneLoaded += SceneLoaded;
				Scene scene = EditorSceneManager.LoadSceneInPlayMode(scenes[i], new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive });
				while (!scene.isLoaded)
				{
					yield return null;
				}
				SceneManager.sceneLoaded -= SceneLoaded;

				void SceneLoaded(Scene scene, LoadSceneMode mode)
				{
					// Use the SceneLoaded event to setup the container before Start() runs
					SetupContainer(MonoInjector.GetContainer(scene)!, RegisterType.Scoped);
					testInjectables[i] = scene.GetRootGameObjects().Single().GetComponent<TestInjectableMonoBehaviour>();
				}
			}

			for (int i = 0; i < testInjectables.Length; i++)
			{
				TestInjectableMonoBehaviour injectable1 = testInjectables[i];
				ValidateInjectable(injectable1, null);

				for (int j = i + 1; j < testInjectables.Length; j++)
				{
					TestInjectableMonoBehaviour injectable2 = testInjectables[j];

					// injectables must have different objects
					Assert.AreNotEqual(injectable1.A, injectable2.A);
					Assert.AreNotEqual(injectable1.B, injectable2.B);
					Assert.AreNotEqual(injectable1.C, injectable2.C);
				}
			}
		}

		private static void ValidateInjectable(TestInjectableMonoBehaviour injectable, Container? parentContainer)
		{
			Assert.IsNotNull(injectable);

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			Container sceneContainer = MonoInjector.Containers[injectable.gameObject.scene];
			Assert.AreEqual(sceneContainer.GetObject<IA>(), injectable.A);
			Assert.AreEqual(sceneContainer.GetObject<IB>(), injectable.B);
			Assert.AreEqual(sceneContainer.GetObject<IC>(), injectable.C);

			if (parentContainer != null)
			{
				Assert.AreEqual(parentContainer.GetObject<IA>(), injectable.A);
				Assert.AreEqual(parentContainer.GetObject<IB>(), injectable.B);
				Assert.AreEqual(parentContainer.GetObject<IC>(), injectable.C);
			}
		}

		private static void SetupScene(string scenePath, bool useDefaultParent, Container container, out Scene scene)
		{
			scene = EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive });

			if (useDefaultParent)
			{
				MonoInjector.DefaultParent = container;
			}
			else
			{
				MonoInjector.SetParent(scene, container);
			}
		}
	}
}