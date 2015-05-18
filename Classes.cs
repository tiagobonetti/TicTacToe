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
    class Player
    {
        public static Player None;
        static Player()
        {
            None = new Player(Player.Type.None);
        }

        public Texture2D _texture;
        public enum Type {
            None,
            Human,
            CPU
        };
        public Type _type;
        public Player(Type type)
        {
            _type = type;
        }
    }

    class Board
    {
        public static Board _base;
        public static Player _p1;
        public static Player _p2;
        public static Vector2 _cell_size;

        static Board()
        {
            _p1 = new Player(Player.Type.Human);
            _p2 = new Player(Player.Type.CPU);
            _base = CleanBoard();
            _base._next = _p1;
            _base._last = _p2;
            _base.BuildBranches();
        }

        public static void LoadContent(ContentManager content)
        {
            // Statr Board Statics
            Board._p1._texture = content.Load<Texture2D>("TicTacToeX");
            Board._p2._texture = content.Load<Texture2D>("TicTacToeO");
            Board._cell_size = new Vector2(Math.Max(Board._p1._texture.Height, Board._p2._texture.Height),
                                           Math.Max(Board._p1._texture.Width, Board._p2._texture.Width));
        }

        public Player[,] _cell;
        public bool _ended;
        public bool _draw;
        public Player _winner;
        public Player _next;
        public Player _last;

        public uint _depth;
        public List<Board> _branches;
        public Board _played;

        public Vector2 _origin;
        public Vector2 _scale;
        public Vector2 _sep;
        public Vector2 _size;
        public float _z;

        public Board(Player[,] cells)
        {
            this._cell = cells;
            _ended = false;
            _draw = false;
            _depth = 0;
            _branches = new List<Board>();
            _origin = new Vector2();
            _scale = new Vector2(1.0f, 1.0f);
            _sep = new Vector2(0.2f, 0.2f);
            _z = 0.5f;
        }

        public static Board CleanBoard()
        {
            return new Board(new Player[,] { { null, null, null }, { null, null, null }, { null, null, null } });
        }

        public Board Clone()
        {
            return new Board((Player[,])_cell.Clone())
            {
                _ended = this._ended,
                _draw = this._draw,
                _winner = this._winner,
                _depth = this._depth,
                _next = this._next,
                _last = this._last,
                _origin = this._origin,
                _scale = this._scale,
                _sep = this._sep,
                _z = this._z
            };
        }

        public void SetMove(uint i, uint j)
        {
            Debug.Assert((!_ended), "Ivalid Move: Game Ended!");
            Debug.Assert((_cell[i, j] == null), "Ivalid Move: Invalid cell");

            Player p = _next;
            _next = _last;
            _last = p;
            _depth++;

            bool line = true, column = true;
            bool main = true, inv = true; // Diagonals
            bool draw = true;

            _cell[i, j] = p;

            for (uint n = 0; n < 3; n++)
            {
                for (uint m = 0; m < 3; m++)
                {
                    draw &= !(_cell[n, m] == null);
                }
                column &= (_cell[i, n] == p);
                line &= (_cell[n, j] == p);
                main &= _cell[n, n] == p;
                inv &= _cell[2 - n, n] == p;
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

        public void BuildBranches()
        {
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    if (_cell[i, j] == null)
                    {
                        Board t = this.Clone();
                        t.SetMove(i, j);
                        if (!t._ended) { t.BuildBranches(); }
                        _branches.Add(t);
                    }
                }
            }
        }

        public void UpdateSize()
        {
            _size = (3 * Vector2.One + 2 * _sep) * _cell_size * _scale;
        }

        public void Update(MouseState mouse)
        {
            float x = mouse.Position.ToVector2().X;
            float y = mouse.Position.ToVector2().Y;

            if(x > _origin.X && x < _origin.X + _size.X &&
               y > _origin.Y && y < _origin.Y + _size.Y )
            {

                // Its hovering the board
            }
        }

        public void Play()
        {
            if (_next._type == Player.Type.Human)
            {
                // ?
            }
            else  // _next._type == Player.Type.CPU 
            {
                PlayMinmax();
            }
        }

        public void Play(Tuple<uint, uint> cell)
        {
            if (_cell[cell.Item1, cell.Item2] == null)
            {
                foreach (Board branch in _branches)
                {
                    if (branch._cell[cell.Item1, cell.Item2] == _next)
                    {
                        _played = branch;
                    }
                }
            }
        }

        public void PlayMinmax()
        {
            List<Board> wins = new List<Board>();
            List<Board> draws = new List<Board>();
            List<Board> loss = new List<Board>();
            foreach (Board branch in _branches)
            {
                int ret = branch.Minmax(_next);
                if (ret > 0)
                {
                    wins.Add(branch);
                }
                else if (ret == 0)
                {
                    draws.Add(branch);
                }
                else  // ret < 0
                {
                    loss.Add(branch);
                }
            }
            if (wins.Count > 0)
            {
                _played = wins.First();
                return;
            }
            if (draws.Count > 0)
            {
                _played = draws.First();
                return;
            }
            if (loss.Count > 0)
            {
                _played = loss.First();
                return;
            }
            Debug.Assert(false, "The only move is not to play <o>!");
            return;
        }

        public int Minmax(Player p)
        {
            if (_ended)
            {
                if (_draw) { return 0; }
                else if (_winner == p) { return 1; }
                return -1;
            }
            int valor;
            if (_last == p)
            {
                valor = int.MaxValue;
                foreach (Board branch in _branches)
                {
                    valor = Math.Min(valor, branch.Minmax(p));
                }
            }
            else
            {
                valor = int.MinValue;
                foreach (Board branch in _branches)
                {
                    valor = Math.Max(valor, branch.Minmax(p));
                }
            }
            return valor;
        }

        public bool CheckCell(Vector2 pos, uint i, uint j)
        {
            Vector2 dest = new Vector2(i * Board._cell_size.X, j * Board._cell_size.Y);
            dest = _origin + (dest * _scale);
            return ((pos.X > dest.X) && (pos.X < (dest.X + Board._cell_size.X)) &&
                   (pos.Y > dest.Y) && (pos.Y < (dest.Y + Board._cell_size.Y)));
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
        public void SwapCell(Tuple<uint, uint> cell)
        {
            if (_cell[cell.Item1, cell.Item2] == null)
            {
                _cell[cell.Item1, cell.Item2] = Board._p1;
                return;
            }
            if (_cell[cell.Item1, cell.Item2] == Board._p1)
            {
                _cell[cell.Item1, cell.Item2] = Board._p2;
                return;
            }
            if (_cell[cell.Item1, cell.Item2] == Board._p2)
            {
                _cell[cell.Item1, cell.Item2] = null;
                return;
            }
            Debug.Assert(false, "Who played this?!");
        }

        public void Draw(SpriteBatch sb)
        {
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    Vector2 dest = new Vector2(i * Board._cell_size.X, j * Board._cell_size.Y);
                    if (_cell[i, j] != null)
                    {
                        sb.Draw(_cell[i, j]._texture,
                                _origin + (dest * _scale),
                                null,
                                Color.White,
                                0.0f, _origin,
                                _scale,
                                SpriteEffects.None, 0.5f);
                    }
                }
            }
        }
    }
}
