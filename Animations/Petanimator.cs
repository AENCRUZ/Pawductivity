using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Pawductivity.Models;

namespace Pawductivity.Animations;

/// Core animation engine for Pawductivity.
/// All rendering is procedural GDI+ — no external assets required.
public class PetAnimator : IDisposable
{
    // ── State ────────────────────────────────────────────────────────────────
    private readonly Panel _canvas;
    private PetType _petType;
    private PetEvolution _stage = PetEvolution.Egg;

    // Idle
    private float _breathPhase;       // 0 → 2π, drives breathing sine
    private int _blinkFrame;        // 0=open, 1-3=closing/closed/opening
    private float _eyeOpenRatio = 1f; // 1=open, 0=closed
    private float _breathY;           // pixel offset for breathing
    private readonly System.Windows.Forms.Timer _idleTimer = new();
    private readonly System.Windows.Forms.Timer _blinkTimer = new();
    private readonly Random _rng = new();

    // Emotion state (driven by pet mood from RefreshAll)
    private bool _happy = false;
    private bool _sad = false;

    // Frame counter (0-3) cycles with breath timer for wag/sparkle alternation
    private int _frame = 0;

    // Overlay effect (XP flash, mood, health, coins, level-up burst)
    private OverlayEffect? _overlay;
    private readonly System.Windows.Forms.Timer _overlayTimer = new();

    // ── Public API ───────────────────────────────────────────────────────────
    public PetAnimator(Panel canvas, PetType petType)
    {
        _canvas = canvas;
        _petType = petType;

        // Double-buffer the canvas to eliminate flicker
        typeof(Panel).GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(_canvas, true);

        _canvas.Paint += OnPaint;

        // Idle breath: ~60 fps tick
        _idleTimer.Interval = 16;
        _idleTimer.Tick += IdleTick;
        _idleTimer.Start();

        // Blink: fires every 3–5 seconds (randomised)
        ScheduleNextBlink();

        // Overlay effect timer
        _overlayTimer.Interval = 16;
        _overlayTimer.Tick += OverlayTick;
    }

    /// Call when the user's pet type changes (e.g. on profile switch).
    public void SetPetType(PetType t) { _petType = t; _canvas.Invalidate(); }

    /// Call whenever pet.Stage changes (including on load) so the correct sprite
    /// is displayed immediately.
    public void SetStage(PetEvolution stage) { _stage = stage; _canvas.Invalidate(); }

    /// Call from RefreshAll() whenever mood stats change.
    /// happy = Mood >= 70, sad = Mood < 40
    public void SetMood(bool happy, bool sad)
    {
        _happy = happy;
        _sad = sad;
        _canvas.Invalidate();
    }

    // ── Trigger methods ──────────────────────────────────────────────────────

    /// Plays XP + Mood gain animation after a task is completed.
    public void TriggerTaskComplete(int xpGained, int coinsGained)
    {
        StartOverlay(new OverlayEffect
        {
            Kind = EffectKind.TaskComplete,
            XpGained = xpGained,
            CoinsGained = coinsGained,
            Progress = 0f,
            Duration = 90,
        });
    }

    /// Plays Health + Mood loss animation when a task goes overdue.
    public void TriggerOverdue()
    {
        StartOverlay(new OverlayEffect
        {
            Kind = EffectKind.Overdue,
            Progress = 0f,
            Duration = 70,
        });
    }

    /// Plays a level-up burst with stage-appropriate colours.
    /// Also advances the internal stage to match the new level.
    public void TriggerLevelUp(int newLevel)
    {
        PetEvolution newStage = newLevel switch
        {
            >= 10 => PetEvolution.Legend,
            >= 7 => PetEvolution.Adult,
            >= 4 => PetEvolution.Junior,
            >= 2 => PetEvolution.Baby,
            _ => PetEvolution.Egg,
        };

        StartOverlay(new OverlayEffect
        {
            Kind = EffectKind.LevelUp,
            NewLevel = newLevel,
            NewStage = newStage,
            OldStage = _stage,
            Progress = 0f,
            Duration = 140,
        });
        _stage = newStage;
    }

