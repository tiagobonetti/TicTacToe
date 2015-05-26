using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace TicTacToe
{
    public class ClickableTexture
    {
        public static ClickableTexture _empty;

        static ClickableTexture()
        {
            _empty = new ClickableTexture();
        }

        public Texture2D _t;
        public Vector2 _pos;
        public Vector2 _size;
        public Vector2 _scale;
        public SpriteEffects _effect;
        public float _z;

        public ClickableTexture()
        {
            _pos = Vector2.Zero;
            _size = Vector2.Zero;
            _scale = Vector2.One;
            _effect = SpriteEffects.None;
            _z = 0.5f;
        }
        public ClickableTexture(Texture2D t)
        {
            _t = t;
            _size = new Vector2(t.Width, t.Height);
            _pos = Vector2.Zero;
            _scale = Vector2.One;
            _effect = SpriteEffects.None;
            _z = 0.5f;
        }
        public void Draw(SpriteBatch sb)
        {
            if (_t != null)
            {
                sb.Draw(_t, _pos, null, Color.White, 0.0f, Vector2.Zero, _scale, _effect, _z);
            }
        }
        public bool IsInside(Vector2 pos)
        {
            return ((pos.X > _pos.X) && (pos.X < (_pos.X + _size.X)) &&
                    (pos.Y > _pos.Y) && (pos.Y < (_pos.Y + _size.Y)) );
        }
    }
}
