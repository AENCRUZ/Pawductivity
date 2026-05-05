using Pawductivity.Animations;
using Pawductivity.Managers;
using Pawductivity.Models;

namespace Pawductivity.Controls;

public class PetAnimationControl : PictureBox
{
    private readonly GameManager _gm;
    private readonly System.Windows.Forms.Timer _petAnimTimer;
    private readonly System.Windows.Forms.Timer _bubbleTimer;
    private readonly System.Windows.Forms.Timer _firstBubbleTimer;
    private readonly List<FloatingEffect> _effects = [];

    private int _petFrame;
    private int _petBounceOffset;
    private int _petBounceStep;
    private PetAnimationState _petState = PetAnimationState.Idle;

    private int _blinkTick;
    private bool _isBlinking;

    private string _bubbleText = "";
    private int _bubbleVisible;
    private ShopItemEffect? _shopEffect;

    private static readonly string[] CatHappy  = ["Purrrr~ 💜", "Meow! ✨", "Mrrrow~", "*purring*", "Nyaa~ 🎵", "Purrfect! 💜"];
    private static readonly string[] CatIdle   = ["Meow?", "...zzzz", "*yawns*", "Mrrp?", "*stares*", "Mew~"];
    private static readonly string[] CatSad    = ["Mrrrow... 😢", "*whimpers*", "Mew... 💧", "Nyo... 😿"];
    private static readonly string[] DogHappy  = ["Woof! 🐾", "WOOF WOOF!!", "*tail wags*", "Bork bork!", "Arf arf! 💛", "Hewwo!! 🐕"];
    private static readonly string[] DogIdle   = ["Woof?", "*sniffs*", "Bork?", "*yawns*", "Arf?", "*tilts head*"];
    private static readonly string[] DogSad    = ["Awoo... 😢", "*whines*", "Woof... 💧", "Arf... 😞"];
    private static readonly string[] CatSick = ["I feel awful... 🤒", "*coughs*", "Help me... 💊", "So dizzy..."];
    private static readonly string[] DogSick = ["I'm not well... 🤒", "*whimpers*", "Woof... 💊", "Feel bad..."];
    private static readonly Random _rng = new();

    public PetAnimationControl(GameManager gm)
    {
        _gm = gm;
        AutoSize = false;
        BackColor = Color.Transparent;

        _petAnimTimer = new System.Windows.Forms.Timer { Interval = 224 };
        _petAnimTimer.Tick += PetAnimTick;
        _petAnimTimer.Start();

        _bubbleTimer = new System.Windows.Forms.Timer { Interval = 5000 };
        _bubbleTimer.Tick += BubbleTick;
        _bubbleTimer.Start();

        _firstBubbleTimer = new System.Windows.Forms.Timer { Interval = 3000 };
        _firstBubbleTimer.Tick += FirstBubbleTick;
        _firstBubbleTimer.Start();
    }

    public void StopAnimation()
    {
        _petAnimTimer.Stop();
        _bubbleTimer.Stop();
        _firstBubbleTimer.Stop();
    }

    public void ShowTaskCompletion(PetChangeResult change)
    {
        if (!change.Success) return;

        AddFloating($"+{change.XpDelta} XP", -60, -78, Color.FromArgb(244, 170, 40));
        if (change.MoodDelta > 0) AddFloating($"+{change.MoodDelta} Mood", 34, -82, Color.FromArgb(220, 80, 150));
        if (change.CoinDelta > 0) AddFloating($"+{change.CoinDelta} Coins", -20, -112, Color.FromArgb(210, 145, 20));

        _bubbleText = "Task done! ✨";
        _bubbleVisible = 18;
        Invalidate();
    }

