using System.Collections;
using Kryz.DI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Kryz.MonoDI.Tests
{
	public class MonoInjectorTests
	{
		[Test]
		public void TestClear()
		{
			MonoInjector.Clear();
			Assert.AreEqual(DependencyInjector.RootContainer, MonoInjector.DefaultParent);
			Assert.AreEqual(0, MonoInjector.Containers.Count, 0);
			Assert.AreEqual(0, MonoInjector.ParentContainers.Count, 0);
		}

		[UnityTest]
		public IEnumerator TestDomainReload()
		{
			EditorUtility.RequestScriptReload();
			yield return new WaitForDomainReload();
			Assert.AreEqual(DependencyInjector.RootContainer, MonoInjector.DefaultParent);
			Assert.AreEqual(0, MonoInjector.Containers.Count, 0);
			Assert.AreEqual(0, MonoInjector.ParentContainers.Count, 0);
		}
	}
}