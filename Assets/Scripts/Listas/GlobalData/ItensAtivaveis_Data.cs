
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Listas Globais/Itens Ativaveis Data")]
public class ItensAtivaveis_Data : ScriptableObject
{
    public List<ItemAtivavel> itensAtivaveis = new List<ItemAtivavel>();
}


[System.Serializable]
public class ItemAtivavel
{
    public string nomeItem;
    public int id;
    public string descricao;
    public Sprite iconSprite;
    public int tempoCowntDown;
    public bool arrastar = false;
    public bool verificarRange = false;
}