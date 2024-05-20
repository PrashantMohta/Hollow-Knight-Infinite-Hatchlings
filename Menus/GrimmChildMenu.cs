namespace RandomCompanions
{

    internal static partial class BetterMenu
    {
        internal static Menu GrimmChildMenu;
        internal static Menu PrepareGrimmChildMenu(){
            return new Menu("Grimmchild Settings",new Element[]{
                new HorizontalOption(
                    "Start With Grimmchild", "Automatically give charm at start of game.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.GrimmChildCharmStart = true;
                        } else {
                            RandomCompanions.Settings.GrimmChildCharmStart = false;
                        }
                    },
                    () => {
                         return RandomCompanions.Settings.GrimmChildCharmStart ? 1 : 0;
                    },
                    Id:"AutoGrimmChild"),

                new HorizontalOption(
                    "Free Grimmchild", "Make charm free to equip.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.GrimmChildCharmFree = true;
                        } else {
                            RandomCompanions.Settings.GrimmChildCharmFree = false;
                        } 
                    },
                    () => {
                         return RandomCompanions.Settings.GrimmChildCharmFree ? 1 : 0;
                    },
                    Id:"FreeGrimmChild"),

                new CustomSlider(
                    "Max Grimmchild Count",
                    (f)=>{
                        RandomCompanions.Settings.GrimmChildMaxCount = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.GrimmChildMaxCount,
                    1,15,true,
                    Id:"GrimmChildCount"),

                new HorizontalOption(
                    "Multi-Level Grimmchildren", "Cycle through all levels of Grimmchild",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.GrimmChildMultiLevel = true;
                        } else {
                            RandomCompanions.Settings.GrimmChildMultiLevel = false;
                        } 
                    },
                    () => {
                         return RandomCompanions.Settings.GrimmChildMultiLevel ? 1 : 0;
                    },
                    Id:"GrimmChildMultiLevel"),

                });
        }

        internal static MenuScreen GetGrimmChildMenu(MenuScreen lastMenu){
            if(GrimmChildMenu == null){
                GrimmChildMenu = PrepareGrimmChildMenu();
            }
            return GrimmChildMenu.GetCachedMenuScreen(lastMenu);
        }
    }
}