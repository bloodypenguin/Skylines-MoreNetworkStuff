using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace MoreNetworkStuff
{
    public static class Util
    {
        public static Type FindType(string className)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types.Where(type => type.Name == className))
                    {
                        return type;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }

        public static bool IsModActive(string modName)
        {
            var plugins = PluginManager.instance.GetPluginsInfo();
            return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.GetInstances<IUserMod>() into instances
                    where instances.Any()
                    select instances[0].Name into name
                    where name == modName
                    select name).Any();
        }

        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
            {
                return;
            }
            foreach (var element in source)
                target.Add(element);
        }

        public static Texture2D LoadTextureFromAssembly(string path, bool readOnly = true)
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (var textureStream = assembly.GetManifestResourceStream(path))
                {
                    return LoadTextureFromStream(readOnly, textureStream);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return null;
            }
        }

        public static UITextureAtlas CreateAtlas(Texture2D[] sprites)
        {
            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            atlas.material = new Material(GetUIAtlasShader());

            Texture2D texture = new Texture2D(0, 0);
            Rect[] rects = texture.PackTextures(sprites, 0);

            for (int i = 0; i < rects.Length; ++i)
            {
                Texture2D sprite = sprites[i];
                Rect rect = rects[i];

                UITextureAtlas.SpriteInfo spriteInfo = new UITextureAtlas.SpriteInfo();
                spriteInfo.name = sprite.name;
                spriteInfo.texture = sprite;
                spriteInfo.region = rect;
                spriteInfo.border = new RectOffset();

                atlas.AddSprite(spriteInfo);
            }
            atlas.material.mainTexture = texture;
            return atlas;
        }

        private static Shader GetUIAtlasShader()
        {
            return UIView.GetAView().defaultAtlas.material.shader;
        }

        private static Texture2D LoadTextureFromStream(bool readOnly, Stream textureStream)
        {
            var buf = new byte[textureStream.Length]; //declare arraysize
            textureStream.Read(buf, 0, buf.Length); // read from stream to byte array
            textureStream.Close();
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(buf);
            tex.Apply(false, readOnly);
            tex.name = Guid.NewGuid().ToString();
            return tex;
        }
    }
}