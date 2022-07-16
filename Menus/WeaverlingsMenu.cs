namespace RandomCompanions
{

    internal static partial class BetterMenu
    {
        internal static Menu WeaverlingsMenu;
        internal static Menu PrepareWeaverlingsMenu(){
            return new Menu("Weaverlings Settngs",new Element[]{
                new HorizontalOption(
                    "Free Weaverlings", "Make Affected charms free.",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => {
                         if((setting == 1)){
                            RandomCompanions.Settings.WeaverlingcharmCost = 0;
                        } else {
                            RandomCompanions.Settings.WeaverlingcharmCost = defaultSettings.WeaverlingcharmCost;
                        } 
                    },
                    () => {
                         return RandomCompanions.Settings.WeaverlingcharmCost == 0 ? 1 : 0;
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