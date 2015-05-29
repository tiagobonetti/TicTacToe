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
    public interface IPositionable
    {
        Vector2 position { get; set; }
        void Draw(SpriteBatch sb);
    }
    public class Slide
    {
        public enum Movement
        {
            Linear,
            Wooble
        }
        IPositionable _obj;
        public Vector2 _offset;
        public float _animation;
        Movement _mov;

        public Vector2 _pos;
        public Vector2 _origin;
        public float _elapsed;

        public Slide(IPositionable obj, Vector2 start, float animationTime, Movement mov = Movement.Linear)
        {
            _obj = obj;
            _offset = start;
            _animation = animationTime;
            _mov = mov;

            _pos = obj.position;
            _origin = new Vector2();
            _origin += _pos;  // Copy original position
            _elapsed = 0.0f;
        }
        public void Reset()
        {
            _elapsed = 0.0f;
        }
        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            if (_elapsed < _animation)
            {
                var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _elapsed += delta;
                _elapsed = Math.Min(_elapsed, _animation);
                _obj.position = _origin + (_offset * (1 - (_elapsed / _animation)));
            }
            _obj.Draw(sb);
        }
    }
}
