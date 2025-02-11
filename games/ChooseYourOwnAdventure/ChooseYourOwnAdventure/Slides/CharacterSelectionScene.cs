﻿using BuildingGames.GameObjects;
using BuildingGames.Models;

namespace BuildingGames.Slides;

public class CharacterSelectionScene : SlideSceneBase
{
    const float padding = 40f;
    const int columns = 3;
    const int rows = 2;

    private int currentTransition = 0;
    private const int transitions = 1;

    private readonly IDictionary<string, Microsoft.Maui.Graphics.IImage> images;
    private readonly IList<Character> characters;
    private readonly ControllerManager controllerManager;
    private int currentCharacterIndex;
    private bool handlingKeyPress = false;

    public CharacterSelectionScene(ControllerManager controllerManager, Pointer pointer) : base(pointer)
    {
        images = new Dictionary<string, Microsoft.Maui.Graphics.IImage>
        {
            ["shaun.png"] = LoadImage("shaun.png")
        };

        this.characters = new List<Character>
        {
            new Character
            {
                Name = "Shaun Lawrence (He/Him)",
                Strengths = new List<string>
                {
                    "Software Engineer/Consultant",
                    "Microsoft MVP",
                    ".NET MAUI CommunityToolkit",
                    "Author of 'Introducing .NET MAUI'"
                },
                Weaknesses = new List<string>
                {
                    "Distracted easily",
                    "Disappearing down what seem like fun little projects but turn out to be very very deep rabbit holes",
                    "Cake"
                },
                ImageName = "shaun.png"
            },
            new Character
            {
                Name = "??",
                IsLocked = true,
                UnlockCriteria = "Listen for 60 minutes to unlock this character"
            },
            new Character
            {
                Name = "??",
                IsLocked = true,
                UnlockCriteria = "Listen for 120 minutes to unlock this character"
            },
            new Character
            {
                Name = "??",
                IsLocked = true,
                UnlockCriteria = "Listen for 180 minutes to unlock this character"
            },
            new Character
            {
                Name = "??",
                IsLocked = true,
                UnlockCriteria = "Listen for 240 minutes to unlock this character"
            },
            new Character
            {
                Name = "??",
                IsLocked = true,
                UnlockCriteria = "Listen for 300 minutes to unlock this character"
            }
        };

        this.controllerManager = controllerManager;
    }

    public override void Progress()
    {
        // If we are complete then fire the Next event.
        if (currentTransition == transitions)
        {
            base.Progress();
        }

        currentTransition++;
    }

    public override void Render(ICanvas canvas, RectF dimensions)
    {
        Styling.RenderTitle("Character selection", canvas, dimensions);

        float tileWidth = ((dimensions.Width / columns) - (padding * (columns + 1))) / rows;
        float yOffset = 200;

        canvas.FillColor = Colors.Black;
        canvas.StrokeColor = Colors.Orange;
        canvas.StrokeSize = 4;
        canvas.SetShadow(new SizeF(0, 0), 0, Colors.Transparent);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                var bounds = new RectF(
                    padding * (x + 1) + (tileWidth * x),
                    padding * (y + 1) + (tileWidth * y) + yOffset,
                    tileWidth,
                    tileWidth);

                canvas.FillRectangle(bounds);

                if (x == 0 &&
                    y == 0)
                {
                    var image = images["shaun.png"];

                    canvas.DrawImage(image, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                }
                else
                {
                    canvas.DrawString(
                        bounds,
                        "?",
                        Styling.TitleColor,
                        Colors.Transparent,
                        1,
                        Styling.Font,
                        (float)Styling.ScaledFontSize(0.15),
                        bounds.Location,
                        HorizontalAlignment.Center,
                        VerticalAlignment.Center);
                }
            }
        }

        // Based on characterIndex;
        var x1 = this.currentCharacterIndex % columns;
        var y1 = this.currentCharacterIndex / columns;

        canvas.DrawRectangle(
            padding * (x1 + 1) + (tileWidth * x1),
            padding * (y1 + 1) + (tileWidth * y1) + yOffset,
            tileWidth,
            tileWidth);

