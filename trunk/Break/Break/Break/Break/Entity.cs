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
    public abstract class Entity : DrawableGameComponent
    {
        public Vector2 position;
        public Rectangle hitbox;
        public DamageType damageType;
        public int strength;
        public bool active;

        public Entity(Game1 game)
            : base(game)
        {
        }

        public virtual int onCollide()
        {
            return 0;
            /* 0- Wall
             * 1- Timey-Wimey
             * 2- Gun
             * 3- Switch
             * 4- Player hitboxes
             * 5- Lock
             * 6- Key
             * 7- Enemy
             */
        }

        public virtual void reset()
        {

        }

        public virtual void getHit(DamageType dmgType)
        {

        }
    }
}
