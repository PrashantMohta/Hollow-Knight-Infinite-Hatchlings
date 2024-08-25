namespace RandomCompanions
{

    internal static partial class BetterMenu
    {
        internal static Menu HatchlingMenu;
        internal static Menu PrepareHatchlingsMenu(){
            return new Menu("Hatchling Settings",new Element[]{
                new HorizontalOption(
                    "Start With Glowing Womb", "Automatically give charm at start of game.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.HatchlingCharmStart = true;
                        } else {
                            RandomCompanions.Settings.HatchlingCharmStart = false;
                        }
                    },
                    () => {
                         return RandomCompanions.Settings.HatchlingCharmStart ? 1 : 0;
                    },
                    Id:"AutoHatchings"),

                new HorizontalOption(
                    "Free Glowing Womb", "Make charm free to equip.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.HatchlingCharmFree = true;
                        } else {
                            RandomCompanions.Settings.HatchlingCharmFree = false;
                        } 
                    },
                    () => {
                        return RandomCompanions.Settings.HatchlingCharmFree ? 1 : 0;
                    },
                    Id:"FreeHatchlings"),

                new HorizontalOption(
                    "Quick Spawn", "Remove delays when spawning hatchlings.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.HatchlingSpawnTime = 0f;
                        } else {
                            RandomCompanions.Settings.HatchlingSpawnTime = defaultSettings.HatchlingSpawnTime;
                        } 
                    },
                    () => {
                        return RandomCompanions.Settings.HatchlingSpawnTime == 0 ? 1 : 0;
                    },
                    Id:"QuickHatchlings"),

                new CustomSlider(
                    "Max Hatchlings Count",
                    (f)=>{
                        RandomCompanions.Settings.HatchlingMaxCount = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.HatchlingMaxCount,
                    4,15,true,
                    Id:"HatchlingsCount"),
                    
                new CustomSlider(
                    "Hatchling Soul Cost",
                    (f)=>{
                        RandomCompanions.Settings.HatchlingSoulCost = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.HatchlingSoulCost,
                    0,15,true,
                    Id:"HatchlingSoulCost")
                });
        }

        internal static MenuScreen GetHatchlingsMenu(MenuScreen lastMenu){
            if(HatchlingMenu == null){
                HatchlingMenu = PrepareHatchlingsMenu();
            }
            return HatchlingMenu.GetCachedMenuScreen(lastMenu);
        }
    }
}