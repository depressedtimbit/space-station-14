using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Starlight.Casino.Slots;

public abstract partial class SharedSlotsMachineComponent : Component
{
    [Serializable, NetSerializable]
    public enum SlotMachineUiKey
    {
        Key,
    }

    [Serializable, NetSerializable]
    public enum PlayerAction
    {
        StartSpin,
        Eject,
        ChangeBet,
        RequestData
    }

    [DataField]
    public int[] BetDenomination = [1, 10, 100];

    /// <summary>
    /// payouts for each symbol.
    /// item1 is the result for 2 matching symbols, item2 for 3 matching symbols.
    /// </summary>
    /// <remarks>
    /// how do balance gamble box?
    /// for every float in this table use the formula float*(NumInWeightTable^3)
    /// add the result to a sum
    /// make sure the sum is equal to 1!
    ///
    /// the final payout uses the formula
    /// Float * currentBet * RTPValue * (Reel^3)
    /// </remarks>
    [DataField]
    public Dictionary<Symbols, (float, float)> PayoutTable = new()
    {
        {Symbols.Blank,       (0.00f,         0.00f)},
        {Symbols.Fruit,       (0.00001f,      0.00003f)},  // 0.01000  0.03000
        {Symbols.Bell,        (0.00001f,      0.00003f)},  // 0.01000  0.03000
        {Symbols.Cherry,      (0.00005f,      0.00009f)},  // 0.02564  0.04608
        {Symbols.SingleBar,   (0.0002f,       0.0004f)},   // 0.04320  0.08640
        {Symbols.DoubleBar,   (0.002f,        0.004f)},    // 0.05400  0.10800
        {Symbols.TripleBar,   (0.1f,          0.45668f)},  // 0.10000  0.45668
    };

    [DataField]
    public Dictionary<Symbols, int> WeightTable = new()
    {
        {Symbols.Blank,         18},
        {Symbols.Fruit,         10},
        {Symbols.Bell,          10},
        {Symbols.Cherry,        08},
        {Symbols.SingleBar,     06},
        {Symbols.DoubleBar,     03},
        {Symbols.TripleBar,     01},
        //                      56
    };

    /// <remarks>
    /// this must be the sum of all ints in WeightTable
    /// </remarks>
    [DataField]
    public const int ReelLength = 56;

    [Serializable, NetSerializable]
    public enum Currency
    {
        Credits,
        Speso,
        Telecrystal
    }

    [Serializable, NetSerializable]
    public enum Symbols : byte
    {
        Blank,
        Fruit,
        Bell,
        Cherry,
        SingleBar,
        DoubleBar,
        TripleBar,
        Spinning
    }

    [Serializable, NetSerializable]
    public sealed class SlotMachinePlayerActionMessage(PlayerAction action) : BoundUserInterfaceMessage
    {
        public readonly PlayerAction Action = action;
    }

    [Serializable, NetSerializable, Virtual]
    public class SlotMachineDataUpdateMessage : BoundUserInterfaceMessage
    {
        public readonly Symbols RightSlot;
        public readonly Symbols MiddleSlot;
        public readonly Symbols LeftSlot;
        public readonly float Tokens;
        public readonly int Bet;
        public readonly Currency BettingCurrency;
        public readonly bool ControlsLocked;
        public SlotMachineDataUpdateMessage(Symbols rightSlot, Symbols middleSlot, Symbols leftSlot, float tokens,
            Currency bettingCurrency, int bet, bool controlsLocked)
        {
            RightSlot = rightSlot;
            MiddleSlot = middleSlot;
            LeftSlot = leftSlot;
            Tokens = tokens;
            Bet = bet;
            BettingCurrency = bettingCurrency;
            ControlsLocked = controlsLocked;
        }
    }

}


