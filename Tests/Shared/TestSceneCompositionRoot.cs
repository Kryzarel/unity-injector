using System;
using Kryz.DI;
using Kryz.DI.Tests;

namespace Kryz.UnityDI.Tests
{
	public class TestSceneCompositionRoot : SceneCompositionRoot
	{
		public static event Action<TestSceneCompositionRoot>? OnRegister;

		protected override void Register(IScopeBuilder builder)
		{
			builder.Register<IA, A>(Lifetime.Singleton);
			builder.Register<IB, B>(Lifetime.Singleton);
			builder.Register<IC, C>(Lifetime.Singleton);
			OnRegister?.Invoke(this);
		}
	}
}