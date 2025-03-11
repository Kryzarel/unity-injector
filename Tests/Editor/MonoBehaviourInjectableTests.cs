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

namespace Kryz.UnityDI.Tests.Editor
{
	public class MonoBehaviourInjectableTests
	{
		private static readonly string Scene1 = PackagePath.Path + "/Tests/Shared/Test Scene MonoInjectable 1.unity";
		private static readonly string Scene2 = PackagePath.Path + "/Tests/Shared/Test Scene MonoInjectable 2.unity";

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
				IContainer container = SetupContainer(Lifetime.Singleton);
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

			IContainer container = SetupContainer(Lifetime.Singleton);

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
					// SetupContainer(UnityInjector.GetContainer(scene)!, RegisterType.Scoped);
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

		private static void ValidateInjectable(TestInjectableMonoBehaviour injectable, IContainer? parentContainer)
		{
			Assert.IsNotNull(injectable);

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			IContainer sceneContainer = UnityInjector.Containers[injectable.gameObject.scene];
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

		private static void SetupScene(string scenePath, bool useDefaultParent, IContainer container, out Scene scene)
		{
			scene = EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive });

			if (useDefaultParent)
			{
				UnityInjector.DefaultParent = container;
			}
			else
			{
				UnityInjector.SetParent(scene, container);
			}
		}
	}
}