using UnityEngine;

public class DontDestroyScript : MonoBehaviour
{
    private static DontDestroyScript _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
