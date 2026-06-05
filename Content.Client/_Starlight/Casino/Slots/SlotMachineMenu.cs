using System.Numerics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Utility;
using static Content.Shared._Starlight.Casino.Slots.SharedSlotsMachineComponent;

namespace Content.Client._Starlight.Casino.Slots;

public sealed class SlotMachineMenu : DefaultWindow
{
    private readonly Label _tokensLabel;

    private readonly Button[] _configButtons = new Button[2];
    private readonly AnimatedTextureRect[] _reels = new AnimatedTextureRect[3];
    private readonly Button _betAmountButton;
    private readonly Button _spinButton;
    private readonly ResPath _reelTex = new ("/Textures/_Starlight/Markers/jobs.rsi");

    public event Action<PlayerAction>? OnPlayerAction;

    public SlotMachineMenu()
    {
        MinSize = SetSize = new Vector2(500, 300);
        Title = Loc.GetString("slotmachine-menu-title");

        var grid = new GridContainer { Rows = 3 };

        var reelGrids = new BoxContainer() { HorizontalExpand =  true, VerticalExpand = true };

        var specifier = new SpriteSpecifier.Rsi(_reelTex, "surgeon");
        _reels[0] = new AnimatedTextureRect() { };
        _reels[0].SetFromSpriteSpecifier(specifier);
        _reels[0].DisplayRect.Stretch = TextureRect.StretchMode.KeepAspectCentered;
        _reels[0].DisplayRect.TextureScale = new Vector2(2f);
        _reels[1] = new AnimatedTextureRect() {  };
        _reels[1].SetFromSpriteSpecifier(specifier);
        _reels[1].DisplayRect.Stretch = TextureRect.StretchMode.KeepAspectCentered;
        _reels[1].DisplayRect.TextureScale = new Vector2(2f);
        _reels[2] = new AnimatedTextureRect() {  };
        _reels[2].SetFromSpriteSpecifier(specifier);
        _reels[2].DisplayRect.Stretch = TextureRect.StretchMode.KeepAspectCentered;
        _reels[2].DisplayRect.TextureScale = new Vector2(2f);

        reelGrids.AddChild(_reels[0]);
        reelGrids.AddChild(_reels[1]);
        reelGrids.AddChild(_reels[2]);
        grid.AddChild(reelGrids);

        _tokensLabel = new Label { Text = Loc.GetString("slotmachine-tokens-label") };
        grid.AddChild(_tokensLabel);

        _betAmountButton = new Button { Text = Loc.GetString("slotmachine-bet-amount-label") };
        _betAmountButton.OnPressed +=
            _ => OnPlayerAction?.Invoke(PlayerAction.ChangeBet);
        grid.AddChild(_betAmountButton);

        _spinButton = new Button { Text = Loc.GetString("slotmachine-spin-button") };
        _spinButton.OnPressed +=
            _ => OnPlayerAction?.Invoke(PlayerAction.StartSpin);
        grid.AddChild(_spinButton);

        ContentsContainer.AddChild(grid);
    }

    public void UpdateInfo(SlotMachineDataUpdateMessage message)
    {
        _betAmountButton.Text = $"Betting: {message.Bet} {message.BettingCurrency.ToString()}";
        _tokensLabel.Text = $"{message.Tokens}";

        _reels[0].SetFromSpriteSpecifier(GetSymbolSpecifier(message.LeftSlot));
        _reels[1].SetFromSpriteSpecifier(GetSymbolSpecifier(message.MiddleSlot));
        _reels[2].SetFromSpriteSpecifier(GetSymbolSpecifier(message.RightSlot));

        _betAmountButton.Disabled = message.ControlsLocked;
    }

    public SpriteSpecifier GetSymbolSpecifier(Symbols symbol)
    {
        var state = symbol switch
        {
            Symbols.Blank => "blank",
            Symbols.Fruit => "fruit",
            Symbols.Bell => "bell",
            Symbols.Cherry => "cherry",
            Symbols.SingleBar => "singlebar",
            Symbols.DoubleBar => "doublebar",
            Symbols.TripleBar => "triplebar",
            Symbols.Spinning => "spinning",
            _ => "blank"
        };

        return new SpriteSpecifier.Rsi(_reelTex, state);
    }
}
