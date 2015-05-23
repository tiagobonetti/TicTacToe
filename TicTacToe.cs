using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace TicTacToe
{
    public class TicTacToe : Game
    {
        GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        // TODO: Add sprite fonts
        //SpriteFont _f;

        public Board _current;
        public KeyboardState _keyboard_last;
        public MouseState _mouse_last;

        public TicTacToeFSM _fsm;

        public TicTacToe()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            base.Initialize();
            // TODO: BUG
            _keyboard_last = Keyboard.GetState();
            _mouse_last = Mouse.GetState();
            _fsm = new TicTacToeFSM(this);
            IsMouseVisible = true;
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Board.LoadContent(Content);
            Line.LoadContent(GraphicsDevice);

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
            _fsm.Update();
            _keyboard_last = Keyboard.GetState();
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
                SamplerState.AnisotropicWrap,
                null,
                null,
                null,
                null);

            _fsm.Draw();
            _spriteBatch.End();
        }
    }
}
