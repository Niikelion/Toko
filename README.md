![Toko Logo](Assets~/TokoWide512x1024.png)
# Toko

Toko is a state management library for Unity.

It focuses on simplifying the boilerplate around data flow
and making it easier to write event-based code instead of update-based.

Core parts of the library:

- [Contexts](#contexts)
- [Signals](#signals)
- [Resource containers](#resource-containers)
- [Scope functions](#scope-functions)
- [Roadmap](#roadmap)

## Contexts

Contexts provide a way to provide ambient values that can be used from anywhere.

Context is simply a value of type `IContext<T>`.
Context should be created as a static readonly field, so that you don't have to pass it as an argument which would defeat the purpose.

```csharp
public class Terrain: MonoBehaviour
{
    public static readonly Context<Terrain> Context = new(null);
}
```

After defining the context, we can provide some value in this context.

```csharp
public void PlaceBlock(Block block) 
{
    using (Context.Provide(this));
    
    chunks[block.pos.chunk].PlaceBlock(block);
    block.OnPlaced();
}
```

Now it's time for the secret juice - after `using` statement,
every method that is called will be executed inside the context with our value provided.

This means that we can access it without passing it as an argument.
This works for all sub-calls, and persists during execution of coroutines and async methods.

```csharp
public class Block
{
    Terrain currentTerrain;
    //...
    void OnPlaced() 
    {
        terrain = Terrain.Context.Value;
        //place flower on top
        terrain?.Place("Flower", pos + Vector3Int.Up);
    }
}
```

This may not look very useful at the first glance,
but when you can't make the `Terrain` singleton, and also need to pass down the call tree entity that placed the block it quickly becomes messy.
Toko contexts simplify dealing with system instances that can't be made into singletons.

But what about delegates and objects created inside your context?
They will only be constructed inside it, so you will need to pass this context along by yourself.
For objects, you can capture the current context in the constructor and provide it inside the methods to propagate it.

Delegates can be wrapped using `Extend` method that captures current context value and provides it before calling your delegate.

```csharp
Input.OnNewPlayerDetected += GameManager.Context.Extend(SpawnPlayer);
```

## Signals

Signals empower you to write event driven code that reacts to value changes instead of checking values during Update or in coroutine.

### Trigger

Triggers are signals that do not carry any value and work as events that can be passed around as values.
`Trigger` implements `ISignal` interface, that provides `Event` that you can subscribe to.
You can also use the `Subscribe` and `Unsubscribe` methods.

Every `ISignal<T>` signal can be converted to a trigger using `ToTrigger()` method.

Example:
```csharp
using var trigger = new Trigger();
trigger.Subscribe(() => Debug.Log("Triggered"));

trigger.Dispatch();
```

### Signal

Signals work as events with single argument that can be passed around as values.
`Signal<T>` implements `ISignal<T>` interface, that provides `Event` that you can subscribe to.
You can also use the `Subscribe` and `Unsubscribe` methods.

Example:
```csharp
using var signal = new Signal<int>();
signal.Subscribe(v => Debug.Log(v));

signal.Dispatch(5);
```

### Var

Variables are wrappers around values that expose event fired on every value assignment.
They implement `IValueSignal<T>` interface, that extends `ISignal<T>`.
You can read the value with `Value` property, or cast just it to `T`.
Additionally, it implements the `IDependableSignal` interface, which allows detecting it as a dependency.

Example:
```csharp
using var variable = new Var<int>(0);
variable.Subscribe(v => Debug.Log(v));

variable.Value += 5;
Debug.Log(variable.Value);
```

### Val

Values represent signals that are computed based on some dependencies.
You don't event need to provide the list of the dependencies, they just need to implement `IDependableSignal` interface.
`Val` class implements the `IDependentOnSignals` interface, which allows it to track the `IDependableSignal` uses during computation.
It implements the `IDependableSignal` as well, so it can be used as a dependency itself.

Example:
```csharp
using var a = new Var<float>(4f);
using var b = new Var<float>(5f);
using var h = new Var<float>(7f);
using var area = new Val<float>(() => a * b);
using var volume = new Val<float>(() => area * h);

area.Subscribe(v => Debug.Log($"area: {area.Value}"));
volume.Subscribe(v => Debug.Log($"volume: {volume.Value}"));
a.Value = 1;
b.Value = 3;
h.Value = 2;
```

### Effect

Effects work similar to the values, but don't produce any value as the result and cannot be used as dependencies.
`Effect` class implements the `IDependentOnSignals` and `ISignal` interfaces.

Example:
```csharp
using var a = new Var<float>(5f);
using var _ = new Effect(() => Debug.Log(a.Value));
```

## Resource containers

To make cleaning after the observables easier, you can inherit from `MonoBehaviourWithResources` class.
This way, you can call `Use(someResource)` to add it to the list of `IDisposable`s that will be disposed during `OnDestroy`.

## Scope functions

Scope functions are designed to work as closely as possible to the [Kotlin scope functions](https://kotlinlang.org/docs/scope-functions.html).
They are extension methods that can be called on any object, even when its value is null.
When it is null, the computation will not be performed, and null will be returned.
Available extensions:

- `Let` - map value using provided delegate
- `Run` - return value generated using provided delegate
- `Apply` - calls provided delegate with current value as only argument and return current value.
- `Also` - same as `Apply` but accepts delegate without arguments
- `When` - when `condition` is `true` it maps value using provided delegate to some value of the same type or any of the base types or interfaces

## Roadmap

### V2

* [x] `Trigger` - simple trigger
* [x] `Signal` - value emitter
* [x] `Var`- subscribable variable
* [x] `Val` - value that automatically recalculates
* [x] `Effect` - callback with automatic dependencies
* [x] `Cache` - `ISignal<T>` wrapper that drops events when value was not changed, also stores the last value
* [ ] `Debounce` - `ISignal` and `ISignal<T>` wrapper that drops excessive events when sent too fast

### V3

* [ ] subscription scopes
* [ ] disabling / enabling effects