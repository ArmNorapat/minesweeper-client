using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Utilities
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
                OnCreated();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnCreated()
        {
            //Do nothing
        }
    }
}