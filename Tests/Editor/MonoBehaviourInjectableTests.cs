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

		[UnityTearDown]
		public IEnumerator UnityTearDown()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				yield return new ExitPlayMode();
			}
		}

		[UnityTest]
		public IEnumerator TestDifferentContainers([Values(true, false)] bool useDefaultParent)
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			yield return new EnterPlayMode();

			Container container1 = new();
			SetupContainer(container1, RegisterType.Scoped);
			SetupScene(Scene1, useDefaultParent, container1, out Scene scene1);
			while (!scene1.isLoaded)
			{
				yield return null;
			}
			TestInjectableMonoBehaviour injectable1 = GetAndValidateInjectable(container1, scene1);

			Container container2 = new();
			SetupContainer(container2, RegisterType.Scoped);
			SetupScene(Scene2, useDefaultParent, container2, out Scene scene2);
			while (!scene2.isLoaded)
			{
				yield return null;
			}
			TestInjectableMonoBehaviour injectable2 = GetAndValidateInjectable(container2, scene2);

			// injectable1 and injectable2 should have different objects
			Assert.AreNotEqual(injectable1.A, injectable2.A);
			Assert.AreNotEqual(injectable1.B, injectable2.B);
			Assert.AreNotEqual(injectable1.C, injectable2.C);
		}

		[UnityTest]
		public IEnumerator TestSameContainers([Values(true, false)] bool useDefaultParent)
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			yield return new EnterPlayMode();

			Container container = new();
			SetupContainer(container, RegisterType.Scoped);

			SetupScene(Scene1, useDefaultParent, container, out Scene scene1);
			while (!scene1.isLoaded)
			{
				yield return null;
			}
			TestInjectableMonoBehaviour injectable1 = GetAndValidateInjectable(container, scene1);

			SetupScene(Scene2, useDefaultParent, container, out Scene scene2);
			while (!scene2.isLoaded)
			{
				yield return null;
			}
			TestInjectableMonoBehaviour injectable2 = GetAndValidateInjectable(container, scene2);

			// injectable1 and injectable2 should have the same objects
			Assert.AreEqual(injectable1.A, injectable2.A);
			Assert.AreEqual(injectable1.B, injectable2.B);
			Assert.AreEqual(injectable1.C, injectable2.C);
		}

		private static TestInjectableMonoBehaviour GetAndValidateInjectable(Container parentContainer, Scene scene)
		{
			TestInjectableMonoBehaviour injectable = scene.GetRootGameObjects().Single().GetComponent<TestInjectableMonoBehaviour>();
			Assert.IsNotNull(injectable);

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			Container sceneContainer = MonoInjector.Containers[scene];
			Assert.AreEqual(sceneContainer.GetObject<IA>(), injectable.A);
			Assert.AreEqual(sceneContainer.GetObject<IB>(), injectable.B);
			Assert.AreEqual(sceneContainer.GetObject<IC>(), injectable.C);

			Assert.AreEqual(parentContainer.GetObject<IA>(), injectable.A);
			Assert.AreEqual(parentContainer.GetObject<IB>(), injectable.B);
			Assert.AreEqual(parentContainer.GetObject<IC>(), injectable.C);
			return injectable;
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