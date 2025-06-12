using System;
using System.Collections;
using System.Collections.Generic;
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
	public class MonoBehaviourInjectableTests
	{
		private static readonly string CompositionRootScene = PackagePath.Path + "/Tests/Shared/Test Scene CompositionRoot.unity";
		private static readonly string MonoInjectableScene1 = PackagePath.Path + "/Tests/Shared/Test Scene MonoInjectable 1.unity";
		private static readonly string MonoInjectableScene2 = PackagePath.Path + "/Tests/Shared/Test Scene MonoInjectable 2.unity";

		private static readonly Lifetime[] lifetimes = (Lifetime[])Enum.GetValues(typeof(Lifetime));

		[UnitySetUp]
		public IEnumerator UnitySetUp()
		{
			UnityInjector.Clear();
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			yield return new EnterPlayMode();
		}

		[UnityTearDown]
		public IEnumerator UnityTearDown()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				yield return new ExitPlayMode();
			}
			UnityInjector.Clear();
		}

		[Test]
		public void TestEverything([ValueSource(nameof(lifetimes))] Lifetime lifetime)
		{
			// Arrange
			UnityInjector.PushContainer(builder =>
			{
				builder.Register<IA, A>(lifetime);
				builder.Register<IB, B>(lifetime);
				builder.Register<IC, C>(lifetime);
			});

			// Act
			Scene scene = EditorSceneManager.LoadSceneInPlayMode(CompositionRootScene, new LoadSceneParameters(LoadSceneMode.Additive));
			TestInjectableMonoBehaviour[] compositionRootInjectables = GetInjectables(scene);

			scene = EditorSceneManager.LoadSceneInPlayMode(MonoInjectableScene1, new LoadSceneParameters(LoadSceneMode.Additive));
			TestInjectableMonoBehaviour[] monoInjectable1Injectables = GetInjectables(scene);

			scene = EditorSceneManager.LoadSceneInPlayMode(MonoInjectableScene2, new LoadSceneParameters(LoadSceneMode.Additive));
			TestInjectableMonoBehaviour[] monoInjectable2Injectables = GetInjectables(scene);

			// Assert
			AssertAgainstContainer(compositionRootInjectables, UnityInjector.CurrentParent, objectsMatchContainer: false); // Should never match because composition root re-registers the same types
			AssertAgainstContainer(monoInjectable1Injectables, UnityInjector.CurrentParent, objectsMatchContainer: lifetime == Lifetime.Singleton); // Should only match when lifetime is singleton
			AssertAgainstContainer(monoInjectable2Injectables, UnityInjector.CurrentParent, objectsMatchContainer: lifetime == Lifetime.Singleton); // Should only match when lifetime is singleton

			AssertAgainstInjectables(compositionRootInjectables, compositionRootInjectables, objectsMatch: true); // Composition Root Scene, lifetime is always singleton, objects should always match
			AssertAgainstInjectables(monoInjectable1Injectables, monoInjectable1Injectables, objectsMatch: lifetime != Lifetime.Transient); // Same scene, should match except when lifetime is transient
			AssertAgainstInjectables(monoInjectable2Injectables, monoInjectable2Injectables, objectsMatch: lifetime != Lifetime.Transient); // Same scene, should match except when lifetime is transient
			AssertAgainstInjectables(compositionRootInjectables, monoInjectable1Injectables, objectsMatch: false); // One of the scenes has composition root, should never match
			AssertAgainstInjectables(compositionRootInjectables, monoInjectable2Injectables, objectsMatch: false); // One of the scenes has composition root, should never match
			AssertAgainstInjectables(monoInjectable1Injectables, monoInjectable2Injectables, objectsMatch: lifetime == Lifetime.Singleton); // Different scenes, no composition root, should only match when lifetime is singleton
		}

		private static TestInjectableMonoBehaviour[] GetInjectables(Scene scene)
		{
			List<TestInjectableMonoBehaviour> injectables = new();
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				if (gameObject.TryGetComponent(out TestInjectableMonoBehaviour injectable))
				{
					injectables.Add(injectable);
				}
			}
			return injectables.ToArray();
		}

		private static void AssertAgainstContainer(TestInjectableMonoBehaviour[] injectables, IContainer container, bool objectsMatchContainer)
		{
			foreach (TestInjectableMonoBehaviour injectable in injectables)
			{
				Assert.IsNotNull(injectable.A);
				Assert.IsNotNull(injectable.B);
				Assert.IsNotNull(injectable.C);

				IContainer sceneContainer = UnityInjector.SceneContainers[injectable.gameObject.scene];
				Assert.AreEqual(sceneContainer.GetObject<IA>(), injectable.A);
				Assert.AreEqual(sceneContainer.GetObject<IB>(), injectable.B);
				Assert.AreEqual(sceneContainer.GetObject<IC>(), injectable.C);

				Action<object?, object?> assertEquality = objectsMatchContainer ? Assert.AreEqual : Assert.AreNotEqual;
				assertEquality(container.GetObject<IA>(), injectable.A);
				assertEquality(container.GetObject<IB>(), injectable.B);
				assertEquality(container.GetObject<IC>(), injectable.C);
			}
		}

		private static void AssertAgainstInjectables(TestInjectableMonoBehaviour[] injectables1, TestInjectableMonoBehaviour[] injectables2, bool objectsMatch)
		{
			Action<object?, object?> assertEquality = objectsMatch ? Assert.AreEqual : Assert.AreNotEqual;

			foreach (TestInjectableMonoBehaviour injectable1 in injectables1)
			{
				foreach (TestInjectableMonoBehaviour injectable2 in injectables2)
				{
					Assert.IsNotNull(injectable1.A);
					Assert.IsNotNull(injectable1.B);
					Assert.IsNotNull(injectable1.C);

					Assert.IsNotNull(injectable2.A);
					Assert.IsNotNull(injectable2.B);
					Assert.IsNotNull(injectable2.C);

					assertEquality(injectable1.A, injectable2.A);
					assertEquality(injectable1.B, injectable2.B);
					assertEquality(injectable1.C, injectable2.C);
				}
			}
		}
	}
}