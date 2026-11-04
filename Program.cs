namespace Sylnode.App;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        //  Applies the high DPI mode of the project file: without it the process
        //  is DPI unaware, and Windows hands it a downscaled copy of a high
        //  resolution screen, which looks pixelated once enlarged on the TV.

        ApplicationConfiguration.Initialize ();
        Application.Run (new MainForm ());
    }
}
