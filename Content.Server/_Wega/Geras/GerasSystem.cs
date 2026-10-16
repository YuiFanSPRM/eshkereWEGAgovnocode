using Content.Server.Actions;
using Content.Server.Polymorph.Systems;
using Content.Server.Popups;
using Content.Shared.Geras;
using Content.Shared.Zombies;
using Robust.Shared.Player;

namespace Content.Server.Geras;

/// <inheritdoc/>
public sealed partial class GerasSystem : SharedGerasSystem
{
    [Dependency] private ActionsSystem _actions = default!;
    [Dependency] private PolymorphSystem _polymorph = default!;
    [Dependency] private PopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GerasComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<GerasComponent, MorphIntoGeras>(OnMorphIntoGeras);
        SubscribeLocalEvent<GerasComponent, EntityZombifiedEvent>(OnZombification);
    }

    private void OnMapInit(Entity<GerasComponent> ent, ref MapInitEvent args)
    {
        _actions.AddAction(ent, ref ent.Comp.GerasActionEntity, ent.Comp.GerasAction);
    }

    private void OnZombification(Entity<GerasComponent> ent, ref EntityZombifiedEvent args)
    {
        _actions.RemoveAction(ent.Owner, ent.Comp.GerasActionEntity);
    }

    private void OnMorphIntoGeras(Entity<GerasComponent> ent, ref MorphIntoGeras args)
    {
        if (args.Handled || HasComp<ZombieComponent>(ent))
            return;

        var geras = _polymorph.PolymorphEntity(ent, ent.Comp.GerasPolymorphId);
        if (geras == null)
            return;

        _popup.PopupEntity(Loc.GetString("geras-popup-morph-message-others", ("entity", geras.Value)), geras.Value, Filter.PvsExcept(geras.Value), true);
        _popup.PopupEntity(Loc.GetString("geras-popup-morph-message-user"), geras.Value, geras.Value);

        args.Handled = true;
    }
}
