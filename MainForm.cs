//	Copyright © 2025, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Microsoft.Win32;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Sylnode.App;

public partial class MainForm : Form
{
    public MainForm()
    {
        this.UpdateScreens ();
        this.pictureBox = new CaptionPictureBox ();
        this.captureTimer = this.CreateTimer ();
        this.coffeeIcon = MainForm.CreateCoffeeIcon ();

        this.InitializeComponent ();
        this.ConfigureFormForSecondMonitor ();
        this.Load += this.HandleMainFormLoad;

        SystemEvents.DisplaySettingsChanged += this.HandleSystemEventsDisplaySettingsChanged;

        this.trayIcon = this.CreateTrayIcon ();
    }

    /*************************************************************************/
    public void SetActiveIcon(Icon icon)
    {
        this.Icon = icon;
    }

    /*************************************************************************/

    private System.Windows.Forms.Timer CreateTimer()
    {
        var captureTimer = new System.Windows.Forms.Timer ();
        captureTimer.Interval = 50;
        captureTimer.Tick += this.HandleCaptureTimerTick;
        return captureTimer;
    }

    private NotifyIcon CreateTrayIcon()
    {
        ContextMenuStrip trayMenu = new ContextMenuStrip ();
        trayMenu.Items.Add ("Activer", null, this.HandleOnShow);
        trayMenu.Items.Add ("Quitter", null, this.HandleOnExit);

        var trayIcon = new NotifyIcon
        {
            Text = "Sylnode",
            Icon = this.coffeeIcon,
            ContextMenuStrip = trayMenu,
            Visible = true
        };

        return trayIcon;
    }

    private static Icon CreateCoffeeIcon()
    {
        Icon coffeeIcon;
        using (var ms = new MemoryStream (Properties.Resources.coffee))
        {
            coffeeIcon = new Icon (ms);
        }

        return coffeeIcon;
    }

    /*************************************************************************/
    
    private void CaptureAndDisplayScreen()
    {
        var primaryScreen = Screen.PrimaryScreen
            ?? throw new InvalidOperationException ();
        
        var bounds = primaryScreen.Bounds;
        var bitmap = new Bitmap (bounds.Width, bounds.Height);
        
        using (var graphics = Graphics.FromImage (bitmap))
        {
            graphics.CopyFromScreen (bounds.Location, Point.Empty, bounds.Size);
        }

        this.DisplayImage (bitmap);
    }

    private void ConfigureFormForSecondMonitor()
    {
        this.TopMost = true;

        this.FormBorderStyle = this.hasMutipleScreens
            ? FormBorderStyle.None
            : FormBorderStyle.SizableToolWindow;
        this.ControlBox = false;
        this.Text = string.Empty;

        // Attempt to use the second monitor if available.

        this.StartPosition = FormStartPosition.Manual;
        this.Location = this.targetScreen.Bounds.Location;
        this.Size = this.targetScreen.Bounds.Size;

        if (this.hasMutipleScreens == false)
        {
            int dx = this.Size.Width / 4;
            int dy = this.Size.Height / 4;
            this.Location = new Point (this.Location.X + this.Size.Width - dx, this.Location.Y);
            this.Size = new Size (dx, dy);
        }
    }

    /*************************************************************************/

    private void ToggleCapturing()
    {
        if (this.captureTimer.Enabled)
        {
            this.StopCapturing ();
        }
        else
        {
            this.StartCapturing ();
        }
    }

    private void StartCapturing()
    {
        this.SetActiveIcon (IconHelper.CreateBadgeIcon (this.coffeeIcon));
        this.pictureBox.Text = "";
        this.pictureBox.DisplayImage = true;
        this.pictureBox.Invalidate ();
        
        this.captureTimer.Start ();
    }

    private void StopCapturing()
    {
        this.SetActiveIcon (this.coffeeIcon);
        this.pictureBox.Text = "Sylnode - écran gelé";
        this.pictureBox.Invalidate ();
        
        this.captureTimer.Stop ();
    }

