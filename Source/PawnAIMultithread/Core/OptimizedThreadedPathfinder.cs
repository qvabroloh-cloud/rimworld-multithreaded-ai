using System;
using System.Collections.Generic;
using System.Threading;
using Verse;
using RimWorld;

namespace PawnAIMultithread.Core
{
    public class OptimizedThreadedPathfinder
    {
        private static readonly int ThreadCount = Math.Max(1, Environment.ProcessorCount / 2);
        private static readonly Queue<PathfindingTask> PathfindingQueue = new Queue<PathfindingTask>();
        private static readonly object QueueLock = new object();
        private static Thread[] _pathfindingThreads;
        private static bool _threadsShouldRun = true;
        private static int _tasksProcessed = 0;
        private static int _cacheHits = 0;

        public class PathfindingTask
        {
            public Pawn Pawn;
            public IntVec3 Destination;
            public PathEndMode PathEndMode;
            public int CreatedTick;
            public Path ResultPath;
            public bool IsCompleted;
        }

        public static void Initialize()
        {
            _threadsShouldRun = true;
            _tasksProcessed = 0;
            _cacheHits = 0;
            _pathfindingThreads = new Thread[ThreadCount];

            for (int i = 0; i < ThreadCount; i++)
            {
                _pathfindingThreads[i] = new Thread(PathfindingWorker)
                {
                    Name = $"PawnAI-Pathfinder-{i}",
                    IsBackground = true
                };
                _pathfindingThreads[i].Start();
            }

            Log.Message($"[PawnAI Multithread] Инициализировано {ThreadCount} потоков поиска пути с кэшированием");
        }

        public static void Shutdown()
        {
            _threadsShouldRun = false;
            lock (QueueLock)
            {
                PathfindingQueue.Clear();
                Monitor.PulseAll(QueueLock);
            }

            foreach (var thread in _pathfindingThreads)
            {
                thread?.Join(5000);
            }

            Log.Message($"[PawnAI Multithread] Статистика: обработано {_tasksProcessed} маршрутов, попаданий кэша: {_cacheHits}");
        }

        public static void EnqueuePathfindingTask(Pawn pawn, IntVec3 destination, PathEndMode peMode)
        {
            if (pawn?.Map == null)
                return;

            if (PathCache.TryGetCachedPath(pawn, destination, out var cachedPath))
            {
                Interlocked.Increment(ref _cacheHits);
                return;
            }

            var task = new PathfindingTask
            {
                Pawn = pawn,
                Destination = destination,
                PathEndMode = peMode,
                CreatedTick = Find.TickManager.TicksGame
            };

            lock (QueueLock)
            {
                PathfindingQueue.Enqueue(task);
                Monitor.Pulse(QueueLock);
            }
        }

        private static void PathfindingWorker()
        {
            while (_threadsShouldRun)
            {
                PathfindingTask task = null;

                lock (QueueLock)
                {
                    if (PathfindingQueue.Count > 0)
                    {
                        task = PathfindingQueue.Dequeue();
                    }
                    else
                    {
                        Monitor.Wait(QueueLock, 100);
                        continue;
                    }
                }

                if (task != null)
                {
                    try
                    {
                        ProcessPathfindingTask(task);
                        Interlocked.Increment(ref _tasksProcessed);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[PawnAI Multithread] Ошибка при поиске пути: {ex.Message}");
                    }
                }
            }
        }

        private static void ProcessPathfindingTask(PathfindingTask task)
        {
            if (task.Pawn == null || task.Pawn.Map == null)
                return;

            if (PathCache.TryGetCachedPath(task.Pawn, task.Destination, out var cachedPath))
            {
                task.ResultPath = cachedPath;
                task.IsCompleted = true;
                return;
            }

            var pathFinder = task.Pawn.Map.pathFinder;
            var path = pathFinder.FindPath(task.Pawn.Position, task.Destination, task.Pawn, task.PathEndMode);

            if (path != null && path.Valid)
            {
                PathCache.CachePath(task.Pawn, task.Destination, path);
            }

            task.ResultPath = path;
            task.IsCompleted = true;
        }

        public static void ClearCache()
        {
            PathCache.Clear();
            Log.Message("[PawnAI Multithread] Кэш маршрутов очищен");
        }

        public static void ClearExpiredCacheEntries()
        {
            PathCache.ClearExpiredEntries();
        }

        public static int GetQueueSize()
        {
            lock (QueueLock)
            {
                return PathfindingQueue.Count;
            }
        }

        public static int GetCacheSize()
        {
            return PathCache.GetCacheSize();
        }

        public static float GetCacheHitRate()
        {
            int total = _tasksProcessed + _cacheHits;
            return total > 0 ? (float)_cacheHits / total : 0f;
        }
    }
}