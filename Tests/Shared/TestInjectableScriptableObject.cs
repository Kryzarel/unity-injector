using Kryz.DI.Tests;

namespace Kryz.UnityDI.Tests
{
	public class TestInjectableScriptableObject : ScriptableObject<IA, IB, IC>
	{
		public IA? A;
		public IB? B;
		public IC? C;

		protected override void Init(IA a, IB b, IC c)
		{
			A = a;
			B = b;
			C = c;
		}
	}
}