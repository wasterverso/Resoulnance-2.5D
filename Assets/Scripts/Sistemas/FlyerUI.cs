using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyerUI : MonoBehaviour
{
    public Image sombra_img;
    public Image splash_img;
    public Image skin_img;
    public Image arma_img;
    [SerializeField] AnimationSprite animSpriteScript;

    public void AtivarSplash(bool ativar)
    {
        splash_img.gameObject.SetActive(ativar);
    }
}