    private void UpdateScreens()
    {
        Screen[] screens = Screen.AllScreens;
        this.hasMutipleScreens = (screens.Length > 1);
        this.targetScreen = this.hasMutipleScreens
            ? screens[1]
            : Screen.PrimaryScreen
                ?? throw new InvalidOperationException ("No screens found");
    }

    private void DisplayImage(Image image)
    {
        if (this.pictureBox.InvokeRequired)
        {
            this.pictureBox.Invoke (new Action (() => this.DisplayImage (image)));
        }
        else
        {
            if (this.pictureBox.Image is not null)
            {
                this.pictureBox.Image.Dispose ();
                this.pictureBox.Image = null;
            }
            this.pictureBox.Image = image;
            this.Invalidate ();
        }
    }

    /*************************************************************************/

    private void HandleOnShow(object? sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Normal;
        this.ShowInTaskbar = true;
        this.Activate ();
    }

    private void HandleOnExit(object? sender, EventArgs e)
    {
        this.trayIcon.Visible = false;
        Application.Exit ();
    }

    private void HandleSystemEventsDisplaySettingsChanged(object? sender, EventArgs e)
    {
        this.UpdateScreens ();
        this.ConfigureFormForSecondMonitor ();
    }

    private void HandleMainFormLoad(object? sender, EventArgs e)
    {
        bool success = Win32.RegisterHotKey (
            this.Handle, 1,
            Win32.MOD_CONTROL, (int)Keys.Oem2);
        if (!success)
        {
            int errorCode = Marshal.GetLastWin32Error ();
            MessageBox.Show ($"Failed to register hotkey. Error code: {errorCode}");
        }
    }

    private void HandleCaptureTimerTick(object? sender, EventArgs e)
    {
        this.CaptureAndDisplayScreen ();
    }

    private void HandleHotkeyAction()
    {
        this.ToggleCapturing ();
    }

    /*************************************************************************/
    
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Win32.WM_HOTKEY)
        {
            int id = m.WParam.ToInt32 ();
            if (id == 1)
            {
                this.HandleHotkeyAction ();
            }
        }
        base.WndProc (ref m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        Win32.UnregisterHotKey (this.Handle, 1);
        SystemEvents.DisplaySettingsChanged -= this.HandleSystemEventsDisplaySettingsChanged;
        base.OnFormClosing (e);
    }

    /*************************************************************************/

    private void InitializeComponent()
    {
        this.SuspendLayout ();

        this.pictureBox.Text = "Sylnode - Presser Ctrl-§ pour activer la recopie d'écran";
        this.pictureBox.Font = new Font ("Segoe UI", 14, FontStyle.Bold);
        this.pictureBox.Dock = DockStyle.Fill;
        this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // Set SizeMode to Zoom
        this.pictureBox.BackColor = Color.Black; // Set background color to black
        this.Controls.Add (this.pictureBox);

        this.Icon = this.coffeeIcon;
        this.BackColor = Color.Black;

        this.ResumeLayout (false);
    }

    /*************************************************************************/

    #region Win32 P/Invoke Interop

    private static class Win32
    {
        // Modifier keys codes
        public const int MOD_ALT = 0x0001;
        public const int MOD_CONTROL = 0x0002;
        public const int MOD_SHIFT = 0x0004;
        public const int MOD_WIN = 0x0008;

        // Windows message ID for hotkeys
        public const int WM_HOTKEY = 0x0312;

        [DllImport ("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, int vk);

        [DllImport ("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }

    #endregion

    /*************************************************************************/
    
    private bool hasMutipleScreens;
    private Screen targetScreen = default!;
    private readonly Icon coffeeIcon;
    private readonly CaptionPictureBox pictureBox;
    private readonly System.Windows.Forms.Timer captureTimer;
    private readonly NotifyIcon trayIcon;
}
