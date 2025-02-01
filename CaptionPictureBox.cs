//	Copyright © 2025, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ComponentModel;

namespace Sylnode.App;

public sealed class CaptionPictureBox : PictureBox
{
    public CaptionPictureBox()
    {
        this.SetStyle (
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint, true);
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Visible)]
    public bool DisplayImage { get; set; }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (this.DisplayImage)
        {
            base.OnPaint (e);
        }
        else
        {
            e.Graphics.FillRectangle (Brushes.Black, this.ClientRectangle);
        } 

        if (this.Text.Length == 0)
        {
            return;
        }

        var rectangle = new Rectangle (0, 0, this.ClientSize.Width, 32);

        using (SolidBrush semiTransBrush = new SolidBrush (Color.FromArgb (128, Color.Black)))
        {
            e.Graphics.FillRectangle (semiTransBrush, rectangle);
        }

        TextRenderer.DrawText (
            e.Graphics, this.Text, this.Font, rectangle,
            Color.White,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.Left);
    }
}
