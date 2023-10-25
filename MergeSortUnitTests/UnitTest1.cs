using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Xunit;
using Xunit.Abstractions;
using MergeSort;

namespace MergeSortUnitTests
{
    public class MergeSortTests
    {
        private readonly ITestOutputHelper _output;

        public MergeSortTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private int[] GenerateRandomArray(int size)
        {
            var sw = new Stopwatch();
            sw.Start();
            Random rand = new Random();
            int[] data = new int[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = rand.Next(1, 10000);
            }

            sw.Stop();
            _output.WriteLine($"Generated array in {sw.ElapsedMilliseconds} ms");

            return data;
        }

        // [Theory]
        // [InlineData(100000, 1)]
        // [InlineData(100000, 2)]
        // [InlineData(100000, 4)]
        // [InlineData(100000, 8)]
        // [InlineData(100000, 16)]
        // [InlineData(1000000, 1)]
        // [InlineData(1000000, 2)]
        // [InlineData(1000000, 4)]
        // [InlineData(1000000, 8)]
        // [InlineData(1000000, 16)]
        // [InlineData(2000000, 1)]
        // [InlineData(2000000, 2)]
        // [InlineData(2000000, 4)]
        // [InlineData(2000000, 8)]
        // [InlineData(2000000, 16)]
        // [InlineData(3000000, 1)]
        // [InlineData(3000000, 2)]
        // [InlineData(3000000, 4)]
        // [InlineData(3000000, 8)]
        // [InlineData(3000000, 16)]
        // [InlineData(4000000, 1)]
        // [InlineData(4000000, 2)]
        // [InlineData(4000000, 4)]
        // [InlineData(4000000, 8)]
        // [InlineData(4000000, 16)]
        // [InlineData(10000000, 1)]
        // [InlineData(10000000, 2)]
        // [InlineData(10000000, 4)]
        // [InlineData(10000000, 8)]
        // [InlineData(10000000, 16)]
        // [InlineData(100000000, 1)]
        // [InlineData(100000000, 2)]
        // [InlineData(100000000, 4)]
        // [InlineData(100000000, 8)]
        // [InlineData(100000000, 16)]
        // [InlineData(200000000, 1)]
        // [InlineData(200000000, 2)]
        // [InlineData(200000000, 4)]
        // [InlineData(200000000, 8)]
        // [InlineData(200000000, 16)]
        public void Test_FullParallelSort_MultiElementArray(int arraySize, int coreCount)
        {
            _output.WriteLine(
                $"Starting Test_FullParallelSort_MultiElementArray with ArraySize: {arraySize}, CoreCount: {coreCount}");

            int iterations = 10;
            double totalElapsedTime = 0;
            int[] arr = GenerateRandomArray(arraySize);

            for (int i = 0; i < iterations; i++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                int[] sortedArr = HighLevelFullParallel.FullParallelMergeSort(arr, coreCount);

                stopwatch.Stop();
                totalElapsedTime += stopwatch.ElapsedMilliseconds;

                _output.WriteLine($"Iteration {i + 1} - Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
            }

            double averageTime = totalElapsedTime / iterations;
            _output.WriteLine($"Average elapsed time for {iterations} iterations: {averageTime} ms");

            // Record the average time in the CSV
            var record = new { ArraySize = arraySize, CoreCount = coreCount, AverageElapsedTime = averageTime };
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);

            using (var writer = new StreamWriter("performance_data.csv", append: true))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecord(record);
                csv.NextRecord();
            }

            _output.WriteLine("Test_FullParallelSort_MultiElementArray passed");
        }

        private void RunTestAndRecord(string testName, int[] arr, int coreCount, Func<int[], int, int[]> sortMethod)
        {
            _output.WriteLine($"Starting {testName} with ArraySize: {arr.Length}, CoreCount: {coreCount}");

            int iterations = 10;
            double totalElapsedTime = 0;

            for (int i = 0; i < iterations; i++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                int[] sortedArr = sortMethod(arr, coreCount);

                stopwatch.Stop();
                totalElapsedTime += stopwatch.ElapsedMilliseconds;

                _output.WriteLine($"Iteration {i + 1} - Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
            }

            double averageTime = totalElapsedTime / iterations;
            _output.WriteLine($"Average elapsed time for {iterations} iterations: {averageTime} ms");

            // Record the average time in the CSV
            var record = new { ArraySize = arr.Length, CoreCount = coreCount, AverageElapsedTime = averageTime };
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);

            using (var writer = new StreamWriter($"{testName}_performance_data.csv", append: true))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecord(record);
                csv.NextRecord();
            }

