using System;
using System.Drawing;
using System.Windows.Forms;
using Pawductivity.Models;

namespace Pawductivity.Animations;

public class AnimationPanel : Panel
{
    public PetAnimator Animator { get; }

    public AnimationPanel(PetType petType)
    {
        // ── True WinForms transparency ────────────────────────────────────────
        // SetStyle must come BEFORE any BackColor assignment.
        SetStyle(
            ControlStyles.SupportsTransparentBackColor |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint,
            true);

        BackColor = Color.Transparent;   // now actually transparent

        Animator = new PetAnimator(this, petType);
    }

    // ── Tell WinForms to paint the parent background first ────────────────────
    // Without this override the area behind the panel stays black in many themes.
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Walk up to the first non-transparent parent and ask IT to paint
        // the region covered by this panel, giving us real see-through.
        if (Parent != null)
        {
            // Map this panel's client rect into the parent's coordinate space
            var parentPoint = Parent.PointToClient(PointToScreen(Point.Empty));

            e.Graphics.TranslateTransform(-parentPoint.X, -parentPoint.Y);

            using var clip = new Region(new Rectangle(parentPoint, Size));
            e.Graphics.Clip = clip;

            // Ask parent to render its background into our graphics context
            var parentArgs = new PaintEventArgs(e.Graphics,
                new Rectangle(parentPoint, Size));
            InvokePaintBackground(Parent, parentArgs);
            InvokePaint(Parent, parentArgs);

            e.Graphics.ResetTransform();
            e.Graphics.ResetClip();
        }
        else
        {
            base.OnPaintBackground(e);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) Animator.Dispose();
        base.Dispose(disposing);
    }
}