using System.Linq;
using UnityEngine;

/// <summary>
/// Базовый класс для синглтона в Unity
/// </summary>
/// <typeparam name="T">Тип класса-наследника</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    private static readonly object _lock = new object();

    /// <summary>
    /// Глобальный доступ к экземпляру синглтона
    /// </summary>
    public static T Instance
    {
        get
        {

            lock (_lock)
            {
                return _instance;
            }
        }
    }

    /// <summary>
    /// Виртуальный метод для инициализации синглтона
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);

            // Дополнительная инициализация
            Initialize();
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Удаляем дубликат экземпляра {typeof(T)}");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Переопределите этот метод для кастомной инициализации
    /// </summary>
    protected virtual void Initialize()
    {
        // Базовая реализация пуста
    }


}