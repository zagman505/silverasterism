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
    public class Player : Entity
    {
        #region fields

        public Vector2 room;
        public Texture2D [] textures = new Texture2D[4];
        public Texture2D[] textures_sword = new Texture2D[4];
        public Texture2D[] textures_gun = new Texture2D[4];
        public float speed = 4;
        public int facing = 0; //0 is front, 1 is left, 2 is back, 3 is right

        public SoundEffect sword;
        public SoundEffect gun;

        public Vector2 spawnPoint; // What is our spawn point?

        public bool dead; // Are we alive/updating?
        public bool autopilot; // Are we on autopilot/are we a ghost?

        public double timer; // Timer of how long is left in the present state.
        public List<ControlState> stateList; // List of controller states.
        public List<int> timeList; // List of times to remain in the above states. Originally stored as double durations in seconds, now stored as number of updates.
        public int index; // Our position in the lists of states.

        public int gunCooldown = 10;
        public int gunTimer = 10;

        public int swordCooldown = 10;
        public int swordTimer = 10;

        public bool canstab = false;
        public bool isShooting = false;
        public bool isStabbing = false;

        public int health;
        public int damageInvincibility = 2000;
        public int elapsed = 0;

        private Game1 game;

        public SoundEffect unlock;
        public SoundEffect breakEffect;
        public SoundEffect oof;

        #endregion

        public Player(Game1 game, List<ControlState> states, List<int> times, Vector2 spawn) : base(game)
        {
            this.game = game;
            spawnPoint = spawn;
            position = spawn;
            room = game.currentLevel.initialScreen;
            hitbox = new Rectangle((int)position.X, (int)position.Y, 64, 64);
            stateList = new List<ControlState>(states);
            timeList = new List<int>(times);
            index = 0;

            this.DrawOrder = 99;

            health = 100;

            timer = timeList[0];
            autopilot = true;

            if (game.currentChapter > 0)
            {
                canstab = true;
            }

            if (stateList.Count == 0)
                dead = true;
            else
                dead = false;
        }

        public Player(Game1 game, Vector2 spawn) : base(game)
        {
            this.game = game;
            room = game.currentLevel.initialScreen;
            spawnPoint = spawn;
            position = spawn;
            hitbox = new Rectangle((int)position.X, (int)position.Y, 64, 64);
            stateList = new List<ControlState>();
            timeList = new List<int>();
            index = 0;

            health = 100;

            this.DrawOrder = 99;

            if (game.currentChapter > 0)
            {
                canstab = true;
            }

            //timer = timeList[0];
            autopilot = false;
        }

        public override void Initialize()
        {
            //position = new Vector2(50, 50);
            index = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            textures[0] = game.Content.Load<Texture2D>("sprites/player_front");
            textures[1] = game.Content.Load<Texture2D>("sprites/player_side");
            textures[2] = game.Content.Load<Texture2D>("sprites/player_back");
            textures[3] = game.Content.Load<Texture2D>("sprites/player_sideR");

            textures_sword[0] = game.Content.Load<Texture2D>("sprites/player_front_sword1");
            textures_sword[1] = game.Content.Load<Texture2D>("sprites/player_side_swordL");
            textures_sword[2] = game.Content.Load<Texture2D>("sprites/player_back_sword");
            textures_sword[3] = game.Content.Load<Texture2D>("sprites/player_side_swordR");

            textures_gun[0] = game.Content.Load<Texture2D>("sprites/player_front_gun");
            textures_gun[1] = game.Content.Load<Texture2D>("sprites/player_side_gunL");
            textures_gun[2] = game.Content.Load<Texture2D>("sprites/player_back_gun");
            textures_gun[3] = game.Content.Load<Texture2D>("sprites/player_side_gunR");

            unlock = game.Content.Load<SoundEffect>("sounds/unlock1");
            breakEffect = game.Content.Load<SoundEffect>("sounds/break-time");
            sword = game.Content.Load<SoundEffect>("sounds/swoosh");
            gun = game.Content.Load<SoundEffect>("sounds/laser");
            oof = game.Content.Load<SoundEffect>("sounds/oof");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            game.spriteBatch.Begin();

            // chooses which sprite to draw based on whether player is attacking or not
            if (isShooting)
            {
                if (autopilot) game.spriteBatch.Draw(textures_gun[facing], position - game.cameraPos, Color.DarkGray);
                else game.spriteBatch.Draw(textures_gun[facing], position - game.cameraPos, Color.White);
                if (gunTimer < 0) isShooting = false;
            }
            /*else if (isStabbing)
            {
                game.spriteBatch.Draw(textures_sword[facing], position - game.cameraPos, Color.White);
                isStabbing = false;
            }*/
            else
            {
                if (autopilot) game.spriteBatch.Draw(textures[facing], position - game.cameraPos, Color.DarkGray);
                else game.spriteBatch.Draw(textures[facing], position - game.cameraPos, Color.White);
            }

            game.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (!game.isPaused)
            {
                ControlState state = ControlState.None;
                Vector2 formerPosition = position;
                if (!dead) // If we're in motion!
                {
                    // Autopilot code!
                    if (autopilot)
                    {
                        room.X = (int)(position.Y / 480);
                        room.Y = (int)(position.X / 800);
                        if (index < stateList.Count)
                        {
                            if (timer <= 0)
                            {
                                if (index < stateList.Count - 1)
                                    index++; // Increment our index for the next go-round.
                                else
                                    dead = true;
                                timer = timeList[index];
                            }
                            state = stateList[index];
                            //timer -= gameTime.ElapsedGameTime.TotalSeconds; // count down until the next event.         // Removed because it's using the float time rather than update times.
                            timer -= 1;
                        }
                        else
                        {
                            dead = true;
                        }

                    } // Done with that stuff!
                    else
                    {
                        state = game.currentEvents; // if not on autopilot, fetch the state from the game.
                        if (stateList.Count == 0)
                        {
                            stateList.Add(state);
                            timeList.Add(0);
                        }
                        if (state == stateList[stateList.Count - 1])
                            timeList[timeList.Count - 1]++;
                        else
                        {
                            stateList.Add(state);
                            timeList.Add(0);
                        }
                        // Check if can scroll to right
                        if (room.Y + 1 == game.currentLevel.dimensions.Y)
                        {
                            if (position.X - game.cameraPos.X > GraphicsDevice.Viewport.Width - 64)
                            {
                                position.X = formerPosition.X;
                            }
                        }
                        else
                        {
                            if (position.X - game.cameraPos.X > GraphicsDevice.Viewport.Width - 32 &&
                                game.currentLevel.grid[(int)room.X, (int)room.Y + 1].accessible)
                            {
                                room.Y++;
                                game.scroll(0);
                            }
                        }
                        // Check if can scroll to left
                        if (room.Y == 0)
                        {
                            if (position.X - game.cameraPos.X < 0)
                            {
                                position.X = formerPosition.X;
                            }
                        }
                        else
                        {
                            if (position.X - game.cameraPos.X < -32 &&
                                game.currentLevel.grid[(int)room.X, (int)room.Y - 1].accessible)
                            {
                                room.Y--;
                                game.scroll(2);
                            }
                        }
                        // Check if can scroll up
                        if (room.X == 0)
                        {
                            if (position.Y - game.cameraPos.Y < 0)
                            {
                                position.Y = formerPosition.Y;
                            }
                        }
                        else
                        {
                            if (position.Y - game.cameraPos.Y < -32 &&
                                game.currentLevel.grid[(int)room.X - 1, (int)room.Y].accessible)
                            {
                                room.X--;
                                game.scroll(1);
                            }
                        }
                        // Check if can scroll down
                        if (room.X + 1 == game.currentLevel.dimensions.X)
                        {
                            if (position.Y - game.cameraPos.Y > GraphicsDevice.Viewport.Height - 64)
                            {
                                position.Y = formerPosition.Y;
                            }
                        }
                        else
                        {
                            if (!autopilot && position.Y - game.cameraPos.Y > GraphicsDevice.Viewport.Height - 32 &&
                                game.currentLevel.grid[(int)room.X + 1, (int)room.Y].accessible)
                            {
                                room.X++;
                                game.scroll(3);
                            }
                        }
                    }
                    
                    // controls for attacking!
                    if (swordTimer < 0)
                    {
                        if (canstab && (state & ControlState.A) != ControlState.None)
                        {
                            stab();
                        }
                    }
                    else
                    {
                        swordTimer--;
                    }
                    if (game.currentLevel.canShoot)
                    {
                        if (gunTimer < 0)
                        {
                            if ((state & ControlState.B) != ControlState.None)
                            {
                                shoot();
                            }
                        }
                        else
                        {
                            gunTimer--;
                        }
                    }

                    // Behavior and suchlike go here. Move when we want to move, etc.
                    if ((state & ControlState.XPlus) != ControlState.None)
                    {
                        position.X += speed;
                        facing = 3;
                    }
                    if ((state & ControlState.XMinus) != ControlState.None)
                    {
                        position.X -= speed;
                        facing = 1;
                    }
                    if ((state & ControlState.YPlus) != ControlState.None)
                    {
                        position.Y -= speed;
                        facing = 2;
                    }
                    if ((state & ControlState.YMinus) != ControlState.None)
                    {
                        position.Y += speed;
                        facing = 0;
                    }
                }

                //if (position.X > GraphicsDevice.Viewport.Width - 64 || position.X < 0) position.X = formerPosition.X;
                //if (position.Y > GraphicsDevice.Viewport.Height - 64 || position.Y < 0) position.Y = formerPosition.Y;

                hitbox.X = (int)position.X;
                hitbox.Y = (int)position.Y;

                if (elapsed < damageInvincibility)
                {
                    elapsed += gameTime.ElapsedGameTime.Milliseconds;
                }

                // Collision reaction/detection, etc
                foreach (Entity e in game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects)
                {
                    if (hitbox.Intersects(e.hitbox))
                    {
                        Rectangle overlap;
                        switch (e.onCollide())
                        {
                            case 0:
                                // Walls, gates, shutters
                                if (e.active)
                                {
                                    overlap = Rectangle.Intersect(hitbox, e.hitbox);
                                    if (overlap.Width > overlap.Height)
                                    {
                                        if (position.Y < e.position.Y) position.Y -= overlap.Height;
                                        else position.Y += overlap.Height;
                                    }
                                    else
                                    {
                                        if (position.X < e.position.X) position.X -= overlap.Width;
                                        else position.X += overlap.Width;
                                    }
                                }
                                else
                                {
                                    // Do nothing
                                }
                                break;
                            case 1:
                                // Timey-wimeys
                                if (game.currentChapter == 0) breakEffect.Play();
                                game.currentChapter++;
                                game.startGame(game.currentChapter, game.saveFile);
                                break;
                            case 2:
                                // Gun
                                e.active = false;
                                game.Components.Remove(e);
                                game.currentLevel.canShoot = true;
                                break;
                            case 3:
                                // Switch acts as a wall to player
                                overlap = Rectangle.Intersect(hitbox, e.hitbox);
                                if (overlap.Width > overlap.Height)
                                {
                                    if (position.Y < e.position.Y) position.Y -= overlap.Height;
                                    else position.Y += overlap.Height;
                                }
                                else
                                {
                                    if (position.X < e.position.X) position.X -= overlap.Width;
                                    else position.X += overlap.Width;
                                }
                                break;
                            case 4:
                                // Collision with player's own attacks, do nothing
                                break;
                            case 5:
                                // Collision with locked doors
                                // Door is active and no keys, acts as wall
                                if (e.active && game.currentLevel.availableKeys == 0)
                                {
                                    overlap = Rectangle.Intersect(hitbox, e.hitbox);
                                    if (overlap.Width > overlap.Height)
                                    {
                                        if (position.Y < e.position.Y) position.Y -= overlap.Height;
                                        else position.Y += overlap.Height;
                                    }
                                    else
                                    {
                                        if (position.X < e.position.X) position.X -= overlap.Width;
                                        else position.X += overlap.Width;
                                    }
                                }
                                // Door is active and key, removes door
                                else if (e.active && game.currentLevel.availableKeys > 0)
                                {
                                    e.active = false;
                                    unlock.Play();
                                    game.currentLevel.availableKeys--;
                                }
                                // Door is inactive
                                else
                                {
                                    // Do nothing
                                }
                                break;
                            case 6:
                                // Pick up keys
                                if (e.active)
                                {
                                    e.active = false;
                                    game.currentLevel.availableKeys++;
                                }
                                break;
                            case 7:
                                // Get hit by enemies
                                if (e.active)
                                {
                                    if (elapsed >= damageInvincibility)
                                    {
                                        oof.Play();
                                        getHit(e.damageType);
                                    }
                                    else
                                    {
                                        // Do nothing, temporary invincibility
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        public override void getHit(DamageType dmgType)
        {
            elapsed = 0;
            switch (dmgType)
            {
                case DamageType.DamagePlayer:
                    health -= 5;
                    break;
                case DamageType.DamagePlayer2:
                    health -= 10;
                    break;
            }
            switch (facing)
            {
                case 0:
                    position.Y -= 64;
                    break;
                case 1:
                    position.X += 64;
                    break;
                case 2:
                    position.Y += 64;
                    break;
                case 3:
                    position.X -= 64;
                    break;
            }
            if (health <= 0)
            {
                game.resetLevel();
            }
        }

        public override void reset()
        {
            autopilot = true;
            dead = false;
            if (timeList.Count > 0) timer = timeList[0];
            position = spawnPoint;
            index = 0;
        }

        public void stab()
        {
            // update character sprite
            isStabbing = true;

            swordTimer = swordCooldown;

            // determine hitbox placement by facing
            int dx = 0, dy = 0, w = 0, h = 0;
            string spriteName = "";
            switch (facing)
            {
                case 0:
                    dx = 0;
                    dy = 64;
                    w = 64;
                    h = 64;
                    spriteName = "sword_D";
                    break;
                case 1:
                    w = 64;
                    h = 64;
                    dx = -64;
                    dy = 0;
                    spriteName = "sword_L";
                    break;
                case 2:
                    w = 64;
                    h = 64;
                    dx = 0;
                    dy = -64;
                    spriteName = "sword";
                    break;
                case 3:
                    w = 64;
                    h = 64;
                    dx = 64;
                    dy = 0;
                    spriteName = "sword_R";
                    break;
            }

            Vector2 stabHere = new Vector2(position.X + dx, position.Y + dy);

            // spawn hitbox in front
            Hitbox stab = new Hitbox(this.game, room, spriteName, stabHere, new Vector2(w, h), Vector2.Zero, 10, DamageType.DamageEnemy2, 5, 0);
            sword.Play();
            game.Components.Add(stab);
        }

        public void shoot()
        {
            // update character sprite
            isShooting = true;

            // spawn moving hitbox with laser

            gunTimer = gunCooldown;
            Vector2 vel = new Vector2(0, 0);
            string laser = "laser_";
            switch (facing)
            {
                case 0:
                    vel.Y = speed * 2;
                    laser += "v";
                    break;
                case 1:
                    vel.X = -speed * 2;
                    laser += "h";
                    break;
                case 2:
                    vel.Y = -speed * 2;
                    laser += "v";
                    break;
                case 3:
                    vel.X = speed * 2;
                    laser += "h";
                    break;
            }
            Hitbox shoot = new Hitbox(this.game, room, laser, position + new Vector2(32, 32), new Vector2(16, 16), vel, 20, DamageType.DamageEnemy, 1, 1);
            gun.Play();
            game.Components.Add(shoot);
        }
    }
}
