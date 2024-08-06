// AOT test for Suika

using Suika.Data;
using Suika.Platforms.Windows;

Window window = new Window();
window.Create(WindowOptions.Default);
window.SetupImGui();
window.Render();
window.Destroy();