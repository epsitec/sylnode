﻿namespace Sylnode.App;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles ();
        Application.SetCompatibleTextRenderingDefault (false);
        Application.Run (new MainForm ());
    }
}
