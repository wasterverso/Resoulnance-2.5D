using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using UnityEngine;

namespace Resoulnance.AvatarCustomization
{
    public class ListAvatarCustom : MonoBehaviour
    {
        public static ListAvatarCustom Instance;

        [Header("Controle De dados")]
        public AvatarCustom_Data customData;

        [Header("Player Custom")]
        public AvatarCustom avatarCustom { private set; get; } = null;

        public event Action<AvatarCustom> OnAvatarCustomAtualizado;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public AvatarCustom NovoAvatarCustom()
        {
            AvatarCustom novoAvat = new AvatarCustom();

            novoAvat.acessorios = customData.acessorios[0];
            novoAvat.cabelos = customData.cabelos[0];
            novoAvat.rostos = customData.rostos[0];
            novoAvat.corpo = customData.corpo[0];
            novoAvat.roupas = customData.roupas[0];
            novoAvat.pes = customData.pes[0];

            novoAvat.cabelos.cor = Color.white;
            novoAvat.corpo.cor = Color.white;
            novoAvat.pes.cor = Color.white;

            return novoAvat;
        }

        public void SalvarAvatarCustom(AvatarCustom avatCustom, bool salvarNoCloud)
        {
            if (avatCustom == null)
            {
                Debug.LogError("playerCustom é nulo!");
                return;
            }

            avatarCustom = avatCustom;

            OnAvatarCustomAtualizado?.Invoke(avatCustom);

            if (salvarNoCloud)
                SalvarAvatarNoCloud(avatCustom);
        }

        public async void SalvarAvatarNoCloud(AvatarCustom avatCustom)
        {
            AvatarCustomSerializable avatarSerializable = SerializarAvatarCustom();
            string json = JsonUtility.ToJson(avatarSerializable, true);
            var playerData = new Dictionary<string, object> { { "AvatarCustomData", json } };

            try
            {
                await CloudSaveService.Instance.Data.Player.SaveAsync(playerData, new SaveOptions(new PublicWriteAccessClassOptions()));
                Debug.Log($"Avatar custom data saved: {string.Join(", ", playerData)}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao salvar player custom data: {e.Message}");
            }
        }

        public async Task CarregarAvatarCloud()
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "AvatarCustomData" }, new LoadOptions(new PublicReadAccessClassOptions()));

            if (playerData.ContainsKey("AvatarCustomData"))
            {
                string json = playerData["AvatarCustomData"].Value.GetAs<string>();
                avatarCustom = DesserializarAvatarCustom(json);

                OnAvatarCustomAtualizado?.Invoke(avatarCustom);
            }
            else
            {
                Debug.Log("[ListAvatarCustom] Player não tem avatar no cloud");
            }
        }

        public AvatarCustomSerializable SerializarAvatarCustom()
        {
            AvatarCustom playerCustom = avatarCustom;

            if (playerCustom == null)
            {
                Debug.LogError("Falha ao carregar os dados do player custom.");
                return null;
            }

            // Converte as cores de UnityEngine.Color para string
            string cabelosCor = ColorUtility.ToHtmlStringRGBA(playerCustom.cabelos.cor);
            string corpoCor = ColorUtility.ToHtmlStringRGBA(playerCustom.corpo.cor);
            string pesCor = ColorUtility.ToHtmlStringRGBA(playerCustom.pes.cor);

            // Cria uma nova instância de PlayerCustomData_SaveLobby e atribui os dados
            AvatarCustomSerializable playerCustomDataSaveLobby = new AvatarCustomSerializable
            {
                acessoriosId = playerCustom.acessorios.id,
                cabelosId = playerCustom.cabelos.id,
                cabelosCor = cabelosCor,
                rostosId = playerCustom.rostos.id,
                corpoId = playerCustom.corpo.id,
                corpoCor = corpoCor,
                roupasId = playerCustom.roupas.id,
                pesId = playerCustom.pes.id,
                pesCor = pesCor
            };

            return playerCustomDataSaveLobby;
        }

        public AvatarCustom DesserializarAvatarCustom(string customDataJson)
        {
            if (string.IsNullOrEmpty(customDataJson))
            {
                Debug.LogError("Dados customizados estão vazios ou nulos.");
                return null;
            }

            // Desserializa a string JSON para PlayerCustomData_SaveLobby
            AvatarCustomSerializable data = JsonUtility.FromJson<AvatarCustomSerializable>(customDataJson);

            // Cria uma nova instância de PlayerCustom e atribui os dados desserializados
            AvatarCustom playerCustom = new AvatarCustom
            {
                acessorios = EncontrarItemPorId(customData.acessorios, data.acessoriosId),
                cabelos = EncontrarItemPorId(customData.cabelos, data.cabelosId),
                rostos = EncontrarItemPorId(customData.rostos, data.rostosId),
                corpo = EncontrarItemPorId(customData.corpo, data.corpoId),
                roupas = EncontrarItemPorId(customData.roupas, data.roupasId),
                pes = EncontrarItemPorId(customData.pes, data.pesId)
            };

            // Converte as strings de volta para UnityEngine.Color
            if (ColorUtility.TryParseHtmlString("#" + data.cabelosCor, out Color cabelosCor))
            {
                playerCustom.cabelos.cor = cabelosCor;
            }
            if (ColorUtility.TryParseHtmlString("#" + data.corpoCor, out Color corpoCor))
            {
                playerCustom.corpo.cor = corpoCor;
            }
            if (ColorUtility.TryParseHtmlString("#" + data.pesCor, out Color pesCor))
            {
                playerCustom.pes.cor = pesCor;
            }

            return playerCustom;
        }

        private Item_AvatarCustom EncontrarItemPorId(List<Item_AvatarCustom> lista, int id)
        {
            return lista.Find(item => item.id == id);
        }
    }
}

