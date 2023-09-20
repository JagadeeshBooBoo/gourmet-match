using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public static class GAEventManager
{
    public static bool IsInitialized { get; private set; }

    static GAEventManager()
    {

    }

    public static void Init()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();
        IsInitialized = true;
    }

    public static void LogLevelStartEvent(int level)
    {
        CheckGAInit();
        if (level < 1)
        {
            throw new System.Exception("Level num should start from 1");
        }

        var lvl = "level_" + level.ToString("000");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, lvl);
    }

    public static void LogLevelEndEvent(int level)
    {
        CheckGAInit();
        if (level < 1)
        {
            throw new System.Exception("Level num should start from 1");
        }

        var lvl = "level_" + level.ToString("000");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, lvl);
    }

    public static void LogLevelFailEvent(int level)
    {
        CheckGAInit();
        if (level < 1)
        {
            throw new System.Exception("Level num should start from 1");
        }

        var lvl = "level_" + level.ToString("000");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, lvl);
    }

    public static void LogLevelUpEvent(int level)
    {
        CheckGAInit();
        if (level < 1)
        {
            throw new System.Exception("Level num should start from 1");
        }

        var lvl = "level_" + level.ToString("000");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Undefined, lvl);
    }


    public static LevelProgressTimeData LogLevelStartEventWithTime(int level)
    {
        CheckGAInit();
        if (level < 1)
        {
            throw new System.Exception("Level num should start from 1");
        }

        var lvl = "level_" + level.ToString("000");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, lvl);
        return new LevelProgressTimeData(level, Time.time);
    }

    public static void LogLevelEndEventWithTime(LevelProgressTimeData data)
    {
        CheckGAInit();
        if (data.level < 1)
        {
            throw new System.Exception("Level num should start from 1");
        }
        var lvl = "level_" + data.level.ToString("000");
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, lvl);

        var timeDiff = Time.time - data.levelStartTime;
        GameAnalytics.NewDesignEvent("level_time:" + lvl, timeDiff);
    }

    public struct LevelProgressTimeData
    {
        public int level { get; private set; }
        public float levelStartTime { get; private set; }

        public LevelProgressTimeData(int lvl, float levelStartTime)
        {
            this.level = lvl;
            this.levelStartTime = levelStartTime;
        }
    }

    static void CheckGAInit()
    {
        if (IsInitialized)
            return;
        throw new System.Exception("GameAnalytics is not initialized yet. Call GAEventManager.Init() to init the GA");
    }
}
