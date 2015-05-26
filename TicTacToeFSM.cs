﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public interface IMainState
    {
        void Enter(TicTacToeFSM fsm);
        void Update(TicTacToeFSM fsm);
        void Draw(TicTacToeFSM fsm);
        void Leave(TicTacToeFSM fsm);
    }

    public class MenuState : IMainState
    {
        void IMainState.Enter(TicTacToeFSM fsm)
        {
        }
        void IMainState.Update(TicTacToeFSM fsm)
        {
            if (MouseMgr._left_down)
            {
                fsm.ChangeState(TicTacToeFSM.State.Playing);
            }
        }
        void IMainState.Draw(TicTacToeFSM fsm)
        {
            Text.DrawArial(fsm._game._spriteBatch, new Vector2(100.0f, 100.0f), "Menu", Color.White);
        }
        void IMainState.Leave(TicTacToeFSM fsm)
        {
        }
    }

    public class PlayingState : IMainState
    {
        void IMainState.Enter(TicTacToeFSM fsm)
        {
            IPlayer p1 = new HumanPlayer();
            p1.texture = fsm._x;
            IPlayer p2 = new CPUPlayer();
            p2.texture = fsm._o;
            fsm._board = new Board(p1, p2);
        }
        void IMainState.Update(TicTacToeFSM fsm)
        {
            fsm._board.Update(Mouse.GetState());
            fsm._board = fsm._board._next.Play(fsm._board);
            if (fsm._board._ended)
            {
                fsm.ChangeState(TicTacToeFSM.State.Result);
            }
        }
        void IMainState.Draw(TicTacToeFSM fsm)
        {
            fsm._board.Draw(fsm._game._spriteBatch);
        }
        void IMainState.Leave(TicTacToeFSM fsm)
        {
        }
    }

    public class ResultState : IMainState
    {
        void IMainState.Enter(TicTacToeFSM fsm)
        {
        }
        void IMainState.Update(TicTacToeFSM fsm)
        {
            if (MouseMgr._left_down)
            {
                fsm.ChangeState(TicTacToeFSM.State.Menu);
            }
        }
        void IMainState.Draw(TicTacToeFSM fsm)
        {

            IPlayer winner = fsm._board._winner;
            SpriteBatch sb = fsm._game._spriteBatch;
            Vector2 pos = new Vector2(0.0f, 0.0f);

            if (winner == null)
            {
                Text.DrawArial(sb, pos, "Meat and Metal, tied again.", Color.White);
            }
            else // We have a winner
            {
                winner.WinMsg(sb, pos);
            }
            fsm._board.Draw(sb);
        }
        void IMainState.Leave(TicTacToeFSM fsm)
        {
        }
    }

    public class TicTacToeFSM
    {
        public enum State
        {
            Menu,
            Playing,
            Result
        }
        static Dictionary<State, IMainState> _state_dic;
        static TicTacToeFSM()
        {
            _state_dic = new Dictionary<State, IMainState>();
            _state_dic[State.Menu] = new MenuState();
            _state_dic[State.Playing] = new PlayingState();
            _state_dic[State.Result] = new ResultState();
        }

        public TicTacToe _game;
        public State _state;
        public Board _board;
        public Texture2D _x;
        public Texture2D _o;

        public TicTacToeFSM(TicTacToe main)
        {
            _state = State.Menu;
            _game = main;
        }
        public void LoadContent(ContentManager content)
        {
            _x = content.Load<Texture2D>("TicTacToeX");
            _o = content.Load<Texture2D>("TicTacToeO");
        }
        public void ChangeState(State state)
        {
            _state_dic[_state].Leave(this);
            _state = state;
            _state_dic[_state].Enter(this);
        }
        public void Update()
        {
            _state_dic[_state].Update(this);
        }
        public void Draw()
        {
            _state_dic[_state].Draw(this);
        }

    }
}
