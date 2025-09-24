using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resoulnance.AvatarCustomization
{
    [CreateAssetMenu(fileName = "NewCustomData", menuName = "Listas Globais/Player Custom Data")]
    public class AvatarCustom_Data : ScriptableObject
    {
        public List<Item_AvatarCustom> acessorios = new List<Item_AvatarCustom>();
        public List<Item_AvatarCustom> cabelos = new List<Item_AvatarCustom>();
        public List<Item_AvatarCustom> rostos = new List<Item_AvatarCustom>();
        public List<Item_AvatarCustom> corpo = new List<Item_AvatarCustom>();
        public List<Item_AvatarCustom> roupas = new List<Item_AvatarCustom>();
        public List<Item_AvatarCustom> pes = new List<Item_AvatarCustom>();
    }

    [System.Serializable]
    public class Item_AvatarCustom
    {
        [Header("Controles")]
        public int id;
        public Color cor = Color.white;
        public Sprite padrao_Sprite;

        [Header("Frames")]
        public List<Sprite> frames = new List<Sprite>();
    }

    [System.Serializable]
    public class AvatarCustom
    {
        public Item_AvatarCustom acessorios;
        public Item_AvatarCustom cabelos;
        public Item_AvatarCustom rostos;
        public Item_AvatarCustom corpo;
        public Item_AvatarCustom roupas;
        public Item_AvatarCustom pes;
    }

    [System.Serializable]
    public class AvatarCustomSerializable
    {
        public int acessoriosId;
        public int cabelosId;
        public string cabelosCor;
        public int rostosId;
        public int corpoId;
        public string corpoCor;
        public int roupasId;
        public int pesId;
        public string pesCor;
    }
}
