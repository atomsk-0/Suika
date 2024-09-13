// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Backends.OpenGL3;
using Mochi.DearImGui.Backends.Win32;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using Suika.Data;
using Suika.Types.Interfaces;
using TerraFX.Interop.Windows;
using GL = Silk.NET.OpenGL.GL;
using Texture = Suika.Data.Texture;
using WGL = Silk.NET.WGL.WGL;

#pragma warning disable CA1416
#pragma warning disable CA1806

namespace Suika.Platforms.Windows.Backends;

public unsafe class Win32OpenGl : IBackend
{
    private GL gl = null!;
    private WGL wgl = null!;
    private IWindow window = null!;

    private HGLRC hrc;
    private HDC hdc;

    private int backendWidth, backendHeight;
    private Vector2 imguiWindowSize;

    public bool Setup(IWindow windowInstance, in AppOptions options)
    {
        wgl = new WGL(new DefaultNativeContext("opengl32"));
        gl = new GL(new DefaultNativeContext("opengl32"));

        window = windowInstance;
        HWND handle = (HWND)windowInstance.GetHandle();
        HDC hDc = TerraFX.Interop.Windows.Windows.GetDC(handle);
        var pfd = new PIXELFORMATDESCRIPTOR
        {
            nSize = (ushort)sizeof(PIXELFORMATDESCRIPTOR),
            nVersion = 1,
            dwFlags = PFD.PFD_DRAW_TO_WINDOW | PFD.PFD_SUPPORT_OPENGL | PFD.PFD_DOUBLEBUFFER,
            iPixelType = PFD.PFD_TYPE_RGBA,
            cColorBits = 32,
        };

        window.OnResize = onWindowResize;

        int pf = TerraFX.Interop.Windows.Windows.ChoosePixelFormat(hDc, &pfd);
        if (pf == 0) return false;
        if (TerraFX.Interop.Windows.Windows.SetPixelFormat(hDc, pf, &pfd) == false) return false;
        TerraFX.Interop.Windows.Windows.ReleaseDC(handle, hDc);

        hdc = TerraFX.Interop.Windows.Windows.GetDC(handle);
        hrc = (HGLRC)wgl.CreateContext(hdc);

        wgl.MakeCurrent(hdc, hrc);

        if (options.VSync)
        {
            if (enableVsync() == false)
            {
                Console.WriteLine("Failed to enable VSync"); // TODO: Replace with logging system
            }
        }

        ImGui.CreateContext();
        Win32ImBackend.InitForOpenGL(handle);
        OpenGL3ImBackend.Init();

        return true;
    }


    public void Reset()
    {
        // nothing to do here
    }


    public void Destroy()
    {
        OpenGL3ImBackend.Shutdown();
        Win32ImBackend.Shutdown();
        ImGui.DestroyContext();

        wgl.MakeCurrent(0, 0);
        TerraFX.Interop.Windows.Windows.ReleaseDC((HWND)window.GetHandle(), hdc);
        wgl.DeleteContext(hrc);
    }


    public void Render(Action renderAction)
    {
        if (TerraFX.Interop.Windows.Windows.IsIconic((HWND)window.GetHandle())) // Maybe move this to the window class
        {
            Thread.Sleep(10);
            return;
        }

        OpenGL3ImBackend.NewFrame();
        Win32ImBackend.NewFrame();
        ImGui.NewFrame();

        ImGui.SetNextWindowPos(Vector2.Zero, ImGuiCond.Once, Vector2.Zero);
        ImGui.SetNextWindowSize(imguiWindowSize, ImGuiCond.Always);
        ImGui.Begin("suika_imgui_window", null, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration);
        ImGui.End();

        ImGui.ShowDemoWindow();

        ImGui.EndFrame();

        ImGui.Render();

        gl.Viewport(0, 0, (uint)backendWidth, (uint)backendHeight);
        gl.ClearColor(Color.Black);
        gl.Clear(ClearBufferMask.ColorBufferBit);
        OpenGL3ImBackend.RenderDrawData(ImGui.GetDrawData());

        TerraFX.Interop.Windows.Windows.SwapBuffers(hdc);
    }


    public Texture LoadTextureFromFile(string path)
    {
        throw new NotImplementedException();
    }


    public Texture LoadTextureFromMemory(Stream stream)
    {
        throw new NotImplementedException();
    }


    private void onWindowResize(int width, int height)
    {
        backendWidth = width;
        backendHeight = height;
        imguiWindowSize = new Vector2(width, height);
    }

    private bool disableVsync()
    {
        if (wglExtensionSupported("WGL_EXT_swap_control"))
        {
            var wglSwapIntervalExt = (delegate* unmanaged<int, int>)wgl.GetProcAddress("wglSwapIntervalEXT");
            if (wglSwapIntervalExt == null) return false;
            wglSwapIntervalExt(0);
            return true;
        }
        return false;
    }

    private bool enableVsync()
    {
        if (wglExtensionSupported("WGL_EXT_swap_control"))
        {
            var wglSwapIntervalExt = (delegate* unmanaged<int, int>)wgl.GetProcAddress("wglSwapIntervalEXT");
            if (wglSwapIntervalExt == null) return false;
            wglSwapIntervalExt(1);
            return true;
        }
        return false;
    }


    // https://stackoverflow.com/questions/589064/how-to-enable-vertical-sync-in-opengl
    private bool wglExtensionSupported(string extensionName)
    {
        var wglGetExtensionsStringExt = (delegate* unmanaged<void>)wgl.GetProcAddress("wglGetExtensionsStringEXT");
        if (wglGetExtensionsStringExt == null) return false;
        sbyte* extensions = ((delegate* unmanaged<void*, sbyte*>)wglGetExtensionsStringExt)(null);
        return extensions != null && new string(extensions).Contains(extensionName);
    }
}