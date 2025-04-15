using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Kryz.UnityDI.Tests.Editor
{
	public class UnityInjectorTests
	{
		[Test]
		public void TestClear()
		{
			UnityInjector.Clear();
			Assert.IsNull(UnityInjector.DefaultContainer);
			Assert.AreEqual(0, UnityInjector.Containers.Count, 0);
			Assert.AreEqual(0, UnityInjector.ParentContainers.Count, 0);
		}

		[UnityTest]
		public IEnumerator TestDomainReload()
		{
			EditorUtility.RequestScriptReload();
			yield return new WaitForDomainReload();
			Assert.IsNull(UnityInjector.DefaultContainer);
			Assert.AreEqual(0, UnityInjector.Containers.Count, 0);
			Assert.AreEqual(0, UnityInjector.ParentContainers.Count, 0);
		}
	}
}