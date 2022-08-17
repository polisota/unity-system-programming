using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = System.Random;
using UnityEngine.Jobs;

public class Jobs1 : MonoBehaviour //Task 1
{
    [SerializeField] private int arrayLength = 10;
    [SerializeField] private int maxValue = 10;
    [SerializeField] private int defValue = 0;    

    void Start()
    {
        NativeArray<int> nativeArray = new NativeArray<int>(arrayLength, Allocator.TempJob);
        Random rnd = new Random();
       
        for (var i = 0; i < nativeArray.Length; i++)
        {
            nativeArray[i] = rnd.Next(3,70);
            Debug.Log(nativeArray[i]);
        }

        MyJob myJob = new MyJob()
        {
            array = nativeArray,
            maxLimitValue = maxValue,
            defaultValue = defValue
        };
        
        JobHandle jobHandle = new JobHandle();
        jobHandle = myJob.Schedule();
        jobHandle.Complete();

        for (var i = 0; i < nativeArray.Length; i++)
        {
            Debug.Log($"Element {i} rez: {nativeArray[i]}");
        }

        nativeArray.Dispose();
    }

}

public struct MyJob : IJob
{
    public NativeArray<int> array;

    public int maxLimitValue;
    public int defaultValue;

    public void Execute()
    {
        for (var i = 0; i < array.Length; i++)        
        {
            if (array[i] > maxLimitValue)
            {
                array[i] = defaultValue;                
            }
            
        }
    }
}