﻿// This file is part of Pong, a Game written in C# with the Aximo Game Engine. Web: https://github.com/kbitGit/pong_axengine, https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo;
using Aximo.Engine;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Windows;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common.Input;

namespace Pong
{
    internal class Paddle : Actor
    {

        public static float Speed { get; set; } = 20.0f;

        public Vector2 WorldSize { get; set; }

        public Key Up { get; set; }
        public Key Down { get; set; }

        private SceneComponent gfx;

        public Paddle(Vector2 position, Vector2 size)
        {
            SetupPaddle(position, size);
        }

        private void SetupPaddle(Vector2 position, Vector2 size)
        {
            gfx = new RoundedCubeComponent()
            {
                Name = "gfx",
                RelativeTranslation = new Vector3(position.X * 0.95f, position.Y, 0),
                RelativeScale = new Vector3(size.X, size.Y, 1),
                Material = new Material()
                {
                    Color = new Vector4(0.5f, 1f, 0.5f, 1f),
                    CastShadow = true,
                    Ambient = 0.3f,
                    Shininess = 32.0f,
                    SpecularStrength = 0.5f,
                },
            };
            AddComponent(gfx);
        }

        public Box2 Bounds { get; private set; }

        public override void UpdateFrame()
        {
            var delta = (float)Application.Current.UpdateCounter.Elapsed.TotalMilliseconds / 1000.0f;
            var keyboardState = WindowContext.Current.Window.KeyboardState;

            if (keyboardState.IsKeyDown(Up) && (gfx.RelativeTranslation.Y + (gfx.RelativeScale.Y / 2)) < WorldSize.Y / 2)
            {
                gfx.RelativeTranslation = gfx.RelativeTranslation + new Vector3(0, Speed * delta, 0);
            }
            if (keyboardState.IsKeyDown(Down) && (gfx.RelativeTranslation.Y - (gfx.RelativeScale.Y / 2)) > -WorldSize.Y / 2)
            {
                gfx.RelativeTranslation = gfx.RelativeTranslation - new Vector3(0, Speed * delta, 0);
            }
            Bounds = BoxHelper.FromCenteredSize(gfx.RelativeTranslation.Xy, gfx.RelativeScale.Xy);
        }

        public bool CollidesWithBall(Vector2 ballPosition, float radius)
        {
            return CollidesWithBall(Bounds, ballPosition, radius);
        }

        private static bool CollidesWithBall(Box2 box, Vector2 ballPosition, float radius)
        {
            var top = box.Max.Y;
            var left = box.Min.X;
            var bottom = box.Min.Y;
            var right = box.Max.X;

            bool checkIfInSquare = ballPosition.X.IsBetween(bottom, top) && ballPosition.Y.IsBetween(left, right);
            bool checkCorners = ballPosition.IsInCornerRadius(top, bottom, left, right, radius);

            bool topOrBotCollision = false;
            if (ballPosition.X.IsBetween(left, right))
                topOrBotCollision = (Math.Abs(ballPosition.Y - top) <= radius) || (Math.Abs(bottom - ballPosition.Y) <= radius);

            bool leftOrRightCollision = false;
            if (ballPosition.Y.IsBetween(bottom, top))
                leftOrRightCollision = (Math.Abs(ballPosition.X - right) <= radius) || (Math.Abs(left - ballPosition.X) <= radius);

            return checkIfInSquare || checkCorners || topOrBotCollision || leftOrRightCollision;
        }
    }
}
