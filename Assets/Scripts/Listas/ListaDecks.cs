using Resoulnance.Cartas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;


[System.Serializable]
public class Deck
{
    public string nomeDeck;
    public List<Carta> cartas = new List<Carta>();
    public ItemAtivavel itemAtivavel;

    public bool IsDefault => (cartas.Count < 3);
}

public class ListaDecks : MonoBehaviour
{
    public static ListaDecks Instance;

    [Header("Controle De dados")]
    public Cartas_Data cartasData;
    public ItensAtivaveis_Data itensData;

    [Header("Decks")]
    public List<Deck> meusDecksUniversais = new List<Deck>(3);
    public int deckAtual = 0;

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

    public async void SalvarDecks()
    {
        MeusDecksSaveData saveData = new MeusDecksSaveData();

        foreach (Deck deck in meusDecksUniversais)
        {
            List<int> novosIds = new List<int>();

            DeckSaveData deckSaveData = new DeckSaveData();
            deckSaveData.name = deck.nomeDeck;
            deckSaveData.itemId = deck.itemAtivavel.id;

            foreach (var ids in deck.cartas)
            {
                novosIds.Add(ids.id);
            }

            deckSaveData.cartaIds = novosIds;
            saveData.myDecks.Add(deckSaveData);
        }

        string json = JsonUtility.ToJson(saveData, true);
        var playerData = new Dictionary<string, object> { { "MeusDecks", json } };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
            Debug.Log($"Decks salvos com sucesso!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao salvar decks: {e.Message}");
        }
    }

    public async Task CarregarDecks()
    {
        if (meusDecksUniversais.Count != 0) return;

        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "MeusDecks" });

        if (playerData.ContainsKey("MeusDecks"))
        {
            string json = playerData["MeusDecks"].Value.GetAs<string>();
            MeusDecksSaveData saveData = JsonUtility.FromJson<MeusDecksSaveData>(json);

            meusDecksUniversais.Clear();
            foreach (var deckSaveData in saveData.myDecks)
            {
                Deck deck = new Deck
                {
                    nomeDeck = deckSaveData.name,
                    cartas = new List<Carta>(),
                    itemAtivavel = itensData.itensAtivaveis[deckSaveData.itemId]
                };

                foreach (var cartaId in deckSaveData.cartaIds)
                {
                    Carta carta = cartasData.cartas.Find(c => c.id == cartaId);
                    if (carta != null)
                    {
                        deck.cartas.Add(carta);
                    }
                }

                meusDecksUniversais.Add(deck);
            }
        }

        while (meusDecksUniversais.Count < 3)
        {
            Deck deck = new Deck
            {
                nomeDeck = $"Deck {meusDecksUniversais.Count + 1}",
                cartas = new List<Carta>(),
                itemAtivavel = itensData.itensAtivaveis[0]
            };
            meusDecksUniversais.Add(deck);
        }
    }
}

[System.Serializable]
public class MeusDecksSaveData
{
    public List<DeckSaveData> myDecks = new List<DeckSaveData>();
}
