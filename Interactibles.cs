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
    public class ClickableArea
    {
        public Texture2D _t;
        public Vector2 _pos;
        public Vector2 _size;
        public Vector2 _scale;
        public SpriteEffects _effect;
        public Color _color;
        public float _z;

        public ClickableArea()
        {
            _pos = Vector2.Zero;
            _size = Vector2.Zero;
            _scale = Vector2.One;
            _effect = SpriteEffects.None;
            _color = Color.White;
            _z = 0.5f;
        }
        public ClickableArea(Texture2D t)
        {
            _t = t;
            _size = new Vector2(t.Width, t.Height);
            _pos = Vector2.Zero;
            _scale = Vector2.One;
            _effect = SpriteEffects.None;
            _color = Color.White;
            _z = 0.5f;
        }
        public void Draw(SpriteBatch sb)
        {
            if (_t != null)
            {
                sb.Draw(_t, _pos, null, _color, 0.0f, Vector2.Zero, _scale, _effect, _z);
            }
        }
        public void DrawText(SpriteBatch sb, string text)
        {
            Primitives.DrawText(sb, _pos + new Vector2(10.0f, 10.0f), text, _color);
        }
        public bool IsInside(Vector2 pos)
        {
            return ((pos.X > _pos.X) && (pos.X < (_pos.X + _size.X)) &&
                    (pos.Y > _pos.Y) && (pos.Y < (_pos.Y + _size.Y)));
        }
        public void DrawSquare(SpriteBatch sb)
        {
            Vector2 end = _pos + _size;
            Primitives.DrawRectangle(sb, _pos, end, Color.Multiply(Color.CornflowerBlue, 0.9f), 0.1f);
            Primitives.DrawRectangle(sb, _pos, end, Color.Multiply(Color.White, 0.9f), 0.0f, 1);
        }
    }

    public class ActionButton
    {
        public ClickableArea _area;
        public string _name;

        public ActionButton(string name, Vector2 pos, Vector2 size)
        {
            _name = name;
            _area = new ClickableArea();
            _area._pos = pos;
            _area._size = size;
        }
        public bool Update()
        {
            return (MouseMgr._left_down && _area.IsInside(MouseMgr._pos));
        }
        public void Draw(SpriteBatch sb) {
            _area.DrawText(sb, _name);
            _area.DrawSquare(sb);
        }
    }


    public class DiffButton
    {
        public CpuBase.Difficulty _diff;
        public ClickableArea _area;
        public string _name;

        public DiffButton()
        {
            _diff = CpuBase.Difficulty.Easy;
            _area = new ClickableArea();
        }
        public void Update()
        {
            if (MouseMgr._left_down && _area.IsInside(MouseMgr._pos))
            {
                switch (_diff)
                {
                    case CpuBase.Difficulty.Hard:
                        _diff = CpuBase.Difficulty.Easy;
                        break;
                    case CpuBase.Difficulty.Normal:
                        _diff = CpuBase.Difficulty.Hard;
                        break;
                    case CpuBase.Difficulty.Easy:
                        _diff = CpuBase.Difficulty.Normal;
                        break;
                    default:
                        Debug.Assert(false, "No patric, DarkSouls is not a difficulty!");
                        break;
                }
            }
        }
        public void Draw(SpriteBatch sb)
        {
            _area.DrawText(sb, _diff.ToString());
            _area.DrawSquare(sb);
        }
    }
}
