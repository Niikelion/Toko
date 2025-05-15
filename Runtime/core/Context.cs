using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;

namespace Toko.Core
{
    [PublicAPI]
    public interface IContext<T>
    {
        public T Value { get; }

        public Finally Provide(T value);
    }
    
    [PublicAPI]
    public class UseOutsideScopeException : Exception
    {
        public UseOutsideScopeException() : base("Cannot read context value outside context scope") { }
    }
    
    [PublicAPI]
    public class Context<T> : IContext<T>
    {
        public T Value => valueStack.Value.TryPeek(out var value) ? value : throw new UseOutsideScopeException();

        private readonly AsyncLocal<Stack<T>> valueStack = new();

        public Context(T initialValue) => (valueStack.Value ??= new()).Push(initialValue);

        public Finally Provide(T value)
        {
            var stack = valueStack.Value;
            stack.Push(value);
            return new(() => stack.Pop());
        }
    }

    [PublicAPI]
    public static class ContextExtensions
    {
        public static Action Extend<T>([NotNull] this IContext<T> context, [NotNull] Action action)
        {
            var value = context.Value;
            return () => { using (context.Provide(value)) action(); };
        }
        public static Action<T1> Extend<T, T1>([NotNull] this IContext<T> context, [NotNull] Action<T1> action)
        {
            var value = context.Value;
            return arg1 => { using (context.Provide(value)) action(arg1); };
        }
        public static Action<T1, T2> Extend<T, T1, T2>([NotNull] this IContext<T> context, [NotNull] Action<T1, T2> action)
        {
            var value = context.Value;
            return (arg1, arg2) => { using (context.Provide(value)) action(arg1, arg2); };
        }
        public static Action<T1, T2, T3> Extend<T, T1, T2, T3>([NotNull] this IContext<T> context, [NotNull] Action<T1, T2, T3> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3) => { using (context.Provide(value)) action(arg1, arg2, arg3); };
        }
        public static Action<T1, T2, T3, T4> Extend<T, T1, T2, T3, T4>([NotNull] this IContext<T> context, [NotNull] Action<T1, T2, T3, T4> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4); };
        }
        public static Action<T1, T2, T3, T4, T5> Extend<T, T1, T2, T3, T4, T5>([NotNull] this IContext<T> context, [NotNull] Action<T1, T2, T3, T4, T5> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4, arg5) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4, arg5); };
        }
        public static Action<T1, T2, T3, T4, T5, T6> Extend<T, T1, T2, T3, T4, T5, T6>([NotNull] this IContext<T> context, [NotNull] Action<T1, T2, T3, T4, T5, T6> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4, arg5, arg6) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4, arg5, arg6); };
        }
        public static Action<T1, T2, T3, T4, T5, T6, T7> Extend<T, T1, T2, T3, T4, T5, T6, T7>([NotNull] this IContext<T> context, [NotNull] Action<T1, T2, T3, T4, T5, T6, T7> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4, arg5, arg6, arg7); };
        }
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Extend<T, T1, T2, T3, T4, T5, T6, T7, T8>([NotNull] this IContext<T> context, [NotNull] Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            var value = context.Value;
            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => { using (context.Provide(value)) action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); };
        }
    }
}