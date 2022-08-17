using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = System.Random;
using UnityEngine.Jobs;

public class Jobs2 : MonoBehaviour //Task 2
{
    [SerializeField] private int positionsLength = 10;
    [SerializeField] private int velocitiesLength = 10;    

    void Start()
    {
        NativeArray<Vector3> positions = new NativeArray<Vector3>(positionsLength, Allocator.Persistent);
        NativeArray<Vector3> velocities = new NativeArray<Vector3>(velocitiesLength, Allocator.Persistent);
        NativeArray<Vector3> finalPositions = new NativeArray<Vector3>(positionsLength, Allocator.Persistent);

        positions = FillArray(positionsLength);
        velocities = FillArray(velocitiesLength);

        FinalPositionsJob finalPositionsJob = new FinalPositionsJob()
        {
            Positions = positions,
            Velocities = velocities,
            FinalPositions = finalPositions
        };

        JobHandle jobHand = new JobHandle();
        jobHand = finalPositionsJob.Schedule(finalPositions.Length, 0);
        jobHand.Complete();

        for (var i = 0; i < finalPositions.Length; i++)
        {
            Debug.Log($"Element {i} rez: {finalPositions[i]}");
        }

        positions.Dispose();
        velocities.Dispose();
        finalPositions.Dispose();
    }

    private NativeArray<Vector3> FillArray(int arrayLength)
    {
        NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(arrayLength, Allocator.Persistent);
        Random rnd = new Random();

        for (var i = 0; i < nativeArray.Length; i++)
        {
            float randomX = rnd.Next(-100, 100);
            float randomY = rnd.Next(-100, 100);
            float randomZ = rnd.Next(-100, 100);
            nativeArray[i] = new Vector3((float)randomX, (float)randomY, (float)randomZ);
            //Debug.Log($"Element {i}: {nativeArray[i]}");
        }

        return nativeArray;
    }
}

public struct FinalPositionsJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> Positions;
    [ReadOnly] public NativeArray<Vector3> Velocities;
    [WriteOnly] public NativeArray<Vector3> FinalPositions;

    public void Execute(int index)
    {
        FinalPositions[index] = Positions[index] + Velocities[index];
    }
}