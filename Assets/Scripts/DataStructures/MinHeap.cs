using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DataStructures
{
    /// <summary>
    /// Generic Heap Class to create a priority Queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MinHeap<T> where T : IComparable<T>
    {
        //The heap is implemented by a list
        private List<T> heap;

        //Size of the heap
        private int size;

        //Constructor for the Heap
        public MinHeap()
        {
            this.size = 0;
            this.heap = new List<T>();
        }

        //Get the parent node
        private int Parent(int i)
        {
            return (i - 1) / 2;
        }

        //Get the left child
        private int LeftChild(int i)
        {
            return 2 * i + 1;
        }

        //Get tje right child
        private int RightChild(int i)
        {
            return 2 * i + 2;
        }

        /// <summary>
        /// Build Heap from list
        /// Time Complexity: O(n)
        /// </summary>
        /// <param name="values"></param>
        public void BuildHeap(List<T> values)
        {
            heap.Clear();
            heap.AddRange(values); // Copy values to heap (O(n))

            // Set the size of the heap
            // The last non-leaf node is at index (n/2) - 1
            this.size = values.Count;
            int lastNonLeafIndex = (this.size / 2) - 1;
            for (int i = lastNonLeafIndex; i >= 0; i--)
            {
                HeapifyDown(i); // Heapify each non-leaf node (O(log n))
            }
        }

        /// <summary>
        /// Heapify up the heap 
        /// Time Complexity: O(log n)
        /// </summary>
        /// <param name="index"></param>
        public void HeapifyUp(int index)
        {
            while (index > 0 && heap[index].CompareTo(heap[Parent(index)]) < 0)
            {
                Swap(index, Parent(index));
                index = Parent(index);
            }
        }

        /// <summary>
        /// Heapify down the heap 
        /// Time Complexity: O(log n)
        /// </summary>
        /// <param name="index"></param>
        public void HeapifyDown(int index)
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

        /// <summary>
        /// Insert Element to the heap 
        /// Time Complexity: O(log n)
        /// </summary>
        /// <param name="key"></param>
        public void Insert(T key)
        { 
            heap.Add(key);
            size++;
            HeapifyUp(size - 1);
        }

        /// <summary>
        /// Extract the min value from the heap
        /// Time Complexity: O(log n)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// // Method to calculate the parent index of a given index
        /// Time Complexity: O(1)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T GetMin()
        {
            if (size == 0)
                throw new InvalidOperationException("Heap is empty");

            return heap[0];
        }

        /// <summary>
        /// Method to calculate the left child index of a given index
        /// Time Complexity: O(1)
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return this.size == 0;
        }

        /// <summary>
        ///  Method to calculate the right child index of a given index
        ///  Time Complexity: O(1)
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return this.size;
        } 

        /// <summary>
        /// Remove a node from the heap 
        /// Time Complexity: O(log n)
        /// </summary>
        /// <param name="node"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void RemoveNode(T node)
        {
            int index = heap.IndexOf(node);
            if (index == -1)
            {
                throw new InvalidOperationException("Node not found in heap");
            }

            heap[index] = heap[size - 1];
            heap.RemoveAt(size - 1);
            size--;
            HeapifyDown(index);
        }

        /// <summary>
        /// Return the Heap
        /// </summary>
        /// <returns></returns>
        public List<T> GetHeap()
        {
            return this.heap;
        }

        /// <summary>
        /// Swap between two elements in heap
        /// Time Complexity: O(log n)
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        // 
        private void Swap(int index1, int index2)
        {
            T temp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = temp;
        }

        /// <summary>
        /// Get the list in a sorted fashion
        /// This Function will not modify the heap while creating the 
        /// Priority Queue
        /// Time complexity of: O(n log n)
        /// </summary>
        /// <returns></returns>
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