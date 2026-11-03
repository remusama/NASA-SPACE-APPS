using UnityEngine;
using UnityEditor;
using System.IO;

namespace SpaceStation5
{
    /// <summary>
    /// Generates procedural high-detail sci-fi PBR textures and normal maps.
    /// </summary>
    public static class TextureGenerator
    {
        public static void GenerateAllTextures()
        {
            string dir = "Assets/SpaceStation5/Textures";
            if (!AssetDatabase.IsValidFolder(dir))
            {
                AssetDatabase.CreateFolder("Assets/SpaceStation5", "Textures");
            }

            GenerateHullTextures(dir);
            GenerateFloorTextures(dir);
            GenerateWallTextures(dir);
            GenerateTechScreenTexture(dir);
            GenerateSolarTextures(dir);
            GenerateSpaceSkybox(dir);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[TextureGenerator] All textures generated and imported successfully!");
        }

        private static void GenerateHullTextures(string dir)
        {
            int size = 512;
            Texture2D albedo = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Texture2D normal = new Texture2D(size, size, TextureFormat.RGBA32, true);

            Color baseWhite = new Color(0.90f, 0.91f, 0.93f);
            Color seamDark = new Color(0.25f, 0.27f, 0.30f);
            Color panelAlt = new Color(0.84f, 0.85f, 0.88f);
            Color rivetColor = new Color(0.4f, 0.42f, 0.45f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int px = x % (size / 4);
                    int py = y % (size / 4);
                    int panelX = x / (size / 4);
                    int panelY = y / (size / 4);

                    bool isBorder = (px < 4 || px >= (size / 4) - 4 || py < 4 || py >= (size / 4) - 4);
                    bool isSeam = (px < 2 || px >= (size / 4) - 2 || py < 2 || py >= (size / 4) - 2);

                    bool altPanel = ((panelX + panelY) % 2 == 0);
                    Color pColor = altPanel ? baseWhite : panelAlt;

                    float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.04f - 0.02f;
                    pColor += new Color(noise, noise, noise);

                    bool isRivet = false;
                    if ((px == 6 || px == (size / 4) - 7) && (y % 16 == 0)) isRivet = true;
                    if ((py == 6 || py == (size / 4) - 7) && (x % 16 == 0)) isRivet = true;

                    if (isSeam)
                    {
                        albedo.SetPixel(x, y, seamDark);
                        normal.SetPixel(x, y, new Color(0.5f, 0.5f, 0.7f, 1f));
                    }
                    else if (isBorder)
                    {
                        Color bevel = Color.Lerp(pColor, seamDark, 0.4f);
                        albedo.SetPixel(x, y, bevel);
                        normal.SetPixel(x, y, new Color(0.5f, 0.5f, 0.85f, 1f));
                    }
                    else if (isRivet)
                    {
                        albedo.SetPixel(x, y, rivetColor);
                        normal.SetPixel(x, y, new Color(0.55f, 0.55f, 0.95f, 1f));
                    }
                    else
                    {
                        albedo.SetPixel(x, y, pColor);
                        normal.SetPixel(x, y, new Color(0.5f, 0.5f, 1.0f, 1f));
                    }
                }
            }
            albedo.Apply();
            normal.Apply();

            SaveAndImportTexture(albedo, $"{dir}/T_Hull_Panels_Albedo.png", false);
            SaveAndImportTexture(normal, $"{dir}/T_Hull_Panels_Normal.png", true);
        }

        private static void GenerateFloorTextures(string dir)
        {
            int size = 512;
            Texture2D albedo = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Texture2D normal = new Texture2D(size, size, TextureFormat.RGBA32, true);

            Color floorBase = new Color(0.12f, 0.13f, 0.15f);
            Color floorGrid = new Color(0.20f, 0.22f, 0.25f);
            Color hazardYellow = new Color(0.95f, 0.75f, 0.10f);
            Color hazardBlack = new Color(0.08f, 0.08f, 0.08f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int dx = (x + y) % 16;
                    int dy = (x - y + size) % 16;
                    bool isDiamond = (dx < 4 && dy < 4);

                    Color c = isDiamond ? floorGrid : floorBase;

                    if (x < 32 || x > size - 32)
                    {
                        bool isStripe = ((x + y) / 16) % 2 == 0;
                        c = isStripe ? hazardYellow : hazardBlack;
                    }

                    if (x % 128 < 3 || y % 128 < 3)
                    {
                        c = new Color(0.05f, 0.05f, 0.06f);
                        normal.SetPixel(x, y, new Color(0.5f, 0.5f, 0.6f, 1f));
                    }
                    else
                    {
                        float bump = isDiamond ? 0.65f : 0.5f;
                        normal.SetPixel(x, y, new Color(bump, bump, 1f, 1f));
                    }

                    albedo.SetPixel(x, y, c);
                }
            }
            albedo.Apply();
            normal.Apply();

