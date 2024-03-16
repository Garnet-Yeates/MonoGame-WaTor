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
    /// - Adding at the end of the tail<br/>
    /// - Iterating through all elements<br/>
    /// - Self removal of a <see cref="DoubleLinkedNode{T}"/> through a reference to that Node<br/>
    /// - Clearing all elements from the list<br/><br/>
    /// There is no built in way to access/insert/remove an element by index. Elements also cannot be removed by value, only by reference to their <see cref="DoubleLinkedNode{T}"/>. The reason is because this
    /// Data Structure was designed to only have O(1) methods inside of it. In our WaTor simulation, this data structure (used in tandem with <see cref="GroupedList{T}"/>) 
    /// is used to efficiently contain each living <see cref="Entity"/>. Every frame, this List will be iterated through to call the Update() methods on all entities. Every 
    /// Entity has a reference to their ListNode and this List itself, so when they die they can call List.Remove(theirListNode). With this setup, adding and removing entities
    /// stays at O(1) efficiency so we have less overhead.<br/><br/>
    /// Two more benefits of using a List that is separate from the EntityInterceptionGrid for updating entities:<br/>
    /// - Instead of doing MaxEntityCount*2 iterations, we only have to do ActualEntityCount*1 iterations. If updating via grid loop, on top of having to do iterations for blank 
    /// spaces, we would also have to do one initial sweep where when we process an Entity we set a flag called "AlreadyUpdatedThisLoop" so that if it moves into a spot that we 
    /// have yet to process (i.e to the right, or down) it doesn't get processed twice. Then we would have to do a second sweep to clear the flags.<br/>
    /// - By using <see cref="GroupedList{T}"/> along with this setup, we get to put different types of entities into different
    /// group orderings so that we can process them with a specific order in mind (e.g putting shark list at group 0 so they get to make their moves before the fish).
    /// </summary>
    public class DoubleLinkedList<T> : IEnumerable<T>
    {
        private DoubleLinkedNode<T> head;
        private DoubleLinkedNode<T> tail;

        public int Size { get; private set; } = 0;

        public DoubleLinkedNode<T> Add(T e)
        {
            DoubleLinkedNode<T> n = new(e);
            Add(n);
            return n;
        }

        public void Add(DoubleLinkedNode<T> n)
        {
            if (head == null)
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
            else if (n == head) // implied that n.next isn't null
            {
                head = n.next;
                n.next.prev = null;
            }
            else if (n == tail) // implied that n.prev isn't null
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