﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrowEngineUI.MonoGame.Avalonia;

public class AvaloniaGameRenderer
{
    private readonly GraphicsDevice _device;
    private readonly SpriteBatch _spriteBatch;
    private bool _isDirty;
    private RenderTargetBinding[] _previousRenderTargets;
    private RenderTarget2D? _renderTarget;
    private Point _resolution;
    private Vector2 _scale;
    private Point _virtualResolution;

    public AvaloniaGameRenderer(GraphicsDevice device)
    {
        _device = device;
        _virtualResolution =
            new Point(device.Adapter.CurrentDisplayMode.Width, device.Adapter.CurrentDisplayMode.Height);
        _resolution.X = _device.Viewport.Width;
        _resolution.Y = _device.Viewport.Height;
        _spriteBatch = new SpriteBatch(_device);
        _isDirty = false;
        _previousRenderTargets = Array.Empty<RenderTargetBinding>();

        CreateRenderTarget();
    }

    private Point VirtualResolution
    {
        get => _virtualResolution;
        set
        {
            if (_virtualResolution == value) return;
            _virtualResolution = value;
            _isDirty = true;
        }
    }

    private Point Resolution
    {
        get => _resolution;
        set
        {
            if (_resolution == value) return;
            _resolution = value;
            _isDirty = true;
        }
    }

    private Color ClearColor { get; } = Color.CornflowerBlue;

    [MemberNotNull(nameof(_renderTarget))]
    private void CreateRenderTarget()
    {
        try
        {
            _renderTarget?.Dispose();
        }
        catch
        {
            // ignored
        }

        _renderTarget = new RenderTarget2D(_device, _virtualResolution.X, _virtualResolution.Y);
    }

    public void Begin()
    {
        if (_isDirty) Update();

        _previousRenderTargets = _device.GetRenderTargets();
        _device.SetRenderTarget(_renderTarget);
        _device.Clear(ClearColor);
    }

    public void End()
    {
        var position = Resolution.ToVector2() / 2.0f;
        var origin = VirtualResolution.ToVector2() / 2.0f;

        _device.SetRenderTargets(_previousRenderTargets);
        _device.Clear(ClearColor);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_renderTarget, position, null, Color.White, 0, origin, _scale, SpriteEffects.None, 0);
        _spriteBatch.End();
    }

    private void Update()
    {
        _isDirty = false;

        _scale = Resolution.ToVector2() / VirtualResolution.ToVector2();
        _scale = new Vector2(Math.Min(_scale.X, _scale.Y), Math.Min(_scale.X, _scale.Y));
        CreateRenderTarget();
    }
}