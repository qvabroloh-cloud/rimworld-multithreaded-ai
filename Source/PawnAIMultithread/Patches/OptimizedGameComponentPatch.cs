using HarmonyLib;
using Verse;
using RimWorld;
using PawnAIMultithread.Core;

namespace PawnAIMultithread.Patches
{
    [HarmonyPatch(typeof(Game), nameof(Game.LoadGame))]
    public static class OptimizedGameLoadPatch
    {
        public static void Postfix()
        {
            OptimizedThreadedPathfinder.Initialize();
            OptimizedThreadedPawnAI.Initialize();
            Log.Message("[PawnAI Multithread] Оптимизированная система инициализирована");
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.ExposeData))]
    public static class OptimizedGameSavePatch
    {
        public static void Prefix()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                OptimizedThreadedPathfinder.Shutdown();
                OptimizedThreadedPawnAI.Shutdown();
                Log.Message("[PawnAI Multithread] Система завершена перед сохранением");
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Tick))]
    public static class OptimizedPawnTickPatch
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance.IsHashIntervalTick(10))
            {
                OptimizedThreadedPawnAI.EnqueuePawnForAIUpdate(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.JobTrackerTick))]
    public static class OptimizedJobTrackerTickPatch
    {
        public static void Postfix(Pawn_JobTracker __instance)
        {
            var pawn = __instance.pawn;
            if (pawn != null && pawn.jobs?.curJob?.targetA.IsValid == true)
            {
                if (pawn.IsHashIntervalTick(15))
                {
                    OptimizedThreadedPathfinder.EnqueuePathfindingTask(pawn, pawn.jobs.curJob.targetA.Cell, PathEndMode.OnCell);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Map), nameof(Map.MapTick))]
    public static class OptimizedMapTickPatch
    {
        public static void Postfix(Map __instance)
        {
            if (__instance.IsHashIntervalTick(250))
            {
                OptimizedThreadedPathfinder.ClearExpiredCacheEntries();
            }
        }
    }
}