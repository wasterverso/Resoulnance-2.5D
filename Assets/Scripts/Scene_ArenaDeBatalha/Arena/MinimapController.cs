using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MinimapController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera mapCamera;

    [Header("Limites do Mapa")]
    [SerializeField] private float limiteMapEsquerdo = -66f;
    [SerializeField] private float limiteMapDireito = 66f;
    [SerializeField] private float limiteMapFrente = 16f;
    [SerializeField] private float limiteMapTras = -30f;

    [Header("Configuração da câmera")]
    [SerializeField] private float cameraAltura = 7f;

    private bool estaMostrandoMapCamera = false;

    private void Start()
    {
        if (mapCamera != null)
        {
            mapCamera.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AtualizarPosicaoCamera(eventData);

        if (mapCamera != null)
        {
            estaMostrandoMapCamera = true;
            mapCamera.gameObject.SetActive(true);
            mainCamera.gameObject.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (estaMostrandoMapCamera)
        {
            AtualizarPosicaoCamera(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (mapCamera != null && estaMostrandoMapCamera)
        {
            estaMostrandoMapCamera = false;
            mapCamera.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
        }
    }

    private void AtualizarPosicaoCamera(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            // Normalizar de -0.5..0.5 para 0..1
            float normalizedX = (localPoint.x / minimapRect.rect.width) + 0.5f;
            float normalizedY = (localPoint.y / minimapRect.rect.height) + 0.5f;

            // Mapear para os limites do mundo
            float worldX = Mathf.Lerp(limiteMapEsquerdo, limiteMapDireito, normalizedX);
            float worldZ = Mathf.Lerp(limiteMapTras, limiteMapFrente, normalizedY);

            // Atualizar posição da câmera
            mapCamera.transform.position = new Vector3(worldX, cameraAltura, worldZ);
        }
    }
}
