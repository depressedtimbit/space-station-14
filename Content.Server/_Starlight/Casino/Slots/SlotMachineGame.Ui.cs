using static Content.Shared._Starlight.Casino.Slots.SharedSlotsMachineComponent;

namespace Content.Server._Starlight.Casino.Slots;

public sealed partial class SlotMachineGame
{
    private void UpdateUi(EntityUid uid, SlotMachineComponent component) =>
        _uiSystem.ServerSendUiMessage(uid, SlotMachineUiKey.Key, new SlotMachineDataUpdateMessage
        (
            _reels[0], _reels[1], _reels[2],
            _playCredit,
            0,
            component.BetDenomination[_currentBet],
            _spinning
        ));
}
