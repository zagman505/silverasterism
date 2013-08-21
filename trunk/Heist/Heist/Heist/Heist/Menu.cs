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
    /* To add new menus and menu items: 
     * Add the menu type into MenuData.MenuType, or add the menu items into MenuData.MenuItem. 
     * If adding menu items, need to add corresponding textures to content. Textures are named by their index position on MenuData.MenuItem, and currently are sized 300x100 px
     * Add the menu into MenuData.MenuOptions. Each menu type is linked to a dictionary of menu items linked to positions.
     * Add the code for handling each menu option into Menu.SelectOption()
     */
    class Menu
    {
        public bool OnTop;
        public MenuData.MenuType currentMenu;
        public Stack<MenuData.MenuType> lastMenus = new Stack<MenuData.MenuType>();
        public List<MenuItem> options;
        public List<Texture2D> optionTextures = new List<Texture2D>();
        private Game game;

        public Menu(Game game)
        {
            OnTop = true;
            LoadMenuOptions(MenuData.MenuType.MainMenu);
            this.game = game;
        }

        public void LoadMenuOptions(MenuData.MenuType currentMenu)
        {
            this.currentMenu = currentMenu;
            options = new List<MenuItem>();
            foreach (MenuData.MenuItem i in MenuData.MenuOptions[currentMenu].Keys)
            {
                options.Add(new MenuItem(i, currentMenu));
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var o in options)
            {
                if (o.area.Contains(MouseInput.position))
                {
                    o.hovered = true;
                    if (MouseInput.leftClicked)
                    {
                        SelectOption(o);
                    }
                }
                else o.hovered = false;
            }
        }

        public void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            spritebatch.Begin();
            foreach (var o in options)
            {
                if (o.hovered) spritebatch.Draw(optionTextures[(int)o.itemType], o.position, Color.Red);
                else spritebatch.Draw(optionTextures[(int)o.itemType], o.position, Color.Black);
            }
            spritebatch.End();
        }

        private void SelectOption(MenuItem o)
        {
            switch (o.itemType)
            {
                case MenuData.MenuItem.Back:
                    if (lastMenus.Count == 0) OnTop = false;
                    else LoadMenuOptions(lastMenus.Pop());
                    break;
                case MenuData.MenuItem.Campaign:
                    lastMenus.Push(currentMenu);
                    LoadMenuOptions(MenuData.MenuType.CampaignMenu);
                    break;
                case MenuData.MenuItem.Options:
                    lastMenus.Push(currentMenu);
                    LoadMenuOptions(MenuData.MenuType.OptionsMenu);
                    break;
                case MenuData.MenuItem.Graphics:
                    lastMenus.Push(currentMenu);
                    LoadMenuOptions(MenuData.MenuType.GraphicsMenu);
                    break;
                case MenuData.MenuItem.Exit:
                    game.Exit();
                    break;
            }
        }
    }

    class MenuItem
    {
        public MenuData.MenuItem itemType;
        public Vector2 position;
        public Rectangle area;
        public bool hovered = false;
        public bool selected = false;

        public MenuItem(MenuData.MenuItem i, MenuData.MenuType currentMenu)
        {
            itemType = i;
            position = MenuData.MenuOptions[currentMenu][itemType];
            area = new Rectangle((int)position.X, (int)position.Y, 300, 100);
        }
    }

    static class MenuData
    {
        public static int width = 800;
        public static int height = 600;
        public enum MenuType { MainMenu, CampaignMenu, OptionsMenu, GraphicsMenu }
        public enum MenuItem { Back, Campaign, Options, Exit, Graphics }
        public static Dictionary<MenuType, Dictionary<MenuItem, Vector2>> MenuOptions = new Dictionary<MenuType, Dictionary<MenuItem, Vector2>> { 
                                                                                        { MenuType.MainMenu, new Dictionary<MenuItem, Vector2> {
                                                                                                                { MenuItem.Campaign, new Vector2((width/2)-150, 100) },
                                                                                                                { MenuItem.Options, new Vector2((width/2)-150, 200) },
                                                                                                                { MenuItem.Exit, new Vector2((width/2)-150, 300) }
                                                                                                                                                } },
                                                                                        { MenuType.CampaignMenu, new Dictionary<MenuItem, Vector2> {
                                                                                                                { MenuItem.Back, new Vector2((width/2)-150, 100) } 
                                                                                                                                                    } },
                                                                                        { MenuType.OptionsMenu, new Dictionary<MenuItem, Vector2> {
                                                                                                                { MenuItem.Graphics, new Vector2((width/2)-150, 100) },
                                                                                                                { MenuItem.Back, new Vector2((width/2)-150, 200) } 
                                                                                                                                                    } },
                                                                                        { MenuType.GraphicsMenu, new Dictionary<MenuItem, Vector2> {
                                                                                                                { MenuItem.Back, new Vector2((width/2)-150, 100) } 
                                                                                                                                                    } }
                                                                                                                                                                                                            };
    }
}
