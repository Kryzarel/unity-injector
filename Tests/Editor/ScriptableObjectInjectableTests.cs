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
	public class ScriptableObjectInjectableTests
	{
		private static readonly string Scene = PackagePath.Path + "/Tests/Shared/Test Scene ScriptableInjectable.unity";
		private static readonly string Asset = PackagePath.Path + "/Tests/Shared/Test Injectable Scriptable Object.asset";

		[UnitySetUp]
		public IEnumerator UnitySetUp()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
				yield return new EnterPlayMode();
				UnityInjector.PushContainer(builder =>
				{
					builder.Register<IA, A>(Lifetime.Singleton);
					builder.Register<IB, B>(Lifetime.Singleton);
					builder.Register<IC, C>(Lifetime.Singleton);
				});
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
			yield return EditorSceneManager.LoadSceneAsyncInPlayMode(Scene, new LoadSceneParameters(LoadSceneMode.Single));

			MonoWithScriptableObjectReference? mono = Object.FindAnyObjectByType<MonoWithScriptableObjectReference>();
			Assert.IsNotNull(mono);

			ScriptableObject? scriptableObject = mono.ScriptableObject;
			Assert.IsNotNull(scriptableObject);

			TestInjectableScriptableObject injectable = (TestInjectableScriptableObject)scriptableObject!;

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			Assert.AreEqual(UnityInjector.CurrentParent.GetObject<IA>(), injectable.A);
			Assert.AreEqual(UnityInjector.CurrentParent.GetObject<IB>(), injectable.B);
			Assert.AreEqual(UnityInjector.CurrentParent.GetObject<IC>(), injectable.C);
		}

		[Test]
		public void TestProjectAsset()
		{
			TestInjectableScriptableObject injectable = AssetDatabase.LoadAssetAtPath<TestInjectableScriptableObject>(Asset);

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			Assert.AreEqual(UnityInjector.CurrentParent.GetObject<IA>(), injectable.A);
			Assert.AreEqual(UnityInjector.CurrentParent.GetObject<IB>(), injectable.B);
			Assert.AreEqual(UnityInjector.CurrentParent.GetObject<IC>(), injectable.C);
		}

		[Test]
		public void TestCreatedAsset()
		{
			TestInjectableScriptableObject injectable = ScriptableObject.CreateInstance<TestInjectableScriptableObject>();

			Assert.IsNotNull(injectable.A);
			Assert.IsNotNull(injectable.B);
			Assert.IsNotNull(injectable.C);

			Assert.AreEqual(UnityInjector.CurrentParent.GetObject<IA>(), injectable.A);
			Assert.AreEqual(UnityInjector.CurrentParent.GetObject<IB>(), injectable.B);
			Assert.AreEqual(UnityInjector.CurrentParent.GetObject<IC>(), injectable.C);
		}
	}
}