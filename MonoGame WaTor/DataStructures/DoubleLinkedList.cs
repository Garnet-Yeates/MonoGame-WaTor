using MonoGame_WaTor.GameObjects;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame_WaTor.DataStructures
{
    public class DoubleLinkedNode<T>
    {
        public T value;
        public DoubleLinkedNode<T> prev;
        public DoubleLinkedNode<T> next;

        public DoubleLinkedNode(T e)
        {
            value = e;
        }
    }

    /// <summary>
    /// Represents a Double Linked List data structure that only allows:<br/>
    /// - Adding before the head<br/>
    /// - Adding after the tail<br/>
    /// - Iterating through all elements<br/>
    /// - Self removal of a <see cref="DoubleLinkedNode{T}"/> through a reference to that Node<br/>
    /// - Clearing all elements from the list<br/><br/>
    /// There is no built in way to access/insert/remove an element by index. Elements also cannot be removed by value, only by reference to their <see cref="DoubleLinkedNode{T}"/>. Not only this,
    /// but there is no validation to make sure that a Node removed through its reference is even a part of this List. The reason is because this
    /// Data Structure was designed to only have O(1) methods inside of it. In our WaTor simulation, this data structure (used in tandem with <see cref="PriorityGroupedList{T}"/>) 
    /// is used to efficiently contain each living <see cref="Entity"/>. Every frame, this List will be iterated through to call the Update() methods on all entities. Every 
    /// Entity has a reference to their ListNode and this List itself, so when they die they can call List.Remove(theirListNode). With this setup, adding and removing entities
    /// stays at O(1) efficiency so we have less overhead.<br/><br/>
    /// </summary>
    public class DoubleLinkedList<T> : IEnumerable<T>
    {
        private DoubleLinkedNode<T> head;
        private DoubleLinkedNode<T> tail;

        public int Size { get; private set; } = 0;

        public DoubleLinkedNode<T> AddFirst(T e)
        {
            DoubleLinkedNode<T> n = new(e);
            AddFirst(n);
            return n;
        }

        public void AddFirst(DoubleLinkedNode<T> n)
        {
            if (tail == null)
            {
                head = n;
                tail = n;
            }
            else

            {
                n.next = head;
                head.prev = n;
                head = n;
            }

            Size++;
        }

        public DoubleLinkedNode<T> AddLast(T e)
        {
            DoubleLinkedNode<T> n = new(e);
            AddLast(n);
            return n;
        }

        public void AddLast(DoubleLinkedNode<T> n)
        {
            if (tail == null)
            {
                head = n;
                tail = n;
            }
            else
            {
                tail.next = n;
                n.prev = tail;
                tail = n;
            }

            Size++;
        }

        public void Remove(DoubleLinkedNode<T> n)
        {
            Size--;
            if (n == head && n == tail) // implied that this is the only node in the list
            {
                head = null;
                tail = null;
            }
            else if (n == head) // implied that n.next isn't null, and n.prev is null
            {
                head = n.next;
                n.next.prev = null;
            }
            else if (n == tail) // implied that n.prev isn't null, and n.next is null
            {
                tail = n.prev;
                n.prev.next = null;
            }
            else // implied that n.prev and n.next aren't null
            {
                n.prev.next = n.next;
                n.next.prev = n.prev;
            }
        }

        public void Clear()
        {
            head = null;
            tail = null;
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            var curr = head;
            while (curr is not null)
            {
                yield return curr.value;
                curr = curr.next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}