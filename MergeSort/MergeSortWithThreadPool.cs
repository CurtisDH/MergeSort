using System.Collections.Concurrent;

namespace MergeSort;

public static class MergeSortWithThreadPool
{
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

    public static int[] FullParallelMergeSortWithThreadPool(int[] arr, int maxThreads)
    {
        ThreadPool threadPool = new ThreadPool(maxThreads);

        if (arr.Length <= 1)
        {
            return arr;
        }

        ConcurrentBag<int[]> sortedBags = new ConcurrentBag<int[]>();
        int taskCount = maxThreads * 2;

        ManualResetEvent resetEvent = new ManualResetEvent(false);
        int tasksRemaining = taskCount;

        for (int i = 0; i < taskCount; i++)
        {
            int index = i;
            threadPool.QueueTask(() =>
            {
                int segmentSize = arr.Length / taskCount;
                int start = index * segmentSize;
                int end = (index == taskCount - 1) ? arr.Length : start + segmentSize;
                int[] segment = new int[end - start];
                Array.Copy(arr, start, segment, 0, segment.Length);

                int[] sortedSegment = MergeSort(segment);
                sortedBags.Add(sortedSegment);

                if (Interlocked.Decrement(ref tasksRemaining) == 0)
                {
                    resetEvent.Set();
                }
            });
        }

        resetEvent.WaitOne(); // Wait for all tasks to complete

        while (sortedBags.Count > 1)
        {
            ConcurrentBag<int[]> newBags = new ConcurrentBag<int[]>();
            var pairs = sortedBags.ToArray().Select((item, index) => new { item, index })
                .GroupBy(x => x.index / 2)
                .Select(g => g.Select(x => x.item).ToArray());

            foreach (var pair in pairs)
            {
                if (pair.Length == 2)
                {
                    newBags.Add(Merge(pair[0], pair[1]));
                }
                else
                {
                    newBags.Add(pair[0]);
                }
            }

            sortedBags = newBags;
        }

        threadPool.StopAll();

        return sortedBags.Single();
    }
}