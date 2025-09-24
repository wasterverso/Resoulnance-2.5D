using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resoulnance.Flyers
{
    [CreateAssetMenu(fileName = "NewPersonagemData", menuName = "Listas Globais/Personagem Data")]
    public class Personagem_Data : ScriptableObject
    {
        [Header("Name/Id")]
        public string nome;
        public int id;

        [Header("Disponibilidade")]
        public bool Ativo = false;
        public bool podeComprar = false;

        [Header("Info Dados")]
        public Classes classe;
        public int vida;
        public int escudo;
        public float velocidade;
        public int cargasDash;
        public Tipagem Afinidade1;
        public Tipagem Afinidade2;
        public Tipagem NaoAfinidade1;
        public Tipagem NaoAfinidade2;

        [Header("Atk Basico")]
        public bool isRanged = false;
        public float VelDeAtk;
        public int danoAtk;
        public float rangeAtk;

        [Header("Suprema")]
        public Sprite supremaIcon;
        public float recargaSuprema;
        public bool arrastarSuprema;

        [Header("Descrições")]
        public string passivaDesc;
        public string supremaDesc;

        [Header("Imagens")]
        public Sprite quadradoSprite;
        public Sprite kdaSprite;

        [Header("Loja")]
        public int moedasValor;
        public int cristaisValor;

        [Header("Decks")]
        public Deck deckFavorito;
        public List<Deck> decks = new List<Deck>(3);

        [Header("Skins")]
        public List<Skin> skins = new List<Skin>(1);

        [Header("Prefabs")]
        public GameObject prefabPersonagem;
    }
}

