using System.Collections.Concurrent;

namespace MergeSort;

public static class HighLevelFullParallel
{
    public static int[] FullParallelMergeSort(int[] arr, int numCores)
    {
        if (arr.Length <= 1)
        {
            return arr; // No need to sort a single-element array
        }

        if (numCores <= 1 || arr.Length < numCores)
        {
            Console.WriteLine("Falling back to sequential sort");
            return Program.MergeSort(arr);
        }

        // C# Parallel For handles thread distribution allowing for more granular tasks
        int taskCount = numCores * 2;
        ConcurrentBag<int[]> sortedBags = new ConcurrentBag<int[]>();

        // todo what if we do batches of tasks, e.g. if the threshold of 1,000,000 then its more efficient to do a new thread, otherwise, the task creation could be detrimental?
        Parallel.For(0, taskCount, (i) =>
        {
            int segmentSize = arr.Length / taskCount;
            int start = i * segmentSize;
            int end = (i == taskCount - 1) ? arr.Length : start + segmentSize;
            int[] segment = new int[end - start];
            Array.Copy(arr, start, segment, 0, segment.Length);

            int[] sortedSegment = Program.MergeSort(segment);
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
                    newBags.Add(Program.Merge(pair[0], pair[1]));
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
}