    /// Plays coin gain animation.
    public void TriggerCoinGain(int coins)
    {
        StartOverlay(new OverlayEffect
        {
            Kind = EffectKind.CoinGain,
            CoinsGained = coins,
            Progress = 0f,
            Duration = 60,
        });
    }

    /// Plays the shop item purchase animation.
    public void TriggerShopItem(string itemName)
    {
        var kind = itemName switch
        {
            "Star Cookie" => EffectKind.ShopEat,
            "Strawberry Milk" => EffectKind.ShopHealth,
            _ => EffectKind.ShopMood,
        };

        StartOverlay(new OverlayEffect
        {
            Kind = kind,
            ItemName = itemName,
            Progress = 0f,
            Duration = 80,
        });
    }

    // ── Internal idle loop ───────────────────────────────────────────────────
    private void IdleTick(object? s, EventArgs e)
    {
        _breathPhase += 0.06f;
        if (_breathPhase > MathF.PI * 2) _breathPhase -= MathF.PI * 2;

        // Advance frame counter every ~32 ticks (~0.5 s) for wag alternation
        if ((int)(_breathPhase / 0.06f) % 32 == 0)
            _frame = (_frame + 1) % 4;

        float amplitude = _stage == PetEvolution.Egg ? 1.5f : 2.5f;
        _breathY = MathF.Sin(_breathPhase) * amplitude;
        _canvas.Invalidate();
    }

    private void ScheduleNextBlink()
    {
        if (_stage == PetEvolution.Egg) { ScheduleNextBlinkDelayed(); return; }
        int ms = _rng.Next(3000, 5000);
        var t = new System.Windows.Forms.Timer { Interval = ms };
        t.Tick += (s, e) => { t.Stop(); t.Dispose(); PlayBlink(); };
        t.Start();
    }

    private void ScheduleNextBlinkDelayed()
    {
        var t = new System.Windows.Forms.Timer { Interval = 500 };
        t.Tick += (s, e) => { t.Stop(); t.Dispose(); ScheduleNextBlink(); };
        t.Start();
    }

    private void PlayBlink()
    {
        _blinkFrame = 1;
        _blinkTimer.Interval = 60;
        _blinkTimer.Tick -= BlinkTick;
        _blinkTimer.Tick += BlinkTick;
        _blinkTimer.Start();
    }

    private void BlinkTick(object? s, EventArgs e)
    {
        _blinkFrame++;
        _eyeOpenRatio = _blinkFrame switch
        {
            1 => 0.5f,
            2 => 0f,
            3 => 0.5f,
            _ => 1f,
        };
        if (_blinkFrame >= 4)
        {
            _blinkTimer.Stop();
            _blinkFrame = 0;
            _eyeOpenRatio = 1f;
            ScheduleNextBlink();
        }
    }

    // ── Overlay system ───────────────────────────────────────────────────────
    private void StartOverlay(OverlayEffect fx)
    {
        _overlay = fx;
        _overlayTimer.Start();
    }

    private void OverlayTick(object? s, EventArgs e)
    {
        if (_overlay is null) { _overlayTimer.Stop(); return; }
        _overlay.Progress++;
        if (_overlay.Progress >= _overlay.Duration)
        {
            _overlay = null;
            _overlayTimer.Stop();
        }
        _canvas.Invalidate();
    }

    // ── Paint ────────────────────────────────────────────────────────────────
    private void OnPaint(object? s, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        int cx = _canvas.Width / 2;
        int cy = _canvas.Height / 2;

        bool inTransition = _overlay?.Kind == EffectKind.LevelUp;
        float breathOffset = inTransition ? 0 : _breathY;

        g.TranslateTransform(0, breathOffset);
        DrawCurrentSprite(g, cx, cy);
        g.ResetTransform();

        if (_overlay is not null)
            DrawOverlay(g, _overlay, cx, cy);
    }

