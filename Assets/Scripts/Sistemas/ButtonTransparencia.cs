using UnityEngine;
using UnityEngine.UI;

public class ButtonTransparencia : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
    }
}
