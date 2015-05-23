using Microsoft.Xna.Framework;
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
            if (fsm._game._mouse_last.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                fsm.ChangeState(TicTacToeFSM.State.Playing);
            }
        }
        void IMainState.Draw(TicTacToeFSM fsm)
        {
        }
        void IMainState.Leave(TicTacToeFSM fsm)
        {
        }
    }
    public class PlayingState : IMainState
    {
        void IMainState.Enter(TicTacToeFSM fsm)
        {
            fsm._game._current = Board._base;
        }
        void IMainState.Update(TicTacToeFSM fsm)
        {
            if (fsm._game._current._ended)
            {
                fsm.ChangeState(TicTacToeFSM.State.Result);
                return;
            }
            if (fsm._game._mouse_last.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Tuple<uint, uint> cell = fsm._game._current.CheckMouse(Mouse.GetState().Position.ToVector2());
                if (cell != null)
                {
                    fsm._game._current.Play(cell);
                    fsm._game._current = fsm._game._current._played;
                    if (!fsm._game._current._ended)
                    {
                        fsm._game._current.PlayMinmax();
                        fsm._game._current = fsm._game._current._played;
                    }
                }
            }
        }
        void IMainState.Draw(TicTacToeFSM fsm)
        {
            fsm._game._current.Draw(fsm._game._spriteBatch);
        }
        void IMainState.Leave(TicTacToeFSM fsm)
        {
            Board._base.CleanPlayed();
        }
    }
    public class ResultState : IMainState
    {
        void IMainState.Enter(TicTacToeFSM fsm)
        {
        }
        void IMainState.Update(TicTacToeFSM fsm)
        {
            if (fsm._game._mouse_last.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                fsm.ChangeState(TicTacToeFSM.State.Menu);
            }
        }
        void IMainState.Draw(TicTacToeFSM fsm)
        {
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
        public State _state;
        public TicTacToe _game;

        static Dictionary<State, IMainState> _state_dic;
        static TicTacToeFSM()
        {
            _state_dic = new Dictionary<State, IMainState>();
            _state_dic[State.Menu] = new MenuState();
            _state_dic[State.Playing] = new PlayingState();
            _state_dic[State.Result] = new ResultState();
        }
        public TicTacToeFSM(TicTacToe main)
        {
            _state = State.Menu;
            _game = main;
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