    // ── Stage-aware sprite routing ───────────────────────────────────────────
    // Routes to smooth GDI+ sprite methods (DashboardForm design)
    private void DrawCurrentSprite(Graphics g, int cx, int cy)
    {
        bool blinking = _eyeOpenRatio < 0.9f;

        if (_petType == PetType.Cat)
        {
            switch (_stage)
            {
                case PetEvolution.Egg:
                    CatSprites.DrawEgg(g, cx, cy, _breathY);
                    break;
                case PetEvolution.Baby:
                    CatSprites.DrawBaby(g, cx, cy, _eyeOpenRatio, _happy, _sad, _frame, blinking);
                    break;
                case PetEvolution.Junior:
                    CatSprites.DrawJunior(g, cx, cy, _eyeOpenRatio, _happy, _sad, _frame, blinking);
                    break;
                case PetEvolution.Adult:
                    CatSprites.DrawAdult(g, cx, cy, _eyeOpenRatio, _happy, _sad, _frame, blinking);
                    break;
                case PetEvolution.Legend:
                    CatSprites.DrawLegend(g, cx, cy, _eyeOpenRatio, _breathPhase, _happy, _sad, _frame, blinking);
                    break;
            }
        }
        else
        {
            switch (_stage)
            {
                case PetEvolution.Egg:
                    DogSprites.DrawEgg(g, cx, cy, _breathY);
                    break;
                case PetEvolution.Baby:
                    DogSprites.DrawBaby(g, cx, cy, _eyeOpenRatio, _happy, _sad, _frame, blinking);
                    break;
                case PetEvolution.Junior:
                    DogSprites.DrawJunior(g, cx, cy, _eyeOpenRatio, _breathPhase, _happy, _sad, _frame, blinking);
                    break;
                case PetEvolution.Adult:
                    DogSprites.DrawAdult(g, cx, cy, _eyeOpenRatio, _breathPhase, _happy, _sad, _frame, blinking);
                    break;
                case PetEvolution.Legend:
                    DogSprites.DrawLegend(g, cx, cy, _eyeOpenRatio, _breathPhase, _happy, _sad, _frame, blinking);
                    break;
            }
        }
    }

    // ── Overlay effects ──────────────────────────────────────────────────────
    private void DrawOverlay(Graphics g, OverlayEffect fx, int cx, int cy)
    {
        float t = fx.Progress / fx.Duration; // 0 → 1
        switch (fx.Kind)
        {
            case EffectKind.TaskComplete:
                DrawFloatyText(g, $"+{fx.XpGained} XP ⭐", cx - 30, (int)(cy - 20 - t * 40), Paw.XpColor, t);
                DrawFloatyText(g, $"+😸 Mood", cx - 25, (int)(cy + 5 - t * 30), Paw.MoodColor, t);
                if (fx.CoinsGained > 0)
                    DrawFloatyText(g, $"+{fx.CoinsGained} 🪙", cx - 20, (int)(cy + 25 - t * 25), Paw.CoinColor, t);
                DrawSparkles(g, cx, cy, t, Paw.XpColor);
                break;

            case EffectKind.Overdue:
                DrawFloatyText(g, "-❤️ Health", cx - 28, (int)(cy - 20 - t * 35), Paw.DmgRed, t, descend: true);
                DrawFloatyText(g, "-😸 Mood", cx - 22, (int)(cy + 5 - t * 25), Paw.DmgRed, t, descend: true);
                DrawShake(g, cx, cy, t);
                break;

            case EffectKind.LevelUp:
                DrawLevelUpBurst(g, cx, cy, t, fx.NewLevel, fx.OldStage, fx.NewStage);
                break;

            case EffectKind.CoinGain:
                DrawFloatyText(g, $"+{fx.CoinsGained} 🪙", cx - 20, (int)(cy - 15 - t * 40), Paw.CoinColor, t);
                DrawSparkles(g, cx, cy, t, Paw.CoinColor);
                break;

            case EffectKind.ShopEat:
                DrawFloatyText(g, "Nom nom! 🍪", cx - 30, (int)(cy - 20 - t * 35), Paw.HealthColor, t);
                DrawFloatyText(g, "+❤️ +😸", cx - 20, (int)(cy + 5 - t * 25), Paw.MoodColor, t);
                DrawHearts(g, cx, cy, t, Color.FromArgb(255, 180, 200));
                break;

            case EffectKind.ShopHealth:
                DrawFloatyText(g, "+❤️ Health", cx - 25, (int)(cy - 20 - t * 40), Paw.HealthColor, t);
                DrawSparkles(g, cx, cy, t, Paw.HealthColor);
                DrawExpandingRing(g, cx, cy, t, Paw.HealthColor, ringScale: 55f);
                break;

            case EffectKind.ShopMood:
                DrawFloatyText(g, "+😸 Mood", cx - 22, (int)(cy - 20 - t * 40), Paw.MoodColor, t);
                DrawSparkles(g, cx, cy, t, Paw.MoodColor);
                DrawHearts(g, cx, cy, t, Color.FromArgb(255, 220, 100));
                break;
        }
    }

