using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame_WaTor.DataStructures
{
    /// <summary>
    /// Represents an amalgamation of DoubleLinkedLists, indexed by priority. Each DoubleLinkedList within the GroupedList
    /// has a "GroupIndex", and lower indeces are hit first by the Enumerator of this list. When adding an element to this list, you
    /// must specify a groupIndex that the element will be added to. We use the PriorityGroupedList in this application so that
    /// certain entities have a higher priority (lower index) and thus are evaluated first by the game update loop. For example,
    /// Sharks have a GroupIndex of 0 meaning that they are iterated over first so Sharks will always make their move before fish do.
    /// </summary>
    public class PriorityGroupedList<T> : IEnumerable<T>
    {
        private DoubleLinkedList<T>[] listGroups;
        public int Capacity { get; private set; }
        public byte GroupsCount { get; private set; }
        public int TotalElements { get; private set; }

        public PriorityGroupedList(int initialCapacity = 1)
        {
            Capacity = initialCapacity;
            GroupsCount = 0;
            listGroups = new DoubleLinkedList<T>[initialCapacity];
            for (int i = 0; i < listGroups.Length; i++)
                listGroups[i] = new DoubleLinkedList<T>();
        }

        public void EnsureCapacity()
        {
            if (GroupsCount > Capacity)
            {
                var old = listGroups;
                listGroups = new DoubleLinkedList<T>[Capacity = GroupsCount * 2];
                for (int i = 0; i < listGroups.Length; i++)
                {
                    if (i < old.Length)
                    {
                        listGroups[i] = old[i];
                    }
                    else
                    {
                        listGroups[i] = new DoubleLinkedList<T>();
                    }
                }
            }
        }

        public GroupedListNode<T> AddFirst(T e, byte groupIndex)
        {
            TotalElements++;
            if (groupIndex >= byte.MaxValue)
            {
                throw new Exception($"Group index cannot be higher than {byte.MaxValue - 1} ");
            }
            if (GroupsCount < groupIndex + 1)
            {
                GroupsCount = (byte)(groupIndex + 1);
                EnsureCapacity();
            }

            GroupedListNode<T> listNode = new(e, groupIndex);
            listGroups[groupIndex].AddFirst(listNode);
            return listNode;
        }

        public GroupedListNode<T> AddLast(T e, byte groupIndex)
        {
            TotalElements++;
            if (groupIndex >= byte.MaxValue)
            {
                throw new Exception($"Group index cannot be higher than {byte.MaxValue - 1} ");
            }
            if (GroupsCount < groupIndex + 1)
            {
                GroupsCount = (byte)(groupIndex + 1);
                EnsureCapacity();
            }

            GroupedListNode<T> listNode = new(e, groupIndex);
            listGroups[groupIndex].AddLast(listNode);
            return listNode;
        }

        public void Remove(GroupedListNode<T> n)
        {
            TotalElements--;
            listGroups[n.GroupIndex].Remove(n);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var list in listGroups)
            {
                foreach (var element in list)
                {
                    yield return element;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class GroupedListNode<T> : DoubleLinkedNode<T>
    {
        public readonly byte GroupIndex;

        public GroupedListNode(T e, byte group) : base(e)
        {
            GroupIndex = group;
        }
    }
}