    public void ShowOverduePenalty(PetChangeResult change)
    {
        if (!change.Success) return;

        if (change.HealthDelta < 0) AddFloating($"{change.HealthDelta} Health", -62, -78, Color.FromArgb(210, 70, 70));
        if (change.MoodDelta < 0) AddFloating($"{change.MoodDelta} Mood", 30, -82, Color.FromArgb(125, 105, 190));

        _bubbleText = change.AffectedTasks == 1 ? "A task is overdue..." : $"{change.AffectedTasks} tasks overdue...";
        _bubbleVisible = 20;
        Invalidate();
    }

    public void ShowShopPurchase(PetChangeResult change)
    {
        if (!change.Success || change.Item is null) return;

        var item = change.Item;
        _shopEffect = new ShopItemEffect(item, 24);

        if (change.HealthDelta > 0) AddFloating($"+{change.HealthDelta} Health", -70, -82, Color.FromArgb(215, 65, 80));
        if (change.MoodDelta > 0) AddFloating($"+{change.MoodDelta} Mood", 36, -82, Color.FromArgb(220, 80, 150));

        _bubbleText = item.Name switch
        {
            "Star Cookie" => "Nom nom!",
            "Strawberry Milk" => "So refreshing!",
            "Cozy Blanket" => "So cozy!",
            "Rainbow Toy" => "Play time!",
            "Pink Ribbon" => "So cute!",
            "Flower Crown" => "Lovely!",
            _ => "Thank you!"
        };
        _bubbleVisible = 20;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        int cx = Width / 2;
        int cy = 130 + _petBounceOffset;

        bool isCat = _gm.Pet is CatPet;
        bool isHappy = _petState == PetAnimationState.Happy;
        bool isSad = _petState == PetAnimationState.Sad;
        var stage = _gm.Pet.Stage;
        bool blinking = _isBlinking;

        bool isSick = _petState == PetAnimationState.Sick;
        if (isCat) PetRenderer.DrawCat(g, cx, cy, isHappy, isSad, _petFrame, stage, blinking);
        else       PetRenderer.DrawDog(g, cx, cy, isHappy, isSad, _petFrame, stage, blinking);

        DrawShopEffect(g, cx, cy);
        DrawFloatingEffects(g, cx, cy);

        if (_bubbleVisible > 0 && !string.IsNullOrEmpty(_bubbleText))
            PetRenderer.DrawSpeechBubble(g, cx - 56, cy - 60, _bubbleText);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAnimation();
            _petAnimTimer.Dispose();
            _bubbleTimer.Dispose();
            _firstBubbleTimer.Dispose();
        }