    // ── Level-up burst ───────────────────────────────────────────────────────
    private void DrawLevelUpBurst(Graphics g, int cx, int cy, float t,
                                   int newLevel, PetEvolution oldStage, PetEvolution newStage)
    {
        bool isStageChange = newStage != oldStage;
        StageTransition transition = newLevel switch
        {
            2 => StageTransition.EggToBaby,
            4 => StageTransition.BabyToJunior,
            7 => StageTransition.JuniorToAdult,
            >= 10 => StageTransition.AdultToLegend,
            _ => StageTransition.Generic,
        };

        if (_petType == PetType.Cat)
            DrawCatLevelUp(g, cx, cy, t, transition, isStageChange);
        else
            DrawDogLevelUp(g, cx, cy, t, transition, isStageChange);
    }

    private static void DrawCatLevelUp(Graphics g, int cx, int cy, float t,
                                        StageTransition stage, bool isStageChange)
    {
        switch (stage)
        {
            case StageTransition.EggToBaby:
                if (t < 0.5f)
                {
                    float tp = t * 2f;
                    DrawShards(g, cx, cy, tp, Paw.EarPink, count: 8);
                    DrawExpandingRing(g, cx, cy, tp, Paw.EggShell, ringScale: 50f);
                    DrawFloatyText(g, "🥚 Cracking...", cx - 40, (int)(cy - 45 - tp * 10), Paw.EggShell, tp);
                }
                else
                {
                    float tp = (t - 0.5f) * 2f;
                    DrawExpandingRing(g, cx, cy, tp, Paw.EarPink, ringScale: 55f);
                    DrawHearts(g, cx, cy, tp, Paw.EarPink);
                    DrawFloatyText(g, "🐱 Born! Meow~", cx - 44, (int)(cy - 50 - tp * 12), Paw.CatOrange, tp);
                }
                break;

            case StageTransition.BabyToJunior:
                DrawHearts(g, cx, cy, t, Paw.EarPink);
                DrawExpandingRing(g, cx, cy, t, Paw.NosePink, ringScale: 60f);
                DrawFloatyText(g, "🐱→🐈 Growing up!", cx - 55, (int)(cy - 50 - t * 12), Paw.NosePink, t);
                break;

            case StageTransition.JuniorToAdult:
                DrawDoubleRing(g, cx, cy, t, Paw.XpColor);
                DrawSparkles(g, cx, cy, t, Paw.XpColor);
                DrawSparkles(g, cx, cy, t * 0.8f, Color.White);
                DrawFloatyText(g, "🐈 Fully Grown! ✨", cx - 52, (int)(cy - 52 - t * 14), Paw.XpColor, t);
                break;

            case StageTransition.AdultToLegend:
                if (t < 0.35f)
                {
                    float tp = t / 0.35f;
                    float alpha = 1f - tp;
                    int a = (int)(alpha * 180);
                    using var flash = new SolidBrush(Color.FromArgb(a, Color.White));
                    g.FillRectangle(flash, 0, 0, 999, 999);
                    DrawExpandingRing(g, cx, cy, tp, Color.Gold, ringScale: 90f, thickness: 5f);
                }
                else
                {
                    float tp = (t - 0.35f) / 0.65f;
                    DrawExpandingRing(g, cx, cy, tp, Color.Gold, ringScale: 80f, thickness: 4f);
                    DrawExpandingRing(g, cx, cy, tp * 0.75f, Color.White, ringScale: 65f, thickness: 2f);
                    DrawSparkles(g, cx, cy, tp, Color.Gold);
                    DrawSparkles(g, cx, cy, tp, Color.White);
                    DrawDiamondBurst(g, cx, cy, tp, Color.Gold);
                    DrawFloatyText(g, "✨ LEGEND CAT! ✨", cx - 56, (int)(cy - 55 - tp * 16), Color.Gold, tp);
                }
                break;

            default:
                DrawExpandingRing(g, cx, cy, t, Paw.XpColor, ringScale: 50f);
                DrawSparkles(g, cx, cy, t, Color.Gold);
                DrawFloatyText(g, "Level Up! ⭐", cx - 36, (int)(cy - 48 - t * 10), Paw.XpColor, t);
                break;
        }
    }

