![Toko Logo](Assets~/TokoWide512x1024.png)
# Toko

Toko is a state management library for Unity.

It focuses on simplifying the boilerplate around data flow
and making it easier to write event-based code instead of update-based.

Core parts of the library:

- [Contexts](#contexts)
- [Observables](#observables)
- [Resource containers](#resource-containers)
- [Scope functions](#scope-functions)

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

## Observables

Interfaces `IObservable` and `IObersable<T>: IObservable` represent update sources.
The second variant is also a value emitter.
You can subscribe to the plain `OnUpdate` event of the source to react to all updates.
To watch value changes and also get the value in an argument, you can subscribe to `OnChange` event.

If you just want something similar to the C# events, you should use `Observable` and `Observable<T>` implementations of these interfaces.

Public events coupled with some public field that are fired on their update can be replaced with `Variable<T>` observable.
It fires events on every value change.
Additionally, when subscribing to the `OnChange` event with some delegate, it will be immediately called with the current value.

The last building block of the Toko observables is `Computation` class.
It can be used to easily compute some values based on other value emitters.
It automatically subscribes to value emitters from the provided dependency list and if any of them changes, recalculates the value.
Before firing its own events, it checks if the computed value actually changed.
You can provide your own comparison function to control or even disable this behavior.

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