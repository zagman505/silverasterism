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
    public class Menu
    {
        private Game1 game;

        public bool newGame;
        public bool loadGame;
        public bool savesLoaded;

        // Selected option
        public int selected;
        /* Options:
         * 1- New Game
         * 2- Load Game
         * 3- Quit
         * */
        
        public int save1Chapter = 0;
        public int save2Chapter = 0;
        public int save3Chapter = 0;

        public int delay = 150;
        public int elapsed = 0;

        public Texture2D splashScreen;
        public Texture2D logo;
        public SpriteFont font;
        public SpriteFont title_font;

        public List<String> mainOptions;
        public List<String> saveFileOptions;


        public Menu(Game1 game)
        {
            this.game = game;

            newGame = false;
            loadGame = false;
            savesLoaded = false;
            selected = 0;

            mainOptions = new List<String>();
            mainOptions.Add("New Game");
            mainOptions.Add("Load Game");
            mainOptions.Add("Quit");
            
            saveFileOptions = new List<String>();
        }

        public void Draw(GameTime gameTime)
        {
            splashScreen = game.titleScreen;
            logo = game.logo;
            font = game.font;
            title_font = game.title_font;
            game.spriteBatch.Begin();
            game.spriteBatch.Draw(splashScreen, new Vector2(0,0), Color.White);
            game.spriteBatch.Draw(logo, new Vector2(8, 400), Color.White);
                                     

            if (newGame || loadGame)
            {
                if (newGame)
                {
                    game.spriteBatch.DrawString(title_font, "New Game", new Vector2(600, 150), Color.White);
                    game.spriteBatch.DrawString(font, "Starting a new game \non an existing file \nwill overwrite that file", new Vector2(600, 325), Color.White);
                }
                else
                {
                    game.spriteBatch.DrawString(title_font, "Load Game", new Vector2(600, 150), Color.White);
                }
                drawSaveSelectScreen();
            }
            else
            {
                game.spriteBatch.DrawString(title_font, "Break", new Vector2(600, 150), Color.White);
                for (int i = 0; i < mainOptions.Count; i++)
                {
                    if (i == selected)
                    {
                        game.spriteBatch.DrawString(font, mainOptions[i], new Vector2(600, 200 + (25 * i)), Color.DarkRed);
                    }
                    else
                    {
                        game.spriteBatch.DrawString(font, mainOptions[i], new Vector2(600, 200 + (25 * i)), Color.White);
                    }
                }

            }
            game.spriteBatch.End();
        }

        public void drawSaveSelectScreen()
        {
            if (!savesLoaded)
            {
                loadSaves();
                saveFileOptions.Add("Save 1 - Chapter " + save1Chapter);
                saveFileOptions.Add("Save 2 - Chapter " + save2Chapter);
                saveFileOptions.Add("Save 3 - Chapter " + save3Chapter);
                saveFileOptions.Add("Go back");
                savesLoaded = true;
            }
            for (int i = 0; i < saveFileOptions.Count; i++)
            {
                if (i == selected)
                {
                    game.spriteBatch.DrawString(font, saveFileOptions[i], new Vector2(600, 200 + (25 * i)), Color.DarkRed);
                }
                else
                {
                    game.spriteBatch.DrawString(font, saveFileOptions[i], new Vector2(600, 200 + (25 * i)), Color.White);
                }
            }
        }

        public void loadSaves()
        {
            // Load save file 1
            try
            {
                StreamReader reader = new StreamReader(@"../../../../BreakContent/saves/1.txt");
                string line = reader.ReadLine();
                save1Chapter = int.Parse(line[0].ToString());
                reader.Close();
            }
            catch (Exception e)
            {
                save1Chapter = 0;
            }
            // Load save file 2
            try
            {
                StreamReader reader = new StreamReader(@"../../../../BreakContent/saves/2.txt");
                string line = reader.ReadLine();
                save2Chapter = int.Parse(line[0].ToString());
                reader.Close();
            }
            catch (Exception e)
            {
                save2Chapter = 0;
            }
            // Load save file 3
            try
            {
                StreamReader reader = new StreamReader(@"../../../../BreakContent/saves/3.txt");
                string line = reader.ReadLine();
                save3Chapter = int.Parse(line[0].ToString());
                reader.Close();
            }
            catch (Exception e)
            {
                save3Chapter = 0;
            }
        }

        public void Update(GameTime gameTime)
        {
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            
            if (newGame || loadGame)
            {
                // Move stick up
                if (padState.ThumbSticks.Left.Y > game.y_threshhold && elapsed > delay)
                {
                    selected = (selected + saveFileOptions.Count - 1) % saveFileOptions.Count;
                    elapsed = 0;
                }
                // Move stick down
                if (padState.ThumbSticks.Left.Y < -game.y_threshhold && elapsed > delay)
                {
                    selected = (selected + saveFileOptions.Count + 1) % saveFileOptions.Count;
                    elapsed = 0;
                }
                if (padState.Buttons.A == ButtonState.Pressed && elapsed > delay)
                {
                    game.isPaused = false;

                    switch (selected)
                    {
                        case 0:
                            if (newGame) game.startGame(0, 1);
                            else game.startGame(save1Chapter, 1);
                            break;
                        case 1:
                            if (newGame) game.startGame(0, 2);
                            else game.startGame(save2Chapter, 2);
                            break;
                        case 2:
                            if (newGame) game.startGame(0, 3);
                            else game.startGame(save3Chapter, 3);
                            break;
                        case 3:
                            newGame = false;
                            loadGame = false;
                            break;
                    }
                    selected = 0;
                    elapsed = 0;
                }
                if (padState.Buttons.B == ButtonState.Pressed && elapsed > delay)
                {
                    newGame = false;
                    loadGame = false;
                    selected = 0;
                    elapsed = 0;
                }
            }
            else
            {
                // Move stick up
                if (padState.ThumbSticks.Left.Y > game.y_threshhold && elapsed > delay)
                {
                    selected = (selected + mainOptions.Count - 1) % mainOptions.Count;
                    elapsed = 0;
                }
                // Move stick down
                if (padState.ThumbSticks.Left.Y < -game.y_threshhold && elapsed > delay)
                {
                    selected = (selected + mainOptions.Count + 1) % mainOptions.Count;
                    elapsed = 0;
                }
                if (padState.Buttons.A == ButtonState.Pressed && elapsed > delay)
                {
                    switch (selected)
                    {
                        case 0:
                            newGame = true;
                            break;
                        case 1:
                            loadGame = true;
                            break;
                        case 2:
                            game.Exit();
                            break;
                    }
                    selected = 0;
                    elapsed = 0;
                }
            }
            if (elapsed < delay)
            {
                elapsed += gameTime.ElapsedGameTime.Milliseconds;
            }
        }
    }
}
