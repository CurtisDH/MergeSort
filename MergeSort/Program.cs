﻿using System.Diagnostics;

namespace MergeSort
{
    static class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int[] largeData = GenerateLargeData(100000000);
            stopwatch.Stop();
            Console.WriteLine($"Data generation took:{stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Reset();

            // foreach (int num in largeData)
            // {
            //     Console.Write(num + " ");
            // }

            stopwatch.Start();
            int[] sortedData = MergeSort(largeData);
            stopwatch.Stop();
            Console.WriteLine($"Data sorting took:{stopwatch.ElapsedMilliseconds} ms");

            // foreach (int num in sortedData)
            // {
            //     Console.Write(num + " ");
            // }
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