//	Copyright © 2026, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Sylnode.App;

/// <summary>
/// The display topologies offered by Windows-P.
/// </summary>
public enum DisplayTopology
{
    Unknown  = 0,
    Internal = 1,   // PC screen only
    Clone    = 2,   // Duplicate
    Extend   = 4,   // Extend
    External = 8,   // Second screen only
}

/// <summary>
/// Reads and switches the display topology, as Windows-P does.
/// </summary>
public static class DisplayTopologyManager
{
    /// <summary>
    /// Gets the active topology, or <see cref="DisplayTopology.Unknown"/> when
    /// Windows cannot tell it.
    /// </summary>
    public static DisplayTopology GetCurrent()
    {
        for (int attempt = 0; attempt < 3; attempt++)
        {
            if (Win32.GetDisplayConfigBufferSizes (Win32.QDC_DATABASE_CURRENT, out var pathCount, out var modeCount) != Win32.ERROR_SUCCESS)
            {
                return DisplayTopology.Unknown;
            }

            var paths = Marshal.AllocHGlobal ((int) pathCount * Win32.PathInfoSize);
            var modes = Marshal.AllocHGlobal ((int) modeCount * Win32.ModeInfoSize);

            try
            {
                var result = Win32.QueryDisplayConfig (
                    Win32.QDC_DATABASE_CURRENT,
                    ref pathCount, paths,
                    ref modeCount, modes,
                    out var topology);

                if (result == Win32.ERROR_SUCCESS)
                {
                    return (DisplayTopology) topology;
                }

                //  The displays changed between the two calls: ask again for
                //  the buffer sizes.

                if (result != Win32.ERROR_INSUFFICIENT_BUFFER)
                {
                    return DisplayTopology.Unknown;
                }
            }
            finally
            {
                Marshal.FreeHGlobal (paths);
                Marshal.FreeHGlobal (modes);
            }
        }

        return DisplayTopology.Unknown;
    }

    /// <summary>
    /// Switches to the topology, with the layout Windows last used for these
    /// displays (position, resolution, main screen).
    /// </summary>
    /// <returns><c>true</c> when Windows applied the topology.</returns>
    public static bool Apply(DisplayTopology topology)
    {
        if (topology == DisplayTopology.Unknown)
        {
            return false;
        }

        var result = Win32.SetDisplayConfig (0, IntPtr.Zero, 0, IntPtr.Zero, Win32.SDC_APPLY | (uint) topology);

        return result == Win32.ERROR_SUCCESS;
    }

    /*************************************************************************/

    #region Win32 P/Invoke Interop

    private static class Win32
    {
        public const uint QDC_DATABASE_CURRENT = 0x00000004;
        public const uint SDC_APPLY = 0x00000080;

        public const int ERROR_SUCCESS = 0;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;

        // Sizes of DISPLAYCONFIG_PATH_INFO and DISPLAYCONFIG_MODE_INFO; their
        // content is not read, only the topology identifier is.
        public const int PathInfoSize = 72;
        public const int ModeInfoSize = 64;

        [DllImport ("user32.dll")]
        public static extern int GetDisplayConfigBufferSizes(uint flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

        [DllImport ("user32.dll")]
        public static extern int QueryDisplayConfig(
            uint flags,
            ref uint numPathArrayElements, IntPtr pathArray,
            ref uint numModeInfoArrayElements, IntPtr modeInfoArray,
            out uint currentTopologyId);

        [DllImport ("user32.dll")]
        public static extern int SetDisplayConfig(uint numPathArrayElements, IntPtr pathArray, uint numModeInfoArrayElements, IntPtr modeInfoArray, uint flags);
    }

    #endregion
}
