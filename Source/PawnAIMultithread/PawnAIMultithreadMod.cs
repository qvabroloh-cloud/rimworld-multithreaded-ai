using Verse;
using PawnAIMultithread.Core;
using PawnAIMultithread.Debug;

namespace PawnAIMultithread
{
    public class PawnAIMultithreadMod : Mod
    {
        public static PawnAIMultithreadSettings Settings;

        public PawnAIMultithreadMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<PawnAIMultithreadSettings>();
            var harmony = new HarmonyLib.Harmony("PawnAIMultithread.Core");
            harmony.PatchAll();
            Log.Message("[PawnAI Multithread] Мод успешно загружен с оптимизациями");
        }

        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("=== Настройки PawnAI Multithread ===", -1f);
            listing.Gap();

            listing.Label("Основные функции:");
            listing.CheckboxLabeled("Включить многопоточный поиск пути", ref Settings.enableMultithreadedPathfinding);
            listing.CheckboxLabeled("Включить многопоточный ИИ пешек", ref Settings.enableMultithreadedPawnAI);
            listing.CheckboxLabeled("Включить кэширование маршрутов", ref Settings.enablePathCaching);
            listing.Gap();

            listing.Label("Параметры потоков:");
            listing.Label($"Потоков поиска пути: {Settings.pathfindingThreadCount}", -1f);
            Settings.pathfindingThreadCount = (int)listing.Slider(Settings.pathfindingThreadCount, 1, 8);

            listing.Label($"Потоков ИИ: {Settings.pawnAIThreadCount}", -1f);
            Settings.pawnAIThreadCount = (int)listing.Slider(Settings.pawnAIThreadCount, 1, 8);
            listing.Gap();

            listing.Label("Параметры кэша:");
            listing.Label($"Размер кэша маршрутов: {Settings.maxCacheSize}", -1f);
            Settings.maxCacheSize = (int)listing.Slider(Settings.maxCacheSize, 500, 5000, roundTo: 100f);

            listing.Label($"Lifetime кэша (тики): {Settings.cacheLifetimeTicks}", -1f);
            Settings.cacheLifetimeTicks = (int)listing.Slider(Settings.cacheLifetimeTicks, 50, 500);
            listing.Gap();

            listing.Label("Отладка и профилирование:");
            if (listing.ButtonText("Профилировать производительность"))
            {
                PawnAIProfiler.ProfilePerformance();
            }

            if (listing.ButtonText("Показать советы по оптимизации"))
            {
                PawnAIProfiler.PrintOptimizationTips();
            }

            if (listing.ButtonText("Очистить кэш маршрутов"))
            {
                OptimizedThreadedPathfinder.ClearCache();
                Messages.Message("Кэш маршрутов очищен", MessageTypeDefOf.TaskCompletion);
            }

            listing.Gap();
            listing.Label("Статистика:", -1f);
            listing.Label($"Размер кэша: {OptimizedThreadedPathfinder.GetCacheSize()}", -1f);
            listing.Label($"Очередь поиска: {OptimizedThreadedPathfinder.GetQueueSize()}", -1f);
            listing.Label($"Попадания кэша: {(OptimizedThreadedPathfinder.GetCacheHitRate() * 100):F2}%", -1f);

            if (listing.ButtonText("Применить настройки"))
            {
                Log.Message("[PawnAI Multithread] Настройки применены");
            }

            listing.End();
        }

        public override string SettingsCategory()
        {
            return "PawnAI Multithread";
        }
    }

    public class PawnAIMultithreadSettings : ModSettings
    {
        public bool enableMultithreadedPathfinding = true;
        public bool enableMultithreadedPawnAI = true;
        public bool enablePathCaching = true;
        public int pathfindingThreadCount = 2;
        public int pawnAIThreadCount = 2;
        public int maxCacheSize = 2000;
        public int cacheLifetimeTicks = 200;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableMultithreadedPathfinding, "enableMultithreadedPathfinding", true);
            Scribe_Values.Look(ref enableMultithreadedPawnAI, "enableMultithreadedPawnAI", true);
            Scribe_Values.Look(ref enablePathCaching, "enablePathCaching", true);
            Scribe_Values.Look(ref pathfindingThreadCount, "pathfindingThreadCount", 2);
            Scribe_Values.Look(ref pawnAIThreadCount, "pawnAIThreadCount", 2);
            Scribe_Values.Look(ref maxCacheSize, "maxCacheSize", 2000);
            Scribe_Values.Look(ref cacheLifetimeTicks, "cacheLifetimeTicks", 200);
        }
    }
}