using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Break
{
    class Switch : Entity
    {
        #region fields

        public int onDuration;
        public int onElapsed;
        public Texture2D on;
        public Texture2D off;
        private Game1 game;
        

        #endregion

        public Switch(Game1 game, Vector2 pos, int duration) : base(game)
        {
            this.game = game;
            position = pos;
            onDuration = duration;
            hitbox = new Rectangle((int)pos.X, (int)pos.Y, 64, 64);
            active = false;
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            on = game.Content.Load<Texture2D>("sprites/switch_on");
            off = game.Content.Load<Texture2D>("sprites/switch_off");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            game.spriteBatch.Begin();
            if(active) game.spriteBatch.Draw(on, position - game.cameraPos, Color.White);
            else game.spriteBatch.Draw(off, position - game.cameraPos, Color.White);
            game.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (active && onElapsed < onDuration)
            {
                onElapsed += gameTime.ElapsedGameTime.Milliseconds;
                //Console.WriteLine(onElapsed + "/" + onDuration);
            }
            if (onElapsed >= onDuration)
            {
                active = false;
                //Console.WriteLine("Status changed: true -> false");
                game.currentLevel.activeSwitches--;
                onElapsed = 0;
            }
        }

        public override int onCollide()
        {
            return 3;
        }

        public override void reset()
        {
            active = false;
            onElapsed = 0;
            game.currentLevel.activeSwitches = 0;
        }
    }
}
