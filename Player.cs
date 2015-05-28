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
    public interface IPlayer
    {
        Texture2D texture
        {
            get;
            set;
        }
        Board Play(Board board);
        void WinMsg(SpriteBatch sb, Vector2 pos);
    }
    public abstract class BasePlayer
    {
        public enum Type
        {
            None,
            Human,
            AI
        };
        public static IPlayer BuildPlayer(Type type,
                                          BaseAI.Difficulty difficulty = BaseAI.Difficulty.Normal)
        {
            switch (type)
            {
                case Type.None:
                    return null;
                case Type.Human:
                    return new HumanPlayer();
                case Type.AI:
                    return BaseAI.BuildPlayer(difficulty);
                default:
                    break;
            }
            Debug.Assert(false, "Thats not a player type");
            return null;
        }
        public Texture2D _texture;
        public Texture2D texture
        {
            get
            {
                return _texture;
            }
            set
            {
                _texture = value;
            }
        }
    }
    public class HumanPlayer : BasePlayer, IPlayer
    {
        Board IPlayer.Play(Board board)
        {
            if (board._cell_clicked != null)
            {
                board.SetMove(board._cell_clicked.Item1,
                             board._cell_clicked.Item2);
                board._cell_clicked = null;
            }
            return board;
        }
        void IPlayer.WinMsg(SpriteBatch sb, Vector2 pos)
        {
            Primitives.DrawText(sb, pos, "Player Wins!\n\rSuck that software!", Color.White);
        }
    }
    public abstract class BaseAI : BasePlayer
    {
        public enum Difficulty
        {
            Hard,
            Normal,
            Easy
        };
        public static Random _random;
        static BaseAI()
        {
            _random = new Random();
        }
        public static IPlayer BuildPlayer(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Hard:
                    return new HardAI();
                case Difficulty.Normal:
                    return new NormalAI();
                case Difficulty.Easy:
                    return new EasyAI();
                default:
                    break;
            }
            Debug.Assert(false, "No patric, DarkSouls is not a difficulty!");
            return null;
        }

        public void WinMsg(SpriteBatch sb, Vector2 pos)
        {
            Primitives.DrawText(sb, pos, "CPU Wins!\n\rPuny meatbag.", Color.White);
        }
    }

    public class HardAI : BaseAI, IPlayer
    {
        Board IPlayer.Play(Board board)
        {
            Minmax mm = new Minmax(board, this);

            if (mm._wins.Count > 0)
            {
                return mm._wins.ElementAt(_random.Next(mm._wins.Count));
            }
            if (mm._draws.Count > 0)
            {
                return mm._draws.ElementAt(_random.Next(mm._draws.Count));
            }
            if (mm._loss.Count > 0)
            {
                return mm._loss.ElementAt(_random.Next(mm._loss.Count));
            }
            Debug.Assert(false, "The only move is not to play <o>!");
            return null;
        }
    }
    public class NormalAI : BaseAI, IPlayer
    {
        Board IPlayer.Play(Board board)
        {
            if (_random.Next(2) > 0)
            {
                List<Board> b = board.Branches();
                return b.ElementAt(_random.Next(b.Count));
            }
            else
            {
                Minmax mm = new Minmax(board, this);
                if (mm._wins.Count > 0)
                {
                    return mm._wins.ElementAt(_random.Next(mm._wins.Count));
                }
                if (mm._draws.Count > 0)
                {
                    return mm._draws.ElementAt(_random.Next(mm._draws.Count));
                }
                if (mm._loss.Count > 0)
                {
                    return mm._loss.ElementAt(_random.Next(mm._loss.Count));
                }
                Debug.Assert(false, "The only move is not to play <o>!");
                return null;
            }
        }
    }
    public class EasyAI : BaseAI, IPlayer
    {
        Board IPlayer.Play(Board board)
        {
            Minmax mm = new Minmax(board, this);
            if (mm._loss.Count > 0)
            {
                return mm._loss.ElementAt(_random.Next(mm._loss.Count));
            }
            if (mm._draws.Count > 0)
            {
                return mm._draws.ElementAt(_random.Next(mm._draws.Count));
            }
            if (mm._wins.Count > 0)
            {
                return mm._wins.ElementAt(_random.Next(mm._wins.Count));
            }
            Debug.Assert(false, "The only move is not to play <o>!");
            return null;
        }
    }
}
