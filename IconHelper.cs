//	Copyright © 2025, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Sylnode.App;

public class IconHelper
{
    public static Icon CreateBadgeIcon(Icon baseIcon)
    {
        // Convert the base icon to a bitmap.
        Bitmap baseBitmap = baseIcon.ToBitmap ();

        // Create a new bitmap to draw on.
        Bitmap composite = new Bitmap (baseBitmap.Width, baseBitmap.Height);

        using (Graphics g = Graphics.FromImage (composite))
        {
            // High quality drawing settings.
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Draw the base icon.
            g.DrawImage (baseBitmap, 0, 0, baseBitmap.Width, baseBitmap.Height);

            // Determine badge size and position.
            // For example, a circle in the top-right corner.
            int badgeDiameter = Math.Min (baseBitmap.Width, baseBitmap.Height) / 2;
            int badgeX = baseBitmap.Width - badgeDiameter - 2;
            int badgeY = 2;
            Rectangle badgeRect = new Rectangle (badgeX, badgeY, badgeDiameter, badgeDiameter);

            // Draw the badge (a red circle).
            using (Brush badgeBrush = new SolidBrush (Color.Red))
            {
                g.FillEllipse (badgeBrush, badgeRect);
            }
        }

        // Convert the composite bitmap back to an icon.
        // Note: GetHicon() returns a handle that should be destroyed later.
        Icon badgeIcon = Icon.FromHandle (composite.GetHicon ());

        // Optionally, you can copy the icon to a new Icon instance to manage its lifetime.
        // For simplicity, we'll return badgeIcon. In a production app, consider proper cleanup.
        return badgeIcon;
    }

    public static Icon CreateBadgeIcon(Icon baseIcon, string badgeText)
    {
        // Convert the base icon to a bitmap.
        Bitmap baseBitmap = baseIcon.ToBitmap ();

        // Create a new bitmap to draw on.
        Bitmap composite = new Bitmap (baseBitmap.Width, baseBitmap.Height);

        using (Graphics g = Graphics.FromImage (composite))
        {
            // High quality drawing settings.
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Draw the base icon.
            g.DrawImage (baseBitmap, 0, 0, baseBitmap.Width, baseBitmap.Height);

            // Determine badge size and position.
            // For example, a circle in the top-right corner.
            int badgeDiameter = Math.Min (baseBitmap.Width, baseBitmap.Height) / 2;
            int badgeX = baseBitmap.Width - badgeDiameter - 2;
            int badgeY = 2;
            Rectangle badgeRect = new Rectangle (badgeX, badgeY, badgeDiameter, badgeDiameter);

            // Draw the badge (a red circle).
            using (Brush badgeBrush = new SolidBrush (Color.Red))
            {
                g.FillEllipse (badgeBrush, badgeRect);
            }

            // Draw the badge text (white, centered).
            using (Font font = new Font ("Segoe UI", badgeDiameter / 1.5f, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                using (Brush textBrush = new SolidBrush (Color.White))
                {
                    g.DrawString (badgeText, font, textBrush, badgeRect, sf);
                }
            }
        }

        // Convert the composite bitmap back to an icon.
        // Note: GetHicon() returns a handle that should be destroyed later.
        Icon badgeIcon = Icon.FromHandle (composite.GetHicon ());

        // Optionally, you can copy the icon to a new Icon instance to manage its lifetime.
        // For simplicity, we'll return badgeIcon. In a production app, consider proper cleanup.
        return badgeIcon;
    }
}
