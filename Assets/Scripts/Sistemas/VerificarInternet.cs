using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Resoulnance.Sistema
{
    public static class VerificarInternet
    {
        public static async Task<bool> IniciarVerificacao()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("Sem conexão Wifi ou 3G, 4G, 5G.");
                return false;
            }

            using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
            {
                request.timeout = 5;
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                return request.result == UnityWebRequest.Result.Success;
            }
        }
    }
}
