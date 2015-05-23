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
    public class Player
    {
        public static Player None;
        static Player()
        {
            None = new Player(Player.Type.None);
        }

        public Texture2D _texture;
        public enum Type
        {
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

    public class Board
    {
        public static Board _base;
        public static Player _p1;
        public static Player _p2;
        public static Vector2 _cell_size;
        public static Random _random;


        static Board()
        {
            _p1 = new Player(Player.Type.Human);
            _p2 = new Player(Player.Type.CPU);
            _base = CleanBoard();
            _base._next = _p1;
            _base._last = _p2;
            _base.BuildBranches();
            _random = new Random();
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


        public enum Mode
        {
            Inactive,
            Playing,
            Selection
        };
        public Mode _mode;
        public float _inactive;

        public Board(Player[,] cells)
        {
            this._cell = cells;
            _ended = false;
            _draw = false;
            _depth = 0;
            _branches = new List<Board>();
            _origin = new Vector2(100.0f, 100.0f);
            _scale = new Vector2(1.0f, 1.0f);
            _sep = new Vector2(0.2f, 0.2f);
            _z = 0.5f;
            _mode = Board.Mode.Inactive;
            _inactive = 0.5f;
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
            //_size = (3 * Vector2.One + 2 * _sep) * _cell_size * _scale;
            _size = (3 * Vector2.One) * _cell_size * _scale;
        }

        public void Update(MouseState mouse)
        {
            float x = mouse.Position.ToVector2().X;
            float y = mouse.Position.ToVector2().Y;

            if (x > _origin.X && x < _origin.X + _size.X &&
               y > _origin.Y && y < _origin.Y + _size.Y)
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
                _random.Next(wins.Count);
                _played = wins.ElementAt(_random.Next(wins.Count));
                    ;
                return;
            }
            if (draws.Count > 0)
            {
                _played = draws.ElementAt(_random.Next(draws.Count));
                return;
            }
            if (loss.Count > 0)
            {
                _played = loss.ElementAt(_random.Next(loss.Count));
                return;
            }
            Debug.Assert(false, "The only move is not to play <o>!");
            return;
        }

        public List<Board> BranchesInTime()
        {
            List<Board> l = new List<Board>();
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    if (_cell[i, j] == null)
                    {
                        Board t = this.Clone();
                        t.SetMove(i, j);
                        _branches.Add(t);
                    }
                }
            }
            return l;
        }

        public int MinmaxInTime(Player p)
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

        public void CleanPlayed()
        {
            _played = null;
            if (_ended) { return; }
            foreach (Board branch in _branches)
            {
                branch.CleanPlayed();
            }
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
            DrawLines(sb);
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    Vector2 dest = new Vector2(i * Board._cell_size.X, j * Board._cell_size.Y);
                    dest = _origin + (dest * _scale);
                    if (_cell[i, j] != null)
                    {
                        sb.Draw(_cell[i, j]._texture,
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
            Vector2 h1 = new Vector2(0.0f, Board._cell_size.Y);
            h1 = _origin + (h1 * _scale);
            Vector2 h2 = new Vector2(0.0f, Board._cell_size.Y * 2);
            h2 = _origin + (h2 * _scale);
            Vector2 v1 = new Vector2(Board._cell_size.X, 0.0f);
            v1 = _origin + (v1 * _scale);
            Vector2 v2 = new Vector2(Board._cell_size.X * 2, 0.0f);
            v2 = _origin + (v2 * _scale);

            Line.DrawLine(sb, h1, h1 + (_size * Vector2.UnitX));
            Line.DrawLine(sb, h2, h2 + (_size * Vector2.UnitX));
            Line.DrawLine(sb, v1, v1 + (_size * Vector2.UnitY));
            Line.DrawLine(sb, v2, v2 + (_size * Vector2.UnitY));
        }
    }
}
