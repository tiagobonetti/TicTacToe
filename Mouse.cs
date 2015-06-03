using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace TicTacToe {

    public static class MouseMgr {
        public static Vector2 _pos;
        public static MouseState _now;
        public static MouseState _last;
        public static bool _left_down;

        public static void Update(MouseState state) {
            _pos = state.Position.ToVector2();
            _last = _now;
            _now = state;
            _left_down = (_last.LeftButton == ButtonState.Released && _now.LeftButton == ButtonState.Pressed);
        }
    }
}
