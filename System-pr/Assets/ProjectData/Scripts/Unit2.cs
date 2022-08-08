using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Unit2 : MonoBehaviour //Task 2
{

    [SerializeField] private int mSecondsCount = 1000;
    [SerializeField] private int framesCount = 60;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private void Start()
    {
        var ct = cancellationTokenSource.Token;

        Task task1 = Task1(ct);
        Task task2 = Task2(ct);

        //Task task1 = new Task(() => Task1(ct));
        //task1.Start();

        //Task task2 = new Task(() => Task2(ct));
        //task2.Start();

        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }

    private async Task Task1(CancellationToken ct)
    {
        if(ct.IsCancellationRequested)
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

    private void OnDestroy()
    {
        cancellationTokenSource.Dispose();
    }

}
