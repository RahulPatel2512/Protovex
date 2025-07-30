using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Pooler : MonoBehaviour
{
    private static Pooler _instance;

    internal static Pooler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("[Pooler]").AddComponent<Pooler>();
            }

            return _instance;
        }
    }

    [SerializeField] private List<Pool> _pools = new List<Pool>();
    private Dictionary<GameObject, Pool> _map = new Dictionary<GameObject, Pool>();

    internal void Register(GameObject obj, Pool pool)
    {
        _map[obj] = pool;
    }

    internal Pool GetPool(GameObject obj)
    {
        if (!_map.ContainsKey(obj))
        {
            var pool = new Pool();
            _pools.Add(pool);
            pool.Init(obj);
            Register(obj, pool);
        }

        return _map[obj];
    }
}

[System.Serializable]
internal class Pool
{
    [SerializeField, HideInInspector] string _displayName;
    [SerializeField] GameObject _prefab;
    [SerializeField] Transform _root;
    [SerializeField] List<GameObject> _spawned = new List<GameObject>();
    [SerializeField] List<GameObject> _despawned = new List<GameObject>();

    internal int Count => _spawned.Count + _despawned.Count;

    internal void Init(GameObject obj)
    {
        _displayName = obj.name;
        _prefab = obj;
        _root = _prefab.transform.parent;
        if (_root == null)
        {
            _root = Pooler.Instance.transform;
        }
    }

    internal void Create(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Create();
        }
    }

    internal GameObject Spawn(Transform parent = null)
    {
        var obj = _despawned.ElementAtOrDefault(0);
        if (obj == null)
        {
            obj = Create();
        }

        SetSpawned(obj, parent);
        return obj;
    }

    internal void Despawn(GameObject obj)
    {
        if (!_despawned.Contains(obj))
        {
            SetDespawned(obj);
        }
    }

    private void SetSpawned(GameObject obj, Transform parent = null)
    {
        obj.SetActive(true);
        obj.transform.SetParent(parent != null ? parent : _root);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        _despawned.Remove(obj);
        _spawned.Add(obj);
    }

    private void SetDespawned(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(_root);
        _despawned.Add(obj);
        _spawned.Remove(obj);
    }

    private GameObject Create()
    {
        if (_prefab != null)
        {
            var obj = GameObject.Instantiate(_prefab, _root);
            Pooler.Instance.Register(obj, this);
            obj.name = obj.name.Replace("(Clone)", "");
            obj.SetActive(false);
            _despawned.Add(obj);
            return obj;
        }

        return null;
    }
}

internal static class PoolingUtils
{
    public static T Spawn<T>(this T obj, Transform parent = null) where T : Component
    {
        return Pooler.Instance.GetPool(obj.gameObject).Spawn(parent).GetComponent<T>();
    }

    public static GameObject Spawn(this GameObject obj, Transform parent = null)
    {
        return Pooler.Instance.GetPool(obj).Spawn(parent);
    }

    public static void Despawn<T>(this T obj) where T : Component
    {
        if (obj == null) return;
        Pooler.Instance.GetPool(obj.gameObject).Despawn(obj.gameObject);
    }

    public static void Despawn(this GameObject obj)
    {
        if (obj == null) return;
        Pooler.Instance.GetPool(obj).Despawn(obj);
    }

    public static void Pool<T>(this T obj, int count) where T : Component
    {
        Pool(obj.gameObject, count);
    }

    public static void Pool(this GameObject obj, int count)
    {
        var pool = Pooler.Instance.GetPool(obj);
        pool.Create(Mathf.Clamp(count - pool.Count, 0, count));
    }
}