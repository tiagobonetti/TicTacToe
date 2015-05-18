using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TicTacToe
{
    class Player
    {
        public char _mark;

        public Player(char mark)
        {
            _mark = mark;
        }
    }

    class Board
    {
        public static Texture2D _x, _o;
        public static Vector2 _tsize;

        public char[,] _cell;
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

        public Board(char[,] cells)
        {
            this._cell = cells;
            _ended = false;
            _draw = false;
            _depth = 0;
            _branches = new List<Board>();
            _origin = new Vector2();
            _scale = new Vector2(1.0f, 1.0f);
            _sep = new Vector2(0.2f, 0.2f);
        }

        public static Board CleanBoard()
        {
            return new Board(new char[,] { { ' ', ' ', ' ' }, { ' ', ' ', ' ' }, { ' ', ' ', ' ' } });
        }

        public Board Clone()
        {
            return new Board((char[,])_cell.Clone())
            {
                _ended = this._ended,
                _draw = this._draw,
                _winner = this._winner,
                _depth = this._depth,
                _next = this._next,
                _last = this._last,
                _origin = this._origin,
                _scale = this._scale,
                _sep = this._sep
            };
        }

        public void SetMove(uint i, uint j)
        {
            Debug.Assert((!_ended), "Ivalid Move: Game Ended!");
            Debug.Assert((_cell[i, j] == ' '), "Ivalid Move: Invalid cell");

            Player p = _next;
            _next = _last;
            _last = p;
            _depth++;

            bool line = true, column = true;
            bool main = true, inv = true; // Diagonals
            bool draw = true;

            _cell[i, j] = p._mark;

            for (uint n = 0; n < 3; n++)
            {
                for (uint m = 0; m < 3; m++)
                {
                    draw &= !(_cell[n, m] == ' ');
                }
                column &= (_cell[i, n] == p._mark);
                line &= (_cell[n, j] == p._mark);
                main &= _cell[n, n] == p._mark;
                inv &= _cell[2 - n, n] == p._mark;
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
                    if (_cell[i, j] == ' ')
                    {
                        Board t = this.Clone();
                        t.SetMove(i, j);
                        if (!t._ended) { t.BuildBranches(); }
                        _branches.Add(t);
                    }
                }
            }
        }

        public bool Play(Player p, Tuple<uint, uint> cell)
        {
            Debug.Assert(p == _next, "Wrong Player!");
            if (_cell[cell.Item1, cell.Item2] == ' ')
            {

                foreach (Board branch in _branches)
                {
                    if (branch._cell[cell.Item1, cell.Item2] == p._mark)
                    {
                        _played = branch;
                        return true;
                    }
                }
            }
            return false;
        }

        public void PlayMinmax(Player p)
        {
            Debug.Assert(p == _next, "Wrong CPU!");
            List<Board> wins = new List<Board>();
            List<Board> draws = new List<Board>();
            List<Board> loss = new List<Board>();
            foreach (Board branch in _branches)
            {
                int ret = branch.Minmax(p);
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

           if (loss.Count > 0)
            {
                _played = loss.First();
                return;
            }
           if (draws.Count > 0)
            {
                _played = draws.First();
                return;
            }
            if (wins.Count > 0)
            {
                _played = wins.First();
                return;
            }
            Debug.Assert(p == _next, "The only move is not to play <o>!");
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
            Vector2 dest = new Vector2(i * Board._tsize.X, j * Board._tsize.Y);
            dest = _origin + (dest * _scale);
            return ((pos.X > dest.X) && (pos.X < (dest.X + Board._tsize.X)) &&
                   (pos.Y > dest.Y) && (pos.Y < (dest.Y + Board._tsize.Y)));
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
            switch (_cell[cell.Item1, cell.Item2])
            {
                case ' ':
                    _cell[cell.Item1, cell.Item2] = 'X';
                    break;
                case 'X':
                    _cell[cell.Item1, cell.Item2] = 'O';
                    break;
                case 'O':
                    _cell[cell.Item1, cell.Item2] = ' ';
                    break;
                default:
                    Debug.Assert(false, "WTF is this char doing here!");
                    break;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    Vector2 dest = new Vector2(i * Board._tsize.X, j * Board._tsize.Y);
                    switch (_cell[i, j])
                    {
                        case ' ':
                            break;
                        case 'X':
                            sb.Draw(_x, _origin + (dest * _scale), null, Color.White, 0.0f, _origin, _scale, SpriteEffects.None, 0.5f);
                            break;
                        case 'O':
                            sb.Draw(_o, _origin + (dest * _scale), null, Color.White, 0.0f, _origin, _scale, SpriteEffects.None, 0.5f);
                            break;
                        default:
                            Debug.Assert(false, "WTF is this char doing here!");
                            break;
                    }
                }
            }
        }
    }
}