    private static void DrawDogLevelUp(Graphics g, int cx, int cy, float t,
                                        StageTransition stage, bool isStageChange)
    {
        switch (stage)
        {
            case StageTransition.EggToBaby:
                if (t < 0.5f)
                {
                    float tp = t * 2f;
                    DrawShards(g, cx, cy, tp, Paw.DogCream, count: 10);
                    DrawExpandingRing(g, cx, cy, tp, Paw.EggShell, ringScale: 50f);
                    DrawFloatyText(g, "🥚 Hatching...", cx - 42, (int)(cy - 45 - tp * 10), Paw.DogBrown, tp);
                }
                else
                {
                    float tp = (t - 0.5f) * 2f;
                    DrawExpandingRing(g, cx, cy, tp, Paw.MoodColor, ringScale: 55f);
                    DrawPawPrints(g, cx, cy, tp, Paw.DogBrown);
                    DrawFloatyText(g, "🐶 Woof! Born!", cx - 42, (int)(cy - 50 - tp * 12), Paw.DogBrown, tp);
                }
                break;

            case StageTransition.BabyToJunior:
                DrawPawPrints(g, cx, cy, t, Paw.DogBrown);
                DrawExpandingRing(g, cx, cy, t, Paw.MoodColor, ringScale: 60f);
                DrawFloatyText(g, "🐶→🐕 Good boy!", cx - 52, (int)(cy - 50 - t * 12), Paw.MoodColor, t);
                break;

            case StageTransition.JuniorToAdult:
                DrawDoubleRing(g, cx, cy, t, Paw.DogBrown);
                DrawSparkles(g, cx, cy, t, Paw.MoodColor);
                DrawSparkles(g, cx, cy, t * 0.8f, Paw.DogCream);
                DrawFloatyText(g, "🐕→🦮 So Strong!", cx - 52, (int)(cy - 52 - t * 14), Paw.DogBrown, t);
                break;

            case StageTransition.AdultToLegend:
                if (t < 0.35f)
                {
                    float tp = t / 0.35f;
                    float alpha = 1f - tp;
                    int a = (int)(alpha * 160);
                    using var flash = new SolidBrush(Color.FromArgb(a, Color.Goldenrod));
                    g.FillRectangle(flash, 0, 0, 999, 999);
                    DrawExpandingRing(g, cx, cy, tp, Color.Goldenrod, ringScale: 90f, thickness: 5f);
                }
                else
                {
                    float tp = (t - 0.35f) / 0.65f;
                    DrawExpandingRing(g, cx, cy, tp, Color.Goldenrod, ringScale: 80f, thickness: 4f);
                    DrawExpandingRing(g, cx, cy, tp * 0.75f, Paw.MoodColor, ringScale: 65f, thickness: 2f);
                    DrawSparkles(g, cx, cy, tp, Color.Goldenrod);
                    DrawPawPrints(g, cx, cy, tp, Color.Goldenrod);
                    DrawFloatyText(g, "✨ LEGEND DOG! ✨", cx - 56, (int)(cy - 55 - tp * 16), Color.Goldenrod, tp);
                }
                break;

            default:
                DrawExpandingRing(g, cx, cy, t, Paw.MoodColor, ringScale: 50f);
                DrawSparkles(g, cx, cy, t, Color.Gold);
                DrawFloatyText(g, "Level Up! ⭐", cx - 36, (int)(cy - 48 - t * 10), Paw.MoodColor, t);
                break;
        }
    }

