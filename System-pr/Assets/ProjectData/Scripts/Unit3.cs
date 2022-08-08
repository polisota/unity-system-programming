using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Unit3 : MonoBehaviour //Task 3
{

    [SerializeField] private int mSecondsCount = 1000;
    [SerializeField] private int framesCount = 60;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private async void Start()
    {
        var ct = cancellationTokenSource.Token;

        if(await WhatTaskFasterAsync(ct, Task1(ct), Task2(ct)))
        {
            Debug.Log($"Task1 was faster.");
        }
        else
        {
            Debug.Log($"Task2 was faster.");
        }

        cancellationTokenSource.Dispose();
    }

    private async Task Task1(CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }
        else
        {
            await Task.Delay(mSecondsCount);
            Debug.Log("Task1 finished");
        }
    }

    private async Task Task2(CancellationToken ct)
    {
        var framesLeft = framesCount;

        while (framesLeft > 0)
        {
            if (ct.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            framesLeft--;
            await Task.Yield();
        }

        Debug.Log($"Task2 finished");
    }

    private async Task<bool> WhatTaskFasterAsync(CancellationToken ct, Task task1, Task task2)
    {
        var leader = await Task.WhenAny(task1, task2);

        if (leader == task1 && !ct.IsCancellationRequested)
        {
            cancellationTokenSource.Cancel();
            return true;
        }
        else
        {
            cancellationTokenSource.Cancel();
            return false;
        }        
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Dispose();
    }

}
