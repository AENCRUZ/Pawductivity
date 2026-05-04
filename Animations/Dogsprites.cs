using System;
using System.Drawing;
using Pawductivity.Models;

namespace Pawductivity.Animations;

internal static class DogSprites
{
    // ── Egg ──────────────────────────────────────────────────────────────────
    /// 🥚 Cream-tinted egg with tiny paw print on shell.
    public static void DrawEgg(Graphics g, int cx, int cy, float breathY)
    {
        int wobble = ((int)breathY % 2 == 0) ? -4 : 4;

        using var shellBrush = new SolidBrush(Color.FromArgb(255, 245, 228));
        using var shellPen = new Pen(Color.FromArgb(210, 185, 140), 2f);
        var eggRect = new System.Drawing.Rectangle(cx - 22 + wobble, cy - 38, 44, 56);
        g.FillEllipse(shellBrush, eggRect);
        g.DrawEllipse(shellPen, eggRect);

        // Spots
        using var spotBrush = new SolidBrush(Color.FromArgb(60, 190, 155, 100));
        g.FillEllipse(spotBrush, cx - 14 + wobble, cy - 20, 10, 8);
        g.FillEllipse(spotBrush, cx + 4 + wobble, cy - 10, 8, 6);

        // Crack lines
        using var crackPen = new Pen(Color.FromArgb(180, 155, 110), 1.5f);
        g.DrawLine(crackPen, cx + wobble, cy - 8, cx + 8 + wobble, cy - 18);
        g.DrawLine(crackPen, cx + 8 + wobble, cy - 18, cx + 4 + wobble, cy - 26);

        // Dog ear peek
        using var earBrush = new SolidBrush(Color.FromArgb(200, 165, 110));
        g.FillEllipse(earBrush, cx - 24 + wobble, cy - 30, 14, 20);

        using var zFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        using var zBrush = new SolidBrush(Color.FromArgb(160, 150, 180, 120));
        g.DrawString("z", zFont, zBrush, cx + 20 + wobble, cy - 42);
        g.DrawString("Z", zFont, zBrush, cx + 28 + wobble, cy - 52);
    }

