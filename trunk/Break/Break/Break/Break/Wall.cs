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
    class Wall : Entity
    {
        public Texture2D texture;
        public Texture2D open;
        public Texture2D closed;
        public Vector2 gridPos;
        public int type; // 0 = regular, 1 = gate, 2 = emergency shutter
        public int switchCount;

        public bool canChange;

        public Game1 game;

        public Wall(Game1 game, Vector2 position, Vector2 gridPos, int type, int switches) : base(game)
        {
            this.game = game;
            this.type = type;
            this.position = position;
            this.gridPos = gridPos;
            hitbox = new Rectangle((int)position.X, (int)position.Y, 64, 64);
            switch (type)
            {
                case 0: // Normal wall
                    active = true;
                    canChange = false;
                    switchCount = switches - 1;
                    break;
                case 1: // Gate
                    active = true;
                    canChange = true;
                    switchCount = switches;
                    break;
                case 2: // Emergency shutter
                    active = false;
                    canChange = true;
                    switchCount = switches;
                    break;
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            if (type == 0)
            {
                if (gridPos.X == 0) texture = game.Content.Load<Texture2D>("sprites/wall_T");
                if (gridPos.X == 6) texture = game.Content.Load<Texture2D>("sprites/wall_B");
                if (gridPos.Y == 0) texture = game.Content.Load<Texture2D>("sprites/wall_L");
                if (gridPos.Y == 11) texture = game.Content.Load<Texture2D>("sprites/wall_R");
                if (gridPos.X == 0 && gridPos.Y == 0) texture = game.Content.Load<Texture2D>("sprites/wall_TL");
                if (gridPos.X == 0 && gridPos.Y == 11) texture = game.Content.Load<Texture2D>("sprites/wall_TR");
                if (gridPos.X == 6 && gridPos.Y == 0) texture = game.Content.Load<Texture2D>("sprites/wall_BL");
                if (gridPos.X == 6 && gridPos.Y == 11) texture = game.Content.Load<Texture2D>("sprites/wall_BR");
            }
            else
            {
                open = game.Content.Load<Texture2D>("sprites/gate_open");
                closed = game.Content.Load<Texture2D>("sprites/gate_3");
            }
            if (switchCount == -2)
            {
                texture = game.Content.Load<Texture2D>("sprites/tardis");
                this.DrawOrder = 100;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            game.spriteBatch.Begin();
            if (type == 0 && switchCount > -2) game.spriteBatch.Draw(texture, position - game.cameraPos, Color.White);
            if (switchCount == -2) game.spriteBatch.Draw(texture, position - game.cameraPos - new Vector2(0, 32), Color.White);
            if (type > 0 && active == true)
            {
                game.spriteBatch.Draw(closed, position - game.cameraPos, Color.White);
            }
            if (type > 0 && active == false)
            {
                game.spriteBatch.Draw(open, position - game.cameraPos, Color.White);
            }
            game.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (type > 0 && game.currentLevel.activeSwitches >= switchCount && canChange)
            {
                active = !active;
                canChange = false;
            }
        }

        public override int onCollide()
        {
            return 0;
        }

        public override void reset()
        {
            switch (type)
            {
                case 0: // Normal wall
                    active = true;
                    canChange = false;
                    break;
                case 1: // Gate
                    active = true;
                    canChange = true;
                    break;
                case 2: // Emergency shutter
                    active = false;
                    canChange = true;
                    break;
            }
        }
    }
}
