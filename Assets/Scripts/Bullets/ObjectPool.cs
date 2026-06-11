using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    private readonly Stack<GameObject> pooledObjects = new Stack<GameObject>(128);
    private readonly HashSet<int> pooledInstanceIds = new HashSet<int>();

    [SerializeField]
    private int amountToPool = 20;
    [SerializeField]
    private GameObject bulletPrefab;

    public GameObject BulletPrefab => bulletPrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    [Server]
    public void PrewarmServer()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError($"{nameof(ObjectPool)}: bulletPrefab is not assigned.");
            return;
        }

        for (int i = 0; i < amountToPool; i++)
            CreateNewInactiveInstance();
    }

    private GameObject CreateNewInactiveInstance()
    {
        GameObject obj = Instantiate(bulletPrefab);
        obj.SetActive(false);
        PushIfNotAlreadyPooled(obj);
        return obj;
    }

    [Server]
    public GameObject GetServerObject()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError($"{nameof(ObjectPool)}: bulletPrefab is not assigned.");
            return null;
        }

        while (pooledObjects.Count > 0)
        {
            var go = PopInternal();
            if (go == null)
                continue;

            // Safety: never hand out an object that is still spawned.
            // This can happen if something returns the same instance twice.
            if (go.TryGetComponent(out NetworkIdentity identity) &&
                identity.netId != 0 &&
                NetworkServer.spawned.ContainsKey(identity.netId))
            {
                continue;
            }

            return go;
        }

        return CreateNewInactiveInstance();
    }

    // Backwards-compatible name for any older call sites.
    [Server]
    public GameObject GetPooledObject() => GetServerObject();

    // Server-only return. In Host mode, Mirror will call the client-side unspawn handler,
    // so we must not double-return the same instance.
    [Server]
    public void ReturnServerObject(GameObject go)
    {
        if (go == null) return;
        PushIfNotAlreadyPooled(go);
    }

    public void RegisterClientHandlers()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError($"{nameof(ObjectPool)}: bulletPrefab is not assigned.");
            return;
        }

        NetworkClient.RegisterPrefab(bulletPrefab, SpawnHandler, UnspawnHandler);
    }

    private GameObject SpawnHandler(Vector3 position, uint assetId)
    {
        GameObject go = null;
        while (pooledObjects.Count > 0 && go == null)
        {
            go = PopInternal();
            if (go == null)
                continue;

            // Safety: don't reuse an object that the client still considers spawned.
            if (go.TryGetComponent(out NetworkIdentity identity) &&
                identity.netId != 0 &&
                NetworkClient.spawned.ContainsKey(identity.netId))
            {
                go = null;
            }
        }

        if (go == null)
            go = Instantiate(bulletPrefab);

        go.transform.position = position;
        go.SetActive(true);
        return go;
    }

    private void UnspawnHandler(GameObject spawned)
    {
        if (spawned == null) return;
        spawned.SetActive(false);
        PushIfNotAlreadyPooled(spawned);
    }

    private GameObject PopInternal()
    {
        var go = pooledObjects.Pop();
        if (go != null)
            pooledInstanceIds.Remove(go.GetInstanceID());
        return go;
    }

    private void PushIfNotAlreadyPooled(GameObject go)
    {
        if (go == null) return;

        int id = go.GetInstanceID();
        if (!pooledInstanceIds.Add(id))
            return;

        pooledObjects.Push(go);
    }
}