            _output.WriteLine($"{testName} passed");
        }

        [Theory]
        [InlineData(100000, 1)]
        [InlineData(100000, 2)]
        [InlineData(100000, 4)]
        [InlineData(100000, 8)]
        [InlineData(100000, 16)]
        [InlineData(1000000, 1)]
        [InlineData(1000000, 2)]
        [InlineData(1000000, 4)]
        [InlineData(1000000, 8)]
        [InlineData(1000000, 16)]
        [InlineData(2000000, 1)]
        [InlineData(2000000, 2)]
        [InlineData(2000000, 4)]
        [InlineData(2000000, 8)]
        [InlineData(2000000, 16)]
        [InlineData(3000000, 1)]
        [InlineData(3000000, 2)]
        [InlineData(3000000, 4)]
        [InlineData(3000000, 8)]
        [InlineData(3000000, 16)]
        [InlineData(4000000, 1)]
        [InlineData(4000000, 2)]
        [InlineData(4000000, 4)]
        [InlineData(4000000, 8)]
        [InlineData(4000000, 16)]
        [InlineData(10000000, 1)]
        [InlineData(10000000, 2)]
        [InlineData(10000000, 4)]
        [InlineData(10000000, 8)]
        [InlineData(10000000, 16)]
        [InlineData(100000000, 1)]
        [InlineData(100000000, 2)]
        [InlineData(100000000, 4)]
        [InlineData(100000000, 8)]
        [InlineData(100000000, 16)]
        [InlineData(200000000, 1)]
        [InlineData(200000000, 2)]
        [InlineData(200000000, 4)]
        [InlineData(200000000, 8)]
        [InlineData(200000000, 16)]
        public void Test_HighLevelFullParallel_MultiElementArray(int arraySize, int coreCount)
        {
            int[] arr = GenerateRandomArray(arraySize);
            RunTestAndRecord("HighLevelFullParallel", arr, coreCount, HighLevelFullParallel.FullParallelMergeSort);
        }

        [Theory]
        [InlineData(100000, 1)]
        [InlineData(100000, 2)]
        [InlineData(100000, 4)]
        [InlineData(100000, 8)]
        [InlineData(100000, 16)]
        [InlineData(1000000, 1)]
        [InlineData(1000000, 2)]
        [InlineData(1000000, 4)]
        [InlineData(1000000, 8)]
        [InlineData(1000000, 16)]
        [InlineData(2000000, 1)]
        [InlineData(2000000, 2)]
        [InlineData(2000000, 4)]
        [InlineData(2000000, 8)]
        [InlineData(2000000, 16)]
        [InlineData(3000000, 1)]
        [InlineData(3000000, 2)]
        [InlineData(3000000, 4)]
        [InlineData(3000000, 8)]
        [InlineData(3000000, 16)]
        [InlineData(4000000, 1)]
        [InlineData(4000000, 2)]
        [InlineData(4000000, 4)]
        [InlineData(4000000, 8)]
        [InlineData(4000000, 16)]
        [InlineData(10000000, 1)]
        [InlineData(10000000, 2)]
        [InlineData(10000000, 4)]
        [InlineData(10000000, 8)]
        [InlineData(10000000, 16)]
        [InlineData(100000000, 1)]
        [InlineData(100000000, 2)]
        [InlineData(100000000, 4)]
        [InlineData(100000000, 8)]
        [InlineData(100000000, 16)]
        [InlineData(200000000, 1)]
        [InlineData(200000000, 2)]
        [InlineData(200000000, 4)]
        [InlineData(200000000, 8)]
        [InlineData(200000000, 16)]
        public void Test_MergeSortWithThreadPool_MultiElementArray(int arraySize, int coreCount)
        {
            int[] arr = GenerateRandomArray(arraySize);
            RunTestAndRecord("MergeSortWithThreadPool", arr, coreCount,
                MergeSortWithThreadPool.FullParallelMergeSortWithThreadPool);
        }
    }
}