using PurrNet.Prediction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resoulnance.Cartas
{
    [CreateAssetMenu(fileName = "NewCardData", menuName = "Listas Globais/Cartas Data")]
    public class Cartas_Data : ScriptableObject
    {
        public List<Carta> cartas = new List<Carta>();
    }


    [System.Serializable]
    public class Carta
    {
        [Header("Nome")]
        public string NomeCarta;
        public int id;
        public CartaAtivaPassiva cartaAtiva;

        [Header("Permissoes")]
        public bool podeComprar = true;
        public bool podeColocarDeck = true;

        [Header("Sprites")]
        public Sprite splashSprite;

        [Header("Infos")]
        public float tempoRecarga;
        public EfeitosCarta efeitosCarta;
        public string acao;

        [Header("Atributos")]
        public Tipagem tipo;
        public float valorAtributo1;
        public float valorAtributo2;

        [Header("Controle de Instancia")]
        public bool arrastar = false;
        public bool in_Mapa = false;
        public bool In_Player = false;

        [Header("Efeito Procurar No Range")]
        public bool alvoNoRange = false;
        [Tooltip("O tamanho da imagem: 500x500")]
        public float scalaDoCircle;
        public float rangeProcurar;

        [Header("Prefabs")]
        public GameObject prefab;
        public GameObject hitPrefab;
    }

    [System.Serializable]
    public class EfeitosCarta
    {
        [Header("Alvo")]
        public TipoAlvoFinal Alvo;

        [Header("Tipos")]
        public float dano;
        public float danoReal;
        public float vida;
        public float escudo;
        public float velocidade;
        public float controle;
        public float imunidade;
    }

}
