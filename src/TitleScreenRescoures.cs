using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Wol.Localization
{
    internal static class TitleScreenRescoures
    {
        static string BasePath = Path.Combine(Localization.baseDir, "images");
        //static string ContinuePath = Path.Combine(Localization.baseDir, ZyConfig.LocalLan, "title_continue2_1.png");


        public static Texture2D LoadContinueTex()
        {
            return LoadTexByName("title_continue2_1.png");

        }

        public static Texture2D LoadTexByName(string name)
        {
            var _path = Path.Combine(BasePath, name);
            if (File.Exists(_path))
            {
                Debug.Log($"_path:{_path}");
                using (var fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read))
                {


                    fileStream.Seek(0, SeekOrigin.Begin);
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, (int)fileStream.Length);
                    //int width = 800; int height = 640;
                    Texture2D texture = new Texture2D(2, 2);
                    //Texture2D tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);

                    texture.LoadImage(bytes);
                    texture.Apply();

                    return texture;
                }
            }

            return null;
        }



        public static void ReplaceContinue(this SpriteRenderer sprite)
        {
            //Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            sprite.ReplaceSprite(() => LoadContinueTex());
        }

        public static void ReplaceLoad(this SpriteRenderer sprite)
        {
            //TitleScreenRescoures.LoadTexByName("title_loadgame_cn.png").ReplaceSprite(sprite);
            sprite.ReplaceSprite(() => LoadTexByName("title_switchcharacter.png"));
        }

        public static void ReplaceNewGame(this SpriteRenderer sprite)
        {
            //TitleScreenRescoures.LoadTexByName("title_newgame_cn.png").ReplaceSprite(sprite);
            sprite.ReplaceSprite(() => LoadTexByName("title_newgame_cn.png"));
        }

        public static void ReplaceOptions(this SpriteRenderer sprite)
        {
            sprite.ReplaceSprite(() => LoadTexByName("title_options_ch.png"));
        }

        public static void ReplaceQuite(this SpriteRenderer sprite)
        {
            sprite.ReplaceSprite(() => LoadTexByName("title_quit_cn.png"));
        }
        private static void ReplaceSprite(this SpriteRenderer render, Func<Texture2D> func)
        {
            if (render == null)
            {
                return;
            }
            var texture = func?.Invoke();
            if (texture == null)
            {
                return;
            }
            var rect = render.sprite.rect;
            var height = rect.height;
            var width = rect.width;
            Debug.LogError($"tex.w:{texture.width};tex.h:{texture.height}");
            render.drawMode = SpriteDrawMode.Sliced;
            var sp = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 1f));
            render.sprite = sp;
        }
    }
}
