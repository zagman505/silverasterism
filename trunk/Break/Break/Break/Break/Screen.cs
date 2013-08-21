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
    public class Screen : DrawableGameComponent
    {
        public int chapter;
        public Vector2 posInLevel;
        public char[,] screenGrid;

        public bool accessible;

        private Game1 game;

        public List<Entity> containedObjects;

        public Screen(Game1 game, int chapter, int row, int col) : base(game)
        {
            this.game = game;
            this.chapter = chapter;
            containedObjects = new List<Entity>();
            posInLevel = new Vector2(row, col);

            try
            {
                StreamReader reader = new StreamReader(@"../../../../BreakContent/levels/" + chapter + "/" + row + col + ".txt");
                accessible = true;
                screenGrid = new char[7, 12];
                for (int i = 0; i < 7; i++)
                {
                    string line = reader.ReadLine();
                    for (int j = 0; j < 12; j++)
                    {
                        screenGrid[i, j] = line[j];
                    }
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                accessible = false;
            }

        }
    }
}
