using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Homeworks.homework_1
{
    internal class TasksTest : MonoBehaviour
    {
        private void Start()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task.Run(() => Task1(cancellationToken));
            Task.Run(() => Task2(cancellationToken));
        }
        private async void Task1(CancellationToken cancellationToken)
        {
            await Task.Delay(1000);

            Debug.Log("Task1 comleted");
        }
        private async void Task2(CancellationToken cancellationToken)
        {
            for (int frames = 60; frames > 0; frames--)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Task2 aborted");
                    return;
                }     
                await Task.Yield();
            }

            Debug.Log("Task2 completed");
        }
    }
}

