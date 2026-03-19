using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarDefense.Data
{
    public abstract class BaseTable { }

    /// <summary>
    /// 제네릭 데이터 테이블, int 키로 행에 접근한다
    /// </summary>
    public class Table<T> : BaseTable
    {
        private readonly Dictionary<int, T> data = new Dictionary<int, T>();

        public int Count => data.Count;

        public Table(T[] rows, Func<T, int> keySelector)
        {
            foreach (T row in rows)
            {
                int key = keySelector(row);
                data[key] = row;
            }
        }

        public T Get(int key)
        {
            if (data.TryGetValue(key, out T value))
            {
                return value;
            }

            Debug.LogError($"데이터 테이블 키 찾을 수 없음: {key}");
            return default;
        }

        public bool TryGet(int key, out T value)
        {
            return data.TryGetValue(key, out value);
        }
    }
}