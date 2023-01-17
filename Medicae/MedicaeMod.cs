
using System;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Medicae;

public class Medicae : ModSystem
{
    private ICoreAPI _api;
    private ICoreClientAPI _capi;
    private ICoreServerAPI _sapi;

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        this._api = api;
        if (api.Side == EnumAppSide.Server)
            _sapi = api as ICoreServerAPI;
        else
            _capi = api as ICoreClientAPI;
        
        this._api.RegisterItemClass("ItemTourniquet", typeof(ItemTourniquet));
        this._api.RegisterItemClass("ItemSplint", typeof(ItemSplint));

        this._api.RegisterEntityClass("Health", new EntityProperties());
    }
}