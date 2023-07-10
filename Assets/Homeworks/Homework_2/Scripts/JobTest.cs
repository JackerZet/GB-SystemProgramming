using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Homeworks.homework_2
{
    internal class JobTest : MonoBehaviour
    {
        [SerializeField] private int[] ints = new int[] { 1, 3, 70, 4, 67 };
        
        private NativeArray<int> nativeInts;
        private JobHandle jobHandle;
        private void Start()
        {
            nativeInts = new NativeArray<int>(ints, Allocator.Persistent);
            var job = new ArrayJob { intArray = nativeInts };

            jobHandle = job.Schedule();
            jobHandle.Complete();

            ShowResult(job.intArray);

            nativeInts.Dispose();
        }

        private void ShowResult<T>(NativeArray<T> array) where T : struct
        {
            for (int i = 0; i < array.Length; i++)
            {
                Debug.Log(i + " element: " + array[i]);
            }
        }
    }
}

