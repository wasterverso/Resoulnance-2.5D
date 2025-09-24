using System.Collections.Generic;
using UnityEngine;

namespace Resoulnance.Flyers
{
    [CreateAssetMenu(fileName = "NewFlyerListData", menuName = "Listas Globais/Flyer List Data")]
    public class Flyer_Data : ScriptableObject
    {
        [Header("Personagens")]
        public List<Personagem_Data> personagens = new List<Personagem_Data>();
    }

    [System.Serializable]
    public class Skin
    {
        public string nome;
        public int id;
        public Sprite SplashSprite;
        public Sprite painelSprite;
        public Sprite quadroSprite;
        public GameObject prefab_UI;
        public AnimatorOverrideController skinOverrideController;

        [Header("Arma")]
        public AnimatorOverrideController armaOverrideController;
        public GameObject hitArma;

        [Header("Loja")]
        public int cristaisValor;
        public bool evento = false;
    }
}



