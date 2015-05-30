using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public static class Primitives
    {
        static SpriteFont _text;
        static Texture2D _line;
        static Texture2D _rect;
        static SoundEffect _plop;
        public static void LoadContent(GraphicsDevice dev, ContentManager content)
        {
            _text = content.Load<SpriteFont>("Arial");
            Primitives._line = new Texture2D(dev, 1, 1);
            _line.SetData<Color>(new Color[] { Color.White });
            Primitives._rect = new Texture2D(dev, 1, 1);
            _rect.SetData<Color>(new Color[] { Color.White });
            _plop = content.Load<SoundEffect>("Sounds/plop");
        }
        public static void PlaySound()
        {
            _plop.Play();
        }
        public static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color,
                                    float width = 1.0f, float z = 0.0f)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            sb.Draw(_line,
                    // rectangle defines shape of line and position of start of line
                    new Rectangle((int)start.X,
                                  (int)start.Y,
                                  (int)edge.Length(),       // sb will strech the texture to fill this rectangle
                                  Math.Max(1,(int)width)),  // width of line, change this to make thicker line
                    null,
                    color,                  // colour of line
                    angle,                  // angle of line (calulated above)
                    Vector2.Zero,           // point in line about which to rotate
                    SpriteEffects.None,
                    z);
        }
        public static void DrawRectangle(SpriteBatch sb, Vector2 start, Vector2 end, Color color,
                                         float z = 0.0f, int inflate = 0)
        {
            Vector2 edge = end - start;
            Rectangle rectangle = new Rectangle((int)start.X, (int)start.Y, (int)edge.X, (int)edge.Y);
            rectangle.Inflate(inflate,inflate);
            sb.Draw(_rect,
                    rectangle,
                    null,
                    color,
                    0.0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    z);
        }
        public static void DrawText(SpriteBatch sb, Vector2 pos, string text, Color color,
                                    float scale = 1.0f, float z = 1.0f)
        {
            sb.DrawString(Primitives._text,
                          text,
                          pos,
                          color,
                          0.0f,
                          Vector2.Zero,
                          scale,
                          SpriteEffects.None,
                          z);
        }
    }
}