        var character = this.characters[currentCharacterIndex];

        var font = Styling.Font;
        canvas.Font = font;
        canvas.FontSize = 30;
        canvas.FontColor = Colors.White;

        var columnWidth = dimensions.Width / 2 - padding * 2;

        canvas.DrawString(
            new RectF(dimensions.Center.X + padding, yOffset, columnWidth, 100),
            $"Name:{Environment.NewLine}{character.Name}",
            Styling.TitleColor,
            Colors.Transparent,
            1,
            font,
            (float)Styling.ScaledFontSize(0.04),
            new PointF(dimensions.Center.X + padding, yOffset),
            HorizontalAlignment.Left,
            VerticalAlignment.Center);

        if (character.IsLocked is false)
        {
            canvas.DrawString(
                new RectF(dimensions.Center.X + padding, yOffset + 100, columnWidth, 100),
                $"Strengths:{Environment.NewLine} - {string.Join($"{Environment.NewLine} - ", character.Strengths)}",
                Styling.TitleColor,
                Colors.Transparent,
                1,
                font,
                (float)Styling.ScaledFontSize(0.04),
                new PointF(dimensions.Center.X + padding, yOffset + 100),
                HorizontalAlignment.Left,
                VerticalAlignment.Center);

            canvas.DrawString(
                new RectF(dimensions.Center.X + padding, yOffset + 280, columnWidth, 100),
                $"Weaknesses:{Environment.NewLine} - {string.Join($"{Environment.NewLine} - ", character.Weaknesses)}",
                Styling.TitleColor,
                Colors.Transparent,
                1,
                font,
                (float)Styling.ScaledFontSize(0.04),
                new PointF(dimensions.Center.X + padding, yOffset + 280),
                HorizontalAlignment.Left,
                VerticalAlignment.Center);
        }
        else
        {
            canvas.DrawString(
                new RectF(dimensions.Center.X + padding, yOffset + 100, columnWidth, 100),
                character.UnlockCriteria,
                Styling.TitleColor,
                Colors.Transparent,
                1,
                font,
                30,
                new PointF(dimensions.Center.X + padding, yOffset + 100),
                HorizontalAlignment.Left,
                VerticalAlignment.Center);
        }

        if (currentTransition == 1)
        {
            canvas.DrawString(
                dimensions,
                @"Cake Driven Development",
                Styling.TitleColor,
                Colors.Transparent,
                1,
                Styling.Font,
                (float)Styling.ScaledFontSize(0.058),
                new PointF(40, dimensions.Height * 0.88f),
                HorizontalAlignment.Center,
                VerticalAlignment.Top);
        }

        base.Render(canvas, dimensions);
    }

    public override void Update(double millisecondsSinceLastUpdate)
    {
        base.Update(millisecondsSinceLastUpdate);

        if (controllerManager.Mode == ControlMode.Navigation)
        {
            if (controllerManager.CurrentPressedButton == ControllerButton.Right &&
                !handlingKeyPress)
            {
                handlingKeyPress = true;
                this.currentCharacterIndex = Math.Clamp(this.currentCharacterIndex + 1, 0, this.characters.Count);
            }
            else if (controllerManager.CurrentPressedButton == ControllerButton.Left &&
                !handlingKeyPress)
            {
                handlingKeyPress = true;
                this.currentCharacterIndex = Math.Clamp(this.currentCharacterIndex - 1, 0, this.characters.Count);
            }
            else if (controllerManager.CurrentPressedButton == ControllerButton.Down &&
                !handlingKeyPress)
            {
                handlingKeyPress = true;
                this.currentCharacterIndex = Math.Clamp(this.currentCharacterIndex + columns, 0, this.characters.Count);
            }
            else if (controllerManager.CurrentPressedButton == ControllerButton.Up &&
                !handlingKeyPress)
            {
                handlingKeyPress = true;
                this.currentCharacterIndex = Math.Clamp(this.currentCharacterIndex - columns, 0, this.characters.Count);
            }
            else if (controllerManager.CurrentPressedButton == ControllerButton.None)
            {
                handlingKeyPress = false;
            }
        }
    }
}
