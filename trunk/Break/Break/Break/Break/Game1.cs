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
    [Flags]
    public enum DamageType
    {
        None = 0x0,
        KnockBackPlayer = 0x1,
        KnockBackEnemy = 0x2,
        DamagePlayer = 0x4,
        DamageEnemy = 0x8,
        ActivateStuff = 0x10,
        DamagePlayer2 = 0x20,
        DamageEnemy2 = 0x40
    }
    [Flags]
    public enum ControlState : byte
    {
        None = 0,
        XPlus = 1,
        XMinus = 2,

        YPlus = 4,
        YMinus = 8,

        A = 16,
        B = 32
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public List<Player> ghosts;
        public Vector2 playerSpawn;

        public ControlState currentEvents; // Current controller state.
        
        public bool breakButtonPressed = false; // Are we on autopilot?
        public bool pauseButtonPressed = false;

        public double x_threshhold = .2;
        public double y_threshhold = .2;

        public int saveFile; // Use for saving games?
        public bool newGame;
        public int currentChapter;
        public Level currentLevel;
        public int scrollDirection; // 0 is right, 1 is up, 2 is left, 3 is down
        public int scrollAmount;

        public Vector2 cameraPos;

        public int maxGhosts = 4; // maximum number of ghosts we can have.

        public Menu mainMenu;

        // Game state booleans
        public bool isMainMenu = true;
        public bool isPaused = false;
        public bool isScrolling = false;
        public bool isCutscene = false;

        public Texture2D titleScreen;
        public Texture2D floor_texture;
        public Texture2D logo;
        public SpriteFont font;
        public SpriteFont title_font;

        // Sounds!
        public SoundEffect bgmusic_menu;
        public SoundEffectInstance menu_instance;
        public SoundEffect bgmusic_ch1;
        public SoundEffectInstance ch1_instance;
        public SoundEffect sfx_break;

        public int breakTime;
        public int elapsed = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            mainMenu = new Menu(this);
        }

        public void startGame(int chapter, int saveNum)
        {
            ghosts = new List<Player>();
            Components.Clear();
            isMainMenu = false;

            saveFile = saveNum;
            currentChapter = chapter;
            switch (currentChapter)
            {
                case 0:
                    break;
                case 1:
                    breakTime = 15000;
                    break;
                case 2:
                    breakTime = 30000;
                    break;
                case 3:
                    breakTime = 45000;
                    break;
            }
            newGame = true;
            loadLevel(currentChapter);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            titleScreen = Content.Load<Texture2D>("backgrounds/pocketwatch");
            logo = Content.Load<Texture2D>("silver asterism");
            floor_texture = Content.Load<Texture2D>("sprites/floor_tile");
            font = Content.Load<SpriteFont>("fonts/Arial");
            title_font = Content.Load<SpriteFont>("fonts/ArialTitle");
            bgmusic_menu = Content.Load<SoundEffect>("sounds/24046__bebeto__loop029");
            menu_instance = bgmusic_menu.CreateInstance();
            bgmusic_ch1 = Content.Load<SoundEffect>("sounds/18973__bebeto__loop003-jungle");
            ch1_instance = bgmusic_ch1.CreateInstance();
            sfx_break = Content.Load<SoundEffect>("sounds/break-time");
        }
                
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (isMainMenu)
            {
                if (menu_instance.State == SoundState.Stopped)
                {
                    menu_instance.Volume = 0.75f;
                    menu_instance.IsLooped = true;
                    menu_instance.Play();
                }
                else
                {
                    menu_instance.Resume();
                }
                UpdateMainMenu(gameTime);
            }
            else if (isPaused)
            {
                if (ch1_instance.State == SoundState.Playing)
                {
                    ch1_instance.Pause();
                }
                UpdatePause(padState);
            }
            else if (isScrolling) UpdateScroll(padState);
            else if (isCutscene) currentLevel.cutscene.Update(gameTime);
            else
            {
                if (menu_instance.State == SoundState.Playing)
                {
                    menu_instance.Pause();
                }
                if (ch1_instance.State == SoundState.Stopped)
                {
                    ch1_instance.Volume = 0.75f;
                    ch1_instance.IsLooped = true;
                    ch1_instance.Play();
                }
                else
                {
                    ch1_instance.Resume();
                }
                if (elapsed > breakTime && currentChapter > 0 && currentChapter < 4)
                {
                    
                    resetLevel();
                    elapsed = 0;
                }
                else
                {
                    elapsed += gameTime.ElapsedGameTime.Milliseconds;
                    UpdateGame(gameTime, padState);
                    base.Update(gameTime);
                }
            }
        }

        // Initiate screen scroll
        public void scroll(int direction)
        {
            isScrolling = true;
            scrollDirection = direction;
            scrollAmount = 0;
        }

        public void scrollCamera(int amount)
        {
            scrollAmount += amount;
            switch (scrollDirection)
            {
                case 0:
                    cameraPos.X += amount;
                    break;
                case 1:
                    cameraPos.Y -= amount;
                    break;
                case 2:
                    cameraPos.X -= amount;
                    break;
                case 3:
                    cameraPos.Y += amount;
                    break;
            }
        }

        protected void UpdateScroll(GamePadState padState)
        {
            scrollCamera(10);
            if (scrollDirection % 2 == 0 && scrollAmount >= 800) isScrolling = false;
            if (scrollDirection % 2 != 0 && scrollAmount >= 480) isScrolling = false;
        }

        protected void UpdatePause(GamePadState padState)
        {
            // select options
            if (padState.Buttons.Start == ButtonState.Pressed && !pauseButtonPressed)
            {
                isPaused = false;
                pauseButtonPressed = true;
            }
            if (padState.Buttons.Start == ButtonState.Released) pauseButtonPressed = false;
            if (padState.Buttons.Back == ButtonState.Pressed) // Exit on back button
            {
                saveGame();
                isMainMenu = true;
            }
        }

        public void saveGame()
        {
            StreamWriter writer = new StreamWriter(@"../../../../BreakContent/saves/" + saveFile + ".txt", false);
            writer.WriteLine(currentChapter);
            writer.Close();
            mainMenu.newGame = false;
            mainMenu.loadGame = false;
            mainMenu.savesLoaded = false;
            mainMenu.saveFileOptions.Clear();
        }

        protected void UpdateGame(GameTime gameTime, GamePadState padState) {
            /*if (padState.Buttons.Back == ButtonState.Pressed) // Exit on back button
                this.Exit();*/

            /*if (padState.Buttons.LeftShoulder == ButtonState.Pressed && !breakButtonPressed)
            {
                sfx_break.Play();
                resetLevel();
                breakButtonPressed = true; // prevent us from doublecounting button presses.
            }
            if (padState.Buttons.LeftShoulder == ButtonState.Released)
                breakButtonPressed = false; // now we've released, so we can use a break again.
            */
            // to bring up pause menu
            if (padState.Buttons.Start == ButtonState.Pressed && !pauseButtonPressed)
            {
                isPaused = true;
                pauseButtonPressed = true;
            }
            if (padState.Buttons.Start == ButtonState.Released) 
                pauseButtonPressed = false;

            // Record input as the current event/state
            currentEvents = ControlState.None;
            // Build the control state.
            if (padState.ThumbSticks.Left.X > x_threshhold) currentEvents |= ControlState.XPlus;
            else if (padState.ThumbSticks.Left.X < -x_threshhold) currentEvents |= ControlState.XMinus;
            if (padState.ThumbSticks.Left.Y > y_threshhold) currentEvents |= ControlState.YPlus;
            else if (padState.ThumbSticks.Left.Y < -y_threshhold) currentEvents |= ControlState.YMinus;
            if (padState.Buttons.A == ButtonState.Pressed) currentEvents |= ControlState.A;
            if (padState.Buttons.B == ButtonState.Pressed) currentEvents |= ControlState.B;
        }

        public void UpdateMainMenu(GameTime gameTime)
        {
            mainMenu.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (isMainMenu)
            {
                mainMenu.Draw(gameTime);
            }
            else if (isCutscene)
            {
                currentLevel.cutscene.Draw(gameTime);
            }
            else if (isPaused)
            {
                base.Draw(gameTime);
                DrawPause();
            }
            else
            {
                DrawGame();
                base.Draw(gameTime);
            }                      
                        
        }

        //

        protected void DrawPause()
        {
            //GraphicsDevice.Clear(Color.Crimson);
            spriteBatch.Begin();

            spriteBatch.DrawString(title_font, "GAME PAUSED", new Vector2(285, 150), Color.Black);
            spriteBatch.DrawString(font, "   Press BACK to exit to title menu. \nThe chapter you are on will be saved.", new Vector2(240, 300), Color.Black);

            /*for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    spriteBatch.Draw(floor_texture,
                        new Vector2((float)(((j * 800) / 12) + 1 - (cameraPos.X % 66.66667)),
                            (float)(((i * 480) / 7) + 2 - (cameraPos.Y % 68.5714))), Color.White);
                }
            }*/
            spriteBatch.End();
        }

        protected void DrawGame()
        {
            GraphicsDevice.Clear(Color.Gray);
            spriteBatch.Begin();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    spriteBatch.Draw(floor_texture, 
                        new Vector2((float)(((j * 800) / 12) + 1 - (cameraPos.X % 66.66667)), 
                            (float)(((i * 480) / 7) + 2 - (cameraPos.Y % 68.5714))), Color.White);
                }
            }
            spriteBatch.End();
        }

        public void resetLevel()
        {
            sfx_break.Play();
            // Reset camera
            cameraPos = Vector2.Zero;

            // Reset players
            ghosts[0].reset();
            if (ghosts.Count >= maxGhosts)
            {
                Components.Remove(ghosts[ghosts.Count - 1]); // Remove extra ghosts.
                ghosts.RemoveAt(ghosts.Count - 1);
            }
            foreach (Player p in ghosts)
                p.reset();
            ghosts.Insert(0, new Player(this, playerSpawn));
            Components.Add(ghosts[0]);

            // Reset objects in level
            for (int i = 0; i < currentLevel.dimensions.X; i++)
            {
                for (int j = 0; j < currentLevel.dimensions.Y; j++)
                {
                    if (currentLevel.grid[i, j].accessible)
                    {
                        foreach (Entity e in currentLevel.grid[i, j].containedObjects)
                        {
                            e.reset();
                        }
                    }
                }
            }
        }

        public void loadLevel(int chapter)
        {
            //int screenX = GraphicsDevice.Viewport.Width;
            //int screenY = GraphicsDevice.Viewport.Height;
            //Console.WriteLine(chapter);
            currentLevel = new Level(this, chapter);
            
            for (int i = 0; i < currentLevel.dimensions.X; i++)
            {
                for (int j = 0; j < currentLevel.dimensions.Y; j++)
                {

                    if (currentLevel.grid[i, j].accessible)
                    {
                        for (int k = 0; k < 7; k++)
                        {
                            for (int l = 0; l < 12; l++)
                            {
                                switch (currentLevel.grid[i, j].screenGrid[k, l])
                                {
                                    case '0':
                                        // Empty space
                                        break;
                                    case '1':
                                        // Code for walls
                                        Wall w = new Wall(this, 
                                            new Vector2(((l * 800) / 12) + ((j-currentLevel.initialScreen.Y)*800)+ 1, 
                                                ((k * 480) / 7) + ((i-currentLevel.initialScreen.X)*480) + 2), new Vector2(k, l), 0, 0);
                                        currentLevel.grid[i, j].containedObjects.Add(w);
                                        Components.Add(w);
                                        break;
                                    case '2':
                                        // Code for characters
                                        //cameraPos = new Vector2(currentLevel.initialScreen.Y*800, currentLevel.initialScreen.X*480);
                                        cameraPos = new Vector2(0, 0);

                                        //if (newGame)
                                        //{
                                        playerSpawn = new Vector2(((l * 800) / 12) + ((j - currentLevel.initialScreen.Y) * 800) + 1,
                                            ((k * 480) / 7) + ((i - currentLevel.initialScreen.X) * 480) + 2);
                                        ghosts.Clear();
                                        ghosts.Add(new Player(this, playerSpawn));
                                        Components.Add(ghosts[0]);
                                        //newGame = false;
                                        /*}
                                        else
                                        {
                                            ghosts[0].reset();
                                            if (ghosts.Count >= maxGhosts)
                                            {
                                                Components.Remove(ghosts[ghosts.Count - 1]); // Remove extra ghosts.
                                                ghosts.RemoveAt(ghosts.Count - 1);
                                            }
                                            foreach(Player p in ghosts) 
                                                p.reset();
                                            ghosts.Insert(0, new Player(this, new Vector2((l * 800) / 12, (k*480) / 7)));
                                            Components.Add(ghosts[0]);
                                        }*/
                                        break;
                                    case '3':
                                        // TARDIS
                                        w = new Wall(this, 
                                            new Vector2(((l * 800) / 12) + ((j-currentLevel.initialScreen.Y)*800)+ 1, 
                                                ((k * 480) / 7) + ((i-currentLevel.initialScreen.X)*480) + 2), new Vector2(k, l), 0, -1);
                                        currentLevel.grid[i, j].containedObjects.Add(w);
                                        Components.Add(w);
                                        break;
                                    case '4':
                                        // Small enemy
                                        Enemy e = new Enemy(this, new Vector2(((l * 800) / 12) + ((j - currentLevel.initialScreen.Y) * 800) + 1,
                                                ((k * 480) / 7) + ((i - currentLevel.initialScreen.X) * 480) + 2), new Vector2(i, j), 0);
                                        currentLevel.grid[i, j].containedObjects.Add(e);
                                        Components.Add(e);
                                        break;
                                    case '5':
                                        // Large enemy
                                        e = new Enemy(this, new Vector2(((l * 800) / 12) + ((j - currentLevel.initialScreen.Y) * 800) + 1,
                                                ((k * 480) / 7) + ((i - currentLevel.initialScreen.X) * 480) + 2), new Vector2(i, j), 1);
                                        currentLevel.grid[i, j].containedObjects.Add(e);
                                        Components.Add(e);
                                        break;
                                    case '6':
                                        // Boss
                                        e = new Enemy(this, new Vector2(((l * 800) / 12) + ((j - currentLevel.initialScreen.Y) * 800) + 1,
                                                ((k * 480) / 7) + ((i - currentLevel.initialScreen.X) * 480) + 2), new Vector2(i, j), 2);
                                        currentLevel.grid[i, j].containedObjects.Add(e);
                                        Components.Add(e);
                                        break;
                                    case '7':
                                        // Switch
                                        Switch s = new Switch(this, 
                                            new Vector2(((l * 800) / 12) + ((j - currentLevel.initialScreen.Y) * 800) + 1,
                                                ((k * 480) / 7) + ((i - currentLevel.initialScreen.X) * 480) + 2), currentLevel.switchDuration);
                                        currentLevel.grid[i, j].containedObjects.Add(s);
                                        Components.Add(s);
                                        break;
                                    case '8':
                                        // Key
                                        break;
                                    case '9':
                                        // Timey-wimey
                                        TimeyWimey t = new TimeyWimey(this,
                                            new Vector2(((l * 800) / 12) + ((j - currentLevel.initialScreen.Y) * 800) + 1,
                                                ((k * 480) / 7) + ((i - currentLevel.initialScreen.X) * 480) + 2));
                                        currentLevel.grid[i, j].containedObjects.Add(t);
                                        Components.Add(t);
                                        break;

                                    // Gates (hit the required number of switches to open these)
                                    case 'A':
                                        // 1 switch
                                        w = new Wall(this, 
                                            new Vector2(((l * 800) / 12) + ((j-currentLevel.initialScreen.Y)*800)+ 1, 
                                                ((k * 480) / 7) + ((i-currentLevel.initialScreen.X)*480) + 2), new Vector2(k, l), 1, 1);
                                        currentLevel.grid[i, j].containedObjects.Add(w);
                                        Components.Add(w);
                                        break;
                                    case 'B':
                                        // 2 switches
                                        w = new Wall(this, 
                                            new Vector2(((l * 800) / 12) + ((j-currentLevel.initialScreen.Y)*800)+ 1, 
                                                ((k * 480) / 7) + ((i-currentLevel.initialScreen.X)*480) + 2), new Vector2(k, l), 1, 2);
                                        currentLevel.grid[i, j].containedObjects.Add(w);
                                        Components.Add(w);
                                        break;
                                    case 'C':
                                        // 3 switches
                                        w = new Wall(this, 
                                            new Vector2(((l * 800) / 12) + ((j-currentLevel.initialScreen.Y)*800)+ 1, 
                                                ((k * 480) / 7) + ((i-currentLevel.initialScreen.X)*480) + 2), new Vector2(k, l), 1, 3);
                                        currentLevel.grid[i, j].containedObjects.Add(w);
                                        Components.Add(w);
                                        break;

                                    // Emergency shutters (hit the required number of switches to close these)
                                    case 'a':
                                        // 1 switch
                                        w = new Wall(this, 
                                            new Vector2(((l * 800) / 12) + ((j-currentLevel.initialScreen.Y)*800)+ 1, 
                                                ((k * 480) / 7) + ((i-currentLevel.initialScreen.X)*480) + 2), new Vector2(k, l), 2, 1);
                                        currentLevel.grid[i, j].containedObjects.Add(w);
                                        Components.Add(w);
                                        break;
                                    case 'b':
                                        // 2 switches
                                        w = new Wall(this, 
                                            new Vector2(((l * 800) / 12) + ((j-currentLevel.initialScreen.Y)*800)+ 1, 
                                                ((k * 480) / 7) + ((i-currentLevel.initialScreen.X)*480) + 2), new Vector2(k, l), 2, 2);
                                        currentLevel.grid[i, j].containedObjects.Add(w);
                                        Components.Add(w);
                                        break;
                                    case 'c':
                                        // 3 switches
                                        w = new Wall(this, 
                                            new Vector2(((l * 800) / 12) + ((j-currentLevel.initialScreen.Y)*800)+ 1, 
                                                ((k * 480) / 7) + ((i-currentLevel.initialScreen.X)*480) + 2), new Vector2(k, l), 2, 3);
                                        currentLevel.grid[i, j].containedObjects.Add(w);
                                        Components.Add(w);
                                        break;

                                    // Extras
                                    case 'G':
                                        // Gun
                                        Gun g = new Gun(this, new Vector2(((l * 800) / 12) + ((j - currentLevel.initialScreen.Y) * 800) + 1,
                                                ((k * 480) / 7) + ((i - currentLevel.initialScreen.X) * 480) + 2));
                                        currentLevel.grid[i, j].containedObjects.Add(g);
                                        Components.Add(g);
                                        break;
                                    case 'Z': 
                                        // Locked door
                                        Lock lockedDoor = new Lock(this, new Vector2(((l * 800) / 12) + ((j - currentLevel.initialScreen.Y) * 800) + 1,
                                                ((k * 480) / 7) + ((i - currentLevel.initialScreen.X) * 480) + 2));
                                        currentLevel.grid[i, j].containedObjects.Add(lockedDoor);
                                        Components.Add(lockedDoor);
                                        break;

                                    case 'X':
                                        // Ending splash screen

                                        break;
                                }
                            }
                        }
                    }
                }
            }
            if (currentLevel.cutscene != null)
                isCutscene = true;
        }
    }
}
