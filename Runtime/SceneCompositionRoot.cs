using System;
using Kryz.DI;
using UnityEngine;

namespace Kryz.UnityDI
{
	/// <summary>
	/// <para>You should only have one <see cref="SceneCompositionRoot"/> per scene. Even though, technically, there's nothing preventing you from having multiple.</para>
	/// <para>It is recommended to have only one for cleaner organization and for being consistent with the principles of DI and IoC, where all registrations should happen in a single centralized place.</para>
	/// </summary>
	public abstract class SceneCompositionRoot : MonoBehaviour
	{
		protected virtual void Awake()
		{
			if (UnityInjector.TryGetSceneBuilder(gameObject.scene, out IBuilder? builder))
			{
				Register(builder);
				return;
			}
			throw new InvalidOperationException($"Failed to get {nameof(IBuilder)} for {nameof(GameObject)} \"{name}\" in scene \"{gameObject.scene.name}\"");
		}

		protected abstract void Register(IBuilder builder);
	}
}