using Robust.Client.UserInterface;
using static Content.Shared._Starlight.Casino.Slots.SharedSlotsMachineComponent;

namespace Content.Client._Starlight.Casino.Slots;

public sealed class SlotMachineBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables] private SlotMachineMenu? _menu;


    public void SendAction(PlayerAction action)
        => SendMessage(new SlotMachinePlayerActionMessage(action));

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<SlotMachineMenu>();
        _menu.OnPlayerAction += SendAction;
    }

    protected override void ReceiveMessage(BoundUserInterfaceMessage message)
    {
        if (message is SlotMachineDataUpdateMessage msg)
            _menu?.UpdateInfo(msg);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _menu?.Dispose();
    }
}