        base.Dispose(disposing);
    }

    private void PetAnimTick(object? sender, EventArgs e)
    {
        _petFrame = (_petFrame + 1) % 4;

        int[] sineOffsets = [0, -2, -4, -6, -7, -6, -4, -2];
        _petBounceStep = (_petBounceStep + 1) % sineOffsets.Length;
        _petBounceOffset = sineOffsets[_petBounceStep];

        var mood = _gm.Pet.CurrentMood;
        _petState = mood switch
        {
            PetMood.Happy => PetAnimationState.Happy,
            PetMood.Sad   => PetAnimationState.Sad,
            PetMood.Sick  => PetAnimationState.Sick,
            _             => PetAnimationState.Idle,
        };

        _blinkTick++;
        if (_blinkTick >= 33)
        {
            _isBlinking = true;
            _blinkTick = 0;
        }
        else if (_isBlinking && _blinkTick >= 3)
        {
            _isBlinking = false;
        }

        if (_bubbleVisible > 0) _bubbleVisible--;
        UpdateVisualEffects();

        Invalidate();
    }

    private void FirstBubbleTick(object? sender, EventArgs e)
    {
        _firstBubbleTimer.Stop();
        ShowBubble();
    }

    private void ShowBubble()
    {
        bool isCat = _gm.Pet is CatPet;
        string[] pool = (_petState, isCat) switch
        {
            (PetAnimationState.Happy, true) => CatHappy,
            (PetAnimationState.Sad, true) => CatSad,
            (_, true) => _gm.Pet.IsSick ? CatSick : CatIdle,  // ← change
            (PetAnimationState.Happy, false) => DogHappy,
            (PetAnimationState.Sad, false) => DogSad,
            _ => _gm.Pet.IsSick ? DogSick : DogIdle,  // ← change
        };
        _bubbleText = pool[_rng.Next(pool.Length)];
        _bubbleVisible = 18;
        Invalidate();
    }

    private void BubbleTick(object? sender, EventArgs e)
    {
        if (_rng.Next(10) < 7) ShowBubble();
    }

    private void AddFloating(string text, int xOffset, int yOffset, Color color)
    {
        _effects.Add(new FloatingEffect(text, xOffset, yOffset, color));
    }

    private void UpdateVisualEffects()
    {
        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            _effects[i].Tick();
            if (_effects[i].IsDone) _effects.RemoveAt(i);
        }

        if (_shopEffect is not null)
        {
            _shopEffect.Tick();
            if (_shopEffect.IsDone) _shopEffect = null;
        }
    }

    private void DrawFloatingEffects(Graphics g, int cx, int cy)
    {
        using var font = new Font("Segoe UI", 9f, FontStyle.Bold);
        foreach (var effect in _effects)
        {
            int alpha = Math.Clamp(effect.Alpha, 0, 255);
            using var brush = new SolidBrush(Color.FromArgb(alpha, effect.Color));
            var point = new Point(cx + effect.XOffset, cy + effect.YOffset);
            TextRenderer.DrawText(g, effect.Text, font, point, brush.Color, TextFormatFlags.NoPrefix);
        }
    }

    private void DrawShopEffect(Graphics g, int cx, int cy)
    {
        if (_shopEffect is null) return;

        var (xOffset, yOffset, label) = _shopEffect.Item.Name switch
        {
            "Pink Ribbon" => (0, -82, "sparkle"),
            "Star Cookie" => (_shopEffect.Wiggle, -24, "eating"),
            "Strawberry Milk" => (_shopEffect.Wiggle, -24, "sip"),
            "Flower Crown" => (0, -88, "bloom"),
            "Cozy Blanket" => (0, 10, "warm"),
            "Rainbow Toy" => (_shopEffect.Wiggle * 2, -54, "play"),
            _ => (0, -60, "yay")
        };

        using var emojiFont = new Font("Segoe UI Emoji", 24f, FontStyle.Regular);
        using var labelFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        using var labelBrush = new SolidBrush(Color.FromArgb(_shopEffect.Alpha, PawTheme.Primary));

        var emojiPoint = new PointF(cx + xOffset - 14, cy + yOffset - 16);
        g.DrawString(_shopEffect.Item.Emoji, emojiFont, Brushes.Black, emojiPoint);
        g.DrawString(label, labelFont, labelBrush, cx + xOffset - 14, cy + yOffset + 16);
    }

    private sealed class FloatingEffect
    {
        private const int MaxLife = 18;
        private int _life = MaxLife;

        public FloatingEffect(string text, int xOffset, int yOffset, Color color)
        {
            Text = text;
            XOffset = xOffset;
            YOffset = yOffset;
            Color = color;
        }

        public string Text { get; }
        public int XOffset { get; }
        public int YOffset { get; private set; }
        public Color Color { get; }
        public bool IsDone => _life <= 0;
        public int Alpha => 255 * _life / MaxLife;

        public void Tick()
        {
            _life--;
            YOffset -= 2;
        }
    }

    private sealed class ShopItemEffect
    {
        private readonly int _maxLife;
        private int _life;

        public ShopItemEffect(ShopItem item, int life)
        {
            Item = item;
            _maxLife = life;
            _life = life;
        }

        public ShopItem Item { get; }
        public bool IsDone => _life <= 0;
        public int Alpha => Math.Clamp(255 * _life / _maxLife, 0, 255);
        public int Wiggle => _life % 2 == 0 ? -8 : 8;

        public void Tick() => _life--;
    }
}
