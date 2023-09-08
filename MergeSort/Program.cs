using System.Diagnostics;

namespace MergeSort
{
    class Program
    {
        static void Main(string[] args)
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
            Console.WriteLine($"Data sorting took: {stopwatch.ElapsedMilliseconds} ms");
        }

        static int[] Merge(int[] left, int[] right)
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

        static int[] FullParallelMergeSort(int[] arr, int numCores)
        {
            if (arr.Length <= 1 || numCores <= 1)
            {
                Console.WriteLine("Falling back to sequential sort");
                return MergeSort(arr);
            }

            int segmentSize = arr.Length / numCores;
            int[] sortedData = new int[arr.Length];

            Parallel.For(0, numCores, i =>
            {
                int start = i * segmentSize;
                int end = (i == numCores - 1) ? arr.Length : start + segmentSize;
                int[] segment = new int[end - start];
                Array.Copy(arr, start, segment, 0, end - start);
                int[] sortedSegment = MergeSort(segment);
                Array.Copy(sortedSegment, 0, sortedData, start, sortedSegment.Length);
            });

            while (segmentSize < arr.Length)
            {
                Parallel.For(0, arr.Length / (2 * segmentSize), i =>
                {
                    int start = i * 2 * segmentSize;
                    int mid = start + segmentSize;
                    int end = Math.Min(start + 2 * segmentSize, arr.Length);
                    int[] left = new int[mid - start];
                    int[] right = new int[end - mid];
                    Array.Copy(sortedData, start, left, 0, left.Length);
                    Array.Copy(sortedData, mid, right, 0, right.Length);
                    int[] merged = Merge(left, right);
                    Array.Copy(merged, 0, sortedData, start, merged.Length);
                });

                segmentSize *= 2;
            }

            return sortedData;
        }

        static int[] MergeSort(int[] arr)
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

        static int[] GenerateLargeData(int size)
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