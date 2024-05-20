
namespace RandomCompanions
{

    public partial class RandomCompanions : Mod,ITogglableMod,  IGlobalSettings<GlobalModSettings>,ICustomMenuMod 
    {
        private readonly Dictionary<string, Texture2D> hatchlingTex = new Dictionary<string, Texture2D>();
        private int _HatchlingSelector = -1;
        private int _WeaverlingSelector = -1;
        private int _GrimmChildSelector = -1;
        private System.Random rng = new System.Random();

        private List<float> gcOffsets = new List<float>();

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

        public int grimmChildSelector
        {
            get
            {    if(grimmChildSkins != null){
                    _GrimmChildSelector = firstValidSkin(grimmChildSkins,"Grimm.png",_GrimmChildSelector);
                    return _GrimmChildSelector;
                } else {
                    _GrimmChildSelector = -1;
                    return _GrimmChildSelector;
                }
            }

        }

        private int _gcLevel = 1;
        public int gcLevel {
            get {
                _gcLevel++;
                if(_gcLevel > 4){ _gcLevel = 1;}
                return _gcLevel;
            }
        }

        public ISelectableSkin[] hatchlingSkins,weaverSkins,grimmChildSkins;
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
                filtered = new List<ISelectableSkin>();
                for(var i = 0; i < _installedSkins.Length ; i++){
                    if(_installedSkins[i].Exists("Grimm.png")){
                        filtered.Add(_installedSkins[i]);
                    }
                }
                grimmChildSkins = filtered.ToArray();
            };

            ModHooks.ObjectPoolSpawnHook += Instance_ObjectPoolSpawnHook;
            On.KnightHatchling.Start += KnightHatchling_Start;
            On.WeaverlingEnemyList.OnEnable += WeaverlingEnemyList_OnEnable;
            ModHooks.GetPlayerIntHook += ModifyCharmCost;
            ModHooks.GetPlayerBoolHook += ModifyCharmGot;
            On.PlayMakerFSM.OnEnable += FSMedits;

        }
        private float getUniqueGcOffset(){
            var maxValue = 20;
            var minValue = 1;
            float attempt = 2;
            var tries = 5;
            while(gcOffsets.Contains(attempt) || tries < 0){
                attempt = (float)Math.Round((rng.NextDouble() * (maxValue) + minValue) * 2);
                tries--;
            }
            gcOffsets.Add(attempt);
            return attempt;
        }
        private void FSMedits(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self){
            orig(self);

            if(self.gameObject.tag == "Grimmchild" && self.FsmName == "Control"){
                if( !Settings.attackOption )
                {
                    self.Fsm.GetFsmFloat("Attack Timer").Value = 9999f;
                    self.GetAction<FloatSubtract>("Follow",0).subtract.Value = 0f;
                } else {
                    self.Fsm.GetFsmFloat("Attack Timer").Value = 0.75f;
                    self.GetAction<FloatSubtract>("Follow",0).subtract.Value = 1f;
                }
                if(Settings.GrimmChildMaxCount > 1){
                    if(Settings.GrimmChildMultiLevel){
                        var levelStr = new FsmString();
                        levelStr.Value = gcLevel.ToString();
                        self.GetAction<SendEventByName>("Init",5).sendEvent = levelStr;
                    }
                    self.AddCustomAction("Change",(PlayMakerFSM fsm)=>{
                        var oldTarget = self.GetAction<DistanceFlySmooth>("Follow",11).target;
                        if(oldTarget.Value == HeroController.instance.gameObject){
                            var fakeTarget = new GameObject("RandomCompanions.gc.target");
                            fakeTarget.transform.SetParent(HeroController.instance.gameObject.transform,false);
                            fakeTarget.transform.localPosition = new Vector3((rng.Next(-1,1) > 0 ? 1 : -1)*getUniqueGcOffset()/6,getUniqueGcOffset()/10,0);
                            var targ = new FsmGameObject();
                            targ.Value = fakeTarget;
                            self.GetAction<DistanceFlySmooth>("Follow",11).target = targ;
                        }
                    });
                    
                }
            }

            if(self.gameObject.name == "Charm Effects" && self.FsmName == "Spawn Grimmchild"){
                var spawn = self.GetAction<SpawnObjectFromGlobalPool>("Spawn",2);
                self.AddCustomAction("Spawn", ()=>{
                    if(Settings.GrimmChildMaxCount > 1){
                        for(var i = 1; i < Settings.GrimmChildMaxCount ; i++){
                            spawn.OnEnter();
                        }
                    }
                });
            }

            
            if(self.gameObject.name == "Charm Effects" && self.FsmName == "Hatchling Spawn"){
                self.Fsm.GetFsmInt("Soul Cost").Value = (int)Math.Abs(Settings.HatchlingSoulCost);
                self.Fsm.GetFsmInt("Hatchling Max").Value = (int)Math.Abs(Settings.HatchlingMaxCount);
                self.Fsm.GetFsmInt("Hatch Time").Value = (int)Math.Abs(Settings.HatchlingSpawnTime);
            }

            if(self.gameObject.name == "Charm Effects" && self.FsmName == "Weaverling Control"){
                SpawnObjectFromGlobalPool p1 = self.GetAction<SpawnObjectFromGlobalPool>("Spawn",0);
                SpawnObjectFromGlobalPool p2 = self.GetAction<SpawnObjectFromGlobalPool>("Spawn",1);
                SpawnObjectFromGlobalPool p3 = self.GetAction<SpawnObjectFromGlobalPool>("Spawn",2);
                self.AddCustomAction("Spawn", ()=>{
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
            }
        }
        private bool ModifyCharmGot(string name, bool orig)
        {
            if (name == nameof(PlayerData.gotCharm_22))
                return Settings.HatchlingCharmStart || orig;
            if (name == nameof(PlayerData.gotCharm_39))
                return Settings.WeaverlingCharmStart || orig;
            if (name == nameof(PlayerData.gotCharm_40))
                return Settings.GrimmChildCharmStart || orig;
            return orig;
        }
        private int ModifyCharmCost(string intName, int orig)
        {
            if (intName == nameof(PlayerData.charmCost_22))
                return Settings.HatchlingCharmFree ? 0 : orig;
            if (intName == nameof(PlayerData.charmCost_39))
                return Settings.WeaverlingCharmFree ? 0 : orig;
            if (intName == nameof(PlayerData.charmCost_40))
                return Settings.GrimmChildCharmFree ? 0 : orig;
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

            if(go.tag == "Grimmchild"){
                int skinId = grimmChildSelector;
                if(skinId > -1){
                    Texture2D tex = LoadTex(grimmChildSkins,skinId,"Grimm.png");
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


        public void Unload()
        {
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
