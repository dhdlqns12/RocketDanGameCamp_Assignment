using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using StarDefense.Data;

namespace StarDefense.Managers
{
    /// <summary>
    /// JSON 데이터를 Table로 관리하는 데이터 매니저
    /// </summary>
    public static class DataManager
    {
        private static Dictionary<string, BaseTable> tables = new Dictionary<string, BaseTable>();

        public static bool Initialized { get; private set; }

        /// <summary>
        /// 모든 데이터 테이블을 로드 게임 시작 시 한 번 호출
        /// </summary>
        public static void Init()
        {
            tables.Clear();

            RegisterTable<StageData>("StageData", d => d.stageId);
            RegisterTable<MapData>("MapData", d => d.mapId);
            RegisterTable<EnemyData>("EnemyData", d => d.enemyId);
            RegisterTable<WaveData>("WaveData", d => d.stageId);

            Initialized = true;
            Debug.Log("[DataManager] Initialized.");
        }

        /// <summary>
        /// JSON 배열 파일을 로드하여 Table로 변환, 등록
        /// </summary>
        private static void RegisterTable<T>(string fileName, Func<T, int> keySelector)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

            if (jsonFile == null)
            {
                Debug.LogError($"데이터 파일 없음: {fileName}");
                return;
            }

            T[] rows = JsonConvert.DeserializeObject<T[]>(jsonFile.text);

            if (rows == null)
            {
                Debug.LogError($"데이터 파싱 실패: {fileName}");
                return;
            }

            tables[typeof(T).Name] = new Table<T>(rows, keySelector);
            Debug.Log($"{typeof(T).Name}등록 ({rows.Length} rows)");
        }

        /// <summary>
        /// 타입으로 테이블을 가져옴
        /// </summary>
        public static Table<T> GetTable<T>()
        {
            string key = typeof(T).Name;

            if (!tables.TryGetValue(key, out BaseTable table))
            {
                Debug.LogError($"데이터 테이블 찾을 수 없음: {key}");
                return null;
            }

            if (table is Table<T> casted)
            {
                return casted;
            }

            Debug.LogError($"데이터 테이블 타입 일치하지 않음: {key}");
            return null;
        }

        public static void Clear()
        {
            tables.Clear();
            Initialized = false;
        }
    }
}