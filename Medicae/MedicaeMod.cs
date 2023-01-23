using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using System;
using Vintagestory.API;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Medicae
{
    public class Medicae : ModSystem
    {
        public ICoreAPI _api;
        public ICoreClientAPI _capi;
        public ICoreServerAPI _sapi;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            _api = api;
            _api.RegisterItemClass("MedicaeItemPoultice", typeof(MedicaeItemPoultice));
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            _capi = api;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            _sapi = api;
        }
    }

    // ### POULTICE ITEMS ###
    public class MedicaeItemPoultice : Item
    {
        public SimpleParticleProperties blood = new SimpleParticleProperties(5, 15, ColorUtil.ColorFromRgba(10, 10, 180, 255), new Vec3d(), new Vec3d(), new Vec3f(), new Vec3f());
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel,
            EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            byEntity.World.RegisterCallback((dt) =>
            {
                if (byEntity.Controls.HandUse == EnumHandInteract.HeldItemInteract)
                {
                    IPlayer player = null;
                    if (byEntity is EntityPlayer)
                        player = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);

                    byEntity.World.PlaySoundAt(new AssetLocation("sounds/player/poultice"), byEntity, player);
                }
            }, 200);


            JsonObject attr = slot.Itemstack.Collectible.Attributes;
            if (attr != null && attr["infection"].Exists)
            {
                handling = EnumHandHandling.PreventDefault;
                return;
            }

            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        }

        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity,
            BlockSelection blockSel, EntitySelection entitySel)
        {
            if (byEntity.World is IClientWorldAccessor)
            {
                ModelTransform tf = new ModelTransform();

                tf.EnsureDefaultValues();
                tf.Origin.Set(0f, 0, 0f);
                tf.Translation.X -= Math.Min(1.5f, secondsUsed * 4 * 1.57f);
                tf.Rotation.Y += Math.Min(130f, secondsUsed * 350);

                byEntity.Controls.UsingHeldItemTransformAfter = tf;

                return secondsUsed < 0.75f;
            }

            return true;
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity,
            BlockSelection blockSel, EntitySelection entitySel)
        {
            if (secondsUsed > 0.7f && byEntity.World.Side == EnumAppSide.Server)
            {
                JsonObject attr = slot.Itemstack.Collectible.Attributes;
                float infection = attr["infection"].AsFloat();
                byEntity.ReceiveDamage(new DamageSource()
                {
                    Source = EnumDamageSource.Internal,
                    Type = infection > 0 ? EnumDamageType.Heal : EnumDamageType.Poison
                }, Math.Abs(infection));

                slot.TakeOut(1);
                slot.MarkDirty();
                
                blood.MinPos = byEntity.Pos.XYZ.Add(0, byEntity.LocalEyePos.Y, 0);
                blood.AddPos = new Vec3d(0.5, 0.5, 0.5);
                blood.MinSize = 0.2f;
                blood.MaxSize = 1.0f;

                byEntity.World.SpawnParticles(blood);
            }
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world,
            bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            JsonObject attr = inSlot.Itemstack.Collectible.Attributes;
            if (attr != null && attr["infection"].Exists)
            {
                float infection = attr["infection"].AsFloat();
                dsc.AppendLine(Lang.Get("When used reduces {0} infection over 24 hours.", infection));
            }
        }


        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[]
            {
                new WorldInteraction()
                {
                    ActionLangCode = "heldhelp-heal",
                    MouseButton = EnumMouseButton.Right,
                }
            }.Append(base.GetHeldInteractionHelp(inSlot));
        }
    }
    
    // ### RECEIVE DAMAGE ###
    public class EntityBehaviorBodyTemperature : EntityBehavior
    {
        ITreeAttribute tempTree;
        ICoreAPI api;
        EntityAgent eagent;

        public override void Initialize(EntityProperties properties, JsonObject typeAttributes)
        {
            api = entity.World.Api;
            
            public virtual void OnEntityReceiveDamage()
            {
                
            }
        }
    }
}