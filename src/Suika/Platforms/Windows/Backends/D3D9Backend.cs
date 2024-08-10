// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Suika.Types.Interfaces;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace Suika.Platforms.Windows.Backends;

#pragma warning disable CA1416


public unsafe class D3D9Backend : IBackend
{
    private IDirect3D9* d3d9;
    private D3DPRESENT_PARAMETERS d3dpp;
    private IDirect3DDevice9* device;

    private bool deviceLost;
    private bool firstTime = true;

    public bool Setup(nint windowHandle)
    {
        if ((d3d9 = DirectX.Direct3DCreate9(D3D.D3D_SDK_VERSION)) == null) return false;

        var lD3dpp = new D3DPRESENT_PARAMETERS
        {
            Windowed = true,
            SwapEffect = D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD,
            BackBufferFormat = D3DFORMAT.D3DFMT_UNKNOWN,
            EnableAutoDepthStencil = true,
            AutoDepthStencilFormat = D3DFORMAT.D3DFMT_D16,
            PresentationInterval = D3DPRESENT.D3DPRESENT_INTERVAL_ONE
        };
        IDirect3DDevice9* lDevice = null;

        if (d3d9->CreateDevice(DirectX.D3DADAPTER_DEFAULT, D3DDEVTYPE.D3DDEVTYPE_HAL, (HWND)windowHandle, D3DCREATE.D3DCREATE_HARDWARE_VERTEXPROCESSING, &lD3dpp, &lDevice) < 0) return false;

        d3dpp = lD3dpp;
        device = lDevice;

        return true;
    }

    public void Reset()
    {
        var ld3dpp = d3dpp;
        device->Reset(&ld3dpp);
        d3dpp = ld3dpp;
    }

    public void Destroy()
    {
        if (device != null)
        {
            device->Release();
            device = null;
        }
        if (d3d9 != null)
        {
            d3d9->Release();
            d3d9 = null;
        }
    }

    public void Render(Action renderAction)
    {
        if (deviceLost)
        {
            HRESULT hr = device->TestCooperativeLevel();
            if (hr == D3DERR.D3DERR_DEVICELOST)
            {
                Thread.Sleep(10);
                return;
            }
            if (hr == D3DERR.D3DERR_DEVICENOTRESET) Reset();
            deviceLost = false;
        }

        if (firstTime)
        {
            d3dpp.BackBufferWidth = 800;
            d3dpp.BackBufferHeight = 500;
            Reset();
            firstTime = false;
        }

        device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_ZENABLE, 0);
        device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_ALPHABLENDENABLE, 0);
        device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_SCISSORTESTENABLE, 0);
        device->Clear(0, null, D3DCLEAR.D3DCLEAR_TARGET | D3DCLEAR.D3DCLEAR_ZBUFFER, 0x00000000, 1.0f, 0);

        if (device->BeginScene() >= 0)
        {
            device->EndScene();
        }

        HRESULT result = device->Present(null, null, HWND.NULL, null);
        if (result == D3DERR.D3DERR_DEVICELOST) deviceLost = true;
    }


    public nint LoadImageFromFile(string path)
    {
        throw new NotImplementedException();
    }

    public nint LoadImageFromMemory(Stream stream)
    {
        throw new NotImplementedException();
    }


    public void ImGuiInit()
    {

    }
    public void ImGuiShutdown()
    {

    }
}