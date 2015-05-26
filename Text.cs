using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public static class Text
    {
        static SpriteFont _text;
        public static void LoadContent(ContentManager content)
        {
            Text._text = content.Load<SpriteFont>("Arial");
        }
        public static void DrawArial(SpriteBatch sb, Vector2 pos, string text, Color color )
        {
            sb.DrawString(Text._text, text, pos, color);
        }
    }
}
