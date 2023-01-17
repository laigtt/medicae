using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Medicae
{

    public class Medicae : ModSystem
    {
        private ICoreAPI _api;
        private ICoreClientAPI _capi;
        public static ICoreServerAPI _sapi;

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            _sapi = api;
            
            _sapi.Event.Timer(OnInterval, 3.0);
            
            _sapi.RegisterEntity("ME_Player", typeof(ME_Player));
        }
        
        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            _capi = api;
            
            _capi.RegisterEntity("ME_Player", typeof(ME_Player));
        }
        
        void OnInterval()
        {
            _sapi.SendMessageToGroup(GlobalConstants.GeneralChatGroup, "Hello MEDICAE!", EnumChatType.AllGroups);
        }
    }

    public class ME_Player : EntityPlayer
    {
        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);

            ReceiveDamage(new DamageSource(), 1f);
            Medicae._sapi?.SendMessageToGroup(GlobalConstants.GeneralChatGroup, "Hello MEDICAE!", EnumChatType.AllGroups);
        }

        public override void OnHurt(DamageSource damageSource, float damage)
        {
            //base.OnHurt(damageSource, damage);
        }
    }
}