using UnityEngine;
using UnityEngine.LowLevel;
using System;


namespace ChainPattern
{
    /// <summary>
    /// Updateタイミングの取得用コンポーネント
    /// </summary>
    public class CustomUpdateComponent : MonoBehaviour
    {
        static CustomUpdateComponent instance;
        static event Action onUpdate;

        static void Initialize()
        {
            if (instance != null) return;
            var go = new GameObject("CustomUpdateComponent");
            instance = go.AddComponent<CustomUpdateComponent>();
            DontDestroyOnLoad(go);
        }

        void Update()
        {
            onUpdate?.Invoke();
        }

        public static void AddUpdateListener(Action action)
        {
            Initialize();
            onUpdate += action;
        }

        public static void RemoveUpdateListener(Action action)
        {
            onUpdate -= action;
        }
    }
}
