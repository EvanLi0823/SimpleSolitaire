using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleSolitaire.Editor
{
    /// <summary>
    /// SpriteAtlas自动打包工具
    /// 功能：
    /// 1. 选择文件夹右键 -> 递归遍历所有图片生成图集
    /// 2. 选择多个图片右键 -> 打包成一个图集
    /// 3. 自动处理超出2048x2048的情况，分组并添加后缀_0, _1等
    /// </summary>
    public class SpriteAtlasCreator
    {
        private const int MAX_ATLAS_SIZE = 2048;
        private const int SPRITES_PER_ATLAS = 100; // 每个图集最多放100张图片，避免超出尺寸限制

        /// <summary>
        /// 右键菜单：为选中的文件夹创建SpriteAtlas（递归）
        /// </summary>
        [MenuItem("Assets/创建SpriteAtlas/从文件夹（递归）", false, 20)]
        private static void CreateSpriteAtlasFromFolder()
        {
            // 获取选中的文件夹
            string folderPath = GetSelectedFolderPath();
            if (string.IsNullOrEmpty(folderPath))
            {
                EditorUtility.DisplayDialog("错误", "请选择一个文件夹", "确定");
                return;
            }

            // 递归查找所有Sprite资源
            List<Sprite> sprites = CollectSpritesFromFolder(folderPath, true);

            if (sprites.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", $"文件夹 {Path.GetFileName(folderPath)} 中没有找到图片资源", "确定");
                return;
            }

            // 获取文件夹名作为图集名
            string atlasName = Path.GetFileName(folderPath);

            // 创建图集
            CreateAtlasesForSprites(sprites, folderPath, atlasName);

            AssetDatabase.Refresh();
            Debug.Log($"✅ 成功为文件夹 '{atlasName}' 创建图集，共 {sprites.Count} 张图片");
        }

        /// <summary>
        /// 右键菜单验证：只有选中文件夹时才显示
        /// </summary>
        [MenuItem("Assets/创建SpriteAtlas/从文件夹（递归）", true)]
        private static bool ValidateCreateSpriteAtlasFromFolder()
        {
            return !string.IsNullOrEmpty(GetSelectedFolderPath());
        }

        /// <summary>
        /// 右键菜单：为选中的多个图片创建SpriteAtlas
        /// </summary>
        [MenuItem("Assets/创建SpriteAtlas/从选中的图片", false, 21)]
        private static void CreateSpriteAtlasFromSelection()
        {
            // 获取选中的所有Sprite
            List<Sprite> sprites = GetSelectedSprites();

            if (sprites.Count == 0)
            {
                EditorUtility.DisplayDialog("错误", "请选择至少一张图片（Sprite）", "确定");
                return;
            }

            // 获取第一个选中对象所在的文件夹
            Object firstSelection = Selection.objects[0];
            string assetPath = AssetDatabase.GetAssetPath(firstSelection);
            string folderPath = Path.GetDirectoryName(assetPath);

            // 使用文件夹名作为图集名
            string atlasName = Path.GetFileName(folderPath);

            // 创建图集
            CreateAtlasesForSprites(sprites, folderPath, atlasName);

            AssetDatabase.Refresh();
            Debug.Log($"✅ 成功创建图集 '{atlasName}'，包含 {sprites.Count} 张图片");
        }

        /// <summary>
        /// 右键菜单验证：只有选中图片时才显示
        /// </summary>
        [MenuItem("Assets/创建SpriteAtlas/从选中的图片", true)]
        private static bool ValidateCreateSpriteAtlasFromSelection()
        {
            return GetSelectedSprites().Count > 0;
        }

        #region 核心逻辑

        /// <summary>
        /// 创建图集（支持自动分组）
        /// </summary>
        private static void CreateAtlasesForSprites(List<Sprite> sprites, string folderPath, string atlasName)
        {
            // 计算需要创建的图集数量
            int atlasCount = Mathf.CeilToInt((float)sprites.Count / SPRITES_PER_ATLAS);

            if (atlasCount == 1)
            {
                // 只需要一个图集
                CreateSingleAtlas(sprites, folderPath, atlasName);
            }
            else
            {
                // 需要多个图集，添加后缀_0, _1, _2...
                for (int i = 0; i < atlasCount; i++)
                {
                    int startIndex = i * SPRITES_PER_ATLAS;
                    int count = Mathf.Min(SPRITES_PER_ATLAS, sprites.Count - startIndex);
                    List<Sprite> batch = sprites.GetRange(startIndex, count);

                    string batchAtlasName = $"{atlasName}_{i}";
                    CreateSingleAtlas(batch, folderPath, batchAtlasName);
                }

                Debug.Log($"📦 由于图片数量较多（{sprites.Count}张），已自动分组为 {atlasCount} 个图集");
            }
        }

        /// <summary>
        /// 创建单个SpriteAtlas
        /// </summary>
        private static void CreateSingleAtlas(List<Sprite> sprites, string folderPath, string atlasName)
        {
            // 创建SpriteAtlas对象
            SpriteAtlas atlas = new SpriteAtlas();

            // 配置图集设置
            SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings
            {
                enableRotation = false,
                enableTightPacking = false,
                padding = 2
            };
            atlas.SetPackingSettings(packingSettings);

            // 配置纹理设置（默认平台）
            SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear
            };
            atlas.SetTextureSettings(textureSettings);

            // 配置平台设置
            TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings
            {
                maxTextureSize = MAX_ATLAS_SIZE,
                format = TextureImporterFormat.Automatic,
                textureCompression = TextureImporterCompression.Compressed,
                compressionQuality = 50
            };
            atlas.SetPlatformSettings(platformSettings);

            // 添加Sprite到图集
            Object[] spriteObjects = sprites.Cast<Object>().ToArray();
            atlas.Add(spriteObjects);

            // 保存图集到文件夹
            string atlasPath = Path.Combine(folderPath, $"{atlasName}.spriteatlas");

            // 转换为Unity的Asset路径
            if (!atlasPath.StartsWith("Assets/"))
            {
                atlasPath = atlasPath.Replace(Application.dataPath, "Assets");
            }

            AssetDatabase.CreateAsset(atlas, atlasPath);
            AssetDatabase.SaveAssets();

            // 立即打包图集
            SpriteAtlasUtility.PackAtlases(new[] { atlas }, EditorUserBuildSettings.activeBuildTarget);

            Debug.Log($"📦 创建图集: {atlasPath}（{sprites.Count} 张图片）");
        }

        /// <summary>
        /// 从文件夹收集所有Sprite资源
        /// </summary>
        private static List<Sprite> CollectSpritesFromFolder(string folderPath, bool recursive)
        {
            List<Sprite> sprites = new List<Sprite>();

            // 获取文件夹下的所有资源GUID
            string searchPattern = recursive ? "t:Sprite" : "t:Sprite";
            string searchFolder = folderPath;

            // 转换为Unity Asset路径
            if (!searchFolder.StartsWith("Assets/"))
            {
                searchFolder = searchFolder.Replace(Application.dataPath, "Assets");
            }

            // 查找所有Sprite
            string[] guids = AssetDatabase.FindAssets(searchPattern, new[] { searchFolder });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // 如果不是递归模式，只处理当前文件夹
                if (!recursive)
                {
                    string assetFolder = Path.GetDirectoryName(assetPath).Replace("\\", "/");
                    if (assetFolder != searchFolder.Replace("\\", "/"))
                    {
                        continue;
                    }
                }

                // 加载所有子Sprite（处理Sprite Sheet情况）
                Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

                if (subAssets.Length > 0)
                {
                    // 有子资源（Sprite Sheet）
                    foreach (Object obj in subAssets)
                    {
                        if (obj is Sprite sprite)
                        {
                            sprites.Add(sprite);
                        }
                    }
                }
                else
                {
                    // 单个Sprite
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (sprite != null)
                    {
                        sprites.Add(sprite);
                    }
                }
            }

            return sprites;
        }

        /// <summary>
        /// 获取选中的Sprite列表
        /// </summary>
        private static List<Sprite> GetSelectedSprites()
        {
            List<Sprite> sprites = new List<Sprite>();

            foreach (Object obj in Selection.objects)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);

                // 检查是否是纹理资源
                if (obj is Texture2D || obj is Sprite)
                {
                    // 加载所有子Sprite
                    Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

                    if (subAssets.Length > 0)
                    {
                        foreach (Object subObj in subAssets)
                        {
                            if (subObj is Sprite sprite)
                            {
                                sprites.Add(sprite);
                            }
                        }
                    }
                    else if (obj is Sprite sprite)
                    {
                        sprites.Add(sprite);
                    }
                    else
                    {
                        Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                        if (loadedSprite != null)
                        {
                            sprites.Add(loadedSprite);
                        }
                    }
                }
            }

            return sprites;
        }

        /// <summary>
        /// 获取选中的文件夹路径
        /// </summary>
        private static string GetSelectedFolderPath()
        {
            if (Selection.activeObject == null)
                return null;

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
                return null;

            // 检查是否是文件夹
            if (Directory.Exists(path))
            {
                return path;
            }

            // 检查是否是文件，如果是则返回其父文件夹
            if (File.Exists(path))
            {
                return Path.GetDirectoryName(path);
            }

            return null;
        }

        #endregion
    }
}
