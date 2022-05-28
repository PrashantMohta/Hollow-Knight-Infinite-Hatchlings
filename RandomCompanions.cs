
namespace RandomCompanions
{

    public partial class RandomCompanions : Mod,ITogglableMod,  IGlobalSettings<GlobalModSettings>,ICustomMenuMod 
    {
        private readonly Dictionary<string, Texture2D> hatchlingTex = new Dictionary<string, Texture2D>();
        private int _HatchlingSelector = -1;
        private int _WeaverlingSelector = -1;

        private int firstValidSkin(ISelectableSkin[] arr,string fileName, int currentSkin){
            var i = currentSkin + 1;
            var selectSkin = i;
            if(i >= arr.Length){
                i = 0;
            }
            for(; i < arr.Length ; i++){
                if(arr[i].Exists(fileName)){
                    selectSkin = i;
                    break;
                }
            }
            return selectSkin;
        }
        public int hatchlingSelector
        {
            get
            {   if(hatchlingSkins != null){
                    _HatchlingSelector = firstValidSkin(hatchlingSkins,"Hatchling.png",_HatchlingSelector);
                    return _HatchlingSelector;
                } else {
                    _HatchlingSelector = -1;
                    return _HatchlingSelector;
                }
            }

        }

        public int weaverSelector
        {
            get
            {    if(weaverSkins != null){
                    _WeaverlingSelector = firstValidSkin(weaverSkins,"Weaver.png",_WeaverlingSelector);
                    return _WeaverlingSelector;
                } else {
                    _WeaverlingSelector = -1;
                    return _WeaverlingSelector;
                }
            }

        }

        public ISelectableSkin[] hatchlingSkins,weaverSkins;
        public override void Initialize()
        {
            CustomKnight.CustomKnight.OnReady += (_,e)=>{
               var _installedSkins = CustomKnight.SkinManager.GetInstalledSkins();
                List<ISelectableSkin> filtered = new List<ISelectableSkin>();
                for(var i = 0; i < _installedSkins.Length ; i++){
                    if(_installedSkins[i].Exists("Hatchling.png")){
                        filtered.Add(_installedSkins[i]);
                    }
                }
                hatchlingSkins = filtered.ToArray();
                filtered = new List<ISelectableSkin>();
                for(var i = 0; i < _installedSkins.Length ; i++){
                    if(_installedSkins[i].Exists("Weaver.png")){
                        filtered.Add(_installedSkins[i]);
                    }
                }
                weaverSkins = filtered.ToArray();
            };
            ModHooks.AfterSavegameLoadHook += ModifyHatchling;
            ModHooks.ObjectPoolSpawnHook += Instance_ObjectPoolSpawnHook;
            On.KnightHatchling.Start += KnightHatchling_Start;
            On.WeaverlingEnemyList.OnEnable += WeaverlingEnemyList_OnEnable;
            ModHooks.GetPlayerIntHook += ModifyCharmCost;
            ModHooks.GetPlayerBoolHook += ModifyCharmGot;
        }

        private bool ModifyCharmGot(string name,bool orig){
            if (name == nameof(PlayerData.gotCharm_22))
                return Settings.HatchlingcharmCost == 0;
            if (name == nameof(PlayerData.gotCharm_39))
                return Settings.WeaverlingcharmCost == 0;
            return orig;
        }
        private int ModifyCharmCost(string intName, int orig)
        {
            if (intName == nameof(PlayerData.charmCost_22))
                return Math.Abs(Settings.HatchlingcharmCost);
            if (intName == nameof(PlayerData.charmCost_39))
                return Math.Abs(Settings.WeaverlingcharmCost);
            return orig;
        }

        
        private Texture2D LoadTex(ISelectableSkin[] arr,int skinIndex,string fileName)
        {
            var skin = arr[skinIndex];
            if (!skin.Exists(fileName))
            {
                LogDebug($"File:{fileName} is Not Found in skin: {skin.GetName()}");
                return null;
            }
            var texKey = skin.GetName() + fileName;
            if(hatchlingTex.ContainsKey(texKey))
            {
                return hatchlingTex[texKey];
            }
            else
            {
                var tex = skin.GetTexture(fileName);
                if(skin.shouldCache()){
                    hatchlingTex.Add(texKey, tex);
                }
                return tex;
            }
            
        }

        private void WeaverlingEnemyList_OnEnable(On.WeaverlingEnemyList.orig_OnEnable orig,WeaverlingEnemyList self){
           orig(self);
           if( !Settings.attackOption )
           {
                var damager = self.gameObject.transform.parent.gameObject.Find("Enemy Damager");
                if(damager != null){
                    damager.SetActive(false);
                    damager.layer = (int)PhysLayers.PARTICLE;
                }
                self.gameObject.SetActive(false);
                damager.layer = (int)PhysLayers.PARTICLE;
           } 
        }

