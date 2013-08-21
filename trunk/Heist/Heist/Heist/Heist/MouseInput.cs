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

namespace Heist
{
    static class MouseInput
    {
        static MouseState prevState = Mouse.GetState();
        static bool lc;
        static bool rc;
        static Point pos;
        public static bool leftClicked { get { return lc; } }
        public static bool rightClicked { get { return rc; } }
        public static Point position { get { return pos; } }

        public static void Update()
        {
            MouseState currentState = Mouse.GetState();
            pos = new Point(currentState.X, currentState.Y);
            if (prevState.LeftButton == ButtonState.Pressed && currentState.LeftButton == ButtonState.Released) lc = true;
            else lc = false;
            if (prevState.RightButton == ButtonState.Pressed && currentState.RightButton == ButtonState.Released) rc = true;
            else rc = false;
            prevState = currentState;
        }
    }
}
