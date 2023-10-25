namespace MergeSort;

public static class TaskBased
{
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
        return Program.Merge(leftTask.Result, rightTask.Result);
    }
}