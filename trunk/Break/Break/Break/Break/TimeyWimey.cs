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
    public class TimeyWimey : Entity
    {
        private Game1 game;

        public Texture2D texture;

        public TimeyWimey(Game1 game, Vector2 pos) : base(game)
        {
            this.game = game;
            position = pos;
            hitbox = new Rectangle((int)pos.X, (int)pos.Y, 64, 64);
            active = true;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            switch (game.currentChapter)
            {
                case 0:
                    // Load TARDIS texture
                    texture = game.Content.Load<Texture2D>("sprites/tardis");
                    break;
                case 1:
                    // Load gear texture
                    texture = game.Content.Load<Texture2D>("sprites/tw_gear");
                    break;
                case 2:
                    // Load flux capacitator texture
                    texture = game.Content.Load<Texture2D>("sprites/tw_fluxcapacitor");
                    break;
                case 3:
                    // Load ray sphere texture
                    texture = game.Content.Load<Texture2D>("sprites/tw_raysphere");
                    break;
                case 4:
                    // Load TARDIS texture
                    texture = game.Content.Load<Texture2D>("sprites/tardis");
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            game.spriteBatch.Begin();
            if (game.currentChapter == 0 || game.currentChapter == 4)
                game.spriteBatch.Draw(texture, position-game.cameraPos- new Vector2(0,32), Color.White);
            else 
                game.spriteBatch.Draw(texture, position - game.cameraPos, Color.White);
            game.spriteBatch.End();
        }

        public override int onCollide()
        {
            return 1;
        }
    }
}