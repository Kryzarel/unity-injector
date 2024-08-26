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
		public IEnumerator TestInjectable([Values(true, false)] bool useDefaultParent)
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			yield return new EnterPlayMode();

			SetupContainerAndScene(Scene1, useDefaultParent, out Container container1, out Scene scene1);
			while (!scene1.isLoaded)
			{
				yield return null;
			}
			TestInjectableMonoBehaviour injectable1 = GetAndValidateInjectable(container1, scene1);

			SetupContainerAndScene(Scene2, useDefaultParent, out Container container2, out Scene scene2);
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

		private static TestInjectableMonoBehaviour GetAndValidateInjectable(Container container, Scene scene)
		{
			TestInjectableMonoBehaviour injectable = scene.GetRootGameObjects().Single().GetComponent<TestInjectableMonoBehaviour>();
			Assert.IsNotNull(injectable);

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			Assert.AreEqual(container.GetObject<IA>(), injectable.A);
			Assert.AreEqual(container.GetObject<IB>(), injectable.B);
			Assert.AreEqual(container.GetObject<IC>(), injectable.C);
			return injectable;
		}

		private static void SetupContainerAndScene(string scenePath, bool useDefaultParent, out Container container, out Scene scene)
		{
			container = new Container();
			SetupContainer(container, RegisterType.Scoped);
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