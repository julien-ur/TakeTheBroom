﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public struct PoolItem
{
    public USTask.POSITION MainTaskPos;
    public USTask.POSITION SecondaryTaskPos;

    public PoolItem(USTask.POSITION mainTaskPos, USTask.POSITION secondaryTaskPos)
    {
        MainTaskPos = mainTaskPos;
        SecondaryTaskPos = secondaryTaskPos;
    }

    public int GetTaskCount()
    {
        int taskCount = 0;
        if (MainTaskPos != USTask.POSITION.None)
            taskCount ++;
        if (SecondaryTaskPos != USTask.POSITION.None)
            taskCount ++;

        return taskCount;
    }

    public bool ContainsMainTask()
    {
        return MainTaskPos != USTask.POSITION.None;
    }

    public bool ContainsSecondaryTask()
    {
        return SecondaryTaskPos != USTask.POSITION.None;
    }
}

public class USTaskPoolGenerator
{
    // public enum RELATION { Synchronous, Asynchronous }
    public enum MODE { TRAINING_RING_ONLY, TRAINING_COMPLETE, STUDY }

    private const int TaskPositions = 3;
    private const int RelationNum = 2;

    // private readonly List<USTask.POSITION>[] _asychronousPositionPool;

    private const int SecondaryTaskConditions = 3;
    private const int SecondaryTaskConditionRepetitions = 3;

    private const int MinMainTasksOnlyBeforeSecondaryTask = 3;
    private const int MaxMainTasksOnlyBeforeSecondaryTask = 6;
    
    private const int TrainingMainTaskRepetitions = 20;
    private const int TrainingSecondaryTaskRepetitions = 3;

    public USTaskPoolGenerator()
    {
        // _asychronousPositionPool = CreateAsynchronousPositionsPool();
    }

    public List<PoolItem> GeneratePool(MODE m)
    {
        return m == MODE.TRAINING_RING_ONLY ? GenerateTrainingPool(TrainingMainTaskRepetitions, 0) : GenerateStudyPool();
    }

    private static List<PoolItem> GenerateTrainingPool(int mainTaskRepetitions, int secondaryTaskRepetitions)
    {
        List<PoolItem> actionPool = new List<PoolItem>();

        for (int i = 0; i < mainTaskRepetitions; i++)
        {
            int rndPos = Random.Range(0, TaskPositions);
            var poolItem = new PoolItem((USTask.POSITION)rndPos, USTask.POSITION.None);
            actionPool.Add(poolItem);
        }

        if (secondaryTaskRepetitions > 0)
        {
            for (int i = 0; i < TaskPositions; i++)
            {
                var poolItem = new PoolItem(USTask.POSITION.None, (USTask.POSITION) i);

                for (int j = 0; j < secondaryTaskRepetitions - 1; j++)
                {
                    actionPool.Add(poolItem);
                }
            }

            for (int i = 0; i < TaskPositions; i++)
            {
                var poolItem = new PoolItem(USTask.POSITION.None, (USTask.POSITION)i);
                actionPool.Add(poolItem);
            }
        }
        
        return actionPool;
    }

    private List<PoolItem> GenerateStudyPool()
    {
        List<int> secondaryTaskPool = CreateSecondaryTaskPool();
        var actionPool = new List<PoolItem>();

        int secondaryTaskCounter = Random.Range(MinMainTasksOnlyBeforeSecondaryTask, MaxMainTasksOnlyBeforeSecondaryTask+1);

        while (secondaryTaskPool.Count > 0)
        {
            PoolItem poolItem;

            if (secondaryTaskCounter-- == 0)
            {
                poolItem = CreateDoubleTaskPoolItem(ref secondaryTaskPool);
                secondaryTaskCounter = Random.Range(MinMainTasksOnlyBeforeSecondaryTask, MaxMainTasksOnlyBeforeSecondaryTask+1);
            }
            else
            {
                var rndPos = (USTask.POSITION)Random.Range(0, TaskPositions);
                poolItem = new PoolItem(rndPos, USTask.POSITION.None);
            }
            actionPool.Add(poolItem);
        }
        
        return actionPool;
    }

    private static List<int> CreateSecondaryTaskPool()
    {
        var secondaryTaskPool = new List<int>();

        for (int i = 0; i < SecondaryTaskConditionRepetitions; i++)
        {
            secondaryTaskPool.AddRange(Enumerable.Range(0, SecondaryTaskConditions));
        }

        return secondaryTaskPool;
    }

    private static List<USTask.POSITION>[] CreateAsynchronousPositionsPool()
    {
        var asychronousPool = new List<USTask.POSITION>[3];

        foreach (var pos in USTask.GetPositionsExcluding(new[] { USTask.POSITION.None }))
        {
            var balancedPosList = new List<USTask.POSITION>();
            var remainingPositions = USTask.GetPositionsExcluding(new[] { pos, USTask.POSITION.None });

            foreach (var rp in remainingPositions)
            {
                for (int i = 0; i < SecondaryTaskConditionRepetitions / RelationNum; i++)
                {
                    balancedPosList.Add(rp);
                }
            }

            int rest = SecondaryTaskConditionRepetitions % RelationNum;
            if (rest > 0)
            {
                for (int j = 0; j < rest; j++)
                {
                    balancedPosList.Add(GetRandomPositionExcluding(pos));
                }
            }
            asychronousPool[(int)pos] = balancedPosList;
        }

        return asychronousPool;
    }

    private static PoolItem CreateDoubleTaskPoolItem(ref List<int> secondaryTaskPool)
    {
        int secondaryTaskCondition = TakeRandomConditionFromPool(ref secondaryTaskPool);
        var secondaryTaskPosition = (USTask.POSITION) secondaryTaskCondition;

        //var secondaryTaskPosition = (USTask.POSITION)(secondaryTaskCondition / 2);
        //var taskRelation = (RELATION)(secondaryTaskCondition % 2);

        //USTask.POSITION mainTaskPosition = secondaryTaskPosition;
        //if (taskRelation == RELATION.Asynchronous)
        //{
        //    mainTaskPosition = TakeRandomConditionFromPool(ref _asychronousPositionPool[(int)secondaryTaskPosition]);
        //}

        // var secondaryTaskPosition = (USTask.POSITION)Random.Range(0, SecondaryTaskConditionRepetitions);

        return new PoolItem(USTask.POSITION.None, secondaryTaskPosition);
    }

    private static T TakeRandomConditionFromPool<T>(ref List<T> pool)
    {
        int taskIdx = Random.Range(0, pool.Count);

        T condition = pool[taskIdx];
        pool.RemoveAt(taskIdx);

        return condition;
    }

    private static USTask.POSITION GetRandomPositionExcluding(USTask.POSITION posToExclude)
    {
        USTask.POSITION[] remainingPositions = USTask.GetPositionsExcluding(new []{ posToExclude, USTask.POSITION.None });
        return remainingPositions[Random.Range(0, remainingPositions.Length)];
    }
}
