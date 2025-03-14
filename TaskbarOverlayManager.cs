//	Copyright © 2025, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Sylnode.App;

/// <summary>
/// Utility class for setting taskbar button overlay icons
/// </summary>
public class TaskbarOverlayManager
{
    // COM interface for taskbar functionality
    [ComImport]
    [Guid ("56FDF344-FD6D-11d0-958A-006097C9A090")]
    [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
    private interface ITaskbarList
    {
        void HrInit();
        void AddTab(IntPtr hwnd);
        void DeleteTab(IntPtr hwnd);
        void ActivateTab(IntPtr hwnd);
        void SetActiveAlt(IntPtr hwnd);
    }

    [ComImport]
    [Guid ("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    [InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
    private interface ITaskbarList3 : ITaskbarList
    {
        // ITaskbarList methods
        new void HrInit();
        new void AddTab(IntPtr hwnd);
        new void DeleteTab(IntPtr hwnd);
        new void ActivateTab(IntPtr hwnd);
        new void SetActiveAlt(IntPtr hwnd);

        // ITaskbarList2 methods (skipping ITaskbarList2 definition for brevity)
        void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs (UnmanagedType.Bool)] bool fFullscreen);

        // ITaskbarList3 methods
        void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
        void SetProgressState(IntPtr hwnd, TaskbarProgressState tbpFlags);
        void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
        void UnregisterTab(IntPtr hwndTab);
        void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
        void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, uint dwReserved);
        void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, IntPtr pButtons);
        void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, IntPtr pButtons);
        void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
        void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs (UnmanagedType.LPWStr)] string pszDescription);
        void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs (UnmanagedType.LPWStr)] string pszTip);
        void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);
    }

    // COM class for taskbar functionality
    [ComImport]
    [Guid ("56FDF344-FD6D-11d0-958A-006097C9A090")]
    [ClassInterface (ClassInterfaceType.None)]
    private class TaskbarList { }

    // Taskbar progress states
    public enum TaskbarProgressState : uint
    {
        NoProgress = 0,
        Indeterminate = 0x1,
        Normal = 0x2,
        Error = 0x4,
        Paused = 0x8
    }

    // Singleton instance of the TaskbarOverlayManager
    private static TaskbarOverlayManager? _instance;
    private static readonly object _lock = new object ();

    // Internal reference to the taskbar COM object
    private readonly ITaskbarList3 taskbarList;

    // Private constructor (singleton pattern)
    private TaskbarOverlayManager()
    {
        this.taskbarList = (ITaskbarList3)new TaskbarList ();
        this.taskbarList.HrInit ();
    }

    // Get the singleton instance
    public static TaskbarOverlayManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new TaskbarOverlayManager ();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Sets an overlay icon on the taskbar button of the specified window
    /// </summary>
    /// <param name="form">The form whose taskbar button should be modified</param>
    /// <param name="icon">The overlay icon to display, or null to remove the overlay</param>
    /// <param name="description">Accessibility description of the icon</param>
    public void SetOverlayIcon(Form form, Icon? icon, string? description = null)
    {
        if (form == null)
        {
            throw new ArgumentNullException (nameof (form));
        }

        IntPtr iconHandle = icon?.Handle ?? IntPtr.Zero;
        this.taskbarList.SetOverlayIcon (form.Handle, iconHandle, description ?? string.Empty);
    }

    /// <summary>
    /// Clears any overlay icon on the taskbar button of the specified window
    /// </summary>
    /// <param name="form">The form whose taskbar button should be modified</param>
    public void ClearOverlayIcon(Form form)
    {
        this.SetOverlayIcon (form, null);
    }
}
