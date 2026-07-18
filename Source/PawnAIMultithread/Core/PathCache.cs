using System;
using System.Collections.Generic;
using System.Threading;
using Verse;
using RimWorld;

namespace PawnAIMultithread.Core
{
    public class PathCache
    {
        private class CachedPath
        {
            public Path Path;
            public int CreatedTick;
            public int ExpirationTick;
        }

        private static readonly Dictionary<long, CachedPath> PathCacheDict = new Dictionary<long, CachedPath>();
        private static readonly object CacheLock = new object();
        private const int CACHE_LIFETIME_TICKS = 200;
        private const int MAX_CACHE_SIZE = 2000;

        public static void CachePath(Pawn pawn, IntVec3 destination, Path path)
        {
            if (pawn == null || path == null || !destination.IsValid)
                return;

            long cacheKey = GenerateCacheKey(pawn, destination);

            lock (CacheLock)
            {
                if (PathCacheDict.Count >= MAX_CACHE_SIZE)
                {
                    ClearOldestEntries();
                }

                PathCacheDict[cacheKey] = new CachedPath
                {
                    Path = path,
                    CreatedTick = Find.TickManager.TicksGame,
                    ExpirationTick = Find.TickManager.TicksGame + CACHE_LIFETIME_TICKS
                };
            }
        }

        public static bool TryGetCachedPath(Pawn pawn, IntVec3 destination, out Path cachedPath)
        {
            cachedPath = null;

            if (pawn == null || !destination.IsValid)
                return false;

            long cacheKey = GenerateCacheKey(pawn, destination);
            int currentTick = Find.TickManager.TicksGame;

            lock (CacheLock)
            {
                if (PathCacheDict.TryGetValue(cacheKey, out var cached))
                {
                    if (currentTick < cached.ExpirationTick && cached.Path != null && cached.Path.Valid)
                    {
                        cachedPath = cached.Path;
                        return true;
                    }
                    else
                    {
                        PathCacheDict.Remove(cacheKey);
                    }
                }
            }

            return false;
        }

        public static void InvalidatePathsForPawn(Pawn pawn)
        {
            if (pawn == null)
                return;

            lock (CacheLock)
            {
                var keysToRemove = new List<long>();
                foreach (var kvp in PathCacheDict)
                {
                    if (ExtractPawnFromCacheKey(kvp.Key) == pawn.thingIDNumber)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    PathCacheDict.Remove(key);
                }
            }
        }

        public static void ClearExpiredEntries()
        {
            int currentTick = Find.TickManager.TicksGame;

            lock (CacheLock)
            {
                var expiredKeys = new List<long>();
                foreach (var kvp in PathCacheDict)
                {
                    if (currentTick >= kvp.Value.ExpirationTick || kvp.Value.Path == null || !kvp.Value.Path.Valid)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }

                foreach (var key in expiredKeys)
                {
                    PathCacheDict.Remove(key);
                }
            }
        }

        private static long GenerateCacheKey(Pawn pawn, IntVec3 destination)
        {
            long pawnPart = (long)pawn.thingIDNumber << 32;
            long destPart = ((long)destination.x << 16) | ((long)destination.z & 0xFFFF);
            return pawnPart | destPart;
        }

        private static int ExtractPawnFromCacheKey(long key)
        {
            return (int)(key >> 32);
        }

        private static void ClearOldestEntries()
        {
            int targetSize = MAX_CACHE_SIZE / 2;
            var sortedEntries = new List<KeyValuePair<long, CachedPath>>(PathCacheDict);
            sortedEntries.Sort((a, b) => a.Value.CreatedTick.CompareTo(b.Value.CreatedTick));

            for (int i = 0; i < sortedEntries.Count - targetSize; i++)
            {
                PathCacheDict.Remove(sortedEntries[i].Key);
            }
        }

        public static void Clear()
        {
            lock (CacheLock)
            {
                PathCacheDict.Clear();
            }
        }

        public static int GetCacheSize()
        {
            lock (CacheLock)
            {
                return PathCacheDict.Count;
            }
        }
    }
}