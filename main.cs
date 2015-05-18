using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public class main : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        SpriteFont _f;

        Player _p1 = new Player('X');
        Player _p2 = new Player('O');

        Board _base;
        Board _current;
        byte _index;

        KeyboardState _keyboard_last;
        MouseState _mouse_last;

        public main()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            base.Initialize();
            _p1 = new Player('X');
            _p2 = new Player('O');
            _base = Board.CleanBoard();
            _base._next = _p1;
            _base._last = _p2;
            _base.BuildBranches();
            _current = _base;
            _index = 0;
            _keyboard_last = Keyboard.GetState();
            _mouse_last = Mouse.GetState();
            IsMouseVisible = true;

        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Statr Board Statics
            Board._x = Content.Load<Texture2D>("TicTacToeX");
            Board._o = Content.Load<Texture2D>("TicTacToeO");
            Board._tsize = new Vector2(Math.Max(Board._x.Height, Board._o.Height),
                                       Math.Max(Board._x.Width, Board._o.Width));
            // _f = Content.Load<SpriteFont>("Miramob");
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (_keyboard_last.IsKeyUp(Keys.Space) &&
                Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _index += 1;
                if (_index == _current._branches.Count) { _index = 0; }
            }
            _keyboard_last = Keyboard.GetState();

            if (_mouse_last.LeftButton == ButtonState.Released &&
                Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!_current._ended)
                {
                    Tuple<uint, uint> cell = _current.CheckMouse(Mouse.GetState().Position.ToVector2());
                    if (cell != null)
                    {
                        //_current._branches[_bindex].SwapCell(cell);
                        _current.Play(_p1, cell);
                        _current = _current._played;
                        if (!_current._ended)
                        {
                            _current.PlayMinmax(_p2);
                            _current = _current._played;
                        }
                    }
                }
            }
            _mouse_last = Mouse.GetState();

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            _spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                null);

            _current.Draw(_spriteBatch);
            _spriteBatch.End();
        }
    }
}
