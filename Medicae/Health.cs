using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace Medicae;

public class Health : EntityBehavior
{
    public void OnGameTick(IPlayer player)
    {
        player.Entity.ReceiveDamage(new DamageSource() { DamageTier = 0, Source = EnumDamageSource.Internal, Type = EnumDamageType.Hunger }, 0.2f);
    }
}