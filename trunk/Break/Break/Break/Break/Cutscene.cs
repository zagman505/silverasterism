using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Break
{
    public class Cutscene
    {
        private Game1 game;

        public List<Texture2D> images;
        public List<Vector2> textPositions;
        public List<String> captions;
        public List<Color> colors;

        public int delay = 200;
        public int elapsed = 0;
        public int frame;

        public SpriteFont font;

        public Cutscene(Game1 game)
        {
            this.game = game;

            frame = 0;

            images = new List<Texture2D>();
            textPositions = new List<Vector2>();
            captions = new List<string>();
            colors = new List<Color>();
        }

        public void Draw(GameTime gameTime)
        {
            font = game.font;

            game.spriteBatch.Begin();
            game.spriteBatch.Draw(images[frame], new Vector2(0,0), Color.White);
            game.spriteBatch.DrawString(game.font, captions[frame], textPositions[frame], colors[frame]);
            
            game.spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (padState.Buttons.A == ButtonState.Pressed && elapsed > delay)
            {
                if (frame < images.Count - 1)
                    frame++;
                else
                {
                    game.isCutscene = false;
                    frame = 0;
                }
                elapsed = 0;
            }
            
            if (elapsed < delay)
            {
                elapsed += gameTime.ElapsedGameTime.Milliseconds;
            }
        }
    }
}
