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

namespace Kryz.UnityDI.Tests.Editor
{
	public class ScriptableObjectInjectableTests
	{
		private static readonly string Scene = PackagePath.Path + "/Tests/Shared/Test Scene ScriptableInjectable.unity";
		private static readonly string Asset = PackagePath.Path + "/Tests/Shared/Test Injectable Scriptable Object.asset";

		private IContainer container = null!;

		[UnitySetUp]
		public IEnumerator UnitySetUp()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
				yield return new EnterPlayMode();
				UnityInjector.PushContainer(GetContainerWithRegistrations(Lifetime.Singleton));
				container = UnityInjector.CurrentParent!;
			}
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.isPlaying = false;
			}
			UnityInjector.Clear();
		}

		[UnityTest]
		public IEnumerator TestSceneReference()
		{
			yield return EditorSceneManager.LoadSceneAsyncInPlayMode(Scene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Single });

			MonoWithScriptableObjectReference? mono = Object.FindAnyObjectByType<MonoWithScriptableObjectReference>();
			Assert.IsNotNull(mono);

			ScriptableObject? scriptableObject = mono.ScriptableObject;
			Assert.IsNotNull(scriptableObject);

			TestInjectableScriptableObject injectable = (TestInjectableScriptableObject)scriptableObject!;

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			Assert.AreEqual(container.GetObject<IA>(), injectable.A);
			Assert.AreEqual(container.GetObject<IB>(), injectable.B);
			Assert.AreEqual(container.GetObject<IC>(), injectable.C);
		}

		[Test]
		public void TestProjectAsset()
		{
			TestInjectableScriptableObject injectable = AssetDatabase.LoadAssetAtPath<TestInjectableScriptableObject>(Asset);

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			Assert.AreEqual(container.GetObject<IA>(), injectable.A);
			Assert.AreEqual(container.GetObject<IB>(), injectable.B);
			Assert.AreEqual(container.GetObject<IC>(), injectable.C);
		}

		[Test]
		public void TestCreatedAsset()
		{
			TestInjectableScriptableObject injectable = ScriptableObject.CreateInstance<TestInjectableScriptableObject>();

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			Assert.AreEqual(container.GetObject<IA>(), injectable.A);
			Assert.AreEqual(container.GetObject<IB>(), injectable.B);
			Assert.AreEqual(container.GetObject<IC>(), injectable.C);
		}
	}
}