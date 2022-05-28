
namespace RandomCompanions
{

    public partial class RandomCompanions : Mod,ITogglableMod,  IGlobalSettings<GlobalModSettings>,ICustomMenuMod 
    {
        
        public static GlobalModSettings Settings { get; set; } = new GlobalModSettings();
        public void OnLoadGlobal(GlobalModSettings s)
        {
            Settings = s;
        }

        public GlobalModSettings OnSaveGlobal()
        {
            return RandomCompanions.Settings;
        }

        public  bool ToggleButtonInsideMenu {get;}= true;
        public MenuScreen GetMenuScreen(MenuScreen modListMenu,ModToggleDelegates? toggle){
            return BetterMenu.GetMenu(modListMenu,toggle);
        }

    }
}