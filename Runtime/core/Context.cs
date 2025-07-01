#nullable enable
using System;
using System.Threading;

namespace Toko.Core
{
    public interface IContext<T>
    {
        public T Value { get; }

        public Finally Provide(T value);
    }
    
    public sealed class Context<T> : IContext<T>
    {
        public T Value => valueStack.Value.Peek();

        private readonly AsyncLocal<ImmutableStack<T>> valueStack = new();

        public static Context<T> New => new(default!);
        
        public Context(T initialValue) => valueStack.Value = ImmutableStack<T>.Empty.Push(initialValue);

        public Finally Provide(T value)
        {
            Push(value);
            return new(Pop);
        }

        private void Push(T value) => valueStack.Value = valueStack.Value.Push(value);
        private void Pop() => valueStack.Value = valueStack.Value.Pop();
    }
    
    public static class ContextExtensions
    {
        public static Action Extend<T>(this IContext<T> context, Action action)
        {
            var value = context.Value;
            return () => { using (context.Provide(value)) action(); };
        }
        public static Action<T1> Extend<T, T1>(this IContext<T> context, Action<T1> action)
        {
            var value = context.Value;
            return arg1 => { using (context.Provide(value)) action(arg1); };
        }
        public static Action<T1, T2> Extend<T, T1, T2>(this IContext<T> context, Action<T1, T2> action)
        {
            var value = context.Value;
            return (arg1, arg2) => { using (context.Provide(value)) action(arg1, arg2); };
        }
        public static Action<T1, T2, T3> Extend<T, T1, T2, T3>(this IContext<T> context, Action<T1, T2, T3> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3) => { using (context.Provide(value)) action(arg1, arg2, arg3); };
        }
        public static Action<T1, T2, T3, T4> Extend<T, T1, T2, T3, T4>(this IContext<T> context, Action<T1, T2, T3, T4> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4); };
        }
        public static Action<T1, T2, T3, T4, T5> Extend<T, T1, T2, T3, T4, T5>(this IContext<T> context, Action<T1, T2, T3, T4, T5> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4, arg5) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4, arg5); };
        }
        public static Action<T1, T2, T3, T4, T5, T6> Extend<T, T1, T2, T3, T4, T5, T6>(this IContext<T> context, Action<T1, T2, T3, T4, T5, T6> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4, arg5, arg6) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4, arg5, arg6); };
        }
        public static Action<T1, T2, T3, T4, T5, T6, T7> Extend<T, T1, T2, T3, T4, T5, T6, T7>(this IContext<T> context, Action<T1, T2, T3, T4, T5, T6, T7> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4, arg5, arg6, arg7); };
        }
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Extend<T, T1, T2, T3, T4, T5, T6, T7, T8>(this IContext<T> context, Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); };
        }
    }
}