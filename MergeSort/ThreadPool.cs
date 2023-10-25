using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class ThreadPool
{
    private readonly int maxThreads;
    private readonly ConcurrentQueue<Action> tasks = new ConcurrentQueue<Action>();
    private readonly List<Thread> workers = new List<Thread>();
    private bool isStopping = false;

    public ThreadPool(int numThreads)
    {
        maxThreads = numThreads;
        for (int i = 0; i < maxThreads; i++)
        {
            var worker = new Thread(Work);
            worker.Start();
            workers.Add(worker);
        }
    }

    public void QueueTask(Action task)
    {
        if (isStopping)
        {
            throw new InvalidOperationException("ThreadPool is stopping and cannot accept new tasks.");
        }

        tasks.Enqueue(task);
    }

    public void StopAll()
    {
        isStopping = true;
        foreach (var worker in workers)
        {
            worker.Join();
        }
    }

    private void Work()
    {
        while (!isStopping)
        {
            if (tasks.TryDequeue(out var task))
            {
                task.Invoke();
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }
}