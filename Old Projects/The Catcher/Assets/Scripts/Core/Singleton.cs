using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance = null;

    private static object lockSingleton = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            lock (lockSingleton)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                        return instance;

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = string.Format("{0}Singleton", typeof(T).ToString());
                        DontDestroyOnLoad(singleton);
                    }
                }

                return instance;
            }
        }
    }

    void Awake()
    {
        gameObject.name = string.Format("{0}Singleton", typeof(T).ToString());
        DontDestroyOnLoad(this);
    }

    private static bool applicationIsQuitting = false;

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}