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
    }

    public class NonePlayer : IPlayer
    {
        public Texture2D texture
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
        public Board Play(Board board)
        {
            Debug.Assert(false, "NonePLayer donsn't play");
            return board;
        }
    }
    public class HumanPlayer : IPlayer
    {
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
    }
    public class CPUPlayer : IPlayer
    {
        static Random _random;
        static CPUPlayer()
        {
            _random = new Random();
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

        Board IPlayer.Play(Board board)
        {
            List<Board> wins = new List<Board>();
            List<Board> draws = new List<Board>();
            List<Board> loss = new List<Board>();
            List<Board> branches = board.Branches();
            foreach (Board branch in branches)
            {
                int ret = branch.Minmax(this);
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
                return wins.ElementAt(_random.Next(wins.Count));
            }
            if (draws.Count > 0)
            {
                return draws.ElementAt(_random.Next(draws.Count));
            }
            if (loss.Count > 0)
            {
                return loss.ElementAt(_random.Next(loss.Count));
            }
            Debug.Assert(false, "The only move is not to play <o>!");
            return null;
        }
    }
}
