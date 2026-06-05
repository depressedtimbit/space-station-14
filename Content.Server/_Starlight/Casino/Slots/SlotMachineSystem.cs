using Content.Server.Power.Components;
using Content.Shared._Starlight.Casino.Slots;
using Content.Shared.UserInterface;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;

namespace Content.Server._Starlight.Casino.Slots;

public sealed partial class SlotMachineSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SlotMachineComponent, AfterActivatableUIOpenEvent>(OnAfterUiOpen);

        Subs.BuiEvents<SlotMachineComponent>(SharedSlotsMachineComponent.SlotMachineUiKey.Key, subs =>
        {
            subs.Event<SharedSlotsMachineComponent.SlotMachinePlayerActionMessage>(OnPlayerAction);
        });
    }

    private void OnAfterUiOpen(EntityUid uid, SlotMachineComponent component, AfterActivatableUIOpenEvent args)
    {
        component.Game ??= new SlotMachineGame(uid, component, this);

        component.Game.ExecutePlayerAction(uid, SharedSlotsMachineComponent.PlayerAction.RequestData, component);
    }


    public void OnPlayerAction(EntityUid uid, SlotMachineComponent component,  SharedSlotsMachineComponent.SlotMachinePlayerActionMessage msg)
    {
        if (component.Game == null)
            return;
        if (!SharedSlotsMachineComponent.SlotMachineUiKey.Key.Equals(msg.UiKey))
            return;
        if (!TryComp<ApcPowerReceiverComponent>(uid, out var power) || !power.Powered)
            return;

        switch (msg.Action)
        {
            case SharedSlotsMachineComponent.PlayerAction.StartSpin:
            case SharedSlotsMachineComponent.PlayerAction.Eject:
            case SharedSlotsMachineComponent.PlayerAction.ChangeBet:
            case SharedSlotsMachineComponent.PlayerAction.RequestData:
                component.Game.ExecutePlayerAction(uid, msg.Action, component);
                break;
        }
    }

}


