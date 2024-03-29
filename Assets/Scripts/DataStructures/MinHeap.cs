using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.Experimental.GraphView.Port;

namespace Assets.Scripts.DataStructures
{
    public class MinHeap<T> where T : IComparable<T>
    {
        private List<T> heap;
        private int size;

        public MinHeap()
        {
            this.size = 0;
            this.heap = new List<T>();
        }

        private int Parent(int i)
        {
            return (i - 1) / 2;
        }

        private int LeftChild(int i)
        {
            return 2 * i + 1;
        }

        private int RightChild(int i)
        {
            return 2 * i + 2;
        }

        public void BuildHeap(List<T> values)
        {
            heap.Clear();
            heap.AddRange(values); // Copy values to heap (O(n))

            // Start heapifying from the last non-leaf node down to the root
            int lastNonLeafIndex = (this.size / 2) - 1;
            for (int i = lastNonLeafIndex; i >= 0; i--)
            {
                HeapifyDown(i); // Heapify each non-leaf node (O(log n))
            }
        }

        // Time Complexity: O(log n)
        private void HeapifyUp(int index)
        {
            while (index > 0 && heap[index].CompareTo(heap[Parent(index)]) < 0)
            {
                Swap(index, Parent(index));
                index = Parent(index);
            }
        }

        // Time Complexity: O(log n)
        private void HeapifyDown(int index)
        {
            int smallest = index;
            int left = LeftChild(index);
            int right = RightChild(index);

            if (left < size && heap[left].CompareTo(heap[smallest]) < 0)
                smallest = left;

            if (right < size && heap[right].CompareTo(heap[smallest]) < 0)
                smallest = right;

            if (smallest != index)
            {
                Swap(index, smallest);
                HeapifyDown(smallest);
            }
        }

        // Time Complexity: O(log n)
        public void Insert(T key)
        { 
            heap.Add(key);
            size++;
            HeapifyUp(size - 1);
        }

        // Time Complexity: O(log n)
        public T ExtractMin()
        {
            if (size == 0)
                throw new InvalidOperationException("Heap is empty");

            T min = heap[0];
            heap[0] = heap[size - 1];
            heap.RemoveAt(size - 1); // Remove last element after swapping
            size--;
            HeapifyDown(0);
            return min;
        }

        // Time Complexity: O(1)
        public T GetMin()
        {
            if (size == 0)
                throw new InvalidOperationException("Heap is empty");

            return heap[0];
        }

        // Time Complexity: O(1)
        public bool IsEmpty()
        {
            return this.size == 0;
        }
        

        // Time Complexity: O(1)
        public int GetSize()
        {
            return this.size;
        } 

        public List<T> GetHeap()
        {
            return this.heap;
        }

        // Time Complexity: O(log n)
        private void Swap(int index1, int index2)
        {
            T temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }

        public List<T> GetSortedWithoutModifyingHeap()
        {
            // Create a copy of the heap
            MinHeap<T> heapCopy = new MinHeap<T>();
            foreach (T element in this.heap)
            {
                heapCopy.Insert(element);
            }

            List<T> sortedList = new List<T>();

            // Extract minimum element from the copy of the heap until it's empty
            while (!heapCopy.IsEmpty())
            {
                sortedList.Add(heapCopy.ExtractMin());
            }
           
            return sortedList;
        }
    }
}