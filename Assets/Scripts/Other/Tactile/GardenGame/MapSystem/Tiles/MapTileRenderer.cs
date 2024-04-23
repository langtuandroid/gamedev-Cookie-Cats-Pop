using System;
using System.Collections.Generic;
using System.Text;
using TactileModules.GameCore.StreamingAssets;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Tiles
{
    public class MapTileRenderer
    {
        public MapTileRenderer(MapTileDatabase database, Vector2 quadSize, Vector2 origin, Transform parent = null)
        {
            this.database = database;
            this.quadSize = quadSize;
            this.origin = origin;
            this.InitializePool();
            this.textureNameBuilder = new StringBuilder();
            this.renderedQuads = new GameObject[32, 32];
            this.quadToMaterial = new Dictionary<GameObject, MaterialPropertyBlock>();
            this.prevStartX = -1;
            this.prevStartY = -1;
            this.prevEndX = -1;
            this.prevEndY = -1;
            this.parent = parent;
        }

        public void Destroy()
        {
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    this.ClearQuad(i, j);
                }
            }
            this.pool.Clear();
            UnityEngine.Object.Destroy(this.quad);
        }

        private void InitializePool()
        {
            this.quad = this.CreateQuad("Tile", this.parent, this.quadSize * 0.5f, this.quadSize, null);
            this.quad.SetLayerRecursively(19);
            this.quad.hideFlags = HideFlags.HideAndDontSave;
            this.quad.SetActive(false);
            this.pool = new MapTileRenderer.GameObjectPool(this.quad, 4, delegate (GameObject go)
            {
                if (this.parent != null)
                {
                    go.transform.parent = this.parent;
                }
                go.GetComponent<MeshRenderer>().sharedMaterial = GardenGameSetup.Get.tileMaterial;
                this.quadToMaterial.Add(go, new MaterialPropertyBlock());
            });
        }

        public void Render(Camera camera)
        {
            Vector2 vector;
            Vector2 vector2;
            this.GetRenderArea(camera, out vector, out vector2);
            int num = Mathf.FloorToInt(vector.x / this.quadSize.x);
            int num2 = Mathf.FloorToInt(vector.y / this.quadSize.y);
            int num3 = Mathf.FloorToInt(vector2.x / this.quadSize.x);
            int num4 = Mathf.FloorToInt(vector2.y / this.quadSize.y);
            for (int i = this.prevStartY; i <= this.prevEndY; i++)
            {
                for (int j = this.prevStartX; j <= this.prevEndX; j++)
                {
                    if (j < num || i < num2 || j > num3 || i > num4)
                    {
                        this.ClearQuad(j, i);
                    }
                }
            }
            for (int k = num2; k <= num4; k++)
            {
                for (int l = num; l <= num3; l++)
                {
                    if (l < this.prevStartX || k < this.prevStartY || l > this.prevEndX || k > this.prevEndY)
                    {
                        this.RenderQuad(l, k);
                    }
                }
            }
            this.prevStartX = num;
            this.prevStartY = num2;
            this.prevEndX = num3;
            this.prevEndY = num4;
        }

        private Vector3 GetPosition(int x, int y)
        {
            return (Vector3)this.origin + new Vector3((float)x * this.quadSize.x, (float)y * this.quadSize.y, 0f);
        }

        private bool RenderQuad(int x, int y)
        {
            if (x < 0)
            {
                return false;
            }
            if (y < 0)
            {
                return false;
            }
            if (x >= 32)
            {
                return false;
            }
            if (y >= 32)
            {
                return false;
            }
            StreamingAsset textureAsset = this.database.GetTexture(x, y);
            if (textureAsset == null)
            {
                return false;
            }
            GameObject quad = this.pool.Instantiate();
            quad.transform.localPosition = this.GetPosition(x, y);
            this.renderedQuads[x, y] = quad;
            textureAsset.AssetChanged = delegate ()
            {
                if (textureAsset.Asset != null)
                {
                    MaterialPropertyBlock materialPropertyBlock = this.quadToMaterial[quad];
                    materialPropertyBlock.SetTexture("_MainTex", textureAsset.GetAsset<Texture2D>());
                    quad.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
                }
            };
            textureAsset.ForceUseLocal = true;
            textureAsset.Load();
            return true;
        }

        private void ClearQuad(int x, int y)
        {
            if (x < 0)
            {
                return;
            }
            if (y < 0)
            {
                return;
            }
            if (x >= this.renderedQuads.Length)
            {
                return;
            }
            if (y >= this.renderedQuads.Length)
            {
                return;
            }
            if (this.renderedQuads[x, y] == null)
            {
                return;
            }
            this.pool.Destroy(this.renderedQuads[x, y]);
            this.renderedQuads[x, y] = null;
            StreamingAsset texture = this.database.GetTexture(x, y);
            if (texture != null)
            {
                texture.AssetChanged = null;
                texture.Unload();
            }
        }

        private void GetRenderArea(Camera camera, out Vector2 min, out Vector2 max)
        {
            min = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
            max = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
            min -= this.origin;
            max -= this.origin;
        }

        private GameObject CreateQuad(string name, Transform parent, Vector2 quadPosition, Vector2 quadSize, Texture texture)
        {
            Vector2 vector = quadSize * 0.5f;
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(quadPosition.x - vector.x, quadPosition.y + vector.y, 0f),
                new Vector3(quadPosition.x + vector.x, quadPosition.y + vector.y, 0f),
                new Vector3(quadPosition.x + vector.x, quadPosition.y - vector.y, 0f),
                new Vector3(quadPosition.x - vector.x, quadPosition.y - vector.y, 0f)
            };
            mesh.triangles = new int[]
            {
                0,
                1,
                2,
                0,
                2,
                3
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f)
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f)
            };
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            return gameObject;
        }

        public bool Visible
        {
            set
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if (this.renderedQuads[i, j] != null)
                        {
                            this.renderedQuads[i, j].SetActive(value);
                        }
                    }
                }
            }
        }

        private const int GRID_SIZE = 32;

        private GameObject quad;

        private Vector2 quadSize;

        private MapTileRenderer.GameObjectPool pool;

        private readonly StringBuilder textureNameBuilder;

        private readonly GameObject[,] renderedQuads;

        private int prevStartX;

        private int prevStartY;

        private int prevEndX;

        private int prevEndY;

        private readonly Dictionary<GameObject, MaterialPropertyBlock> quadToMaterial;

        private Transform parent;

        private readonly MapTileDatabase database;

        private readonly Vector2 origin;

        public class GameObjectPool
        {
            public GameObjectPool(GameObject prefab, int batchSize, Action<GameObject> onAlloc)
            {
                this.prefab = prefab;
                this.batchSize = batchSize;
                this.instances = new List<GameObject>();
                this.onAlloc = onAlloc;
            }

            public void Alloc(int amount)
            {
                for (int i = 0; i < amount; i++)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
                    this.onAlloc(gameObject);
                    this.Destroy(gameObject);
                }
            }

            public GameObject Instantiate()
            {
                if (this.instances.Count == 0)
                {
                    this.Alloc(this.batchSize);
                }
                int index = this.instances.Count - 1;
                GameObject gameObject = this.instances[index];
                this.instances.RemoveAt(index);
                gameObject.SetActive(true);
                gameObject.transform.localScale = Vector3.one;
                return gameObject;
            }

            public void Destroy(GameObject instance)
            {
                instance.SetActive(false);
                instance.transform.SetParent(null, false);
                this.instances.Add(instance);
            }

            public void Clear()
            {
                for (int i = 0; i < this.instances.Count; i++)
                {
                    UnityEngine.Object.Destroy(this.instances[i]);
                }
                this.instances.Clear();
            }

            public List<GameObject> Instances
            {
                get
                {
                    return this.instances;
                }
            }

            private GameObject prefab;

            private int batchSize;

            private List<GameObject> instances;

            private Action<GameObject> onAlloc;
        }
    }
}
