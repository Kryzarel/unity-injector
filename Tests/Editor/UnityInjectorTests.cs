using System.Collections;
using Kryz.DI;
using Kryz.DI.Tests;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Kryz.UnityDI.Tests.Editor
{
	public class UnityInjectorTests
	{
		[SetUp]
		public void SetUp()
		{
			UnityInjector.Instance.Clear();
		}

		[UnityTest]
		public IEnumerator DomainReload()
		{
			// Assert
			AssertCleanInjector(UnityInjector.Instance);

			// Arrange
			UnityInjector.Instance.PushContainer();

			// Act
			EditorUtility.RequestScriptReload();
			yield return new WaitForDomainReload();

			// Assert
			AssertCleanInjector(UnityInjector.Instance);
		}

		[Test]
		public void Clear([ValueSource(nameof(InjectorValueSource))] UnityInjector unityInjector)
		{
			// Assert
			AssertCleanInjector(unityInjector);

			// Arrange
			unityInjector.PushContainer();

			// Act
			unityInjector.Clear();

			// Assert
			AssertCleanInjector(unityInjector);
		}

		[Test]
		public void PushEmptyContainer([ValueSource(nameof(InjectorValueSource))] UnityInjector unityInjector)
		{
			// Arrange
			IContainer container = new Builder().Build();

			// Act
			unityInjector.PushContainer(container);

			// Assert
			Assert.AreEqual(2, unityInjector.ParentContainers.Count, 0);
			Assert.AreEqual(container, unityInjector.CurrentParent);
			Assert.IsFalse(unityInjector.CurrentParent.TryGetType<IA>(out _));
		}

		[Test]
		public void PushContainer([ValueSource(nameof(InjectorValueSource))] UnityInjector unityInjector)
		{
			// Arrange
			IContainer container = new Builder().Register<IA, A>(Lifetime.Singleton).Build();

			// Act
			unityInjector.PushContainer(container);

			// Assert
			Assert.AreEqual(2, unityInjector.ParentContainers.Count, 0);
			Assert.AreEqual(container, unityInjector.CurrentParent);
			Assert.IsTrue(unityInjector.CurrentParent.TryGetType<IA>(out _));
		}

		private static IEnumerable InjectorValueSource()
		{
			yield return new UnityInjector();
			yield return UnityInjector.Instance;
		}

		private static void AssertCleanInjector(UnityInjector unityInjector)
		{
			Assert.IsNotNull(unityInjector.CurrentParent);
			Assert.AreEqual(unityInjector.CurrentParent, unityInjector.ParentContainers[0]);
			Assert.AreEqual(1, unityInjector.ParentContainers.Count, 0);
			Assert.AreEqual(0, unityInjector.SceneBuilders.Count, 0);
			Assert.AreEqual(0, unityInjector.SceneContainers.Count, 0);
		}
	}
}