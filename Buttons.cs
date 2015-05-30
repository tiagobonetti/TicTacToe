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
    public class BaseButton : IPositionable
    {
        public Vector2 _pos;
        public Vector2 position
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public Vector2 _size;
        public Vector2 _scale;
        public SpriteEffects _effect;
        public Color _color;
        public float _z;
        public BaseButton(Vector2 pos,
                          Vector2 size,
                          Vector2 scale,
                          SpriteEffects effect,
                          Color color,
                          float z)
        {
            _pos = pos;
            _size = size;
            _scale = scale;
            _effect = effect;
            _color = color;
            _z = z;
        }
        virtual public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            Vector2 end = _pos + _size;
            Primitives.DrawRectangle(sb, _pos, end, Color.Multiply(Color.CornflowerBlue, 0.9f), 0.1f);
            Primitives.DrawRectangle(sb, _pos, end, Color.Multiply(Color.White, 0.9f), 0.0f, 1);
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
    }

    public class ClickableButton : BaseButton
    {
        public ClickableButton(Vector2 pos,
                               Vector2 size,
                               Vector2 scale,
                               SpriteEffects effect,
                               Color color,
                               float z)
            : base(pos, size, scale, effect, color, z)
        {
        }
        override public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            base.Draw(gameTime, sb);
        }
    }

    public class ActionButton : ClickableButton
    {
        public string _name;
        public ActionButton(string name, Vector2 pos, Vector2 size)
            : base(pos,
                   size,
                   Vector2.One,
                   SpriteEffects.None,
                   Color.White,
                   0.5f)
        {
            _name = name;
        }
        public bool Update()
        {
            return (MouseMgr._left_down && base.IsInside(MouseMgr._pos));
        }
        override public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            base.Draw(gameTime, sb);
            base.DrawText(sb, _name);
        }
    }

    public interface IOption
    {
        void Next();
        string ToString();
    }
    public class FirstPlayerOption : IOption
    {
        BasePlayer.Type _type;
        public FirstPlayerOption(BasePlayer.Type type = BasePlayer.Type.Human)
        {
            _type = type;
        }
        void IOption.Next()
        {
            switch (_type)
            {
                case BasePlayer.Type.Human:
                    _type = BasePlayer.Type.AI;
                    break;
                case BasePlayer.Type.AI:
                    _type = BasePlayer.Type.Human;
                    break;
                default:
                    Debug.Assert(false, "Thats not a player type");
                    break;
            }
        }
        string IOption.ToString()
        {
            return _type.ToString();
        }
        public static implicit operator BasePlayer.Type(FirstPlayerOption o)
        {
            return o._type;
        }

    }
    public class DifficultyOption : IOption
    {
        public BaseAI.Difficulty _difficulty;
        public DifficultyOption(BaseAI.Difficulty difficulty = BaseAI.Difficulty.Normal)
        {
            _difficulty = difficulty;
        }
        void IOption.Next()
        {
            switch (_difficulty)
            {
                case BaseAI.Difficulty.Hard:
                    _difficulty = BaseAI.Difficulty.Easy;
                    break;
                case BaseAI.Difficulty.Normal:
                    _difficulty = BaseAI.Difficulty.Hard;
                    break;
                case BaseAI.Difficulty.Easy:
                    _difficulty = BaseAI.Difficulty.Normal;
                    break;
                default:
                    Debug.Assert(false, "No patric, DarkSouls is not a difficulty!");
                    break;
            }
        }
        string IOption.ToString()
        {
            return _difficulty.ToString();
        }
        public static implicit operator BaseAI.Difficulty(DifficultyOption d)
        {
            return d._difficulty;
        }
    }

    public class OptionButton : ClickableButton
    {
        IOption _option;
        public OptionButton(IOption option, Vector2 pos, Vector2 size)
            : base(pos,
                   size,
                   Vector2.One,
                   SpriteEffects.None,
                   Color.White,
                   0.5f)
        {
            _option = option;
        }
        public void Update()
        {
            if (MouseMgr._left_down && base.IsInside(MouseMgr._pos))
            {
                _option.Next();
            }
        }
        override public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            base.Draw(gameTime, sb);
            base.DrawText(sb, _option.ToString());
        }
    }
}
