using System;
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
    class Key : Entity
    {
        private Game1 game;
        public Texture2D texture;

        public Key(Game1 game, Vector2 pos) : base(game)
        {
            this.game = game;
            position = pos;
            hitbox = new Rectangle((int)pos.X, (int)pos.Y, 64, 64);
            active = true;
            this.DrawOrder = 98;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            texture = game.Content.Load<Texture2D>("sprites/key");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (active)
            {
                game.spriteBatch.Begin();
                game.spriteBatch.Draw(texture, position - game.cameraPos, Color.White);
                game.spriteBatch.End();
            }
        }

        public override int onCollide()
        {
            return 6;
        }
    }
}
