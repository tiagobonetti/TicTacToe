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
    public interface IState
    {
        StateMachine fsm { set; }
        void Enter();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        void Leave();
    }
    abstract public class BaseState
    {
        public StateMachine _fsm;
        public StateMachine fsm
        {
            set { _fsm = value; }
        }
    }
    public class MenuState : BaseState, IState
    {
        Slide _start_slide;
        ActionButton _start;
        Slide _first_slide;
        OptionButton _first;
        Slide _diff_slide;
        OptionButton _diff;
        void IState.Enter()
        {
            _start = new ActionButton("Start",
                                      new Vector2(100.0f, 250.0f),
                                      new Vector2(200.0f, 50.0f));
            _start_slide = new Slide(_start, new Vector2(-200.0f, 0.0f), 1.0f);

            _first = new OptionButton(_fsm._firstplayer,
                                     new Vector2(100.0f, 310.0f),
                                     new Vector2(200.0f, 50.0f));
            _first_slide = new Slide(_first, new Vector2(-250.0f, 0.0f), 1.0f);

            _diff = new OptionButton(_fsm._difficulty,
                                     new Vector2(100.0f, 370.0f),
                                     new Vector2(200.0f, 50.0f));
            _diff_slide = new Slide(_diff, new Vector2(-300.0f, 0.0f), 1.0f);
        }
        void IState.Update(GameTime gameTime)
        {
            if (_start.Update())
            {
                _fsm.ChangeState(StateMachine.State.Playing);
            }
            _first.Update();
            _diff.Update();
        }
        void IState.Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _start_slide.Draw(gameTime, spriteBatch);
            _first_slide.Draw(gameTime, spriteBatch);
            _diff_slide.Draw(gameTime, spriteBatch);
            Primitives.DrawText(spriteBatch, new Vector2(100.0f, 200.0f), "Menu:", Color.White);
        }
        void IState.Leave()
        {

            IPlayer p1 = new HumanPlayer();
            IPlayer p2 = BaseAI.BuildPlayer(_fsm._difficulty);

            if (_fsm._firstplayer == BasePlayer.Type.AI)
            {
                IPlayer t = p1;
                p1 = p2;
                p2 = t;
            }

            p1.texture = _fsm._x;
            p2.texture = _fsm._o;
            _fsm._board = new Board(p1, p2);
        }
    }

    public class PlayingState : BaseState, IState
    {
        float _cpu_timeout;
        void IState.Enter()
        {
            _cpu_timeout = 1.0f;
        }
        void IState.Update(GameTime gameTime)
        {
            if(_fsm._board._next is BaseAI ) {
                _cpu_timeout -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_cpu_timeout > 0)
                {
                    return;
                }
                _cpu_timeout = 2.0f;
            }
            _fsm._board.Update(Mouse.GetState());
            _fsm._board = _fsm._board._next.Play(_fsm._board);
            if (_fsm._board._ended)
            {
                _fsm.ChangeState(StateMachine.State.Result);
            }
        }
        void IState.Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _fsm._board.Draw(gameTime, spriteBatch);
        }
        void IState.Leave()
        {
        }
    }

    public class ResultState : BaseState, IState
    {
        void IState.Enter()
        {
        }
        void IState.Update(GameTime gameTime)
        {
            if (MouseMgr._left_down)
            {
                _fsm.ChangeState(StateMachine.State.Menu);
            }
        }
        void IState.Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            IPlayer winner = _fsm._board._winner;
            SpriteBatch sb = _fsm._game._spriteBatch;
            Vector2 pos = new Vector2(0.0f, 0.0f);

            if (winner == null)
            {
                Primitives.DrawText(sb, pos, "Meat and Metal, tied again.", Color.White);
            }
            else // We have a winner
            {
                winner.WinMsg(sb, pos);
            }
            _fsm._board.Draw(gameTime, spriteBatch);
        }
        void IState.Leave()
        {
        }
    }

    public class StateMachine
    {
        public enum State
        {
            Menu,
            Playing,
            Result
        }
        Dictionary<State, IState> _state_dic;
        public TicTacToe _game;
        public State _state;
        public DifficultyOption _difficulty;
        public FirstPlayerOption _firstplayer;
        public Board _board;
        public Texture2D _x;
        public Texture2D _o;

        public StateMachine(TicTacToe main)
        {
            _state_dic = new Dictionary<State, IState>();
            _state_dic[State.Menu] = new MenuState();
            _state_dic[State.Playing] = new PlayingState();
            _state_dic[State.Result] = new ResultState();
            foreach (var pair in _state_dic)
            {
                pair.Value.fsm = this;
            }

            _state = State.Menu;
            _firstplayer = new FirstPlayerOption();
            _difficulty = new DifficultyOption();
            _state_dic[_state].Enter();
            _game = main;

        }
        public void LoadContent(ContentManager content)
        {
            _x = content.Load<Texture2D>("TicTacToeX");
            _o = content.Load<Texture2D>("TicTacToeO");
        }
        public void ChangeState(State state)
        {
            _state_dic[_state].Leave();
            _state = state;
            _state_dic[_state].Enter();
        }
        public void Update(GameTime gameTime)
        {
            _state_dic[_state].Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _state_dic[_state].Draw(gameTime, spriteBatch);
        }

    }
}
