using System.Linq;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;
using Robust.Shared.Audio;
using static Content.Shared._Starlight.Casino.Slots.SharedSlotsMachineComponent;

namespace Content.Server._Starlight.Casino.Slots;

/// <summary>
/// A Class to handle all the game-logic of a slot machine
/// </summary>
public sealed partial class SlotMachineGame
{
    [Dependency] private IEntityManager _entityManager = default!;
    [Dependency] private IRobustRandom _random = default!;
    private readonly UserInterfaceSystem _uiSystem = default!;
    private readonly SharedAudioSystem _audioSystem = default!;
    private readonly SlotMachineSystem _slotMachineSystem = default!;

    [ViewVariables]
    private readonly EntityUid _owner = default!;

    [ViewVariables]
    private bool _running = true;

    private bool _spinning = false;

    [ViewVariables]
    private int _revealedSlots = 3;

    [ViewVariables(VVAccess.ReadWrite)]
    private readonly Symbols[] _reels = new Symbols[3];

    [ViewVariables(VVAccess.ReadWrite)]
    private int _playCredit = 100;

    [ViewVariables(VVAccess.ReadWrite)]
    private int _currentBet = 0;

    public SlotMachineGame(EntityUid owner, SlotMachineComponent component, SlotMachineSystem system)
    {
        IoCManager.InjectDependencies(this);
        _audioSystem = _entityManager.System<SharedAudioSystem>();
        _uiSystem = _entityManager.System<UserInterfaceSystem>();
        _slotMachineSystem = _entityManager.System<SlotMachineSystem>();

        _owner = owner;

        for(var r = 0; r < _reels.Length; r++)
        {
            _reels[r] = Symbols.Blank;
        }
    }

    public void ExecutePlayerAction(EntityUid uid, PlayerAction action, SlotMachineComponent component)
    {
        if (!_running)
            return;

        switch (action)
        {
            case PlayerAction.StartSpin:
                if (_revealedSlots > 2)
                {
                    StartSpin(uid, component);
                    break;
                }
                RevealSlot(uid, component);
                if (_revealedSlots > 2)
                    CalculateWin(uid, component);
                break;
            case PlayerAction.ChangeBet:
                if (_spinning)
                    break;
                _currentBet++;
                if (_currentBet == component.BetDenomination.Length)
                    _currentBet = 0;
                UpdateUi(uid, component);
                break;
            case PlayerAction.Eject:
                if (_spinning)
                    break;
                break;
            case PlayerAction.RequestData:
                UpdateUi(uid, component);
                break;
        }
    }

    private void StartSpin(EntityUid uid, SlotMachineComponent component)
    {
        if (_playCredit < component.BetDenomination[_currentBet])
        {
            _audioSystem.PlayPvs(component.RanOutSound, uid, AudioParams.Default.WithVolume(-4f));
            return;
        }

        _playCredit -= component.BetDenomination[_currentBet];
        _spinning = true;
        _revealedSlots = 0;
        for(var r = 0; r < _reels.Length; r++)
        {
            _reels[r] = Symbols.Spinning;
        }
        _audioSystem.PlayPvs(component.SpinSound, uid, AudioParams.Default.WithVolume(-4f));
        UpdateUi(uid, component);
    }

    private void RevealSlot(EntityUid uid, SlotMachineComponent component)
    {
        _reels[_revealedSlots] = GetRandomPull(uid, component);
        _revealedSlots++;
        _audioSystem.PlayPvs(component.SpinStoppedSound, uid, AudioParams.Default.WithVolume(-4f));
        UpdateUi(uid, component);
    }

    public Symbols GetRandomPull(EntityUid uid, SlotMachineComponent component)
    {
        var sum = 0;
        var ran = _random.Next(ReelLength-1);
        foreach (var (key, value) in component.WeightTable)
        {
            sum += value;
            if (ran < sum)
                return key;
        }

        return 0; //fall back to blank
    }

    private void CalculateWin(EntityUid uid, SlotMachineComponent component)
    {
        var winFloat = 0f;
        _spinning = false;
        if (_reels[0] == _reels[1] && _reels[1] == _reels[2])
            winFloat = component.PayoutTable[_reels[1]].Item2;
        else if ( _reels[0] == _reels[1] || _reels[1] == _reels[2]) // matching symbols must be touching, so win|loose|win is still a loose
            winFloat = component.PayoutTable[_reels[1]].Item1;


        var won = (int)MathF.Round(winFloat * component.BetDenomination[_currentBet] * component.RTPValue * MathF.Pow(ReelLength, 3));
        if (won > component.BetDenomination[_currentBet] * 10) //TODO make cutting jackpot wires always/never play this sound
            _audioSystem.PlayPvs(component.JackpotReturnSound, uid, AudioParams.Default.WithVolume(-4f));
        else if (won > component.BetDenomination[_currentBet])
            _audioSystem.PlayPvs(component.PositiveReturnSound, uid, AudioParams.Default.WithVolume(-4f));

        _playCredit += won;
        UpdateUi(uid, component);
    }
}