    // ── Shared stage-effect primitives ───────────────────────────────────────
    private static void DrawExpandingRing(Graphics g, int cx, int cy, float t,
                                           Color color, float ringScale = 70f, float thickness = 3f)
    {
        float ringR = t * ringScale;
        float alpha = t < 0.5f ? t * 2f : 1f - (t - 0.5f) * 2f;
        if (alpha <= 0 || ringR <= 0) return;
        int a = (int)(alpha * 210);
        using var pen = new Pen(Color.FromArgb(a, color), thickness);
        g.DrawEllipse(pen, cx - ringR, cy - ringR, ringR * 2, ringR * 2);
    }

    private static void DrawDoubleRing(Graphics g, int cx, int cy, float t, Color color)
    {
        DrawExpandingRing(g, cx, cy, t, color, ringScale: 70f, thickness: 3f);
        DrawExpandingRing(g, cx, cy, t * 0.6f, color, ringScale: 45f, thickness: 2f);
    }

    private static void DrawShards(Graphics g, int cx, int cy, float t, Color color, int count = 6)
    {
        float alpha = t < 0.5f ? 1f : 1f - (t - 0.5f) * 2f;
        if (alpha <= 0) return;
        int a = (int)(alpha * 220);
        using var brush = new SolidBrush(Color.FromArgb(a, color));
        for (int i = 0; i < count; i++)
        {
            double angle = i * (Math.PI * 2 / count) + t * 0.5;
            float dist = t * 48f;
            float sx = cx + (float)(Math.Cos(angle) * dist);
            float sy = cy + (float)(Math.Sin(angle) * dist);
            var state = g.Save();
            g.TranslateTransform(sx, sy);
            g.RotateTransform(i * 45 + t * 180);
            g.FillRectangle(brush, -4, -2, 8, 4);
            g.Restore(state);
        }
    }

    private static void DrawHearts(Graphics g, int cx, int cy, float t, Color color)
    {
        float alpha = t < 0.6f ? 1f : 1f - (t - 0.6f) / 0.4f;
        if (alpha <= 0) return;
        int a = (int)(alpha * 220);
        using var brush = new SolidBrush(Color.FromArgb(a, color));
        for (int i = 0; i < 5; i++)
        {
            double angle = i * (Math.PI * 2 / 5);
            float dist = t * 52f;
            float hx = cx + (float)(Math.Cos(angle) * dist) - 6;
            float hy = cy + (float)(Math.Sin(angle) * dist) - 6;
            g.FillEllipse(brush, hx, hy, 7, 7);
            g.FillEllipse(brush, hx + 5, hy, 7, 7);
            PointF[] tri = [new(hx, hy + 5), new(hx + 12, hy + 5), new(hx + 6, hy + 13)];
            g.FillPolygon(brush, tri);
        }
    }

    private static void DrawPawPrints(Graphics g, int cx, int cy, float t, Color color)
    {
        float alpha = t < 0.6f ? 1f : 1f - (t - 0.6f) / 0.4f;
        if (alpha <= 0) return;
        int a = (int)(alpha * 200);
        using var brush = new SolidBrush(Color.FromArgb(a, color));
        for (int i = 0; i < 6; i++)
        {
            double angle = i * (Math.PI * 2 / 6) + 0.3;
            float dist = t * 50f;
            float px = cx + (float)(Math.Cos(angle) * dist);
            float py = cy + (float)(Math.Sin(angle) * dist);
            g.FillEllipse(brush, px - 5, py - 4, 10, 9);
            g.FillEllipse(brush, px - 5, py - 9, 4, 4);
            g.FillEllipse(brush, px - 1, py - 11, 4, 4);
            g.FillEllipse(brush, px + 3, py - 9, 4, 4);
        }
    }

