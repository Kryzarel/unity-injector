using System.Collections;
using Kryz.DI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Kryz.UnityDI.Tests
{
	public class UnityInjectorTests
	{
		[Test]
		public void TestClear()
		{
			UnityInjector.Clear();
			Assert.AreEqual(DependencyInjector.RootContainer, UnityInjector.DefaultParent);
			Assert.AreEqual(0, UnityInjector.Containers.Count, 0);
			Assert.AreEqual(0, UnityInjector.ParentContainers.Count, 0);
		}

		[UnityTest]
		public IEnumerator TestDomainReload()
		{
			EditorUtility.RequestScriptReload();
			yield return new WaitForDomainReload();
			Assert.AreEqual(DependencyInjector.RootContainer, UnityInjector.DefaultParent);
			Assert.AreEqual(0, UnityInjector.Containers.Count, 0);
			Assert.AreEqual(0, UnityInjector.ParentContainers.Count, 0);
		}
	}
}