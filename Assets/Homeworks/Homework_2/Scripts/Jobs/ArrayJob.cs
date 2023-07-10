using Unity.Collections;
using Unity.Jobs;

namespace Homeworks.homework_2
{
    internal struct ArrayJob : IJob
    {
        public NativeArray<int> intArray;

        public void Execute()
        {
            for(int i = 0; i < intArray.Length; i++)
            {
                if (intArray[i] > 10)
                {
                    intArray[i] = 0;                
                }               
            }
        }
    }
}
