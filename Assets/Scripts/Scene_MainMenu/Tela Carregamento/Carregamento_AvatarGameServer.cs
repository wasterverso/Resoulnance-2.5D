using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resoulnance.AvatarCustomization;
using System;
using UnityEngine;

public class Carregamento_AvatarGameServer : MonoBehaviour
{
    [Serializable]
    class PlayerLobbyConfig
    {
        public string avatar;   // string JSON
        public string idAuth;
        public bool isReady;
        public int posicao;
        public string nickname;

        // Retorna o avatar já desserializado
        public AvatarCustomSerializable GetAvatar()
        {
            return JsonConvert.DeserializeObject<AvatarCustomSerializable>(avatar);
        }
    }

    [SerializeField] AtribuirDados_AvatarCustom[] listAvatar = new AtribuirDados_AvatarCustom[0];

    public void LoadAvatars(string json)
    {
        JObject root = JObject.Parse(json);

        // Navega até "custom_data.teams"
        var teams = root["custom_data"]["teams"];
        int index = 0;

        foreach (var team in teams)
        {
            foreach (var lobby in team["lobbies"])
            {
                var playerStates = lobby["player_states"];

                foreach (var player in playerStates.Children<JProperty>())
                {
                    // Converte o estado do player
                    PlayerLobbyConfig state = player.Value.ToObject<PlayerLobbyConfig>();

                    AvatarCustom avatarCustom = ListAvatarCustom.Instance.DesserializarAvatarCustom(state.avatar);

                    if (index < listAvatar.Length && listAvatar[index] != null)
                    {
                        listAvatar[index].AtribuirDados(avatarCustom);
                        Debug.Log($"Avatar aplicado no slot {index} -> {state.nickname}");
                    }

                    index++;
                }
            }
        }
    }
}
