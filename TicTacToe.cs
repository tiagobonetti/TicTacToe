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
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public KeyboardState _keyboard_last;
        public TicTacToeFSM _fsm;

        public TicTacToe()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            _fsm = new TicTacToeFSM(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            base.Initialize();
            _keyboard_last = Keyboard.GetState();
            IsMouseVisible = true;
        }
        protected override void LoadContent()
        {
            _fsm.LoadContent(Content);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Primitives.LoadContent(GraphicsDevice, Content);
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            MouseMgr.Update(Mouse.GetState());
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            _fsm.Update();
            _keyboard_last = Keyboard.GetState();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            _spriteBatch.Begin(
                SpriteSortMode.FrontToBack,
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
