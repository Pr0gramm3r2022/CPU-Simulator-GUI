using System;

namespace CpuSchedulingAlgorithms
{
    public static class SchedulingAlgorithms
    {
        public static void ExecuteAlgorithm(string algorithm, double[] burstTimes, int[] priorities = null, double timeQuantum = 0)
        {
            switch (algorithm.ToLower())
            {
                case "fcfs":
                    RunFirstComeFirstServe(burstTimes);
                    break;
                case "sjf":
                    RunShortestJobFirst(burstTimes);
                    break;
                case "priority":
                    if (priorities == null) throw new ArgumentException("Priority values are required for Priority Scheduling.");
                    RunPriorityScheduling(burstTimes, priorities);
                    break;
                case "roundrobin":
                    if (timeQuantum <= 0) throw new ArgumentException("A valid time quantum is required for Round Robin Scheduling.");
                    RunRoundRobin(burstTimes, timeQuantum);
                    break;
                default:
                    throw new ArgumentException("Invalid algorithm selected.");
            }
        }

        private static void RunFirstComeFirstServe(double[] burstTimes)
        {
            double[] waitingTimes = CalculateWaitingTimes(burstTimes);
            double[] turnaroundTimes = CalculateTurnaroundTimes(burstTimes, waitingTimes);
            DisplayResults("First Come First Serve", waitingTimes, turnaroundTimes);
        }

        private static void RunShortestJobFirst(double[] burstTimes)
        {
            Array.Sort(burstTimes);
            double[] waitingTimes = CalculateWaitingTimes(burstTimes);
            double[] turnaroundTimes = CalculateTurnaroundTimes(burstTimes, waitingTimes);
            DisplayResults("Shortest Job First", waitingTimes, turnaroundTimes);
        }

        private static void RunPriorityScheduling(double[] burstTimes, int[] priorities)
        {
            Array.Sort(priorities, burstTimes);
            double[] waitingTimes = CalculateWaitingTimes(burstTimes);
            double[] turnaroundTimes = CalculateTurnaroundTimes(burstTimes, waitingTimes);
            DisplayResults("Priority Scheduling", waitingTimes, turnaroundTimes);
        }

        private static void RunRoundRobin(double[] burstTimes, double timeQuantum)
        {
            int numProcesses = burstTimes.Length;
            double[] remainingTimes = (double[])burstTimes.Clone();
            double[] waitingTimes = new double[numProcesses];
            double[] turnaroundTimes = new double[numProcesses];
            double totalTime = 0;

            while (true)
            {
                bool allProcessesCompleted = true;

                for (int i = 0; i < numProcesses; i++)
                {
                    if (remainingTimes[i] > 0)
                    {
                        allProcessesCompleted = false;

                        if (remainingTimes[i] > timeQuantum)
                        {
                            totalTime += timeQuantum;
                            remainingTimes[i] -= timeQuantum;
                        }
                        else
                        {
                            totalTime += remainingTimes[i];
                            waitingTimes[i] = totalTime - burstTimes[i];
                            remainingTimes[i] = 0;
                        }
                    }
                }

                if (allProcessesCompleted) break;
            }

            for (int i = 0; i < numProcesses; i++)
            {
                turnaroundTimes[i] = burstTimes[i] + waitingTimes[i];
            }

            DisplayResults("Round Robin", waitingTimes, turnaroundTimes);
        }

        private static double[] CalculateWaitingTimes(double[] burstTimes)
        {
            double[] waitingTimes = new double[burstTimes.Length];
            for (int i = 1; i < burstTimes.Length; i++)
            {
                waitingTimes[i] = waitingTimes[i - 1] + burstTimes[i - 1];
            }
            return waitingTimes;
        }

        private static double[] CalculateTurnaroundTimes(double[] burstTimes, double[] waitingTimes)
        {
            double[] turnaroundTimes = new double[burstTimes.Length];
            for (int i = 0; i < burstTimes.Length; i++)
            {
                turnaroundTimes[i] = burstTimes[i] + waitingTimes[i];
            }
            return turnaroundTimes;
        }

        private static void DisplayResults(string algorithmName, double[] waitingTimes, double[] turnaroundTimes)
        {
            double totalWaitingTime = 0, totalTurnaroundTime = 0;
            Console.WriteLine($"{algorithmName} Results:");

            for (int i = 0; i < waitingTimes.Length; i++)
            {
                totalWaitingTime += waitingTimes[i];
                totalTurnaroundTime += turnaroundTimes[i];
                Console.WriteLine($"Process P{i + 1}: Waiting Time = {waitingTimes[i]:F2} ms, Turnaround Time = {turnaroundTimes[i]:F2} ms");
            }

            Console.WriteLine($"\nAverage Waiting Time = {totalWaitingTime / waitingTimes.Length:F2} ms");
            Console.WriteLine($"Average Turnaround Time = {totalTurnaroundTime / turnaroundTimes.Length:F2} ms");
        }
    }
}
