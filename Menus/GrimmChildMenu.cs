namespace RandomCompanions
{

    internal static partial class BetterMenu
    {
        internal static Menu GrimmChildMenu;
        internal static Menu PrepareGrimmChildMenu(){
            return new Menu("Grimmchild Settngs",new Element[]{

                new HorizontalOption(
                    "Free GrimmChild", "Make Affected charms free.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.GrimmChildcharmCost = 0;
                        } else {
                            RandomCompanions.Settings.GrimmChildcharmCost = defaultSettings.WeaverlingcharmCost;
                        } 
                    },
                    () => {
                         return RandomCompanions.Settings.GrimmChildcharmCost == 0 ? 1 : 0;
                    },
                    Id:"FreeGrimmChild"),

                new CustomSlider(
                    "Max GrimmChild Count",
                    (f)=>{
                        RandomCompanions.Settings.GrimmChildMaxCount = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.GrimmChildMaxCount,
                    1,15,true,
                    Id:"GrimmChildCount"),

                new HorizontalOption(
                    "Multi-Level GrimmChilden", "Cycle through all levels of GrimmChild",
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