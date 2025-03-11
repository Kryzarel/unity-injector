using Kryz.DI;
using UnityEngine;

namespace Kryz.UnityDI
{
	public abstract class ScriptableObjectInjectable : ScriptableObject
	{
		protected void OnEnable()
		{
			// Avoid Injection Exceptions during edit time
			if (Application.isPlaying)
			{
				Init();
			}
		}

		public void Init()
		{
			IContainer container = UnityInjector.DefaultParent ?? DependencyInjector.RootContainer;
			container.Injector.Inject(this, container);
		}
	}

	public abstract class ScriptableObject<T1> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1);
	}

	public abstract class ScriptableObject<T1, T2> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2);
	}

	public abstract class ScriptableObject<T1, T2, T3> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30, T31 arg31);
	}

	public abstract class ScriptableObject<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> : ScriptableObjectInjectable
	{
		[Inject]
		protected abstract void Init(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, T21 arg21, T22 arg22, T23 arg23, T24 arg24, T25 arg25, T26 arg26, T27 arg27, T28 arg28, T29 arg29, T30 arg30, T31 arg31, T32 arg32);
	}
}