    // ── Baby Dog ─────────────────────────────────────────────────────────────
    /// 🐶 Tiny round pup — huge eyes, floppy ear nubs, stubby body.
    public static void DrawBaby(Graphics g, int cx, int cy, float eyeOpenRatio,
                                 bool happy, bool sad, int frame, bool blinking)
    {
        var bodyColor = Color.FromArgb(240, 210, 165);

        // Chubby body
        using var bodyBrush = new SolidBrush(bodyColor);
        g.FillEllipse(bodyBrush, cx - 20, cy + 4, 40, 30);
        using var bellyBrush = new SolidBrush(Color.FromArgb(255, 240, 220));
        g.FillEllipse(bellyBrush, cx - 10, cy + 10, 20, 18);

        // Huge round head (puppy proportions)
        g.FillEllipse(bodyBrush, cx - 24, cy - 36, 48, 44);

        // Big floppy ears
        int earFlap = happy ? (frame % 2 == 0 ? 3 : -3) : 0;
        using var earBrush = new SolidBrush(Color.FromArgb(210, 175, 120));
        g.FillEllipse(earBrush, cx - 36, cy - 28 + earFlap, 16, 26);
        g.FillEllipse(earBrush, cx + 20, cy - 28 - earFlap, 16, 26);

        // Round snout
        using var snoutBrush = new SolidBrush(Color.FromArgb(255, 235, 210));
        g.FillEllipse(snoutBrush, cx - 12, cy - 16, 24, 18);
        using var noseBrush = new SolidBrush(Color.FromArgb(80, 50, 20));
        g.FillEllipse(noseBrush, cx - 6, cy - 15, 12, 8);

        CatSprites.DrawFace(g, cx, cy - 20, happy, sad, blinking, 8, true);

        // Tiny wagging tail stub
        int tw = happy ? (frame % 2 == 0 ? 14 : -4) : 4;
        using var tailPen = new Pen(bodyColor, 6f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        g.DrawLine(tailPen, cx + 18, cy + 16, cx + 28 + tw, cy + 6);

        // Little paws
        g.FillEllipse(bodyBrush, cx - 24, cy + 28, 18, 10);
        g.FillEllipse(bodyBrush, cx + 6, cy + 28 - (happy && frame % 2 == 0 ? 4 : 0), 18, 10);

        if (happy) CatSprites.DrawSparkles(g, cx, cy, frame, Color.FromArgb(255, 180, 80), 3, small: true);
        if (sad) CatSprites.DrawTears(g, cx, cy, 2);
    }

    // ── Junior Dog ───────────────────────────────────────────────────────────
    /// 🐕 Adolescent dog — longer ears, visible tail wag, speckles.
    public static void DrawJunior(Graphics g, int cx, int cy, float eyeOpenRatio,
                                   float breathPhase, bool happy, bool sad, int frame, bool blinking)
    {
        var bodyColor = Color.FromArgb(200, 160, 100);
        var earColor = Color.FromArgb(170, 128, 75);

        // Tail — enthusiastic wag when happy
        float wagAngle = happy ? (frame % 2 == 0 ? 35f : -5f) : 10f;
        using var tailPen = new Pen(bodyColor, 8f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        var tailBase = new PointF(cx + 26, cy + 12);
        var tailTip = new PointF(
            tailBase.X + (float)(30 * Math.Cos(wagAngle * Math.PI / 180)),
            tailBase.Y - (float)(30 * Math.Sin(wagAngle * Math.PI / 180)));
        g.DrawLine(tailPen, tailBase, tailTip);

        // Body
        using var bodyBrush = new SolidBrush(bodyColor);
        g.FillEllipse(bodyBrush, cx - 28, cy + 2, 56, 42);
        using var bellyBrush = new SolidBrush(Color.FromArgb(230, 200, 155));
        g.FillEllipse(bellyBrush, cx - 14, cy + 12, 28, 24);

        // Head
        g.FillEllipse(bodyBrush, cx - 28, cy - 42, 56, 50);

        // Floppy ears — bounce with happiness
        int earFlap = happy ? (frame % 2 == 0 ? 4 : -4) : 0;
        using var earBrush = new SolidBrush(earColor);
        g.FillEllipse(earBrush, cx - 40, cy - 34 + earFlap, 18, 32);
        g.FillEllipse(earBrush, cx + 22, cy - 34 - earFlap, 18, 32);

        // Snout
        using var snoutBrush = new SolidBrush(Color.FromArgb(240, 215, 175));
        g.FillEllipse(snoutBrush, cx - 14, cy - 18, 28, 20);
        using var noseBrush = new SolidBrush(Color.FromArgb(80, 50, 20));
        g.FillEllipse(noseBrush, cx - 7, cy - 17, 14, 9);
        using var noseShineBrush = new SolidBrush(Color.FromArgb(120, 255, 255, 255));
        g.FillEllipse(noseShineBrush, cx - 4, cy - 16, 5, 4);

        CatSprites.DrawFace(g, cx, cy - 24, happy, sad, blinking, 10, true);

        // Tongue when happy
        if (happy && frame % 2 == 0)
        {
            using var tongueBrush = new SolidBrush(Color.FromArgb(255, 120, 130));
            g.FillEllipse(tongueBrush, cx - 6, cy - 2, 12, 10);
        }

        // Paws
        using var pawBrush = new SolidBrush(bodyColor);
        int lpY = happy && frame % 2 == 0 ? cy + 36 : cy + 42;
        int rpY = happy && frame % 2 == 1 ? cy + 36 : cy + 42;
        g.FillEllipse(pawBrush, cx - 30, lpY, 22, 13);
        g.FillEllipse(pawBrush, cx + 8, rpY, 22, 13);

        if (happy)
        {
            CatSprites.DrawSparkles(g, cx, cy, frame, Color.FromArgb(255, 200, 80), 4, small: false);
            using var blushBrush = new SolidBrush(Color.FromArgb(70, 255, 130, 100));
            g.FillEllipse(blushBrush, cx - 30, cy - 14, 16, 10);
            g.FillEllipse(blushBrush, cx + 14, cy - 14, 16, 10);
        }
        if (sad) CatSprites.DrawTears(g, cx, cy, 2);
    }

    // ── Adult Dog ────────────────────────────────────────────────────────────
    /// 🦮 Full-grown dog — big wagging tail, blush cheeks, hearts.
    public static void DrawAdult(Graphics g, int cx, int cy, float eyeOpenRatio,
                                  float breathPhase, bool happy, bool sad, int frame, bool blinking)
    {
        var bodyColor = Color.FromArgb(230, 190, 140);
        var earColor = Color.FromArgb(190, 145, 95);

        // Big wagging tail
        float wagAngle = happy ? (frame % 2 == 0 ? 40f : -10f) : 12f;
        using var tailPen = new Pen(bodyColor, 10f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        var tailBase = new PointF(cx + 28, cy + 14);
        var tailTip = new PointF(
            tailBase.X + (float)(34 * Math.Cos(wagAngle * Math.PI / 180)),
            tailBase.Y - (float)(34 * Math.Sin(wagAngle * Math.PI / 180)));
        g.DrawLine(tailPen, tailBase, tailTip);

        // Body
        using var bodyBrush = new SolidBrush(bodyColor);
        g.FillEllipse(bodyBrush, cx - 30, cy, 60, 46);
        using var bellyBrush = new SolidBrush(Color.FromArgb(255, 235, 210));
        g.FillEllipse(bellyBrush, cx - 16, cy + 12, 32, 26);

        // Head
        g.FillEllipse(bodyBrush, cx - 30, cy - 44, 60, 50);

        // Floppy ears
        int earFlap = happy ? (frame % 2 == 0 ? 4 : -4) : 0;
        using var earBrush = new SolidBrush(earColor);
        g.FillEllipse(earBrush, cx - 42, cy - 36 + earFlap, 20, 34);
        g.FillEllipse(earBrush, cx + 22, cy - 36 - earFlap, 20, 34);

        // Snout
        using var snoutBrush = new SolidBrush(Color.FromArgb(255, 225, 195));
        g.FillEllipse(snoutBrush, cx - 14, cy - 18, 28, 20);
        using var noseBrush = new SolidBrush(Color.FromArgb(80, 50, 20));
        g.FillEllipse(noseBrush, cx - 7, cy - 17, 14, 9);
        using var noseShineBrush = new SolidBrush(Color.FromArgb(120, 255, 255, 255));
        g.FillEllipse(noseShineBrush, cx - 4, cy - 16, 5, 4);

        CatSprites.DrawFace(g, cx, cy - 24, happy, sad, blinking, 11, true);

        if (happy && frame % 2 == 0)
        {
            using var tongueBrush = new SolidBrush(Color.FromArgb(255, 120, 130));
            g.FillEllipse(tongueBrush, cx - 6, cy - 2, 12, 10);
            using var tongueLine = new Pen(Color.FromArgb(200, 80, 100), 1.5f);
            g.DrawLine(tongueLine, cx, cy - 2, cx, cy + 8);
        }

        using var pawBrush = new SolidBrush(bodyColor);
        int lpY = happy && frame % 2 == 0 ? cy + 34 : cy + 40;
        int rpY = happy && frame % 2 == 1 ? cy + 34 : cy + 40;
        g.FillEllipse(pawBrush, cx - 32, lpY, 24, 14);
        g.FillEllipse(pawBrush, cx + 8, rpY, 24, 14);

        if (happy)
        {
            using var blushBrush = new SolidBrush(Color.FromArgb(80, 255, 130, 100));
            g.FillEllipse(blushBrush, cx - 32, cy - 14, 16, 10);
            g.FillEllipse(blushBrush, cx + 16, cy - 14, 16, 10);
            CatSprites.DrawHearts(g, cx, cy, frame, 4);
        }
        if (sad) CatSprites.DrawTears(g, cx, cy, 2);
    }

    // ── Legend Dog ───────────────────────────────────────────────────────────
    /// ✨ Legendary dog — golden aura, crown, orbiting glow dots.
    public static void DrawLegend(Graphics g, int cx, int cy, float eyeOpenRatio,
                                   float breathPhase, bool happy, bool sad, int frame, bool blinking)
    {
        // Golden glow aura
        int alpha = frame % 2 == 0 ? 70 : 30;
        using var auraBrush = new SolidBrush(Color.FromArgb(alpha, 255, 200, 80));
        using var aura2Brush = new SolidBrush(Color.FromArgb(alpha / 2, 255, 240, 160));
        g.FillEllipse(auraBrush, cx - 50, cy - 58, 100, 100);
        g.FillEllipse(aura2Brush, cx - 60, cy - 68, 120, 120);

        // Orbiting glow dots
        float glowAlpha = 0.5f + 0.5f * MathF.Sin(breathPhase * 2);
        int ga = (int)(glowAlpha * 150);
        using var glowBrush = new SolidBrush(Color.FromArgb(ga, Color.Goldenrod));
        for (int i = 0; i < 8; i++)
        {
            double angle = i * Math.PI / 4.0 + breathPhase * 0.25;
            float dist = 30 + 3 * MathF.Sin(breathPhase + i);
            float sx = cx + (float)(Math.Cos(angle) * dist) - 3;
            float sy = cy + (float)(Math.Sin(angle) * dist) - 3;
            g.FillEllipse(glowBrush, sx, sy - 10, 6, 6);
        }

        // Crown
        using var crownBrush = new SolidBrush(Color.FromArgb(255, 200, 40));
        Point[] crown =
        [
            new(cx - 18, cy - 64), new(cx - 18, cy - 80),
            new(cx - 8,  cy - 70), new(cx,      cy - 84),
            new(cx + 8,  cy - 70), new(cx + 18, cy - 80),
            new(cx + 18, cy - 64),
        ];
        g.FillPolygon(crownBrush, crown);
        using var jewel1 = new SolidBrush(Color.FromArgb(100, 200, 100));
        using var jewel2 = new SolidBrush(Color.FromArgb(255, 80, 80));
        g.FillEllipse(jewel1, cx - 4, cy - 82, 8, 8);
        g.FillEllipse(jewel2, cx - 16, cy - 78, 6, 6);
        g.FillEllipse(jewel2, cx + 10, cy - 78, 6, 6);

        // Draw adult dog as base — reuse all body/face/ear/tail drawing
        DrawAdult(g, cx, cy, eyeOpenRatio, breathPhase, happy, sad, frame, blinking);

        // Golden sparkle stars
        using var star1 = new SolidBrush(Color.FromArgb(frame % 2 == 0 ? 220 : 80, 255, 215, 0));
        using var star2 = new SolidBrush(Color.FromArgb(frame % 2 == 0 ? 80 : 220, 255, 160, 40));
        CatSprites.DrawSparkle(g, star1, cx + 42, cy - 42, 8);
        CatSprites.DrawSparkle(g, star2, cx - 44, cy - 34, 7);
        CatSprites.DrawSparkle(g, star1, cx + 30, cy - 62, 6);
        CatSprites.DrawSparkle(g, star2, cx - 32, cy - 60, 6);
        CatSprites.DrawSparkle(g, star1, cx, cy - 70, 7);
    }
}