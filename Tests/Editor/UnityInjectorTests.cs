using System.Collections;
using Kryz.DI;
using Kryz.DI.Tests;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Kryz.UnityDI.Tests.Editor
{
	public class UnityInjectorTests
	{
		private static readonly string CompositionRootScene = PackagePath.Path + "/Tests/Shared/Test Scene CompositionRoot.unity";
		private static readonly string MonoInjectableScene1 = PackagePath.Path + "/Tests/Shared/Test Scene MonoInjectable 1.unity";

		[SetUp]
		public void SetUp()
		{
			UnityInjector.Clear();
		}

		[UnityTest]
		public IEnumerator DomainReload()
		{
			// Assert
			AssertCleanInjector();

			// Arrange
			UnityInjector.PushContainer();

			// Act
			EditorUtility.RequestScriptReload();
			yield return new WaitForDomainReload();

			// Assert
			AssertCleanInjector();
		}

		[Test]
		public void Clear()
		{
			// Assert
			AssertCleanInjector();

			// Arrange
			UnityInjector.PushContainer();

			// Act
			UnityInjector.Clear();

			// Assert
			AssertCleanInjector();
		}

		[Test]
		public void PushEmptyContainer()
		{
			// Arrange
			IContainer container = new Builder().Build();

			// Act
			UnityInjector.PushContainer(container);

			// Assert
			Assert.AreEqual(2, UnityInjector.ParentContainers.Count, 0);
			Assert.AreEqual(container, UnityInjector.CurrentParent);
			Assert.IsFalse(UnityInjector.CurrentParent.TryGetType<IA>(out _));
		}

		[Test]
		public void PushContainer()
		{
			// Arrange
			IContainer container = new Builder().Register<IA, A>(Lifetime.Singleton).Build();

			// Act
			UnityInjector.PushContainer(container);

			// Assert
			Assert.AreEqual(2, UnityInjector.ParentContainers.Count, 0);
			Assert.AreEqual(container, UnityInjector.CurrentParent);
			Assert.IsTrue(UnityInjector.CurrentParent.TryGetType<IA>(out _));
		}

		[UnityTest]
		public IEnumerator LoadScene_CompositionRoot()
		{
			// Arrange
			Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			yield return new EnterPlayMode();
			// Dependencies for this scene are registered in TestSceneCompositionRoot component, which lives directly in the scene.
			TestSceneCompositionRoot.OnRegister += OnRegister;

			// Act
			AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(CompositionRootScene, default);

			// Assert
			Assert.AreEqual(0, UnityInjector.SceneBuilders.Count, 0);
			Assert.AreEqual(1, UnityInjector.SceneContainers.Count, 0);
			Assert.IsTrue(UnityInjector.SceneContainers.ContainsKey(scene));

			// Act
			yield return operation;

			// Assert
			static void OnRegister(TestSceneCompositionRoot compositionRoot)
			{
				Scene scene = SceneManager.GetActiveScene();
				Assert.AreEqual(scene, compositionRoot.gameObject.scene);
				Assert.IsTrue(UnityInjector.SceneBuilders.ContainsKey(scene));
				Assert.AreEqual(1, UnityInjector.SceneBuilders.Count, 0);
				Assert.AreEqual(0, UnityInjector.SceneContainers.Count, 0);
			}

			Assert.AreEqual(0, UnityInjector.SceneBuilders.Count, 0);
			Assert.AreEqual(1, UnityInjector.SceneContainers.Count, 0);
			scene = SceneManager.GetActiveScene();
			Assert.IsTrue(UnityInjector.SceneContainers.ContainsKey(scene));

			TestSceneCompositionRoot.OnRegister -= OnRegister;
			yield return new ExitPlayMode();
		}

		[UnityTest]
		public IEnumerator LoadScene_MonoInjectableOnly()
		{
			// Arrange
			Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			yield return new EnterPlayMode();

			// Dependencies for this scene are registered in a new Container in UnityInjector.
			UnityInjector.PushContainer(builder =>
			{
				builder.Register<IA, A>(Lifetime.Singleton);
				builder.Register<IB, B>(Lifetime.Singleton);
				builder.Register<IC, C>(Lifetime.Singleton);
			});

			// Act
			AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(MonoInjectableScene1, default);

			// Assert
			Assert.AreEqual(0, UnityInjector.SceneBuilders.Count, 0);
			Assert.AreEqual(1, UnityInjector.SceneContainers.Count, 0);
			Assert.IsTrue(UnityInjector.SceneContainers.ContainsKey(scene));

			// Act
			yield return operation;

			// Assert
			Assert.AreEqual(0, UnityInjector.SceneBuilders.Count, 0);
			Assert.AreEqual(1, UnityInjector.SceneContainers.Count, 0);
			scene = SceneManager.GetActiveScene();
			Assert.IsTrue(UnityInjector.SceneContainers.ContainsKey(scene));

			yield return new ExitPlayMode();
		}

		private static void AssertCleanInjector()
		{
			Assert.IsNotNull(UnityInjector.CurrentParent);
			Assert.AreEqual(UnityInjector.CurrentParent, UnityInjector.ParentContainers[0]);
			Assert.AreEqual(1, UnityInjector.ParentContainers.Count, 0);
			Assert.AreEqual(0, UnityInjector.SceneBuilders.Count, 0);
			Assert.AreEqual(0, UnityInjector.SceneContainers.Count, 0);
		}
	}
}