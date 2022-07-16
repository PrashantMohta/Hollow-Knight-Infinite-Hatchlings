namespace RandomCompanions
{

    internal static partial class BetterMenu
    {

        internal static GlobalModSettings defaultSettings = new GlobalModSettings();
        internal static Menu MenuRef;
         internal static Menu PrepareMenu(ModToggleDelegates toggleDelegates){
            return new Menu("Random Companions",new Element[]{
                new TextPanel("Note: Settings require a restart of the game to apply properly.",800f),
                Blueprints.CreateToggle(toggleDelegates,"Random Skins", "", "Enabled","Disabled"),
                new HorizontalOption(
                    "Enable Attacks", "Allows minions to attack",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => { RandomCompanions.Settings.attackOption = (setting == 1); },
                    () => RandomCompanions.Settings.attackOption ? 1 : 0,
                    Id:"AttackEnabled"),
                Blueprints.NavigateToMenu("Hatchlings Settings","",()=> GetHatchlingsMenu(MenuRef.menuScreen)),
                Blueprints.NavigateToMenu("Weaverlings Settings","",()=> GetWeaverlingsMenu(MenuRef.menuScreen)),
                Blueprints.NavigateToMenu("Grimmchild Settings","",()=> GetGrimmChildMenu(MenuRef.menuScreen))
            });
        }
        internal static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggleDelegates){
            if(MenuRef == null){
                MenuRef = PrepareMenu((ModToggleDelegates)toggleDelegates);
            }
            return MenuRef.GetCachedMenuScreen(lastMenu);
        }
    }
}