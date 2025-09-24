using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Resoulnance.Cartas;

public class ListaCartas : MonoBehaviour
{
    public static ListaCartas Instance;

    [Header("Controle De dados")]
    public Cartas_Data cartasData;
    public Icons_Data iconsData;

    [Header("Minhas Cartas")]
    public List<Carta> minhasCartas = new List<Carta>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*
    public async void SalvarCartas()
    {
        List<int> ids = minhasCartas.Select(carta => carta.id).ToList();
        string idsString = string.Join(",", ids);
        var playerData = new Dictionary<string, object> { { "MinhasCartas", idsString } };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
            Debug.Log($"Cartas salvas: {string.Join(',', playerData)}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao salvar cartas: {e.Message}");
        }
    }

    public async Task CarregarCartas()
    {
        if (minhasCartas.Count != 0) return;

        minhasCartas.Clear();
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "MinhasCartas" });

        if (playerData.ContainsKey("MinhasCartas"))
        {
            string idsString = playerData["MinhasCartas"].Value.GetAs<string>();

            if (!string.IsNullOrEmpty(idsString))
            {
                // Se os dados da nuvem existirem, carrega as cartas salvas
                string[] idsArray = idsString.Split(',');
                foreach (string idString in idsArray)
                {
                    if (int.TryParse(idString, out int id))
                    {
                        Carta carta = cartasData.cartas.Find(c => c.id == id);
                        if (carta != null)
                        {
                            minhasCartas.Add(carta);
                        }
                    }
                }
            }
        }
    }
    */
}


