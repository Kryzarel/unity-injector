using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Kryz.DI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Kryz.UnityDI.Tests.Editor
{
	public class UnityInjectorTests
	{
		private const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		private static readonly FieldInfo sceneBuildersInfo = typeof(UnityInjector).GetField("sceneBuilders", flags);

		[Test]
		public void TestInstance()
		{
			// Arrange
			UnityInjector unityInjector = new();

			// Assert
			AssertCleanInjector(unityInjector);

			// Act
			unityInjector.Clear();

			// Assert
			AssertCleanInjector(unityInjector);
		}

		[Test]
		public void TestClear()
		{
			// Act
			UnityInjector.Instance.Clear();

			// Assert
			AssertCleanInjector(UnityInjector.Instance);
		}

		[UnityTest]
		public IEnumerator TestDomainReload()
		{
			// Act
			EditorUtility.RequestScriptReload();
			yield return new WaitForDomainReload();

			// Assert
			AssertCleanInjector(UnityInjector.Instance);
		}

		private static void AssertCleanInjector(UnityInjector unityInjector)
		{
			Assert.IsNotNull(unityInjector.CurrentParent);
			Assert.AreEqual(unityInjector.CurrentParent, unityInjector.ParentContainers[0]);
			Assert.AreEqual(1, unityInjector.ParentContainers.Count, 0);
			Assert.AreEqual(0, unityInjector.SceneContainers.Count, 0);

			var sceneBuilders = (Dictionary<Scene, IBuilder>)sceneBuildersInfo.GetValue(unityInjector);
			Assert.AreEqual(0, sceneBuilders.Count, 0);
		}
	}
}