using System;
using System.Collections.Generic;
using System.Threading;
using Verse;
using RimWorld;

namespace PawnAIMultithread.Core
{
    public class OptimizedThreadedPawnAI
    {
        private static readonly int AIThreadCount = Math.Max(1, Environment.ProcessorCount / 3);
        private static Thread[] _aiThreads;
        private static bool _threadsRunning = false;
        private static Queue<Pawn>[] _pawnQueues;
        private static object[] _queueLocks;
        private static int _aiUpdatesProcessed = 0;

        private class PawnState
        {
            public int PawnId;
            public IntVec3 LastPosition;
            public IntVec3 LastTargetCell;
            public int LastUpdateTick;
            public int ConsecutiveIdleTicks;
        }

        public static void Initialize()
        {
            if (_threadsRunning)
                return;

            _threadsRunning = true;
            _pawnQueues = new Queue<Pawn>[AIThreadCount];
            _queueLocks = new object[AIThreadCount];
            _aiUpdatesProcessed = 0;
            _aiThreads = new Thread[AIThreadCount];

            for (int i = 0; i < AIThreadCount; i++)
            {
                _pawnQueues[i] = new Queue<Pawn>();
                _queueLocks[i] = new object();

                int threadIndex = i;
                _aiThreads[i] = new Thread(() => PawnAIWorker(threadIndex))
                {
                    Name = $"PawnAI-Worker-{i}",
                    IsBackground = true
                };
                _aiThreads[i].Start();
            }

            Log.Message($"[PawnAI Multithread] Инициализировано {AIThreadCount} оптимизированных потоков ИИ пешек");
        }

        public static void Shutdown()
        {
            _threadsRunning = false;

            foreach (var queueLock in _queueLocks)
            {
                lock (queueLock)
                {
                    Monitor.PulseAll(queueLock);
                }
            }

            foreach (var thread in _aiThreads)
            {
                thread?.Join(5000);
            }

            Log.Message($"[PawnAI Multithread] Обработано обновлений ИИ: {_aiUpdatesProcessed}");
        }

        public static void EnqueuePawnForAIUpdate(Pawn pawn)
        {
            if (pawn == null || !_threadsRunning)
                return;

            int queueIndex = pawn.GetHashCode() % AIThreadCount;
            lock (_queueLocks[queueIndex])
            {
                _pawnQueues[queueIndex].Enqueue(pawn);
                Monitor.Pulse(_queueLocks[queueIndex]);
            }
        }

        private static void PawnAIWorker(int threadIndex)
        {
            while (_threadsRunning)
            {
                Pawn pawn = null;

                lock (_queueLocks[threadIndex])
                {
                    if (_pawnQueues[threadIndex].Count > 0)
                    {
                        pawn = _pawnQueues[threadIndex].Dequeue();
                    }
                    else
                    {
                        Monitor.Wait(_queueLocks[threadIndex], 50);
                        continue;
                    }
                }

                if (pawn != null && pawn.Spawned && pawn.Map != null)
                {
                    try
                    {
                        UpdatePawnAI(pawn);
                        Interlocked.Increment(ref _aiUpdatesProcessed);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[PawnAI Multithread] Ошибка обновления ИИ пешка: {ex.Message}");
                    }
                }
            }
        }

        private static void UpdatePawnAI(Pawn pawn)
        {
            if (pawn == null || pawn.mindState == null)
                return;

            lock (pawn)
            {
                if (pawn.CurJob != null && pawn.CurJob.targetA.IsValid)
                {
                    IntVec3 destination = pawn.CurJob.targetA.Cell;
                    if (destination.InBounds(pawn.Map))
                    {
                        OptimizedThreadedPathfinder.EnqueuePathfindingTask(pawn, destination, PathEndMode.OnCell);
                    }
                }
            }
        }

        public static int GetQueueSize(int threadIndex)
        {
            if (threadIndex < 0 || threadIndex >= AIThreadCount)
                return 0;

            lock (_queueLocks[threadIndex])
            {
                return _pawnQueues[threadIndex].Count;
            }
        }

        public static int GetTotalQueueSize()
        {
            int total = 0;
            for (int i = 0; i < AIThreadCount; i++)
            {
                total += GetQueueSize(i);
            }
            return total;
        }
    }
}