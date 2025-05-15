using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Toko.Core
{
    [PublicAPI] public static class Continuations
    {
        public delegate TResult Transformer<in TArg, out TResult>([NotNull] TArg arg);
        public delegate TResult Factory<out TResult>();
        public delegate void Action<in TArg>([NotNull] TArg arg);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult Let<TArg, TResult>(this TArg obj, Transformer<TArg, TResult> transformer) => 
            obj is null ? default : transformer(obj);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult Let<TArg, TResult>(this TArg obj, Factory<TResult> factory) =>
            obj is null ? default : factory();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<TArg>(this TArg obj, Action<TArg> action)
        {
            if (obj is not null)
                action(obj);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<TArg>(this TArg obj, Action action)
        {
            if (obj is not null)
                action();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TArg Also<TArg>(this TArg obj, Action<TArg> action)
        {
            if (obj is not null)
                action(obj);
            
            return obj;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TArg Also<TArg>(this TArg obj, Action action)
        {
            if (obj is not null)
                action();
            
            return obj;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TArg When<TArg>(this TArg obj, bool condition, Transformer<TArg, TArg> transformer) =>
            obj is null ? default : condition ? transformer(obj) : obj;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TRes When<TArg, TRes>(this TArg obj, bool condition, Transformer<TRes, TRes> transformer) where TArg : TRes =>
            obj is null ? default : condition ? transformer(obj) : obj;
    }
}