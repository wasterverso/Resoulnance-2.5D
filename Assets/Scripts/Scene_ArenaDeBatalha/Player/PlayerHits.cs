using Resoulnance.Cartas;
using Resoulnance.Scene_Arena;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHits : MonoBehaviour
{
    [Header("Refs int")]
    [SerializeField] private Transform instanceTransform;

    private Dictionary<int, List<GameObject>> objetosPool = new Dictionary<int, List<GameObject>>();

    public void InstanciarHitsCartas(int idCarta, Vector3 rotation = default)
    {
        Carta carta = ArenaReferences.Instance.cartasData.cartas.Find(c => c.id == idCarta);
        if (carta != null)
        {
            VerificarHit(carta.hitPrefab, rotation);
        }
    }

    public void VerificarHit(GameObject prefab, Vector3 rotation)
    {
        int idHit = prefab.GetInstanceID();

        if (!objetosPool.ContainsKey(idHit))
        {
            objetosPool[idHit] = new List<GameObject>();
        }

        GameObject obj = objetosPool[idHit].Find(o => !o.activeSelf);

        if (obj != null)
        {
            obj.SetActive(true);
            obj.transform.position = instanceTransform.position;
            obj.transform.rotation = Quaternion.Euler(rotation);
        }
        else
        {
            Instanciar(prefab, rotation, idHit);
        }
    }

    void Instanciar(GameObject prefab, Vector3 rotation, int idHit)
    {
        GameObject novoObj = Instantiate(prefab, instanceTransform);
        novoObj.transform.localRotation = Quaternion.Euler(rotation);
        objetosPool[idHit].Add(novoObj);
    }
}
