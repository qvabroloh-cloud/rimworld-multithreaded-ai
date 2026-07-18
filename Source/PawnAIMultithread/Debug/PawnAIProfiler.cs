using Verse;
using PawnAIMultithread.Core;

namespace PawnAIMultithread.Debug
{
    public class PawnAIProfiler
    {
        public static void ProfilePerformance()
        {
            Log.Message("=== [PawnAI Multithread] Профилирование производительности ===");
            
            int pathfinderQueueSize = OptimizedThreadedPathfinder.GetQueueSize();
            int cacheSize = OptimizedThreadedPathfinder.GetCacheSize();
            float cacheHitRate = OptimizedThreadedPathfinder.GetCacheHitRate();
            int totalAIQueueSize = OptimizedThreadedPawnAI.GetTotalQueueSize();

            Log.Message($"Размер очереди поиска пути: {pathfinderQueueSize}");
            Log.Message($"Размер кэша маршрутов: {cacheSize}");
            Log.Message($"Процент попаданий кэша: {(cacheHitRate * 100):F2}%");
            Log.Message($"Общий размер очередей ИИ: {totalAIQueueSize}");
            
            var map = Find.CurrentMap;
            if (map != null)
            {
                int totalPawns = 0;
                int activePawns = 0;
                
                foreach (var pawn in map.mapPawns.AllPawns)
                {
                    totalPawns++;
                    if (pawn.IsHashIntervalTick(10))
                    {
                        activePawns++;
                    }
                }

                Log.Message($"Всего пешков на карте: {totalPawns}");
                Log.Message($"Активных пешков: {activePawns}");
                Log.Message($"Процент активности: {(activePawns * 100f / totalPawns):F2}%");
            }

            Log.Message("=== Профилирование завершено ===");
        }

        public static void PrintOptimizationTips()
        {
            Log.Message("=== [PawnAI Multithread] Советы по оптимизации ===");
            
            float cacheHitRate = OptimizedThreadedPathfinder.GetCacheHitRate();
            
            if (cacheHitRate < 0.3f)
            {
                Log.Message("⚠ Низкий процент попаданий кэша. Рассмотрите увеличение lifetime кэша.");
            }
            else if (cacheHitRate > 0.7f)
            {
                Log.Message("✓ Отличный процент попаданий кэша!");
            }

            int pathfinderQueueSize = OptimizedThreadedPathfinder.GetQueueSize();
            if (pathfinderQueueSize > 500)
            {
                Log.Message("⚠ Большая очередь поиска пути. Рассмотрите увеличение потоков.");
            }

            int totalAIQueueSize = OptimizedThreadedPawnAI.GetTotalQueueSize();
            if (totalAIQueueSize > 200)
            {
                Log.Message("⚠ Большая очередь ИИ. Рассмотрите увеличение потоков ИИ.");
            }
        }
    }
}