using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string nome;
        public GameObject prefab;
        public int quantidadeInicial = 4;
        public Transform parent;

        [HideInInspector] public Queue<GameObject> objetos = new();
    }

    [SerializeField] private List<Pool> pools = new();

    private void Awake()
    {
        // Implementa Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Inicializa as pools
        foreach (var pool in pools)
        {
            for (int i = 0; i < pool.quantidadeInicial; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.parent);
                obj.SetActive(false);
                pool.objetos.Enqueue(obj);
            }
        }
    }

    /// <summary>
    /// Pega um objeto da pool pelo nome
    /// </summary>
    public GameObject Spawn(string nome, Vector3 pos, Quaternion rot)
    {
        Pool pool = pools.Find(p => p.nome == nome);
        if (pool == null)
        {
            Debug.LogWarning("Pool não encontrada: " + nome);
            return null;
        }

        GameObject obj;

        if (pool.objetos.Count > 0)
        {
            obj = pool.objetos.Dequeue();
        }
        else
        {
            obj = Instantiate(pool.prefab, pool.parent);
        }

        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Retorna o objeto para a pool
    /// </summary>
    public void Despawn(string nome, GameObject obj)
    {
        obj.SetActive(false);
        Pool pool = pools.Find(p => p.nome == nome);
        if (pool != null)
        {
            pool.objetos.Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}
