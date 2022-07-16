namespace RandomCompanions
{

    internal static partial class BetterMenu
    {
        internal static Menu HatchlingMenu;
        internal static Menu PrepareHatchlingsMenu(){
            return new Menu("Hatchling Settngs",new Element[]{
                new HorizontalOption(
                    "Free Hatchlings", "Make Affected charms free.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.HatchlingcharmCost = 0;
                        } else {
                            RandomCompanions.Settings.HatchlingcharmCost = defaultSettings.HatchlingcharmCost;
                        } 
                    },
                    () => {
                        return RandomCompanions.Settings.HatchlingcharmCost == 0 ? 1 : 0;
                    },
                    Id:"FreeHatchlings"),

                new HorizontalOption(
                    "Quick Spawn", "Remove Delays when spawning hatchlings.",
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