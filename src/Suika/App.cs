using Suika.Data;
using Suika.Platforms.Windows;
using Suika.Types.Interfaces;

namespace Suika;

public class App
{
    public IWindow? Window;
    public void OpenWindow(in WindowOptions options)
    {
        Window = new Window();
        Window.Create(options);
        Window.SetupImGui();
    }

    public void Run()
    {
        Window?.Render();
    }
}