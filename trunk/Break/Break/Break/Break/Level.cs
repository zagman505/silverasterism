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
    public class Level
    {
        public int totalChapters = 3;
        public int chapter;
        public Screen[,] grid;
        public Vector2 dimensions;
        public Vector2 initialScreen;
        public Cutscene cutscene;

        public int availableKeys;
        public int activeSwitches;
        public int switchDuration; // In milliseconds

        public bool canShoot;

        private Game1 game;

        public Level(Game1 game, int chapter)
        {
            this.game = game;
            this.chapter = chapter;
            availableKeys = 0;
            activeSwitches = 0;
            switch (chapter)
            {
                case 0:
                    switchDuration = 0; // No switches
                    canShoot = false;
                    break;
                case 1:
                    switchDuration = 1000; // Only one switch
                    canShoot = false;
                    break;
                case 2:
                    switchDuration = 10000; // Two switches
                    canShoot = true;
                    break;
                case 3:
                    switchDuration = 30000; // Three switches
                    canShoot = true;
                    break;
            }
            StreamReader reader = new StreamReader(@"../../../../BreakContent/levels/chaptersize.txt");
            string line;
            for (int i = 0; i <= totalChapters; i++)
            { // read in the lines, one by one.
                line = reader.ReadLine();

                if (i == chapter)
                {
                    if (line[0] == '^')
                    {
                        cutscene = new Cutscene(game);
                        line = reader.ReadLine(); // read in the next line
                        while (line[0] != '$')
                        {
                            cutscene.images.Add(game.Content.Load<Texture2D>("backgrounds/" + line.Trim()));
                            line = reader.ReadLine();
                            cutscene.textPositions.Add(new Vector2(int.Parse(line.Split()[0]), int.Parse(line.Split()[1])));
                            cutscene.captions.Add(reader.ReadLine().Replace("%%","\n"));
                            line = reader.ReadLine();
                            switch (line[0])
                            {
                                case '0':
                                    cutscene.colors.Add(Color.White);
                                    break;
                                case '1':
                                    cutscene.colors.Add(Color.Black);
                                    break;
                            }
                            line = reader.ReadLine();
                        }
                        line = reader.ReadLine(); // pop off another line so we actually get the chapter size.
                    }


                    dimensions = new Vector2(int.Parse(line[0].ToString()), int.Parse(line[2].ToString()));
                    initialScreen = new Vector2(int.Parse(line[4].ToString()), int.Parse(line[6].ToString()));
                    //Console.WriteLine(initialScreen.X + " " + initialScreen.Y);
                    grid = new Screen[(int)dimensions.X, (int)dimensions.Y];

                    //Console.WriteLine(dimensions.X + "  " + dimensions.Y + " " + initialScreen.X + " " + initialScreen.Y);
                }
                else
                {
                    if (line[0] == '^')
                    {
                        while (line[0] != '$')
                        {
                            line = reader.ReadLine();
                        }
                        line = reader.ReadLine();
                    }
                }
            }
            reader.Close();
            for (int i = 0; i < dimensions.X; i++)
            {
                for (int j = 0; j < dimensions.Y; j++)
                {
                    grid[i, j] = new Screen(game, chapter, i, j);
                }
            }

            /*for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridY; j++)
                {
                    Console.Write(grid[i, j].accessible + "\t");
                }
                Console.Write("\n");
            }*/
        }


    }
}
