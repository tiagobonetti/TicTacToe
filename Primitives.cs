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

namespace TicTacToe {
    public static class Primitives {
        public enum TextAlign {
            Centered,
            TopLeft,
            CenterLeft
        }

        public enum TextFont {
            Arial12,
            Arial20,
            Arial40,
            Arial60B
        }

        static SpriteFont _arial12;
        static SpriteFont _arial20;
        static SpriteFont _arial40;
        static SpriteFont _arial60B;
        static Texture2D _line;
        static Texture2D _rect;
        static SoundEffect _plop;

        public static void LoadContent(GraphicsDevice dev, ContentManager content) {
            _arial12 = content.Load<SpriteFont>("Fonts/Arial12");
            _arial20 = content.Load<SpriteFont>("Fonts/Arial20");
            _arial40 = content.Load<SpriteFont>("Fonts/Arial40");
            _arial60B = content.Load<SpriteFont>("Fonts/Arial60B");
            Primitives._line = new Texture2D(dev, 1, 1);
            _line.SetData<Color>(new Color[] { Color.White });
            Primitives._rect = new Texture2D(dev, 1, 1);
            _rect.SetData<Color>(new Color[] { Color.White });
            _plop = content.Load<SoundEffect>("Sounds/plop");
        }

        public static void PlaySound() {
            _plop.Play();
        }

        public static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color,
                                    float width = 1.0f, float z = 0.0f) {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            sb.Draw(_line,
                // rectangle defines shape of line and position of start of line
                    new Rectangle((int)start.X,
                                  (int)start.Y,
                                  (int)edge.Length(),       // sb will strech the texture to fill this rectangle
                                  Math.Max(1, (int)width)),  // width of line, change this to make thicker line
                    null,
                    color,                  // colour of line
                    angle,                  // angle of line (calulated above)
                    Vector2.Zero,           // point in line about which to rotate
                    SpriteEffects.None,
                    z);
        }

        public static void DrawRectangle(SpriteBatch sb, Vector2 start, Vector2 end, Color color,
                                         float z = 0.0f, int inflate = 0) {
            Vector2 edge = end - start;
            Rectangle rectangle = new Rectangle((int)start.X, (int)start.Y, (int)edge.X, (int)edge.Y);
            rectangle.Inflate(inflate, inflate);
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
                                    float scale = 1.0f,
                                    float z = 1.0f,
                                    TextFont font = TextFont.Arial20,
                                    TextAlign align = TextAlign.Centered) {
            SpriteFont spritefont;
            switch (font) {
                case TextFont.Arial12:
                    spritefont = Primitives._arial12;
                    break;
                case TextFont.Arial20:
                    spritefont = Primitives._arial20;
                    break;
                case TextFont.Arial40:
                    spritefont = Primitives._arial40;
                    break;
                case TextFont.Arial60B:
                    spritefont = Primitives._arial60B;
                    break;
                default:
                    Debug.Assert(false, "Font plz!");
                    spritefont = Primitives._arial12;
                    break;
            }
            Vector2 offset;
            switch (align) {
                case TextAlign.Centered:
                    offset = spritefont.MeasureString(text) * 0.5f;
                    break;
                case TextAlign.TopLeft:
                    offset = Vector2.Zero;
                    break;
                case TextAlign.CenterLeft:
                    offset = spritefont.MeasureString(text) * 0.5f * Vector2.UnitY;
                    break;
                default:
                    Debug.Assert(false, "Align plz!");
                    offset = Vector2.Zero;
                    break;
            }
            sb.DrawString(spritefont,
                           text,
                           pos - offset,
                           color,
                           0.0f,
                           Vector2.Zero,
                           scale,
                           SpriteEffects.None,
                           z);
        }
    }
}
