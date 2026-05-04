using System;
using System.Drawing;
using Pawductivity.Models;

namespace Pawductivity.Animations;

internal static class CatSprites
{
    // ── Egg ──────────────────────────────────────────────────────────────────
    /// 🥚 Wobbling egg with crack lines and sleepy eyes
    public static void DrawEgg(Graphics g, int cx, int cy, float breathY)
    {
        // Reuse frame-based wobble via breathY mapped to an int offset
        int w = ((int)breathY % 2 == 0) ? -3 : 3;

        using var sh = new SolidBrush(Color.FromArgb(252, 248, 240));
        using var sp = new Pen(Color.FromArgb(180, 160, 120), 2f);
        g.FillEllipse(sh, cx - 22 + w, cy - 38, 44, 56);
        g.DrawEllipse(sp, cx - 22 + w, cy - 38, 44, 56);

        // Crack lines (always visible for character)
        using var cp = new Pen(Color.FromArgb(160, 140, 100), 1.5f);
        g.DrawLine(cp, cx + w, cy - 10, cx + 6 + w, cy - 22);
        g.DrawLine(cp, cx + 6 + w, cy - 22, cx + 2 + w, cy - 30);

        // Sleepy dot eyes
        using var eb = new SolidBrush(Color.FromArgb(60, 40, 20));
        g.FillEllipse(eb, cx - 9 + w, cy - 20, 5, 5);
        g.FillEllipse(eb, cx + 4 + w, cy - 20, 5, 5);

        // Z floats
        using var zf = new Font("Segoe UI", 8f, FontStyle.Bold);
        using var zb = new SolidBrush(Color.FromArgb(160, 140, 100, 180));
        g.DrawString("z", zf, zb, cx + 20 + w, cy - 44);
        g.DrawString("Z", zf, zb, cx + 28 + w, cy - 54);
    }

