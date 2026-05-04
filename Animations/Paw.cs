using System.Drawing;

namespace Pawductivity.Animations;

/// Centralized pixel palette for all pet sprites and effects.
internal static class Paw
{
    // ── Cat palette ──────────────────────────────────────────────────────────
    public static readonly Color CatOrange = Color.FromArgb(255, 160, 80);
    public static readonly Color EarPink = Color.FromArgb(255, 180, 170);
    public static readonly Color BellyLight = Color.FromArgb(255, 220, 180);
    public static readonly Color CatStripe = Color.FromArgb(210, 120, 40);   //tabby markings

    // ── Dog palette ──────────────────────────────────────────────────────────
    public static readonly Color DogBrown = Color.FromArgb(180, 120, 60);
    public static readonly Color DogCream = Color.FromArgb(240, 200, 140);
    public static readonly Color DogLight = Color.FromArgb(250, 230, 180);

    // ── Shared ───────────────────────────────────────────────────────────────
    public static readonly Color EyeDark = Color.FromArgb(40, 20, 10);
    public static readonly Color EyeWhite = Color.White;
    public static readonly Color NosePink = Color.FromArgb(255, 130, 140);
    public static readonly Color Whisker = Color.FromArgb(180, 160, 150);
    public static readonly Color BlushPink = Color.FromArgb(255, 180, 190);  //baby blush

    // ── Egg palette ──────────────────────────────────────────────────────────
    public static readonly Color EggShell = Color.FromArgb(255, 248, 230);  // warm white
    public static readonly Color EggHighlight = Color.FromArgb(255, 255, 245);  // specular
    // ── Effects ──────────────────────────────────────────────────────────────
    public static readonly Color XpColor = Color.FromArgb(140, 200, 255);  // periwinkle
    public static readonly Color MoodColor = Color.FromArgb(255, 200, 80);   // sunny yellow
    public static readonly Color HealthColor = Color.FromArgb(255, 80, 120);   // rose
    public static readonly Color CoinColor = Color.FromArgb(255, 215, 0);    // gold
    public static readonly Color DmgRed = Color.FromArgb(220, 50, 50);
}