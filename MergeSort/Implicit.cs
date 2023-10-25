namespace MergeSort;

public static class Implicit
{
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

        return Program.Merge(left, right);
    }
}