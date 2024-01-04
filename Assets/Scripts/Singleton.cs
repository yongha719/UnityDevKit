using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    var go = new GameObject($"{typeof(T)}");
                    instance = go.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    protected virtual bool dontDestroy { get; }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;

            if (dontDestroy)
                DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }
}
