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
    public class Hitbox : Entity
    {
        #region fields

        public Vector2 room;
        public string sprite;
        public Texture2D texture;

        public Vector2 velocity;
        public int duration;

        private Game1 game;

        public int type; // 0 = stab, 1 = shoot, add others as necessary

        public SoundEffect powerup;

        #endregion
        
        public Hitbox(Game1 game, Vector2 rm, string sprite, Vector2 pos, Vector2 size, Vector2 velocity, int duration, DamageType type, int strength, int hitboxType) : base(game)
        {
            this.game = game;
            hitbox.X = (int)pos.X; hitbox.Y = (int)pos.Y; hitbox.Width = (int)size.X; hitbox.Height = (int)size.Y;
            room = rm;
            game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects.Add(this);
            this.sprite = sprite;

            this.velocity = velocity;
            this.position = pos;

            this.strength = strength;
            this.damageType = type;
            this.duration = duration;

            this.type = hitboxType;

            active = true;
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            texture = game.Content.Load<Texture2D>("sprites/" + sprite);
            powerup = game.Content.Load<SoundEffect>("sounds/switch_powerup");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            game.spriteBatch.Begin();
            game.spriteBatch.Draw(texture, position - game.cameraPos, Color.White);
            game.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            // Move!
            position += velocity;
            hitbox.Offset((int)velocity.X, (int)velocity.Y);
            duration--;

            foreach (Entity e in game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects)
            {
                if (hitbox.Intersects(e.hitbox))
                {
                    // Code for attacks interacting with entities
                    switch (e.onCollide())
                    {
                        // Walls, gates, shutters
                        case 0:
                            // Destroy attack if attack is shoot, stabs are unaffected
                            if (type == 1)
                            {
                                //game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects.Remove(this);
                                //game.Components.Remove(this);
                                active = false;
                            }
                            break;
                        // Similar behavior for hitting timey-wimey's
                        case 1:
                            if (type == 1)
                            {
                                //game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects.Remove(this);
                                //game.Components.Remove(this);
                                active = false;
                            }
                            break;
                        // Hitting gun object
                        case 2:
                            // Ignore if attack is sword, not possible if attack is shoot.
                            break;
                        // Hitting a switch
                        case 3:
                            // Only shoot is effective
                            if (type == 1)
                            {
                                e.active = true;
                                powerup.Play();
                                //Console.WriteLine("Status changed: false -> true");
                                game.currentLevel.activeSwitches++;
                                //game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects.Remove(this);
                                //game.Components.Remove(this);
                                active = false;
                            }
                            break;
                        case 4:
                            // Player hitboxes, do nothing upon collide
                            break;
                        case 5:
                            // Locked doors, destroy attack if attack is shoot, stabs are unaffected
                            if (type == 1)
                            {
                                //game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects.Remove(this);
                                //game.Components.Remove(this);
                                active = false;
                            }
                            break;
                        case 6:
                            // Keys, sword can collect
                            if (type == 0 && e.active)
                            {
                                e.active = false;
                                game.currentLevel.availableKeys++;
                            }
                            break;
                        case 7:
                            // Enemies
                            e.getHit(damageType);
                            active = false;
                            break;
                    }
                }
            }

            if (duration < 0 || !active)
            {
                game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects.Remove(this);
                game.Components.Remove(this);
            }
        }

        public override void reset()
        {
            game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects.Remove(this);
            game.Components.Remove(this);
        }

        public override int onCollide()
        {
            return 4;
        }
    }
}
