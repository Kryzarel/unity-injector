using System.Collections;
using Kryz.DI;
using Kryz.DI.Tests;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Kryz.DI.Tests.ContainerTestHelper;

namespace Kryz.MonoDI.Tests
{
	public class SceneTests
	{
		private const string EmptyScene = "Packages/com.kryzarel.monoinjector/Tests/Shared/Test Scene Empty.unity";
		private const string MonoInjectorScene = "Packages/com.kryzarel.monoinjector/Tests/Shared/Test Scene MonoInjector.unity";

		private readonly Container container = new();

		[UnitySetUp]
		public IEnumerator SetUp()
		{
			EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EmptyScene);
			yield return new EnterPlayMode();
			EditorSceneManager.playModeStartScene = null;

			container.Clear();
			SetupContainer(container, RegisterType.Singleton);
			MonoInjector.DefaultParent = container;

			Scene scene = EditorSceneManager.LoadSceneInPlayMode(MonoInjectorScene, default);
			while (!scene.isLoaded)
			{
				yield return null;
			}
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			EditorSceneManager.playModeStartScene = null;
			yield return new ExitPlayMode();
		}

		[Test]
		public void TestInjected1()
		{
			InjectableMonoBehaviour1 injectable1 = Object.FindAnyObjectByType<InjectableMonoBehaviour1>(FindObjectsInactive.Include);
			Assert.IsNotNull(injectable1);
			injectable1.gameObject.SetActive(true);

			Assert.IsNotNull(injectable1.A);
			Assert.IsNotNull(injectable1.B);
			Assert.IsNotNull(injectable1.C);

			Assert.AreEqual(container.GetObject<IA>(), injectable1.A);
			Assert.AreEqual(container.GetObject<IB>(), injectable1.B);
			Assert.AreEqual(container.GetObject<IC>(), injectable1.C);
		}
	}
}