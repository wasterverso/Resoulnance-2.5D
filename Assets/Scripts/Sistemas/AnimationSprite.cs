using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimationSprite : MonoBehaviour
{
    [Header("Tipo De Componente")]
    public bool image = false;
    public bool spriteRenderer = false;

    [Header("Controle")]
    public Sprite[] frames;
    public float framesPerSecond = 6f;
    
    private SpriteRenderer sprite_sprite;
    private Image image_img;

    private void OnEnable()
    {
        if (image)
        {
            image_img = GetComponent<Image>();
            StartCoroutine(Animate());
        }

        if (spriteRenderer)
        {
            sprite_sprite = GetComponent<SpriteRenderer>();
            StartCoroutine(AnimateSprite());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Animate()
    {
        while (true)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                image_img.sprite = frames[i];
                yield return new WaitForSeconds(1.0f / framesPerSecond);
            }
        }
    }

    IEnumerator AnimateSprite()
    {
        while (true)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                sprite_sprite.sprite = frames[i];
                yield return new WaitForSeconds(1.0f / framesPerSecond);
            }
        }
    }
}
