using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Homeworks.homework_2
{
    internal class JobParallelForTest : MonoBehaviour
    {
        [SerializeField] private Vector3[] positions = new Vector3[] {Vector3.zero};
        [SerializeField] private Vector3[] velocities = new Vector3[] { Vector3.zero };

        private NativeArray<Vector3> nativePositions;
        private NativeArray<Vector3> nativeVelocities;
        private NativeArray<Vector3> nativeFinalPositions;

        private JobHandle jobHandle;

        private void Start()
        {
            if (positions.Length != velocities.Length)
            {
                Debug.LogError("Arrays lengths do not match");
                return;
            }
            
            nativePositions = new NativeArray<Vector3>(positions, Allocator.Persistent);
            nativeVelocities = new NativeArray<Vector3>(velocities, Allocator.Persistent);
            nativeFinalPositions = new NativeArray<Vector3>(positions.Length, Allocator.Persistent);

            var job = new PositionsJob { Positions = nativePositions, Velocities = nativeVelocities, FinalPositions = nativeFinalPositions};

            jobHandle = job.Schedule(positions.Length, 0);
            jobHandle.Complete();

            ShowResult(job.FinalPositions);

            nativePositions.Dispose();
            nativeVelocities.Dispose();
            nativeFinalPositions.Dispose();
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
