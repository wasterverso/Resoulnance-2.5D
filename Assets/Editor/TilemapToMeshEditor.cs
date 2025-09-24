using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapToNavMeshMesh : MonoBehaviour
{
    [MenuItem("Tools/Tilemap/Generate NavMesh Mesh (Full)")]
    public static void GenerateNavMeshMesh()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Selecione um Tilemap!");
            return;
        }

        GameObject go = Selection.activeGameObject;
        Tilemap tilemap = go.GetComponent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogWarning("O objeto selecionado não tem Tilemap!");
            return;
        }

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        BoundsInt bounds = tilemap.cellBounds;
        Vector3 cellSize = tilemap.layoutGrid.cellSize;

        // Marca tiles processados
        bool[,] processed = new bool[bounds.size.x, bounds.size.y];

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                int bx = x - bounds.xMin;
                int by = y - bounds.yMin;

                if (processed[bx, by]) continue;

                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cellPos);
                if (tile == null) continue;

                // Expande horizontalmente
                int width = 1;
                while (x + width < bounds.xMax)
                {
                    TileBase nextTile = tilemap.GetTile(new Vector3Int(x + width, y, 0));
                    if (nextTile == null) break;
                    width++;
                }

                // Expande verticalmente
                int height = 1;
                bool canExpand;
                do
                {
                    canExpand = true;
                    for (int ix = 0; ix < width; ix++)
                    {
                        int nx = x + ix;
                        int ny = y + height;
                        if (ny >= bounds.yMax || tilemap.GetTile(new Vector3Int(nx, ny, 0)) == null)
                        {
                            canExpand = false;
                            break;
                        }
                    }
                    if (canExpand) height++;
                } while (canExpand);

                // Marca tiles processados
                for (int ix = 0; ix < width; ix++)
                    for (int iy = 0; iy < height; iy++)
                        processed[bx + ix, by + iy] = true;

                // Cria quad no plano XZ
                Vector3 worldPos = tilemap.CellToWorld(cellPos);
                float quadWidth = width * cellSize.x;
                float quadDepth = height * cellSize.y;

                // Centraliza
                worldPos += new Vector3(quadWidth / 2f, 0, quadDepth / 2f);

                int vertIndex = vertices.Count;
                vertices.Add(worldPos + new Vector3(-quadWidth / 2f, 0, -quadDepth / 2f));
                vertices.Add(worldPos + new Vector3(quadWidth / 2f, 0, -quadDepth / 2f));
                vertices.Add(worldPos + new Vector3(-quadWidth / 2f, 0, quadDepth / 2f));
                vertices.Add(worldPos + new Vector3(quadWidth / 2f, 0, quadDepth / 2f));

                triangles.Add(vertIndex + 0);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 3);
                triangles.Add(vertIndex + 1);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        // Salva mesh
        string path = "Assets/" + tilemap.name + "_NavMeshMesh.asset";
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();

        // Cria/atualiza MeshCollider
        MeshCollider collider = go.GetComponent<MeshCollider>();
        if (collider == null) collider = go.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        collider.convex = false; // importante para NavMesh
        collider.enabled = true;

        // Coloca em layer específica (exemplo: "Default", pode mudar)
        go.layer = LayerMask.NameToLayer("Default");

        Debug.Log($"Mesh para NavMesh gerada com {vertices.Count} vertices e salva em: {path}");
        Debug.Log("Lembre-se de selecionar NavMeshSurface e clicar em Bake para atualizar o NavMesh.");
    }
}
