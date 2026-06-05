using Content.Shared._Starlight.Casino.Slots;
using Robust.Shared.Audio;

namespace Content.Server._Starlight.Casino.Slots;

[RegisterComponent]
public sealed partial class SlotMachineComponent : SharedSlotsMachineComponent
{
    /// <summary>
    /// Return to player value
    /// A value of 90%(0.9) means 10%(0.1) will go to "the house"
    /// </summary>
    [DataField]
    public float RTPValue = 0.95f;

    /// <summary>
    /// The current game instance for this machine
    /// </summary>
    [ViewVariables]
    public SlotMachineGame? Game = null;

    /// <summary>
    /// The sound played when a player spins.
    /// </summary>
    [DataField]
    public SoundSpecifier SpinSound = new SoundPathSpecifier("/Audio/Effects/Arcade/newgame.ogg");

    /// <summary>
    /// The sound played when a player stops a spin
    /// </summary>
    [DataField]
    public SoundSpecifier SpinStoppedSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_attack.ogg");

    /// <summary>
    /// The sound played when the machine returns more than the spin cost.
    /// </summary>
    [DataField]
    public SoundSpecifier PositiveReturnSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_attack.ogg");

    /// <summary>
    /// The sound played when the machine returns a jackpot.
    /// </summary>
    [DataField]
    public SoundSpecifier JackpotReturnSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_heal.ogg");

    /// <summary>
    /// The sound played when the player runs out of play credits.
    /// </summary>
    [DataField]
    public SoundSpecifier RanOutSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_charge.ogg");

}
