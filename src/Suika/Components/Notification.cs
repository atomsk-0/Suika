// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Suika.Data;
using Suika.Util;

namespace Suika.Components;

public static unsafe class Notification
{
    private static readonly List<NotificationData> notifications = [];

    private static Vector2 padding = default;
    private static Color backgroundColor = default;
    private static Color textColor = default;
    private static Color borderColor = default;
    private static float borderThickness = 0f;
    private static float rounding = 0f;
    private static Font? font = null;


    public static void Render()
    {
        if (font == null) return; // No style set so we don't render anything
        DateTime currentTime = DateTime.Now;
        notifications.RemoveAll(n => (currentTime - n.StartTime).TotalSeconds >= n.Duration);

        for (int i = 0; i < notifications.Count; i++)
        {
            var notification = notifications[i];
            float elapsed = (float)(currentTime - notification.StartTime).TotalSeconds;
            float alpha = Math.Min(elapsed / 0.5f, 1.0f); // Fade in effect for 0.5 seconds

            // Calc fade-out effect
            if (elapsed > notification.Duration - 0.5f)
            {
                alpha = Math.Min((notification.Duration - elapsed) / 0.5f, 1.0f);
            }

            ImGui.PushFont(font.ImFont);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, backgroundColor.ToVector4Color((byte)(alpha * 255)));
            ImGui.PushStyleColor(ImGuiCol.Text, textColor.ToVector4Color((byte)(alpha * 255)));
            ImGui.PushStyleColor(ImGuiCol.Border, borderColor.ToVector4Color((byte)(alpha * 255)));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, rounding);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, borderThickness);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, padding);
            Vector2 position = getNotificationPosition(notification, i, ImGui.CalcTextSize(notification.Message, true).X);
            ImGui.SetNextWindowBgAlpha(alpha);
            ImGui.SetNextWindowPos(position, ImGuiCond.Always, new Vector2());
            if (ImGui.Begin($"ntf_{i}{notification.Title}", null, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoFocusOnAppearing))
            {
                Text.Normal(notification.Title, font, textColor);
            }
            ImGui.End();
            ImGui.PopStyleVar(3);
            ImGui.PopStyleColor(3);
            ImGui.PopFont();
        }
    }

    public static void SetStyle(Vector2 padding, Color backgroundColor, Color textColor, Color borderColor, float borderThickness, float rounding, Font font)
    {
        Notification.padding = padding;
        Notification.backgroundColor = backgroundColor;
        Notification.textColor = textColor;
        Notification.borderColor = borderColor;
        Notification.borderThickness = borderThickness;
        Notification.rounding = rounding;
        Notification.font = font;
    }

    private static Vector2 getNotificationPosition(NotificationData notification, int index, float textWidth)
    {
        var io = ImGui.GetIO();
        Vector2 position = new Vector2();
        float offset = index * 30;
        switch (notification.Location)
        {
            case NotificationLocation.BottomRight:
                position = new Vector2(io->DisplaySize.X - (5 + textWidth), io->DisplaySize.Y - 40 - offset);
                break;
        }
        return position;
    }

    public static void ShowNotification(string title, string message, float duration)
    {
        notifications.Add(new NotificationData(title, message, duration, NotificationLocation.BottomRight));
    }
}

public readonly struct NotificationData(string title, string message, float duration, NotificationLocation location)
{
    public readonly string Title = title;
    public readonly string Message = message;
    public readonly DateTime StartTime = DateTime.Now;
    public readonly float Duration = duration;
    public readonly NotificationLocation Location = location;
}

public enum NotificationLocation
{
    BottomRight
}