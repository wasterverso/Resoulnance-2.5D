using PurrNet;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ping_Display : MonoBehaviour
{
    NetworkManager networkManager;

    [Header("UI")]
    [SerializeField] Text ping_txt;
    [SerializeField] Image ping_img;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.LinuxServer) return;

        networkManager = NetworkManager.main;
        networkManager.onLocalPlayerReceivedID += IniciarAtuaLizarPing;
    }

    void IniciarAtuaLizarPing(PlayerID player)
    {
        StartCoroutine(AtualizarPing());
    }

    private IEnumerator AtualizarPing()
    {
        StatisticsManager statisticsManager = NetworkManager.main.GetComponent<StatisticsManager>();
        
        while (true)
        {
            int statusPing = statisticsManager.ping;
            ping_txt.text = $"{statusPing} ms";

            if (statusPing < 100)
            {
                ping_txt.color = Color.green;
                ping_img.color = Color.green;
            }
            else if (statusPing < 200)
            {
                ping_txt.color = Color.yellow;
                ping_img.color = Color.yellow;
            }
            else
            {
                ping_txt.color = Color.red;
                ping_img.color = Color.red;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnDestroy()
    {
        if (Application.platform == RuntimePlatform.LinuxServer) return;

        networkManager.onLocalPlayerReceivedID -= IniciarAtuaLizarPing;
    }
}
