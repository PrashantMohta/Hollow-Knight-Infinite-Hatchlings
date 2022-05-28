namespace RandomCompanions
{

    internal static class BetterMenu
    {

         internal static GlobalModSettings defaultSettings = new GlobalModSettings();
        internal static Menu MenuRef;
         internal static Menu PrepareMenu(ModToggleDelegates toggleDelegates){
            return new Menu("Random Companions",new Element[]{
                Blueprints.CreateToggle(toggleDelegates,"Random Skins", "", "Enabled","Disabled"),
                new HorizontalOption(
                    "Enable Attacks", "Allows minions to attack",
                    new string[] { "Disabled", "Enabled" },
                    (setting) => { RandomCompanions.Settings.attackOption = (setting == 1); },
                    () => RandomCompanions.Settings.attackOption ? 1 : 0,
                    Id:"AttackEnabled"),
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
                    Id:"WeaverlingCount"){ wholeNumbers = true, minValue = 3, maxValue = 15},
                new CustomSlider(
                    "Max Hatchlings Count",
                    (f)=>{
                        RandomCompanions.Settings.HatchlingMaxCount = (int)f;
                    },
                    () => (float)RandomCompanions.Settings.HatchlingMaxCount,
                    Id:"WeaverlingCount"){ wholeNumbers = true, minValue = 0, maxValue = 15}
            });
        }
        internal static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggleDelegates){
            if(MenuRef == null){
                MenuRef = PrepareMenu((ModToggleDelegates)toggleDelegates);
            }
            return MenuRef.GetMenuScreen(lastMenu);
        }
    }
}