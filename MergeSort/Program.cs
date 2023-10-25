using System;
using System.Collections.Concurrent;
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
            int[] sortedData = FullParallelMergeSort(largeData, numCores);
            stopwatch.Stop();
            Console.WriteLine($"FullParallelMergeSort sorting took: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Start();
            int[] sortedData1 = ImplicitParallelMergeSort(largeData);
            stopwatch.Stop();
            Console.WriteLine($"ImplicitParallelMergeSort sorting took: {stopwatch.ElapsedMilliseconds} ms");
            
            stopwatch.Start();
            int[] sortedData2 = TaskBasedMergeSort(largeData);
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

        public static int[] FullParallelMergeSort(int[] arr, int numCores)
        {
            if (arr.Length <= 1)
            {
                return arr; // No need to sort a single-element array
            }

            if (numCores <= 1 || arr.Length < numCores)
            {
                Console.WriteLine("Falling back to sequential sort");
                return MergeSort(arr);
            }

            // C# Parallel For handles thread distribution allowing for more granular tasks
            int taskCount = numCores * 2;
            ConcurrentBag<int[]> sortedBags = new ConcurrentBag<int[]>();

            Parallel.For(0, taskCount, (i) =>
            {
                int segmentSize = arr.Length / taskCount;
                int start = i * segmentSize;
                int end = (i == taskCount - 1) ? arr.Length : start + segmentSize;
                int[] segment = new int[end - start];
                Array.Copy(arr, start, segment, 0, segment.Length);

                int[] sortedSegment = MergeSort(segment);
                sortedBags.Add(sortedSegment);
            });

            while (sortedBags.Count > 1)
            {
                ConcurrentBag<int[]> newBags = new ConcurrentBag<int[]>();

                var pairs = sortedBags.ToArray().Select((item, index) => new { item, index })
                    .GroupBy(x => x.index / 2)
                    .Select(g => g.Select(x => x.item).ToArray());

                Parallel.ForEach(pairs, pair =>
                {
                    if (pair.Length == 2)
                    {
                        newBags.Add(Merge(pair[0], pair[1]));
                    }
                    else
                    {
                        newBags.Add(pair[0]);
                    }
                });
                sortedBags = newBags;
            }

            return sortedBags.Single();
        }

        public static int[] ImplicitParallelMergeSort(int[] arr)
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

            Parallel.Invoke(() => left = ImplicitParallelMergeSort(left),
                () => right = ImplicitParallelMergeSort(right));

            return Merge(left, right);
        }


        public static int[] TaskBasedMergeSort(int[] arr)
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

            // Using Task to manage the parallelism
            var leftTask = Task.Run(() => TaskBasedMergeSort(left));
            var rightTask = Task.Run(() => TaskBasedMergeSort(right));

            Task.WaitAll(leftTask, rightTask);
            return Merge(leftTask.Result, rightTask.Result);
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