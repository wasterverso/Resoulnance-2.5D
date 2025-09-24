using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIconData", menuName = "Listas Globais/Icons Data")]
public class Icons_Data : ScriptableObject
{
    public List<SpriteClass> spriteClasses = new List<SpriteClass>();

    public List<IconAfinidade> iconAfinidade = new List<IconAfinidade>();

    public List<EfeitosCarta_Icons> iconsEfeitos_Cartas = new List<EfeitosCarta_Icons>();

    public List<EfeitosCarta_Icons> iconsElos = new List<EfeitosCarta_Icons>();
}

[System.Serializable]
public class SpriteClass
{
    public string nome;
    public Classes classe_;
    public Sprite sprite;

    public SpriteClass(string nome, Classes clas, Sprite sprite)
    {
        this.nome = nome;
        this.classe_ = clas;
        this.sprite = sprite;
    }
}

[System.Serializable]
public class IconAfinidade
{
    public string tipoAfinidade;
    public Tipagem Afinidade;
    public Sprite SpriteAfinidade;
    public Sprite background;
}

[System.Serializable]
public class EfeitosCarta_Icons
{
    public string nomeIcon;
    public Sprite sprite_;
}