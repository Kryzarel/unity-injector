# Unity Injector

Unity-specific portion of a Dependency Injection framework for C# and Unity. This package contains Unity-specific code for better integration with the engine.

### YouTube Tutorial: https://youtu.be/5GnNG5EmP5U

Made mainly for personal use and educational purposes. Watch the video for a step-by-step tutorial on how it was implemented.

## Installation

### Dependencies

[Unity Injector](https://github.com/Kryzarel/unity-injector) requires the [Dependency Injector](https://github.com/Kryzarel/dependency-injector) package, which in turn requires the [C# Utilities](https://github.com/Kryzarel/c-sharp-utilities) package to be installed.

### Install via Git URL

1. Navigate to your project's Packages folder and open the `manifest.json` file.
2. Add these three lines:
	-	```json
		"com.kryzarel.unity-injector": "https://github.com/Kryzarel/unity-injector.git",
		```
	-	```json
		"com.kryzarel.dependency-injector": "https://github.com/Kryzarel/dependency-injector.git",
		```
	-	```json
		"com.kryzarel.c-sharp-utilities": "https://github.com/Kryzarel/c-sharp-utilities.git",
		```

### Install manually

1. Clone or download the [Unity Injector](https://github.com/Kryzarel/unity-injector), the [Dependency Injector](https://github.com/Kryzarel/dependency-injector) and the [C# Utilities](https://github.com/Kryzarel/c-sharp-utilities) repositories.
2. Copy/paste or move all repository folders directly into your project's Packages folder or into the Assets folder.

## Usage Examples

### Basic Usage

Check the [Dependency Injector](https://github.com/Kryzarel/dependency-injector) repository for usage of the base package.

Before loading a scene:
```csharp
Builder builder = new Builder();
builder.Register<ISingleton, ConcreteSingleton>(Lifetime.Singleton); // Singleton registration (only 1 instance per registration will exist)

IContainer container = builder.Build(); // Build a Container with the registrations. The created Container's registrations are read-only

UnityInjector.PushContainer(container); // Push the Container we just created to UnityInjector's "container stack"

// These scenes will be able to access the registrations from the Container we just pushed
SceneManager.LoadScene("My Scene 1");
SceneManager.LoadScene("My Scene 2");

// This should be done after the scene is no longer being used. After it's unloaded. It's shown here for illustration purposes only:
UnityInjector.PopContainer(container); // Remove the Container from the "container stack"
```

In the scene (make your components inherit `MonoBehaviour<T1, T2, T3, ...>`):
```csharp
// This Component will have access to the Singleton object registered before loading the scene
public class MyComponent : MonoBehaviour<ISingleton>, IComponent
{
	public ISingleton Singleton { get; private set; } = null!;

	protected override void Init(ISingleton arg1)
	{
		Singleton = arg1;
	}
}
```
The `Init()` function runs at the same time as Unity's `Start()` function.

### Registering Inside Scenes

Create a class that inherits from `SceneCompositionRoot` and register your dependencies in the `Register()` method. Drag that component to an object in your scene.
```csharp
public class SceneCompositionRootExample : SceneCompositionRoot
{
	[SerializeField] MyComponent component; // Assign this through the inspector

	protected override void Register(IBuilder builder)
	{
		builder.Register<IComponent>(component);
	}
}
```
Now, MonoBehaviours that inherit from `MonoBehaviour<IComponent>` will have access to the `MyComponent` object via Dependency Injection.

The `Register()` function runs at the same time as Unity's `Awake()` function.

Registrations inside scenes can only be done when the scene is loading. After the `SceneManager.sceneLoaded` event for that scene has fired, no new registrations can happen for that scene. This is by design. Containers are read-only for safety.

## Author

[Kryzarel](https://www.youtube.com/@Kryzarel)

## License

MIT