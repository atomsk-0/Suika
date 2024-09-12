using Mochi.DearImGui;
using Suika;
using Suika.Data;
using Suika.Types.Enums;

var options = AppOptions.DefaultWindows;
options.VSync = true;
options.RenderBackend = RenderBackend.DirectX9;
Application app = new Application(options);
app.SetView(renderView);
app.Run();

return 0;

static void renderView()
{
    ImGui.Text($"HELLO WORLD: {DateTime.UtcNow.ToLongTimeString()}");
}