    private static void DrawDiamondBurst(Graphics g, int cx, int cy, float t, Color color)
    {
        float alpha = t < 0.5f ? t * 2f : 1f - (t - 0.5f) * 2f;
        if (alpha <= 0) return;
        int a = (int)(alpha * 180);
        using var brush = new SolidBrush(Color.FromArgb(a, color));
        float dist = t * 60f;
        (float dx, float dy)[] dirs = [(0, -1), (0, 1), (-1, 0), (1, 0)];
        foreach (var (dx, dy) in dirs)
        {
            float px = cx + dx * dist;
            float py = cy + dy * dist;
            float s = 6 + t * 4;
            PointF[] diamond = [new(px, py - s), new(px + s, py), new(px, py + s), new(px - s, py)];
            g.FillPolygon(brush, diamond);
        }
    }

    private static void DrawSparkles(Graphics g, int cx, int cy, float t, Color color)
    {
        float alpha = t < 0.6f ? 1f : 1f - (t - 0.6f) / 0.4f;
        if (alpha <= 0) return;
        int a = (int)(alpha * 255);
        using var brush = new SolidBrush(Color.FromArgb(a, color));
        for (int i = 0; i < 6; i++)
        {
            double angle = i * Math.PI / 3.0;
            float dist = t * 40;
            float sx = cx + (float)(Math.Cos(angle) * dist) - 3;
            float sy = cy + (float)(Math.Sin(angle) * dist) - 3;
            g.FillEllipse(brush, sx, sy, 6, 6);
        }
    }

    private static void DrawShake(Graphics g, int cx, int cy, float t)
    {
        if (t > 0.5f) return;
        float alpha = 1f - t * 2;
        int a = (int)(alpha * 200);
        using var pen = new Pen(Color.FromArgb(a, Paw.DmgRed), 2);
        int off = 20;
        g.DrawLine(pen, cx - off - 4, cy - 20, cx - off + 4, cy - 28);
        g.DrawLine(pen, cx - off + 4, cy - 20, cx - off - 4, cy - 28);
        g.DrawLine(pen, cx + off - 4, cy - 20, cx + off + 4, cy - 28);
        g.DrawLine(pen, cx + off + 4, cy - 20, cx + off - 4, cy - 28);
    }

    private static void DrawFloatyText(Graphics g, string text, int x, int y,
                                        Color color, float t, bool descend = false)
    {
        float alpha = t < 0.7f ? 1f : 1f - (t - 0.7f) / 0.3f;
        if (alpha <= 0) return;
        int a = (int)(alpha * 255);
        using var font = new Font("Segoe UI Emoji", 9f, FontStyle.Bold);
        using var brush = new SolidBrush(Color.FromArgb(a, color));
        int drawY = descend ? y + (int)(t * 10) : y;
        g.DrawString(text, font, brush, new PointF(x, drawY));
    }

    // ── IDisposable ──────────────────────────────────────────────────────────
    public void Dispose()
    {
        _idleTimer.Stop(); _idleTimer.Dispose();
        _blinkTimer.Stop(); _blinkTimer.Dispose();
        _overlayTimer.Stop(); _overlayTimer.Dispose();
        _canvas.Paint -= OnPaint;
    }
}

// ── Supporting types ─────────────────────────────────────────────────────────
public enum PetType { Cat, Dog }

public enum EffectKind
{
    TaskComplete,
    Overdue,
    LevelUp,
    CoinGain,
    ShopMood,
    ShopHealth,
    ShopEat,
}

internal enum StageTransition
{
    EggToBaby,
    BabyToJunior,
    JuniorToAdult,
    AdultToLegend,
    Generic,
}

internal class OverlayEffect
{
    public EffectKind Kind;
    public float Progress;
    public float Duration;
    public int XpGained;
    public int CoinsGained;
    public int NewLevel;
    public PetEvolution OldStage;
    public PetEvolution NewStage;
    public string ItemName = string.Empty;
}