        private void KnightHatchling_Start(On.KnightHatchling.orig_Start orig, KnightHatchling self)
        {
            orig(self);
            if( !Settings.attackOption ) //remove this can let sommoned lose goal and prevent attacking
                self.enemyRange = null;

            //if you want to let them attack but not die, you can block the "Land" fsm action.
            //if you want to let them attack without damage, you can remove it's component "Damager"
            return;
        }

        private GameObject Instance_ObjectPoolSpawnHook(GameObject go) 
        {
            if (go.tag == "Knight Hatchling")
            {
                int skinId = hatchlingSelector;
                if(skinId > -1){
                    Texture2D tex = LoadTex(hatchlingSkins,skinId,"Hatchling.png");
                    if (tex)
                    {
                        var materialProp = new MaterialPropertyBlock();
                        go.GetComponent<MeshRenderer>().GetPropertyBlock(materialProp);
                        materialProp.SetTexture("_MainTex", tex);
                        go.GetComponent<MeshRenderer>().SetPropertyBlock(materialProp);
                    }
                }
            }
            if (go.tag == "Weaverling")
            {
                int skinId = weaverSelector;
                if(skinId > -1){
                    Texture2D tex = LoadTex(weaverSkins,skinId,"Weaver.png");
                    if (tex)
                    {
                        var materialProp = new MaterialPropertyBlock();
                        go.GetComponent<MeshRenderer>().GetPropertyBlock(materialProp);
                        materialProp.SetTexture("_MainTex", tex);
                        go.GetComponent<MeshRenderer>().SetPropertyBlock(materialProp);
                    }
                }
            }
            return go;
        }

        private void ModifyHatchling(SaveGameData data)
        {
            GameManager.instance.StartCoroutine(HeroFinder());
        }
        private IEnumerator HeroFinder() 
        {
            yield return new WaitWhile(()=>HeroController.instance == null);

            var ce = GameObject.Find("Charm Effects");
            var hatchlingfsm = ce.LocateMyFSM("Hatchling Spawn");
            PlayMakerFSM weaverlingControl = ce.LocateMyFSM("Weaverling Control");

            hatchlingfsm.InsertAction("Equipped", new SetIntValue {intVariable = hatchlingfsm.Fsm.GetFsmInt("Soul Cost"),intValue=Math.Abs(Settings.HatchlingSoulCost) ,everyFrame=false}, 0);
            hatchlingfsm.InsertAction("Equipped", new SetIntValue {intVariable = hatchlingfsm.Fsm.GetFsmInt("Hatchling Max"),intValue=Math.Abs(Settings.HatchlingMaxCount) ,everyFrame=false}, 1);
            hatchlingfsm.InsertAction("Equipped", new SetFloatValue { floatVariable = hatchlingfsm.Fsm.GetFsmFloat("Hatch Time"), floatValue = Math.Abs(Settings.HatchlingSpawnTime + 0.01f), everyFrame = false },2);

            SpawnObjectFromGlobalPool p1 = weaverlingControl.GetAction<SpawnObjectFromGlobalPool>("Spawn",0);
            SpawnObjectFromGlobalPool p2 = weaverlingControl.GetAction<SpawnObjectFromGlobalPool>("Spawn",1);
            SpawnObjectFromGlobalPool p3 = weaverlingControl.GetAction<SpawnObjectFromGlobalPool>("Spawn",2);
            weaverlingControl.AddCustomAction("Spawn", ()=>{
                if(Settings.WeaverlingMaxCount > 3){
                    var j = 1;
                    for(var i = 3; i < Settings.WeaverlingMaxCount ; i++){
                        if(j == 3){
                            p3.OnEnter();
                            j = 1;
                        }else if(j == 2){
                            p2.OnEnter();
                            j = 3;
                        }else if(j == 1){
                            p1.OnEnter();
                            j = 3;
                        }
                    }
                }
            });
            LogDebug("Modify MaxCount");
        }
        public void Unload()
        {
            ModHooks.AfterSavegameLoadHook -= ModifyHatchling;
            ModHooks.ObjectPoolSpawnHook -= Instance_ObjectPoolSpawnHook;
            On.KnightHatchling.Start -= KnightHatchling_Start;
            On.WeaverlingEnemyList.OnEnable -= WeaverlingEnemyList_OnEnable;
            ModHooks.GetPlayerIntHook -= ModifyCharmCost;
            ModHooks.GetPlayerBoolHook -= ModifyCharmGot;
        }

        private string getVersionSafely(){
            return Satchel.AssemblyUtils.GetAssemblyVersionHash();
        }
        public override string GetVersion()
        {
            var version = "Satchel not found";
            try{
                version = getVersionSafely();
            } catch(Exception e){
                Modding.Logger.Log(e.ToString());
            }
            return version;
        }

    }
}
