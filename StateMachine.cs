using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TicTacToe {
    public interface IState {
        StateMachine fsm { set; }
        void Enter();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        void Leave();
    }

    abstract public class BaseState {
        public StateMachine _fsm;
        public StateMachine fsm {
            set { _fsm = value; }
        }
    }

    public class MenuState : BaseState, IState {
        static Random _random;
        static MenuState() {
            _random = new Random();
        }

        ActionButton _start;
        OptionButton _first;
        OptionButton _diff;

        Slide _start_slide;
        Slide _first_slide;
        Slide _diff_slide;

        Vector2 _guide_pos;
        Vector2 _guide_size;

        void IState.Enter() {
            _guide_pos = new Vector2(_fsm._screen.X / 3.0f, _fsm._screen.Y / 2.0f);
            _guide_size = new Vector2(_guide_pos.X, 50.0f);

            _start = new ActionButton("Start",
                                      _guide_pos + Vector2.UnitY * _guide_size,
                                      _guide_size);
            _first = new OptionButton(_fsm._firstplayer,
                                      "First move",
                                      _guide_pos + Vector2.UnitY * _guide_size * 2.1f,
                                      _guide_size);
            _diff = new OptionButton(_fsm._difficulty,
                                     "Difficulty",
                                     _guide_pos + Vector2.UnitY * _guide_size * 3.2f,
                                     _guide_size);

            Vector2 slide_offset = -1.0f * Vector2.One * (_fsm._screen.Y - _guide_size.Y);
            _start_slide = new Slide(_start, slide_offset, 0.8f);
            _first_slide = new Slide(_first, slide_offset, 0.7f);
            _diff_slide = new Slide(_diff, slide_offset, 0.6f);
        }

        void IState.Update(GameTime gameTime) {
            if (_start.Update()) {
                if (_fsm._firstplayer == BasePlayer.Type.AI) {
                    _fsm.ChangeState(StateMachine.State.AI);
                }
                else {
                    _fsm.ChangeState(StateMachine.State.Human);
                }
            }
            _first.Update();
            _diff.Update();
        }

        void IState.Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            _start_slide.Draw(gameTime, spriteBatch);
            _first_slide.Draw(gameTime, spriteBatch);
            _diff_slide.Draw(gameTime, spriteBatch);
            Primitives.DrawText(spriteBatch, _fsm._title_pos, _fsm._title, Color.White,
                                font: Primitives.TextFont.Arial60B);
        }

        void IState.Leave() {

            IPlayer p1 = new HumanPlayer();
            IPlayer p2 = BaseAI.BuildPlayer(_fsm._difficulty);

            if (_fsm._firstplayer == BasePlayer.Type.AI) {
                IPlayer t = p1;
                p1 = p2;
                p2 = t;
            }
            p1.texture = _fsm._x;
            p2.texture = _fsm._o;
            _fsm._board = new Board(p1, p2);
            _fsm._board.position = (_fsm._screen / 2.0f) - (_fsm._board._size / 2.0f);
        }
    }

    public class HumanState : BaseState, IState {
        IPlayer p;

        void IState.Enter() {
            p = _fsm._board._next;
        }

        void IState.Update(GameTime gameTime) {
            _fsm._board.Update(Mouse.GetState());
            Board b = p.Play(_fsm._board);
            if (p == b._next) {
                return;
            }
            if (b._ended) {
                _fsm.ChangeState(StateMachine.State.Result);
            }
            else {
                _fsm._board = b;
                _fsm.ChangeState(StateMachine.State.AI);

            }
        }
        void IState.Draw(GameTime gameTime, SpriteBatch spriteBatch) {
                _fsm._board.Draw(gameTime, spriteBatch);
        }

        void IState.Leave() {
        }
    }

    public class AIState : BaseState, IState {
        IPlayer p;
        float _timeout;
        Wooble _wooble;

        void IState.Enter() {
            p = _fsm._board._next;
            _timeout = 1.0f;
            _wooble = new Wooble(_fsm._board, 5.0f, 1.0f);
        }

        void IState.Update(GameTime gameTime) {
            _timeout -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeout > 0) {
                return;
            }
            Board b = p.Play(_fsm._board);
            _fsm._board = b;
            if (b._ended) {
                _fsm.ChangeState(StateMachine.State.Result);
            }
            else {
                _fsm.ChangeState(StateMachine.State.Human);
            }
        }

        void IState.Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            _wooble.Draw(gameTime, spriteBatch);
        }

        void IState.Leave() {
        }
    }



    public class ResultState : BaseState, IState {
        Vector2 _endmsg_pos;

        void IState.Enter() {
            _endmsg_pos = new Vector2(0.5f, 0.9f) * _fsm._screen;
        }

        void IState.Update(GameTime gameTime) {
            if (MouseMgr._left_down) {
                _fsm.ChangeState(StateMachine.State.Menu);
            }
        }

        void IState.Draw(GameTime gameTime, SpriteBatch spriteBatch) {

            IPlayer winner = _fsm._board._winner;
            Vector2 pos = new Vector2(0.0f, 0.0f);
            if (winner == null) {
                Primitives.DrawText(spriteBatch, _endmsg_pos, "Draw: Perfectly matched.", Color.White,
                    font: Primitives.TextFont.Arial40);
            }
            else // We have a winner
            {
                winner.WinMsg(spriteBatch, _endmsg_pos);
            }
            _fsm._board.Draw(gameTime, spriteBatch);
        }

        void IState.Leave() {
        }
    }

    public class StateMachine {
        public enum State {
            Menu,
            Human,
            AI,
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
        public Vector2 _screen;

        public string _title;
        public Vector2 _title_pos;

        public StateMachine(TicTacToe main) {
            _game = main;
            _screen = new Vector2((float)main._graphics.PreferredBackBufferWidth,
                                  (float)main._graphics.PreferredBackBufferHeight);
            _title = "Tic Tac Toe!";
            _firstplayer = new FirstPlayerOption();
            _difficulty = new DifficultyOption();

            _state_dic = new Dictionary<State, IState>();
            _state_dic[State.Menu] = new MenuState();
            _state_dic[State.Human] = new HumanState();
            _state_dic[State.AI] = new AIState();
            _state_dic[State.Result] = new ResultState();
            foreach (var pair in _state_dic) {
                pair.Value.fsm = this;
            }
            _state = State.Menu;
            _state_dic[_state].Enter();
        }
        public void LoadContent(ContentManager content) {
            _x = content.Load<Texture2D>("TicTacToeX");
            _o = content.Load<Texture2D>("TicTacToeO");
            _title_pos = new Vector2(0.5f, 0.2f) * _screen;
        }
        public void ChangeState(State state) {
            _state_dic[_state].Leave();
            _state = state;
            _state_dic[_state].Enter();
        }
        public void Update(GameTime gameTime) {

            _state_dic[_state].Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            _state_dic[_state].Draw(gameTime, spriteBatch);
        }

    }
}