    // ── Baby Cat ─────────────────────────────────────────────────────────────
    /// 🐱 Tiny round kitten — big head, small body, stubby tail.
    public static void DrawBaby(Graphics g, int cx, int cy, float eyeOpenRatio,
                                 bool happy, bool sad, int frame, bool blinking)
    {
        var fur = Color.FromArgb(255, 220, 170);
        var ear = Color.FromArgb(210, 175, 120);

        // Stubby wagging tail
        int tw = happy ? (frame % 2 == 0 ? 12 : -4) : 4;
        using var tailPen = new Pen(fur, 5f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        g.DrawLine(tailPen, cx + 16, cy + 18, cx + 26 + tw, cy + 8);

        // Chubby body
        using var bb = new SolidBrush(fur);
        g.FillEllipse(bb, cx - 20, cy + 4, 40, 30);
        using var bellyb = new SolidBrush(Color.FromArgb(255, 240, 215));
        g.FillEllipse(bellyb, cx - 10, cy + 10, 20, 18);

        // Big round head
        g.FillEllipse(bb, cx - 24, cy - 36, 48, 44);

        // Small triangular ears
        using var earb = new SolidBrush(ear);
        Point[] el = [new(cx - 20, cy - 32), new(cx - 28, cy - 52), new(cx - 6, cy - 36)];
        Point[] er = [new(cx + 20, cy - 32), new(cx + 28, cy - 52), new(cx + 6, cy - 36)];
        g.FillPolygon(earb, el);
        g.FillPolygon(earb, er);
        using var innerb = new SolidBrush(Color.FromArgb(255, 185, 170));
        Point[] il = [new(cx - 19, cy - 34), new(cx - 24, cy - 48), new(cx - 8, cy - 36)];
        Point[] ir = [new(cx + 19, cy - 34), new(cx + 24, cy - 48), new(cx + 8, cy - 36)];
        g.FillPolygon(innerb, il);
        g.FillPolygon(innerb, ir);

        // Small oval snout + cat triangle nose
        using var snoutb = new SolidBrush(Color.FromArgb(255, 235, 210));
        g.FillEllipse(snoutb, cx - 10, cy - 16, 20, 14);
        using var noseb = new SolidBrush(Color.FromArgb(210, 120, 120));
        g.FillPolygon(noseb, new Point[] { new(cx, cy - 15), new(cx - 4, cy - 10), new(cx + 4, cy - 10) });

        DrawFace(g, cx, cy - 20, happy, sad, blinking, 8, false);

        // Whiskers
        using var wp = new Pen(Color.FromArgb(160, 200, 190, 180), 1.2f);
        g.DrawLine(wp, cx - 24, cy - 12, cx - 6, cy - 11);
        g.DrawLine(wp, cx + 6, cy - 11, cx + 24, cy - 12);

        // Little paws
        g.FillEllipse(bb, cx - 24, cy + 28, 16, 10);
        g.FillEllipse(bb, cx + 8, cy + 28 - (happy && frame % 2 == 0 ? 4 : 0), 16, 10);

        if (happy) DrawSparkles(g, cx, cy, frame, Color.FromArgb(255, 220, 100), 3, small: true);
        if (sad) DrawTears(g, cx, cy, 2);
    }

    // ── Junior Cat ───────────────────────────────────────────────────────────
    /// 🐈 Growing cat — more elongated, proper whiskers, stripe markings.
    public static void DrawJunior(Graphics g, int cx, int cy, float eyeOpenRatio,
                                   bool happy, bool sad, int frame, bool blinking)
    {
        var fur = Color.FromArgb(210, 175, 125);
        var ear = Color.FromArgb(175, 138, 88);

        // Curving tail — sweeps side to side
        float wagAngle = happy ? (frame % 2 == 0 ? 120f : 150f) : 135f;
        using var tailPen = new Pen(fur, 7f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        var tailBase = new PointF(cx - 24, cy + 16);
        var tailTip = new PointF(
            tailBase.X + (float)(36 * Math.Cos(wagAngle * Math.PI / 180)),
            tailBase.Y - (float)(36 * Math.Sin(wagAngle * Math.PI / 180)));
        g.DrawLine(tailPen, tailBase, tailTip);

        // Body
        using var bb = new SolidBrush(fur);
        g.FillEllipse(bb, cx - 28, cy + 2, 56, 42);
        using var bellyb = new SolidBrush(Color.FromArgb(240, 215, 175));
        g.FillEllipse(bellyb, cx - 14, cy + 12, 28, 24);

        // Head
        g.FillEllipse(bb, cx - 28, cy - 42, 56, 50);

        // Triangular ears — perky, upright
        using var earb = new SolidBrush(ear);
        Point[] el = [new(cx - 22, cy - 38), new(cx - 34, cy - 62), new(cx - 6, cy - 44)];
        Point[] er = [new(cx + 22, cy - 38), new(cx + 34, cy - 62), new(cx + 6, cy - 44)];
        g.FillPolygon(earb, el);
        g.FillPolygon(earb, er);
        using var innerb = new SolidBrush(Color.FromArgb(255, 185, 165));
        Point[] il = [new(cx - 21, cy - 40), new(cx - 29, cy - 58), new(cx - 8, cy - 44)];
        Point[] ir = [new(cx + 21, cy - 40), new(cx + 29, cy - 58), new(cx + 8, cy - 44)];
        g.FillPolygon(innerb, il);
        g.FillPolygon(innerb, ir);

        // Snout oval + cat triangle nose
        using var snoutb = new SolidBrush(Color.FromArgb(255, 230, 205));
        g.FillEllipse(snoutb, cx - 13, cy - 18, 26, 18);
        using var noseb = new SolidBrush(Color.FromArgb(210, 120, 120));
        g.FillPolygon(noseb, new Point[] { new(cx, cy - 17), new(cx - 5, cy - 11), new(cx + 5, cy - 11) });
        using var noseShineb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
        g.FillEllipse(noseShineb, cx - 2, cy - 17, 4, 3);

        DrawFace(g, cx, cy - 24, happy, sad, blinking, 10, false);

        // Tongue when happy
        if (happy && frame % 2 == 0)
        {
            using var tongueb = new SolidBrush(Color.FromArgb(255, 140, 150));
            g.FillEllipse(tongueb, cx - 5, cy - 4, 10, 8);
        }

        // Whiskers — two rows each side
        using var wp = new Pen(Color.FromArgb(160, 210, 200, 190), 1.3f);
        g.DrawLine(wp, cx - 30, cy - 12, cx - 8, cy - 11);
        g.DrawLine(wp, cx - 30, cy - 8, cx - 8, cy - 8);
        g.DrawLine(wp, cx + 8, cy - 11, cx + 30, cy - 12);
        g.DrawLine(wp, cx + 8, cy - 8, cx + 30, cy - 8);

        // Paws
        using var pawb = new SolidBrush(fur);
        int lpY = happy && frame % 2 == 0 ? cy + 36 : cy + 42;
        int rpY = happy && frame % 2 == 1 ? cy + 36 : cy + 42;
        g.FillEllipse(pawb, cx - 30, lpY, 22, 13);
        g.FillEllipse(pawb, cx + 8, rpY, 22, 13);

        if (happy)
        {
            using var blushb = new SolidBrush(Color.FromArgb(70, 255, 130, 100));
            g.FillEllipse(blushb, cx - 30, cy - 12, 16, 10);
            g.FillEllipse(blushb, cx + 14, cy - 12, 16, 10);
            DrawSparkles(g, cx, cy, frame, Color.FromArgb(255, 200, 80), 4, small: false);
        }
        if (sad) DrawTears(g, cx, cy, 2);
    }

    // ── Adult Cat ────────────────────────────────────────────────────────────
    /// 🐈 Sleek adult cat — tabby stripes, long sweeping tail.
    public static void DrawAdult(Graphics g, int cx, int cy, float eyeOpenRatio,
                                  bool happy, bool sad, int frame, bool blinking)
    {
        var fur = Color.FromArgb(220, 180, 130);
        var stripe = Color.FromArgb(170, 132, 85);
        var ear = Color.FromArgb(180, 140, 90);

        // Long sweeping tail
        float wagAngle = happy ? (frame % 2 == 0 ? 110f : 145f) : 128f;
        using var tailPen = new Pen(fur, 9f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        var tailBase = new PointF(cx - 26, cy + 18);
        var tailTip = new PointF(
            tailBase.X + (float)(42 * Math.Cos(wagAngle * Math.PI / 180)),
            tailBase.Y - (float)(42 * Math.Sin(wagAngle * Math.PI / 180)));
        g.DrawLine(tailPen, tailBase, tailTip);

        // Body with tabby stripes
        using var bb = new SolidBrush(fur);
        g.FillEllipse(bb, cx - 30, cy, 60, 46);
        using var bellyb = new SolidBrush(Color.FromArgb(255, 235, 205));
        g.FillEllipse(bellyb, cx - 16, cy + 12, 32, 26);
        using var stripePen = new Pen(stripe, 2f);
        g.DrawArc(stripePen, cx - 14, cy + 6, 12, 8, -10, 180);
        g.DrawArc(stripePen, cx + 2, cy + 6, 12, 8, -10, 180);

        // Head
        g.FillEllipse(bb, cx - 30, cy - 44, 60, 50);

        // Forehead stripes
        g.DrawLine(stripePen, cx - 5, cy - 42, cx - 5, cy - 30);
        g.DrawLine(stripePen, cx + 5, cy - 42, cx + 5, cy - 30);

        // Triangular ears
        using var earb = new SolidBrush(ear);
        Point[] el = [new(cx - 24, cy - 40), new(cx - 36, cy - 66), new(cx - 8, cy - 46)];
        Point[] er = [new(cx + 24, cy - 40), new(cx + 36, cy - 66), new(cx + 8, cy - 46)];
        g.FillPolygon(earb, el);
        g.FillPolygon(earb, er);
        using var innerb = new SolidBrush(Color.FromArgb(255, 185, 162));
        Point[] il = [new(cx - 22, cy - 42), new(cx - 31, cy - 62), new(cx - 10, cy - 46)];
        Point[] ir = [new(cx + 22, cy - 42), new(cx + 31, cy - 62), new(cx + 10, cy - 46)];
        g.FillPolygon(innerb, il);
        g.FillPolygon(innerb, ir);

        // Snout + cat nose
        using var snoutb = new SolidBrush(Color.FromArgb(255, 225, 195));
        g.FillEllipse(snoutb, cx - 14, cy - 18, 28, 20);
        using var noseb = new SolidBrush(Color.FromArgb(210, 110, 110));
        g.FillPolygon(noseb, new Point[] { new(cx, cy - 17), new(cx - 5, cy - 11), new(cx + 5, cy - 11) });
        using var noseShineb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
        g.FillEllipse(noseShineb, cx - 2, cy - 17, 5, 3);

        DrawFace(g, cx, cy - 24, happy, sad, blinking, 11, false);

        if (happy && frame % 2 == 0)
        {
            using var tongueb = new SolidBrush(Color.FromArgb(255, 140, 150));
            g.FillEllipse(tongueb, cx - 5, cy - 2, 10, 8);
        }

        // Three whiskers each side
        using var wp = new Pen(Color.FromArgb(170, 220, 210, 200), 1.4f);
        g.DrawLine(wp, cx - 32, cy - 14, cx - 9, cy - 13);
        g.DrawLine(wp, cx - 32, cy - 9, cx - 9, cy - 9);
        g.DrawLine(wp, cx - 30, cy - 18, cx - 9, cy - 14);
        g.DrawLine(wp, cx + 9, cy - 13, cx + 32, cy - 14);
        g.DrawLine(wp, cx + 9, cy - 9, cx + 32, cy - 9);
        g.DrawLine(wp, cx + 9, cy - 14, cx + 30, cy - 18);

        // Paws with toe beans
        using var pawb = new SolidBrush(fur);
        using var beansb = new SolidBrush(Color.FromArgb(215, 165, 135));
        int lpY = happy && frame % 2 == 0 ? cy + 34 : cy + 40;
        int rpY = happy && frame % 2 == 1 ? cy + 34 : cy + 40;
        g.FillEllipse(pawb, cx - 32, lpY, 24, 14);
        g.FillEllipse(pawb, cx + 8, rpY, 24, 14);
        foreach (int bx in new[] { cx - 28, cx - 22, cx - 16 }) g.FillEllipse(beansb, bx, lpY + 3, 5, 4);
        foreach (int bx in new[] { cx + 12, cx + 18, cx + 24 }) g.FillEllipse(beansb, bx, rpY + 3, 5, 4);

        if (happy)
        {
            using var blushb = new SolidBrush(Color.FromArgb(80, 255, 130, 100));
            g.FillEllipse(blushb, cx - 32, cy - 14, 16, 10);
            g.FillEllipse(blushb, cx + 16, cy - 14, 16, 10);
            DrawHearts(g, cx, cy, frame, 3);
        }
        if (sad) DrawTears(g, cx, cy, 2);
    }

    // ── Legend Cat ───────────────────────────────────────────────────────────
    /// ✨ Legendary cat — silver-white with crown, magic aura.
    public static void DrawLegend(Graphics g, int cx, int cy, float eyeOpenRatio,
                                   float breathPhase, bool happy, bool sad, int frame, bool blinking)
    {
        // Purple/blue aura
        int alpha = frame % 2 == 0 ? 55 : 22;
        using var aura1 = new SolidBrush(Color.FromArgb(alpha, 140, 80, 255));
        using var aura2 = new SolidBrush(Color.FromArgb(alpha / 2, 80, 180, 255));
        g.FillEllipse(aura1, cx - 50, cy - 58, 100, 100);
        g.FillEllipse(aura2, cx - 60, cy - 68, 120, 120);

        // Orbiting glow dots
        float glowAlpha = 0.5f + 0.5f * MathF.Sin(breathPhase * 2);
        int ga = (int)(glowAlpha * 160);
        using var glowBrush = new SolidBrush(Color.FromArgb(ga, Color.Gold));
        for (int i = 0; i < 8; i++)
        {
            double angle = i * Math.PI / 4.0 + breathPhase * 0.3;
            float dist = 28 + 3 * MathF.Sin(breathPhase + i);
            float sx = cx + (float)(Math.Cos(angle) * dist) - 3;
            float sy = cy + (float)(Math.Sin(angle) * dist) - 3;
            g.FillEllipse(glowBrush, sx, sy - 10, 6, 6);
        }

        // Crown
        using var crownb = new SolidBrush(Color.FromArgb(255, 200, 40));
        Point[] crown = [new(cx - 18, cy - 66), new(cx - 18, cy - 80), new(cx - 9, cy - 70), new(cx, cy - 84), new(cx + 9, cy - 70), new(cx + 18, cy - 80), new(cx + 18, cy - 66)];
        g.FillPolygon(crownb, crown);
        using var j1 = new SolidBrush(Color.FromArgb(255, 70, 90));
        using var j2 = new SolidBrush(Color.FromArgb(70, 170, 255));
        g.FillEllipse(j1, cx - 4, cy - 82, 8, 8);
        g.FillEllipse(j2, cx - 16, cy - 78, 6, 6);
        g.FillEllipse(j2, cx + 10, cy - 78, 6, 6);

        // Silver-white base — same structure as Adult, lighter palette
        var fur = Color.FromArgb(238, 234, 228);
        var stripe = Color.FromArgb(205, 200, 192);
        var ear = Color.FromArgb(200, 190, 178);

        float wagAngle = happy ? (frame % 2 == 0 ? 110f : 145f) : 128f;
        using var tailPen = new Pen(fur, 9f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        var tailBase = new PointF(cx - 26, cy + 18);
        var tailTip = new PointF(
            tailBase.X + (float)(42 * Math.Cos(wagAngle * Math.PI / 180)),
            tailBase.Y - (float)(42 * Math.Sin(wagAngle * Math.PI / 180)));
        g.DrawLine(tailPen, tailBase, tailTip);

        using var bb = new SolidBrush(fur);
        g.FillEllipse(bb, cx - 30, cy, 60, 46);
        using var bellyb = new SolidBrush(Color.FromArgb(252, 250, 247));
        g.FillEllipse(bellyb, cx - 16, cy + 12, 32, 26);
        using var stripePen = new Pen(stripe, 1.5f);
        g.DrawArc(stripePen, cx - 14, cy + 6, 12, 8, -10, 180);
        g.DrawArc(stripePen, cx + 2, cy + 6, 12, 8, -10, 180);

        g.FillEllipse(bb, cx - 30, cy - 44, 60, 50);
        g.DrawLine(stripePen, cx - 5, cy - 42, cx - 5, cy - 30);
        g.DrawLine(stripePen, cx + 5, cy - 42, cx + 5, cy - 30);

        using var earb = new SolidBrush(ear);
        Point[] el = [new(cx - 24, cy - 40), new(cx - 36, cy - 66), new(cx - 8, cy - 46)];
        Point[] er = [new(cx + 24, cy - 40), new(cx + 36, cy - 66), new(cx + 8, cy - 46)];
        g.FillPolygon(earb, el);
        g.FillPolygon(earb, er);
        using var innerb = new SolidBrush(Color.FromArgb(255, 200, 215));
        Point[] il = [new(cx - 22, cy - 42), new(cx - 31, cy - 62), new(cx - 10, cy - 46)];
        Point[] ir = [new(cx + 22, cy - 42), new(cx + 31, cy - 62), new(cx + 10, cy - 46)];
        g.FillPolygon(innerb, il);
        g.FillPolygon(innerb, ir);

        using var snoutb = new SolidBrush(Color.FromArgb(252, 248, 244));
        g.FillEllipse(snoutb, cx - 14, cy - 18, 28, 20);
        using var noseb = new SolidBrush(Color.FromArgb(200, 140, 170));
        g.FillPolygon(noseb, new Point[] { new(cx, cy - 17), new(cx - 5, cy - 11), new(cx + 5, cy - 11) });

        // Legend cat — teal eyes
        DrawFaceLegendCat(g, cx, cy - 24, happy, sad, blinking, 11);

        if (happy && frame % 2 == 0)
        {
            using var tongueb = new SolidBrush(Color.FromArgb(255, 140, 160));
            g.FillEllipse(tongueb, cx - 5, cy - 2, 10, 8);
        }

        using var wp = new Pen(Color.FromArgb(170, 230, 225, 240), 1.4f);
        g.DrawLine(wp, cx - 32, cy - 14, cx - 9, cy - 13);
        g.DrawLine(wp, cx - 32, cy - 9, cx - 9, cy - 9);
        g.DrawLine(wp, cx + 9, cy - 13, cx + 32, cy - 14);
        g.DrawLine(wp, cx + 9, cy - 9, cx + 32, cy - 9);

        using var pawb = new SolidBrush(fur);
        int lpY = happy && frame % 2 == 0 ? cy + 34 : cy + 40;
        int rpY = happy && frame % 2 == 1 ? cy + 34 : cy + 40;
        g.FillEllipse(pawb, cx - 32, lpY, 24, 14);
        g.FillEllipse(pawb, cx + 8, rpY, 24, 14);

        // Magic sparkles alternating purple/blue
        using var m1 = new SolidBrush(Color.FromArgb(frame % 2 == 0 ? 210 : 80, 200, 100, 255));
        using var m2 = new SolidBrush(Color.FromArgb(frame % 2 == 0 ? 80 : 210, 100, 200, 255));
        DrawSparkle(g, m1, cx + 42, cy - 42, 7);
        DrawSparkle(g, m2, cx - 44, cy - 34, 6);
        DrawSparkle(g, m1, cx + 30, cy - 62, 5);
        DrawSparkle(g, m2, cx - 32, cy - 60, 5);
        DrawSparkle(g, m1, cx, cy - 70, 6);

        if (sad) DrawTears(g, cx, cy, 2);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SHARED DRAWING HELPERS (internal so DogSprites can call DrawSparkle)
    // ═══════════════════════════════════════════════════════════════════════════

    /// Draws eyes + mouth. eyeY is the centre of the eye row.
    internal static void DrawFace(Graphics g, int cx, int eyeY,
                                   bool happy, bool sad, bool blinking,
                                   int eyeSpread, bool isDog)
    {
        int ex = eyeSpread;
        if (blinking)
        {
            using var blinkPen = new Pen(Color.FromArgb(80, 50, isDog ? 20 : 50), 2.5f)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round
            };
            g.DrawArc(blinkPen, cx - ex - 5, eyeY, 12, 6, 0, -180);
            g.DrawArc(blinkPen, cx + ex - 6, eyeY, 12, 6, 0, -180);
        }
        else if (happy)
        {
            using var eyePen = new Pen(Color.FromArgb(80, 50, isDog ? 20 : 50), 3f)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round
            };
            g.DrawArc(eyePen, cx - ex - 6, eyeY - 2, 13, 9, 180, 180);
            g.DrawArc(eyePen, cx + ex - 7, eyeY - 2, 13, 9, 180, 180);
            using var blushb = new SolidBrush(Color.FromArgb(60, 255, 120, 90));
            g.FillEllipse(blushb, cx - ex - 10, eyeY + 6, 14, 9);
            g.FillEllipse(blushb, cx + ex - 4, eyeY + 6, 14, 9);
        }
        else if (sad)
        {
            using var eyePen = new Pen(Color.FromArgb(80, 50, isDog ? 20 : 50), 2.5f);
            g.DrawEllipse(eyePen, cx - ex - 5, eyeY - 4, 11, 10);
            g.DrawEllipse(eyePen, cx + ex - 6, eyeY - 4, 11, 10);
        }
        else
        {
            var eyeColor = Color.FromArgb(80, 50, isDog ? 20 : 50);
            using var eyeBrush = new SolidBrush(eyeColor);
            g.FillEllipse(eyeBrush, cx - ex - 6, eyeY - 4, 12, 11);
            g.FillEllipse(eyeBrush, cx + ex - 6, eyeY - 4, 12, 11);
            using var shineBrush = new SolidBrush(Color.White);
            g.FillEllipse(shineBrush, cx - ex - 3, eyeY - 2, 4, 4);
            g.FillEllipse(shineBrush, cx + ex - 3, eyeY - 2, 4, 4);
        }

        // Mouth
        using var mouthPen = new Pen(Color.FromArgb(isDog ? 140 : 200, isDog ? 90 : 80, isDog ? 50 : 100), 2f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        int my = eyeY + 14;
        if (happy)
        {
            g.DrawArc(mouthPen, cx - 9, my - 2, 9, 7, 0, 180);
            g.DrawArc(mouthPen, cx, my - 2, 9, 7, 0, 180);
        }
        else if (sad)
            g.DrawArc(mouthPen, cx - 7, my, 14, 7, 0, -180);
        else
        {
            g.DrawLine(mouthPen, cx - 4, my + 2, cx, my);
            g.DrawLine(mouthPen, cx, my, cx + 4, my + 2);
        }
    }

    /// Legend cat uses teal eye colour instead of the standard dark
    private static void DrawFaceLegendCat(Graphics g, int cx, int eyeY,
                                           bool happy, bool sad, bool blinking, int eyeSpread)
    {
        int ex = eyeSpread;
        var darkCol = Color.FromArgb(0, 180, 180);
        var whiteCol = Color.LightCyan;

        if (blinking)
        {
            using var blinkPen = new Pen(darkCol, 2.5f)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round
            };
            g.DrawArc(blinkPen, cx - ex - 5, eyeY, 12, 6, 0, -180);
            g.DrawArc(blinkPen, cx + ex - 6, eyeY, 12, 6, 0, -180);
        }
        else if (happy)
        {
            using var eyePen = new Pen(darkCol, 3f)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round
            };
            g.DrawArc(eyePen, cx - ex - 6, eyeY - 2, 13, 9, 180, 180);
            g.DrawArc(eyePen, cx + ex - 7, eyeY - 2, 13, 9, 180, 180);
        }
        else if (sad)
        {
            using var eyePen = new Pen(darkCol, 2.5f);
            g.DrawEllipse(eyePen, cx - ex - 5, eyeY - 4, 11, 10);
            g.DrawEllipse(eyePen, cx + ex - 6, eyeY - 4, 11, 10);
        }
        else
        {
            using var eyeBrush = new SolidBrush(darkCol);
            g.FillEllipse(eyeBrush, cx - ex - 6, eyeY - 4, 12, 11);
            g.FillEllipse(eyeBrush, cx + ex - 6, eyeY - 4, 12, 11);
            using var shineBrush = new SolidBrush(whiteCol);
            g.FillEllipse(shineBrush, cx - ex - 3, eyeY - 2, 4, 4);
            g.FillEllipse(shineBrush, cx + ex - 3, eyeY - 2, 4, 4);
        }

        // Mouth (cat colours)
        using var mouthPen = new Pen(Color.FromArgb(200, 80, 100), 2f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round
        };
        int my = eyeY + 14;
        if (happy)
        {
            g.DrawArc(mouthPen, cx - 9, my - 2, 9, 7, 0, 180);
            g.DrawArc(mouthPen, cx, my - 2, 9, 7, 0, 180);
        }
        else if (sad)
            g.DrawArc(mouthPen, cx - 7, my, 14, 7, 0, -180);
        else
        {
            g.DrawLine(mouthPen, cx - 4, my + 2, cx, my);
            g.DrawLine(mouthPen, cx, my, cx + 4, my + 2);
        }
    }

    internal static void DrawSparkles(Graphics g, int cx, int cy,
                                       int frame, Color color, int count, bool small)
    {
        int sz = small ? 4 : 6;
        int alpha = frame % 2 == 0 ? 220 : 90;
        using var brush = new SolidBrush(Color.FromArgb(alpha, color));
        int[][] positions = small
            ? [[cx + 24, cy - 32], [cx - 26, cy - 26], [cx + 18, cy - 14]]
            : [[cx + 34, cy - 42], [cx - 38, cy - 36], [cx + 26, cy - 18],
               [cx - 20, cy - 54], [cx,      cy - 58]];
        for (int i = 0; i < Math.Min(count, positions.Length); i++)
            DrawSparkle(g, brush, positions[i][0], positions[i][1], sz);
    }

    internal static void DrawTears(Graphics g, int cx, int cy, int count)
    {
        using var tearBrush = new SolidBrush(Color.FromArgb(160, 140, 180, 255));
        if (count >= 1) g.FillEllipse(tearBrush, cx - 18, cy - 12, 5, 9);
        if (count >= 2) g.FillEllipse(tearBrush, cx + 13, cy - 12, 5, 9);
    }

    internal static void DrawHearts(Graphics g, int cx, int cy, int frame, int count)
    {
        int alpha = frame % 2 == 0 ? 220 : 80;
        using var brush = new SolidBrush(Color.FromArgb(alpha, 255, 100, 130));
        int[][] pos = [[cx + 36, cy - 44], [cx - 40, cy - 38],
                       [cx + 28, cy - 60], [cx - 28, cy - 58]];
        for (int i = 0; i < Math.Min(count, pos.Length); i++)
            DrawHeart(g, brush, pos[i][0], pos[i][1], 7);
    }

    internal static void DrawSparkle(Graphics g, Brush brush, int x, int y, int size)
    {
        g.FillEllipse(brush, x - size / 2, y - size / 2, size, size);
        Color c = brush is SolidBrush sb ? sb.Color : Color.Yellow;
        using var pen = new Pen(Color.FromArgb(Math.Max(c.A - 40, 0), c.R, c.G, c.B), 1.5f);
        g.DrawLine(pen, x - size, y, x + size, y);
        g.DrawLine(pen, x, y - size, x, y + size);
        g.DrawLine(pen, x - size + 2, y - size + 2, x + size - 2, y + size - 2);
        g.DrawLine(pen, x + size - 2, y - size + 2, x - size + 2, y + size - 2);
    }

    private static void DrawHeart(Graphics g, Brush brush, int x, int y, int size)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        path.AddBezier(x, y, x - size, y - size, x - size * 2, y + size / 2, x, y + size * 2);
        path.AddBezier(x, y + size * 2, x + size * 2, y + size / 2, x + size, y - size, x, y);
        g.FillPath(brush, path);
    }
}