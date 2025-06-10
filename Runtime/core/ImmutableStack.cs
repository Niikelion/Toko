#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Toko.Core
{
    public sealed class ImmutableStack<T>: IEnumerable<T>
    {
        public static readonly ImmutableStack<T> Empty = new();
        
        public bool IsEmpty => tail == null;

        private readonly T? head;
        private readonly ImmutableStack<T>? tail;

        private ImmutableStack() { }

        private ImmutableStack(T head, ImmutableStack<T> tail)
        {
            this.head = head;
            this.tail = tail;
        }
        public ImmutableStack<T> Push(T value) => new(value, this);
        public ImmutableStack<T> Pop() => tail ?? throw new InvalidOperationException("Stack is empty");
        public T Peek() => IsEmpty ? throw new InvalidOperationException("Stack is empty") : head!;
        
        public IEnumerator<T> GetEnumerator()
        {
            var node = this;
            while (!node!.IsEmpty)
            {
                yield return node.Peek();
                node = node.tail;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}