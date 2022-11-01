using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public interface ISubMenu
    {
        void Update();
        void Draw();
    }

    public sealed class MainSubMenu : ISubMenu
    {
        private ListMenu menu;
        private ShopMenu shopMenu;

        private void OnStartGame()
        {
            Game.Current.StartMatch();
        }

        private void OnEnterShop()
        {

        }

        private void OnEnterScoreboard()
        {

        }

        private void OnEnterSettings()
        {

        }

        private void OnCredits()
        {

        }

        private void OnExit()
        {

        }

        public void Update()
        {
            menu.Position = Game.Current.MainMenu.Animator.Animation.Position;
            menu.Update();
        }

        public void Draw()
        {
            menu.Draw();
        }

        public MainSubMenu()
        {
            shopMenu = new ShopMenu();

            menu = new ListMenu(new Vector3(0, 0, 0));
            menu.AddItem("Играть", OnStartGame);
            //menu.AddItem("Магазин", OnEnterShop);
            menu.AddItem("Настройки", OnEnterSettings);
            menu.AddItem("Рекорды", OnEnterScoreboard);
            menu.AddItem("Выход", OnExit);

            Game.Current.MainMenu.Animator.Play(false);
        }
    }

    public sealed class ShopMenu : ISubMenu
    {
        private ListMenu menu;

        private void OnEnterUpgrades()
        {
            
        }

        private void OnEnterShips()
        {

        }

        private void OnGoBack()
        {
            Game.Current.MainMenu.SetSubMenu(null);
        }

        public ShopMenu()
        {
            menu = new ListMenu(new Vector3(10, 10, 0));
            menu.AddItem("Кораблики", OnEnterUpgrades);
            menu.AddItem("Назад", OnGoBack, ItemFlags.HandleBackKey);
        }

        public void Draw()
        {
            menu.Draw();
        }

        public void Update()
        {
            menu.Update();
        }
    }

    public sealed class MainMenu
    {
        private Background background;
        private ISubMenu subMenu;
        private NativeFont rendererCopy;
        private NativeFont renderer;

        public SmoothAnimator Animator;

        private MainSubMenu mainSub;

        public MainMenu()
        {
            background = new Background();
            Animator = new SmoothAnimator(new UISwipeIn(new Vector3(-120, 0, 0), new Vector3(20, 0, 0)));

            rendererCopy = new NativeFont("Arial", 8);
            renderer = new NativeFont("Arial", 12);
        }

        public void PostInitialize()
        {
            mainSub = new MainSubMenu();
            SetSubMenu(mainSub);
        }
        
        public void SetSubMenu(ISubMenu menu)
        {
            if (menu == null)
                subMenu = mainSub;
            else
                subMenu = menu;
        }

        public void Update()
        {
            Animator.Update();
            subMenu.Update();
        }
        
        public void Draw()
        {
            background.Draw();
        }

        public void DrawUI()
        {
            GUI.DrawTextRelative(renderer, "Личный рекорд: " + Game.Current.Save.HighScore, ListMenu.Normal, 0.01f, 0.9f);

            subMenu.Draw();
            
            renderer.DrawString("(C)2022 Bogdan Nikolaev aka monobogdan", 5, Engine.Current.Window.ViewportHeight - (int)renderer.Size - 5, 255, 255, 255, 255);
        }
    }
}
