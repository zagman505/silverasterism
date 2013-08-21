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
    public class Enemy : Entity
    {
        #region fields

        public Vector2 spawnPoint;
        public Vector2 room;
        public int type; // 0 = small, 1 = large, 2 = boss
        
        public int health;
        public int damageInvincibility = 500;
        public int elapsed = 0;

        public int walkDelay = 2000;
        public int delayElapsed = 0;

        public Texture2D smallMonster;
        public Texture2D[] textures = new Texture2D[4];
        public int facing; // 0 = down, 1 = right, 2 = up, 3 = left
        public float speed = 3;
        public Player target;
        static Random rand = new Random();

        public bool keyDropped;

        private Game1 game;

        public SoundEffect oof;

        #endregion
        
        public Enemy(Game1 game, Vector2 pos, Vector2 rm, int type) : base(game)
        {
            this.game = game;
            this.type = type;
            facing = 0;
            position = pos;
            hitbox = new Rectangle((int)pos.X, (int)pos.Y, 64, 64);
            spawnPoint = pos;
            room = rm;
            active = true;
            keyDropped = false;
            
            switch (type)
            {
                case 0:
                    health = 10;
                    damageType = DamageType.DamagePlayer;
                    break;
                case 1:
                    health = 50;
                    damageType = DamageType.DamagePlayer2;
                    break;
                case 2:
                    health = 100;
                    damageType = DamageType.DamagePlayer2;
                    break;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            oof = game.Content.Load<SoundEffect>("sounds/oof");
            switch (type)
            {
                case 0:
                    if (rand.Next() % 2 == 0) smallMonster = game.Content.Load<Texture2D>("sprites/monster_red");
                    else smallMonster = game.Content.Load<Texture2D>("sprites/monster_purple");
                    break;
                case 1:
                    textures[0] = game.Content.Load<Texture2D>("sprites/knight_front");
                    textures[1] = game.Content.Load<Texture2D>("sprites/knight_sideR");
                    textures[2] = game.Content.Load<Texture2D>("sprites/knight_back");
                    textures[3] = game.Content.Load<Texture2D>("sprites/knight_sideL");
                    break;
                case 2:
                    textures[0] = game.Content.Load<Texture2D>("sprites/knight_front");
                    textures[1] = game.Content.Load<Texture2D>("sprites/knight_sideR");
                    textures[2] = game.Content.Load<Texture2D>("sprites/knight_back");
                    textures[3] = game.Content.Load<Texture2D>("sprites/knight_sideL");
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (active)
            {
                game.spriteBatch.Begin();
                switch (type)
                {
                    case 0:
                        game.spriteBatch.Draw(smallMonster, position - game.cameraPos, Color.White);
                        break;
                    case 1:
                        game.spriteBatch.Draw(textures[facing], position - game.cameraPos, Color.White);
                        break;
                    case 2:
                        game.spriteBatch.Draw(textures[facing], position - game.cameraPos, Color.White);
                        break;
                }
                game.spriteBatch.End();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (active)
            {
                hitbox.Location = new Point((int)position.X, (int)position.Y);
                if (elapsed < damageInvincibility)
                {
                    elapsed += gameTime.ElapsedGameTime.Milliseconds;
                }
                // listen for player
                if (game.ghosts[0].room.Equals(room))
                {
                    //if attacked, update target

                    //if target is in range, attack                

                    //else pursue
                    target = game.ghosts[0];
                    walkTo(target.position);
                }
                else
                {
                    //wander
                    if (delayElapsed > walkDelay)
                    {
                        Vector2 wander = new Vector2(rand.Next(-5, 5), rand.Next(-5, 5));
                        wander.Normalize();
                        walkTo(wander);
                        delayElapsed = 0;
                    }
                    else
                    {
                        delayElapsed += gameTime.ElapsedGameTime.Milliseconds;
                    }
                }
            }
            else if (!active && type > 0 && !keyDropped)
            {
                Key key = new Key(game, position);
                game.currentLevel.grid[(int)room.X, (int)room.Y].containedObjects.Add(key);
                game.Components.Add(key);
                keyDropped = true;
            }
        }

        // Change return object back to Vector2 later!!
        public void walkTo(Vector2 goal) {
            // random trace algorithm
            float dx = position.X - goal.X;
            float dy = position.Y - goal.Y;

            if (dx > 0 && position.X - game.cameraPos.X > 64)
            {
                position.X -= speed;
                facing = 3;
            }
            if (dx < 0 && position.X - game.cameraPos.X < GraphicsDevice.Viewport.Width - 128)
            {
                position.X += speed;
                facing = 1;
            }
            if (dy > 0 && position.Y - game.cameraPos.Y > 64)
            {
                position.Y -= speed;
                facing = 2;
            }
            if (dy < 0 && position.Y - game.cameraPos.Y < GraphicsDevice.Viewport.Height - 128)
            {
                position.Y += speed;
                facing = 0;
            }
            // resolve collisions here?
        }

        public override int onCollide()
        {
            return 7;
        }

        public override void reset()
        {
            position = spawnPoint;
            active = true;
        }

        public override void getHit(DamageType dmgType)
        {
            oof.Play();
            elapsed = 0;
            switch (dmgType)
            {
                case DamageType.DamageEnemy:
                    health -= 2;
                    break;
                case DamageType.DamageEnemy2:
                    health -= 5;
                    switch (facing)
                    {
                        case 0:
                            position.Y -= 64;
                            break;
                        case 1:
                            position.X -= 64;
                            break;
                        case 2:
                            position.Y += 64;
                            break;
                        case 3:
                            position.X += 64;
                            break;
                    }
                    break;
            }
            if (health <= 0)
            {
                active = false;
            }
        }
    }
}
