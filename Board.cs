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
    public class Board
    {
        // Rules
        public IPlayer _p1;
        public IPlayer _p2;
        public IPlayer[,] _cells;
        public bool _ended;
        public bool _draw;
        public IPlayer _winner;
        public IPlayer _next;
        public IPlayer _last;
        public uint _depth;

        // Visual
        public Vector2 _origin;
        public Vector2 _scale;
        public Vector2 _cell_size;
        public Vector2 _sep;
        public Vector2 _size;
        public float _z;

        // Mouse
        public MouseState _mouse_last;
        public Tuple<uint, uint> _cell_clicked;

        // Building Boards
        void Initialize()
        {
            _ended = false;
            _draw = false;
            _depth = 0;
            _origin = new Vector2(100.0f, 100.0f);
            _scale = new Vector2(1.0f, 1.0f);
            _sep = new Vector2(0.2f, 0.2f);
            _z = 0.5f;
            _mouse_last = new MouseState();
        }

        public Board(IPlayer p1, IPlayer p2)
        {
            Initialize();
            _p1 = p1;
            _p2 = p2;
            _last = p2;
            _next = p1;
            _cell_size = new Vector2(Math.Max(_p1.texture.Height, _p2.texture.Height),
                                     Math.Max(_p1.texture.Width, _p2.texture.Width));
            _cells = new IPlayer[,] { { null, null, null }, { null, null, null }, { null, null, null } };
        }

        public Board(Board b)
        {
            // Rules
            _p1 = b._p1;
            _p2 = b._p2;
            _cells = (IPlayer[,])b._cells.Clone();
            _ended = b._ended;
            _draw = b._draw;
            _winner = b._winner;
            _next = b._next;
            _last = b._last;
            _depth = b._depth;

            // Visual
            _origin = b._origin;
            _scale = b._scale;
            _cell_size = b._cell_size;
            _sep = b._sep;
            _size = b._size;
            _z = b._z;

            // Mouse
            _mouse_last = b._mouse_last;
        }

        public void SetMove(uint i, uint j)
        {
            Debug.Assert((!_ended), "Ivalid Move: Game Ended!");
            Debug.Assert((_cells[i, j] == null), "Ivalid Move: Invalid cell");

            IPlayer p = _next;
            _next = _last;
            _last = p;
            _depth++;

            bool line = true, column = true;
            bool main = true, inv = true; // Diagonals
            bool draw = true;

            _cells[i, j] = p;

            for (uint n = 0; n < 3; n++)
            {
                for (uint m = 0; m < 3; m++)
                {
                    draw &= !(_cells[n, m] == null);
                }
                column &= (_cells[i, n] == p);
                line &= (_cells[n, j] == p);
                main &= _cells[n, n] == p;
                inv &= _cells[2 - n, n] == p;
            }
            if (line || column || main || inv)
            {
                _ended = true;
                _winner = p;
                return;
            }
            if (draw)
            {
                _ended = true;
                _draw = true;
                return;
            }
        }

        public void UpdateSize()
        {
            //_size = (3 * Vector2.One + 2 * _sep) * _cell_size * _scale;
            _size = (3 * Vector2.One) * _cell_size * _scale;
        }

        public void Update(MouseState mouse)
        {
            bool click = false;
            if (_mouse_last.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed)
            {
                click = true;
            }
            _mouse_last = mouse;

            float x = mouse.Position.ToVector2().X;
            float y = mouse.Position.ToVector2().Y;

            if (x > _origin.X && x < _origin.X + _size.X &&
               y > _origin.Y && y < _origin.Y + _size.Y)
            {
                // Its hovering the board
                if (click)
                {
                    _cell_clicked = CheckMouse(mouse.Position.ToVector2());
                }
            }
        }

        public List<Board> Branches()
        {
            List<Board> list = new List<Board>();
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    if (_cells[i, j] == null)
                    {
                        Board t = new Board(this);
                        t.SetMove(i, j);
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        public int Minmax(IPlayer p)
        {
            if (_ended)
            {
                if (_draw) { return 0; }
                else if (_winner == p) { return 1; }
                return -1;
            }
            int valor;
            List<Board> branches = Branches();
            if (_last == p)
            {
                valor = int.MaxValue;
                foreach (Board branch in branches)
                {
                    valor = Math.Min(valor, branch.Minmax(p));
                }
            }
            else
            {
                valor = int.MinValue;
                foreach (Board branch in branches)
                {
                    valor = Math.Max(valor, branch.Minmax(p));
                }
            }
            return valor;
        }

        public bool CheckCell(Vector2 pos, uint i, uint j)
        {
            Vector2 dest = new Vector2(i * _cell_size.X, j * _cell_size.Y);
            dest = _origin + (dest * _scale);
            return ((pos.X > dest.X) && (pos.X < (dest.X + _cell_size.X)) &&
                   (pos.Y > dest.Y) && (pos.Y < (dest.Y + _cell_size.Y)));
        }

        public Tuple<uint, uint> CheckMouse(Vector2 pos)
        {
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    if (CheckCell(pos, i, j))
                    {
                        return new Tuple<uint, uint>(i, j);
                    }
                }
            }
            return null;
        }

        public void Draw(SpriteBatch sb)
        {
            DrawLines(sb);
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    Vector2 dest = new Vector2(i * _cell_size.X, j * _cell_size.Y);
                    dest = _origin + (dest * _scale);
                    if (_cells[i, j] != null)
                    {
                        sb.Draw(_cells[i, j].texture,
                                dest,
                                null,
                                Color.White,
                                0.0f,
                                Vector2.One,
                                _scale,
                                SpriteEffects.None,
                                0.5f);
                    }
                }
            }
        }

        public void DrawLines(SpriteBatch sb)
        {
            UpdateSize();
            Vector2 h1 = new Vector2(0.0f, _cell_size.Y);
            h1 = _origin + (h1 * _scale);
            Vector2 h2 = new Vector2(0.0f, _cell_size.Y * 2);
            h2 = _origin + (h2 * _scale);
            Vector2 v1 = new Vector2(_cell_size.X, 0.0f);
            v1 = _origin + (v1 * _scale);
            Vector2 v2 = new Vector2(_cell_size.X * 2, 0.0f);
            v2 = _origin + (v2 * _scale);

            Line.DrawLine(sb, h1, h1 + (_size * Vector2.UnitX));
            Line.DrawLine(sb, h2, h2 + (_size * Vector2.UnitX));
            Line.DrawLine(sb, v1, v1 + (_size * Vector2.UnitY));
            Line.DrawLine(sb, v2, v2 + (_size * Vector2.UnitY));
        }
    }
}
