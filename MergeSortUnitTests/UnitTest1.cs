using System;
using System.Diagnostics;
using System.Linq;
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

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(0, 4)]
        [InlineData(0, 8)]
        [InlineData(0, 16)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, 4)]
        [InlineData(1, 8)]
        [InlineData(1, 16)]
        [InlineData(9, 1)]
        [InlineData(9, 2)]
        [InlineData(9, 4)]
        [InlineData(9, 8)]
        [InlineData(9, 16)]
        [InlineData(100, 1)]
        [InlineData(100, 2)]
        [InlineData(100, 4)]
        [InlineData(100, 8)]
        [InlineData(100, 16)]
        [InlineData(1000, 1)]
        [InlineData(1000, 2)]
        [InlineData(1000, 4)]
        [InlineData(1000, 8)]
        [InlineData(1000, 16)]
        [InlineData(10000, 1)]
        [InlineData(10000, 2)]
        [InlineData(10000, 4)]
        [InlineData(10000, 8)]
        [InlineData(10000, 16)]
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
        public void Test_FullParallelSort_MultiElementArray(int arraySize, int coreCount)
        {
            _output.WriteLine(
                $"Starting Test_FullParallelSort_MultiElementArray with array size {arraySize} and core count {coreCount}");
            int[] arr = GenerateRandomArray(arraySize);

            var stopwatch = new Stopwatch();

            _output.WriteLine($"Number of cores: {coreCount}");
            stopwatch.Start();
            _output.WriteLine($"ArrK{arr.Length}, numcores:{coreCount}");
            int[] sortedArr = Program.FullParallelMergeSort(arr, coreCount);

            stopwatch.Stop();
            _output.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");

            _output.WriteLine("Checking against expected output");
            stopwatch.Reset();

            stopwatch.Start();
            int[] expectedArr = arr.OrderBy(x => x).ToArray();
            stopwatch.Stop();
            _output.WriteLine($"Comparison took:{stopwatch.ElapsedMilliseconds} ms");

            Assert.True(expectedArr.SequenceEqual(sortedArr));

            _output.WriteLine("Test_FullParallelSort_MultiElementArray passed");
        }
    }
}