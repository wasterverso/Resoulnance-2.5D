using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Resoulnance.Cartas;

namespace Resoulnance.Flyers
{
    public class ListaFlyers : MonoBehaviour
    {
        public static ListaFlyers Instance;

        [Header("Controle De dados")]
        public Flyer_Data flyerData;
        [SerializeField] Cartas_Data cartasData;
        [SerializeField] ItensAtivaveis_Data itensData;

        [Header("Meus Personagens")]
        public List<Personagem_Data> meusPersonagens = new List<Personagem_Data>();

        public Personagem_Data flyerEscolhido;


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
        public async void SalvarPersonagens()
        {
            SavePersonagemData saveData = new SavePersonagemData();

            foreach (var personagem in meusPersonagens)
            {
                PersonagemSaveData personagemSaveData = new PersonagemSaveData();
                personagemSaveData.id = personagem.id;
                personagemSaveData.deckAtual = personagem.deckAtual;
                personagemSaveData.usarPadrao = personagem.usarPadrao;

                foreach (var deck in personagem.decks)
                {
                    DeckSaveData deckSaveData = new DeckSaveData();
                    deckSaveData.name = deck.nomeDeck;
                    deckSaveData.itemId = deck.itemAtivavel.id;

                    foreach (var carta in deck.cartas)
                    {
                        deckSaveData.cartaIds.Add(carta.id);
                    }

                    personagemSaveData.decks.Add(deckSaveData);
                }

                saveData.personagens.Add(personagemSaveData);
            }

            string json = JsonUtility.ToJson(saveData, true);
            var playerData = new Dictionary<string, object> { { "MeusPersonagens", json } };

            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
                Debug.Log($"Dados dos personagens salvos: {string.Join(',', playerData)}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao salvar dados dos personagens: {e.Message}");
            }
        }

        public async Task CarregarPersonagens()
        {
            if (meusPersonagens.Count != 0) return;

            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "MeusPersonagens" });

            if (playerData.ContainsKey("MeusPersonagens"))
            {
                string json = playerData["MeusPersonagens"].Value.GetAs<string>();
                SavePersonagemData saveData = JsonUtility.FromJson<SavePersonagemData>(json);

                meusPersonagens.Clear();
                foreach (var personagemSaveData in saveData.personagens)
                {
                    Personagem_Data novoPersona = flyerData.personagens[personagemSaveData.id];

                    novoPersona.decks.Clear();

                    for (int i = 0; i < 3; i++)
                    {
                        Deck deck = new Deck();
                        if (i < personagemSaveData.decks.Count)
                        {
                            DeckSaveData deckSaveData = personagemSaveData.decks[i];
                            deck.nomeDeck = string.IsNullOrEmpty(deckSaveData.name) ? $"Deck {i + 1}" : deckSaveData.name;
                            deck.itemAtivavel = itensData.itensAtivaveis[deckSaveData.itemId];

                            foreach (var cartaId in deckSaveData.cartaIds)
                            {
                                Carta carta = cartasData.cartas.Find(c => c.id == cartaId);
                                if (carta != null)
                                {
                                    deck.cartas.Add(carta);
                                }
                            }
                        }
                        else
                        {
                            deck.nomeDeck = $"Deck {i + 1}";
                            deck.itemAtivavel = itensData.itensAtivaveis[0];
                        }

                        novoPersona.decks.Add(deck);
                    }

                    meusPersonagens.Add(novoPersona);
                }
            }
            else
            {
                meusPersonagens.Add(flyerData.personagens[0]);
                meusPersonagens.Add(flyerData.personagens[1]);
                //meusPersonagens.Add(flyerData.personagens[2]);
            }
        } 
        */
    }
}


[System.Serializable]
public class SavePersonagemData
{
    public List<PersonagemSaveData> personagens = new List<PersonagemSaveData>();
}

[System.Serializable]
public class PersonagemSaveData
{
    public int id;
    public int deckAtual;
    public bool usarPadrao;
    public List<DeckSaveData> decks = new List<DeckSaveData>();
}

[System.Serializable]
public class DeckSaveData
{
    public string name;
    public List<int> cartaIds = new List<int>();
    public int itemId;
}