            SaveAndImportTexture(albedo, $"{dir}/T_Interior_Floor_Albedo.png", false);
            SaveAndImportTexture(normal, $"{dir}/T_Interior_Floor_Normal.png", true);
        }

        private static void GenerateWallTextures(string dir)
        {
            int size = 512;
            Texture2D albedo = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Texture2D normal = new Texture2D(size, size, TextureFormat.RGBA32, true);

            Color wallBase = new Color(0.85f, 0.87f, 0.90f);
            Color wallTrim = new Color(0.18f, 0.20f, 0.24f);
            Color stripeCyan = new Color(0.10f, 0.75f, 0.95f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Color c = wallBase;

                    if (y < 48)
                    {
                        c = wallTrim;
                    }
                    else if (y >= 48 && y < 58)
                    {
                        c = stripeCyan;
                    }
                    else if (y > size - 32)
                    {
                        c = wallTrim;
                    }
                    else if (x % 128 < 6)
                    {
                        c = wallTrim;
                    }

                    float nx = 0.5f;
                    float ny = 0.5f;
                    if (y == 48 || y == 58 || y == size - 32 || (x % 128 < 4))
                    {
                        nx = 0.6f;
                        ny = 0.6f;
                    }

                    albedo.SetPixel(x, y, c);
                    normal.SetPixel(x, y, new Color(nx, ny, 1f, 1f));
                }
            }
            albedo.Apply();
            normal.Apply();

            SaveAndImportTexture(albedo, $"{dir}/T_Interior_Wall_Albedo.png", false);
            SaveAndImportTexture(normal, $"{dir}/T_Interior_Wall_Normal.png", true);
        }

        private static void GenerateTechScreenTexture(string dir)
        {
            int size = 512;
            Texture2D screen = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Color bg = new Color(0.04f, 0.08f, 0.12f, 1f);
            Color cyan = new Color(0.1f, 0.85f, 1.0f, 1f);
            Color green = new Color(0.2f, 0.95f, 0.4f, 1f);
            Color orange = new Color(1.0f, 0.6f, 0.1f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Color c = bg;

                    if (y > size - 40 && y < size - 10 && x > 20 && x < size - 20)
                    {
                        c = cyan;
                    }

                    int cx = 140;
                    int cy = 256;
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (Mathf.Abs(dist - 90f) < 2f || Mathf.Abs(dist - 60f) < 2f || Mathf.Abs(dist - 30f) < 2f)
                    {
                        c = green;
                    }
                    if (dist < 90f && (x == cx || y == cy))
                    {
                        c = green * 0.7f;
                    }

                    if (x > 280 && x < size - 30 && y > 80 && y < size - 60)
                    {
                        int row = (y - 80) / 30;
                        int col = (y - 80) % 30;
                        int barWidth = (int)(Mathf.Sin(row * 1.5f + 2f) * 60f + 120f);
                        if (col > 6 && x < 280 + barWidth)
                        {
                            c = (row % 3 == 0) ? orange : cyan;
                        }
                    }

                    screen.SetPixel(x, y, c);
                }
            }
            screen.Apply();
            SaveAndImportTexture(screen, $"{dir}/T_Tech_Screen_Emission.png", false);
        }

        private static void GenerateSolarTextures(string dir)
        {
            int size = 256;
            Texture2D albedo = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Color blueCell = new Color(0.06f, 0.12f, 0.28f);
            Color goldBusbar = new Color(0.85f, 0.70f, 0.25f);
            Color cellBorder = new Color(0.02f, 0.04f, 0.08f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isBorder = (x % 32 < 2 || y % 32 < 2);
                    bool isBusbar = (x % 16 == 8);

                    Color c = blueCell;
                    if (isBorder) c = cellBorder;
                    else if (isBusbar) c = goldBusbar;

                    albedo.SetPixel(x, y, c);
                }
            }
            albedo.Apply();
            SaveAndImportTexture(albedo, $"{dir}/T_Solar_Panel_Albedo.png", false);
        }

        private static void GenerateSpaceSkybox(string dir)
        {
            int size = 512;
            Texture2D stars = new Texture2D(size, size, TextureFormat.RGBA32, true);
            Random.InitState(42);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Color spaceBg = new Color(0.012f, 0.015f, 0.022f, 1f);

                    float n1 = Mathf.PerlinNoise(x * 0.01f, y * 0.01f);
                    float n2 = Mathf.PerlinNoise((x + 100) * 0.015f, (y + 100) * 0.015f);
                    if (n1 > 0.6f)
                    {
                        spaceBg += new Color(0.04f, 0.02f, 0.08f) * (n1 - 0.6f) * 2f;
                    }
                    if (n2 > 0.65f)
                    {
                        spaceBg += new Color(0.01f, 0.05f, 0.08f) * (n2 - 0.65f) * 2f;
                    }

                    if (Random.value < 0.007f)
                    {
                        float b = Random.Range(0.6f, 1.0f);
                        Color starColor = (Random.value < 0.25f) ? new Color(0.8f, 0.9f, 1.0f) : new Color(1.0f, 0.95f, 0.8f);
                        spaceBg = starColor * b;
                    }

                    stars.SetPixel(x, y, spaceBg);
                }
            }
            stars.Apply();
            SaveAndImportTexture(stars, $"{dir}/T_Deep_Space_Stars.png", false);
        }

        private static void SaveAndImportTexture(Texture2D tex, string path, bool isNormalMap)
        {
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                if (isNormalMap)
                {
                    importer.textureType = TextureImporterType.NormalMap;
                }
                else
                {
                    importer.textureType = TextureImporterType.Default;
                }
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }
        }
    }
}
