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
        public uint _depth;
        public List<Board> _branches;

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
                _depth = this._depth
            };
        }

        public void BuildMove(Player p, uint i, uint j)
        {
            Debug.Assert((!_ended), "Ivalid Move: Game Ended!");
            Debug.Assert((_cell[i, j] == ' '), "Ivalid Move: Invalid cell");

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
            if (line || column || main || inv || draw)
            {
                _ended = true;
                if (draw) { _draw = true; }
                else { _winner = p; }
            }
        }

        public void BuildBranches(Player next, Player last)
        {
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    if (_cell[i, j] == ' ')
                    {
                        Board t = this.Clone();
                        t.BuildMove(next, i, j);
                        if (!t._ended) { t.BuildBranches(last, next); }
                        _branches.Add(t);
                    }
                }
            }
        }

        Player _playing;
        Player _oponent;

        public void Play(Player p, Player oponent, uint i, uint j)
        {
        }

        public int Minmax(Player next, Player last)
        {
            if (_ended)
            {
                if (_winner == _oponent) { return -1; }
                else if (_winner == _playing) { return +1; }
                else if (_draw) { return 0; }
                Debug.Assert(true, "WTF!");
            }
            if (next == _oponent)
            {
                int valor = int.MaxValue;
                foreach (Board branch in _branches)
                {
                    valor = Math.Min(valor, branch.Minmax(last, next));
                }
            }
            else
            {
                int valor = int.MinValue;
                foreach (Board branch in _branches)
                {
                    valor = Math.Max(valor, branch.Minmax(last, next));
                }
            }
            Debug.Assert(true, "WTF!");
            return 0;
        }

        public void Check(Vector2 pos)
        {
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 3; i++)
                {
                    Vector2 dest = new Vector2(i * Board._tsize.X, j * Board._tsize.Y);
                    dest = _origin + (dest * _scale);
                    if ((pos.X > dest.X) && (pos.X < (dest.X + Board._tsize.X)) &&
                        (pos.Y > dest.Y) && (pos.Y < (dest.Y + Board._tsize.Y)))
                    {
                        switch (_cell[i, j])
                        {
                            case ' ':
                                _cell[i, j] = 'X';
                                break;
                            case 'X':
                                _cell[i, j] = 'O';
                                break;
                            case 'O':
                                _cell[i, j] = ' ';
                                break;
                            default:
                                Debug.Assert(false, "WTF is this char doing here!");
                                break;
                        }
                    }
                }
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
