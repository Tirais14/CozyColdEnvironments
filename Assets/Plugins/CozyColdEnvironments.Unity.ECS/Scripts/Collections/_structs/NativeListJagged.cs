//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Jobs;

//#nullable enable
//namespace CCEnvs.UnityX.ECS.Collections
//{
//    public struct NativeListJagged<T> 
//        :
//        INativeDisposable,
//        IDisposable,
//        INativeList<T>,
//        IEnumerable<T>

//        where T : unmanaged
//    {
//        public NativeList<T> Values;
//        public NativeList<int> Lengths;
//        public NativeList<int> Offsets;

//        public int Capacity {
//            [BurstCompile]
//            readonly get => Values.Capacity;
//            [BurstCompile]
//            set => Values.Capacity = value;
//        }

//        public readonly bool IsEmpty {
//            [BurstCompile]
//            get => Values.IsEmpty;
//        }

//        public int Length {
//            [BurstCompile]
//            readonly get => Values.Length;
//            [BurstCompile]
//            set => Values.Length = value;
//        }

//        public T this[int index1, int index2] {
//            [BurstCompile]
//            get => Values[GetValueIndex(index1, index2)];
//            [BurstCompile]
//            set => Values[GetValueIndex(index1, index2)] = value;
//        }

//        T INativeList<T>.this[int index] {
//            [BurstCompile]
//            readonly get => Values[index];
//            [BurstCompile]
//            set => Values[index] = value;
//        }

//        public NativeListJagged(int capacity, AllocatorManager.AllocatorHandle allocator)
//        {
//            Values = new NativeList<T>(capacity, allocator);
//            Lengths = new NativeList<int>(4, allocator);
//            Offsets = new NativeList<int>(4, allocator);
//        }
//        public NativeListJagged(int capacity, int subListCapacity, AllocatorManager.AllocatorHandle allocator)
//        {
//            Values = new NativeList<T>(capacity, allocator);
//            Lengths = new NativeList<int>(subListCapacity, allocator);
//            Offsets = new NativeList<int>(subListCapacity, allocator);
//        }

//        public void Add(int index1, int index2)
//        {

//        }

//        [BurstCompile]
//        public ref T ElementAt(int index1, int index2)
//        {
//            return ref Values.ElementAt(Offsets[index1] + index2);
//        }

//        [BurstCompile]
//        public int GetLength(int index)
//        {
//            if (index >= Lengths.Length)
//                return -1;

//            return Lengths[index];
//        }

//        [BurstCompile]
//        public int GetValueIndex(int index1, int index2)
//        {
//            if (index1 >= Offsets.Length)
//                return -1;

//            return Offsets[index1] + index2;
//        }

//        [BurstCompile]
//        public bool Contains(int index1, int index2)
//        {
//            return GetValueIndex(index1, index2) >= 0;
//        }

//        [BurstCompile]
//        public void Clear() => Values.Clear();

//        [BurstCompile]
//        public JobHandle Dispose(JobHandle inputDeps)
//        {
//            var valuesHandle = Values.Dispose(inputDeps);
//            var lengthsHandle = Lengths.Dispose(valuesHandle);
//            var offsetsHandle = Offsets.Dispose(inputDeps);

//            return JobHandle.CombineDependencies(valuesHandle, lengthsHandle, offsetsHandle);
//        }
//        [BurstCompile]
//        public void Dispose()
//        {
//            Values.Dispose();
//            Lengths.Dispose();
//            Offsets.Dispose();
//        }

//        public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();
//        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

//        [BurstCompile]
//        ref T IIndexable<T>.ElementAt(int index) => ref Values.ElementAt(index);
//    }

//    public static class NativeListJaggedExtensions
//    {
//        public static int IndexOf<T>(this NativeListJagged<T> source, T value)
//            where T : unmanaged, IEquatable<T>
//        {
//            for (int i = 0; i < source.Values.Length; i++)
//            {
//                if (source.Values[i].Equals(value))
//                    return i;
//            }

//            return -1;
//        }

//        public static bool Contains<T>(this NativeListJagged<T> source, T value)
//            where T : unmanaged, IEquatable<T>
//        {
//            return source.IndexOf(value) != -1;
//        }
//    }
//}
