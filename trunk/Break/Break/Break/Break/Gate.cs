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
    class Gate : Entity
    {
        #region fields;

        public List<Switch> switches;
        public int num_switches;
        public bool isClosed;
        public Vector2 position;
        public Texture2D open;
        public Texture2D closed;
        private Game1 game;

        #endregion

        public Gate(Game1 game, Vector2 pos, params Vector2[] switch_pos) : base(game)
        {
            this.game = game;
            position = pos;
            num_switches = switch_pos.Length;

            //create the switches and add to the list
            for (int i = 0; i < num_switches; i++)
            {
                switches.Add(new Switch(game, switch_pos[i]));
            }
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            open = game.Content.Load<Texture2D>("sprites/gate_open");
            closed = game.Content.Load<Texture2D>("sprites/gate_closed");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            game.spriteBatch.Begin();
            if(isClosed) game.spriteBatch.Draw(closed, position - game.cameraPos, Color.White);
            else game.spriteBatch.Draw(open, position - game.cameraPos, Color.White);
            game.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (isClosed)
            {
                int onSwitches = 0;
                // check if switches are all on
                foreach (Switch s in switches)
                {
                    if (s.isOn) onSwitches++;
                }

                if (onSwitches == num_switches) isClosed = false;
            }
        }
    }
}
