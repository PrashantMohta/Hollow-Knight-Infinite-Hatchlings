namespace RandomCompanions
{

    internal static partial class BetterMenu
    {
        internal static Menu WeaverlingsMenu;
        internal static Menu PrepareWeaverlingsMenu(){
            return new Menu("Weaverlings Settings",new Element[]{
                new HorizontalOption(
                    "Start With Weaversong", "Automatically give charm at start of game.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.WeaverlingCharmStart = true;
                        } else {
                            RandomCompanions.Settings.WeaverlingCharmStart = false;
                        }
                    },
                    () => {
                         return RandomCompanions.Settings.WeaverlingCharmStart ? 1 : 0;
                    },
                    Id:"AutoWeaverlings"),

                new HorizontalOption(
                    "Free Weaversong", "Make charm free to equip.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.WeaverlingCharmFree = true;
                        } else {
                            RandomCompanions.Settings.WeaverlingCharmFree = false;
                        } 
                    },
                    () => {
                         return RandomCompanions.Settings.WeaverlingCharmFree ? 1 : 0;
                    },
                    Id:"FreeWeaverlings"),

                new CustomSlider(
                    "Max Weaverling Count",
                    (f)=>{
                        RandomCompanions.Settings.WeaverlingMaxCount = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.WeaverlingMaxCount,
                    3,15,true,
                    Id:"WeaverlingCount"),
                });
        }

        internal static MenuScreen GetWeaverlingsMenu(MenuScreen lastMenu){
            if(WeaverlingsMenu == null){
                WeaverlingsMenu = PrepareWeaverlingsMenu();
            }
            return WeaverlingsMenu.GetCachedMenuScreen(lastMenu);
        }
    }
}