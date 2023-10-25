using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MergeSort
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int[] largeData = GenerateLargeData(100000000);
            stopwatch.Stop();
            Console.WriteLine($"Data generation took: {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            int numCores = Environment.ProcessorCount;
            Console.WriteLine($"Number of available cores: {numCores}");

            stopwatch.Start();
            int[] sortedData = HighLevelFullParallel.FullParallelMergeSort(largeData, numCores);
            stopwatch.Stop();
            Console.WriteLine($"FullParallelMergeSort sorting took: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Start();
            int[] sortedData3 = MergeSortWithThreadPool.FullParallelMergeSortWithThreadPool(largeData, numCores);
            stopwatch.Stop();
            Console.WriteLine($"FullParallelMergeSort sorting took: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Start();
            int[] sortedData1 = Implicit.ImplicitParallelMergeSort(largeData);
            stopwatch.Stop();
            Console.WriteLine($"ImplicitParallelMergeSort sorting took: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Start();
            int[] sortedData2 = TaskBased.TaskBasedMergeSort(largeData);
            stopwatch.Stop();
            Console.WriteLine($"TaskBasedMergeSort sorting took: {stopwatch.ElapsedMilliseconds} ms");
        }

        public static int[] Merge(int[] left, int[] right)
        {
            int[] result = new int[left.Length + right.Length];
            int i = 0, j = 0, k = 0;

            while (i < left.Length && j < right.Length)
            {
                if (left[i] < right[j])
                {
                    result[k++] = left[i++];
                }
                else
                {
                    result[k++] = right[j++];
                }
            }

            while (i < left.Length)
            {
                result[k++] = left[i++];
            }

            while (j < right.Length)
            {
                result[k++] = right[j++];
            }

            return result;
        }


        public static int[] MergeSort(int[] arr)
        {
            if (arr.Length <= 1)
            {
                return arr;
            }

            int mid = arr.Length / 2;
            int[] left = new int[mid];
            int[] right = new int[arr.Length - mid];

            Array.Copy(arr, 0, left, 0, mid);
            Array.Copy(arr, mid, right, 0, arr.Length - mid);

            left = MergeSort(left);
            right = MergeSort(right);

            return Merge(left, right);
        }

        public static int[] GenerateLargeData(int size)
        {
            Random rand = new Random();
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = rand.Next(1, 10000);
            }

            return data;
        }
    }
}