using System;
using System.Collections.Generic;

namespace CpuScheduler
{
    public static class CpuScheduler
    {
        // Round Robin Scheduling
        public static void RoundRobin(List<Process> processes, int timeQuantum)
        {
            int processCount = processes.Count;
            int time = 0;
            Queue<Process> processQueue = new Queue<Process>();

            // Initialize all processes in the queue
            foreach (var process in processes)
            {
                processQueue.Enqueue(process);
            }

            while (processQueue.Count > 0)
            {
                Process currentProcess = processQueue.Dequeue();

                // If burst time is less than or equal to time quantum, process finishes
                if (currentProcess.BurstTime <= timeQuantum)
                {
                    time += currentProcess.BurstTime;
                    currentProcess.BurstTime = 0;
                    Console.WriteLine($"Process {currentProcess.Id} finished at time {time}");
                }
                else
                {
                    // If burst time is greater than time quantum, decrease burst time and put the process back
                    currentProcess.BurstTime -= timeQuantum;
                    time += timeQuantum;
                    processQueue.Enqueue(currentProcess);
                    Console.WriteLine($"Process {currentProcess.Id} executed for {timeQuantum} units, remaining burst time: {currentProcess.BurstTime}");
                }
            }
        }

        // Priority Scheduling
        public static void PriorityScheduling(List<Process> processes)
        {
            var sortedProcesses = new List<Process>(processes);
            sortedProcesses.Sort((p1, p2) => p1.Priority.CompareTo(p2.Priority));  // Sort by priority

            int time = 0;
            foreach (var process in sortedProcesses)
            {
                time += process.BurstTime;
                Console.WriteLine($"Process {process.Id} with priority {process.Priority} executed at time {time}");
            }
        }

        // Deadlock Detection (Banker's Algorithm-like approach)
        public static bool DetectDeadlock(int[,] allocation, int[,] request, int[] available)
        {
            int processCount = allocation.GetLength(0);
            int resourceCount = allocation.GetLength(1);

            int[] work = new int[resourceCount];
            bool[] finish = new bool[processCount];

            // Initialize Work to Available resources
            Array.Copy(available, work, resourceCount);

            bool madeProgress;
            do
            {
                madeProgress = false;
                for (int i = 0; i < processCount; i++)
                {
                    if (!finish[i])
                    {
                        bool canFinish = true;
                        for (int j = 0; j < resourceCount; j++)
                        {
                            if (request[i, j] > work[j])
                            {
                                canFinish = false;
                                break;
                            }
                        }

                        if (canFinish)
                        {
                            // Process can finish, release its allocated resources
                            for (int j = 0; j < resourceCount; j++)
                            {
                                work[j] += allocation[i, j];
                            }
                            finish[i] = true;
                            madeProgress = true;
                        }
                    }
                }
            } while (madeProgress);

            // If any process is not finished, there is a deadlock
            for (int i = 0; i < processCount; i++)
            {
                if (!finish[i])
                {
                    return true; // Deadlock detected
                }
            }
            return false; // No deadlock
        }
    }

    public class Process
    {
        public int Id { get; set; }
        public int BurstTime { get; set; }
        public int ArrivalTime { get; set; }
        public int Priority { get; set; }
    }

    // Test Deadlock Detection
    public class DeadlockAlgo
    {
        public static void Main(string[] args)
        {
            int[,] allocation = {
                { 0, 1, 0 },
                { 2, 0, 0 },
                { 3, 0, 2 },
                { 2, 1, 1 },
                { 0, 0, 2 }
            };

            int[,] request = {
                { 0, 0, 0 },
                { 2, 0, 2 },
                { 0, 0, 0 },
                { 1, 0, 0 },
                { 0, 0, 2 }
            };

            int[] available = { 0, 0, 0 };

            bool hasDeadlock = CpuScheduler.DetectDeadlock(allocation, request, available);

            Console.WriteLine(hasDeadlock 
                ? "Deadlock detected among processes!" 
                : "No deadlock detected.");
        }
    }
}
