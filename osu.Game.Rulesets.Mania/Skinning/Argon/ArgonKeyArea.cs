// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mania.Skinning.Argon
{
    public class ArgonKeyArea : CompositeDrawable, IKeyBindingHandler<ManiaAction>
    {
        private readonly IBindable<ScrollingDirection> direction = new Bindable<ScrollingDirection>();

        private Container directionContainer = null!;
        private Drawable background = null!;

        private Circle hitTargetLine = null!;

        private Container<Circle> bottomIcon = null!;
        private CircularContainer topIcon = null!;

        private Bindable<Color4> accentColour = null!;

        [Resolved]
        private Column column { get; set; } = null!;

        public ArgonKeyArea()
        {
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(IScrollingInfo scrollingInfo)
        {
            const float icon_circle_size = 8;
            const float icon_spacing = 7;
            const float icon_vertical_offset = -30;

            InternalChild = directionContainer = new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = Stage.HIT_TARGET_POSITION,
                Children = new[]
                {
                    new Container
                    {
                        Masking = true,
                        RelativeSizeAxes = Axes.Both,
                        CornerRadius = ArgonNotePiece.CORNER_RADIUS,
                        Child = background = new Box
                        {
                            Name = "Key gradient",
                            Alpha = 0,
                            Blending = BlendingParameters.Additive,
                            RelativeSizeAxes = Axes.Both,
                        },
                    },
                    hitTargetLine = new Circle
                    {
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.BottomCentre,
                        Colour = OsuColour.Gray(196 / 255f),
                        Height = ArgonNotePiece.CORNER_RADIUS * 2,
                        Masking = true,
                    },
                    new Container
                    {
                        Name = "Icons",
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Children = new Drawable[]
                        {
                            bottomIcon = new Container<Circle>
                            {
                                AutoSizeAxes = Axes.Both,
                                Anchor = Anchor.BottomCentre,
                                Origin = Anchor.Centre,
                                Blending = BlendingParameters.Additive,
                                Y = icon_vertical_offset,
                                Children = new[]
                                {
                                    new Circle
                                    {
                                        Size = new Vector2(icon_circle_size),
                                        Anchor = Anchor.BottomCentre,
                                        Origin = Anchor.Centre,
                                    },
                                    new Circle
                                    {
                                        X = -icon_spacing,
                                        Y = icon_spacing * 1.2f,
                                        Size = new Vector2(icon_circle_size),
                                        Anchor = Anchor.BottomCentre,
                                        Origin = Anchor.Centre,
                                    },
                                    new Circle
                                    {
                                        X = icon_spacing,
                                        Y = icon_spacing * 1.2f,
                                        Size = new Vector2(icon_circle_size),
                                        Anchor = Anchor.BottomCentre,
                                        Origin = Anchor.Centre,
                                    },
                                }
                            },
                            topIcon = new CircularContainer
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.Centre,
                                Y = -icon_vertical_offset,
                                Size = new Vector2(22, 14),
                                Masking = true,
                                BorderThickness = 4,
                                BorderColour = Color4.White,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Alpha = 0,
                                        AlwaysPresent = true,
                                    },
                                },
                            }
                        }
                    },
                }
            };

            direction.BindTo(scrollingInfo.Direction);
            direction.BindValueChanged(onDirectionChanged, true);

            accentColour = column.AccentColour.GetBoundCopy();
            accentColour.BindValueChanged(colour =>
                {
                    background.Colour = colour.NewValue.Darken(1f);
                    bottomIcon.Colour = colour.NewValue;
                },
                true);
        }

        private void onDirectionChanged(ValueChangedEvent<ScrollingDirection> direction)
        {
            switch (direction.NewValue)
            {
                case ScrollingDirection.Up:
                    directionContainer.Scale = new Vector2(1, -1);
                    directionContainer.Anchor = Anchor.TopLeft;
                    directionContainer.Origin = Anchor.BottomLeft;
                    break;

                case ScrollingDirection.Down:
                    directionContainer.Scale = new Vector2(1, 1);
                    directionContainer.Anchor = Anchor.BottomLeft;
                    directionContainer.Origin = Anchor.BottomLeft;
                    break;
            }
        }

        public bool OnPressed(KeyBindingPressEvent<ManiaAction> e)
        {
            if (e.Action != column.Action.Value) return false;

            const double lighting_fade_in_duration = 50;
            Color4 lightingColour = accentColour.Value.Lighten(0.9f);

            background
                .FadeTo(1, 40).Then()
                .FadeTo(0.8f, 150, Easing.OutQuint);

            hitTargetLine.FadeColour(Color4.White, lighting_fade_in_duration, Easing.OutQuint);
            hitTargetLine.TransformTo(nameof(EdgeEffect), new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = lightingColour.Opacity(0.7f),
                Radius = 20,
            }, lighting_fade_in_duration, Easing.OutQuint);

            topIcon.ScaleTo(0.9f, lighting_fade_in_duration, Easing.OutQuint);
            topIcon.TransformTo(nameof(EdgeEffect), new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = lightingColour.Opacity(0.1f),
                Radius = 20,
            }, lighting_fade_in_duration, Easing.OutQuint);

            bottomIcon.FadeColour(Color4.White, lighting_fade_in_duration, Easing.OutQuint);

            foreach (var circle in bottomIcon)
            {
                circle.TransformTo(nameof(EdgeEffect), new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Glow,
                    Colour = lightingColour.Opacity(0.3f),
                    Radius = 60,
                }, lighting_fade_in_duration, Easing.OutQuint);
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<ManiaAction> e)
        {
            if (e.Action != column.Action.Value) return;

            const double lighting_fade_out_duration = 300;
            Color4 lightingColour = accentColour.Value.Lighten(0.9f).Opacity(0);

            background.FadeTo(0, lighting_fade_out_duration, Easing.OutQuint);

            topIcon.ScaleTo(1f, 200, Easing.OutQuint);
            topIcon.TransformTo(nameof(EdgeEffect), new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = lightingColour,
                Radius = 20,
            }, lighting_fade_out_duration, Easing.OutQuint);

            hitTargetLine.FadeColour(OsuColour.Gray(196 / 255f), lighting_fade_out_duration, Easing.OutQuint);
            hitTargetLine.TransformTo(nameof(EdgeEffect), new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = lightingColour,
                Radius = 25,
            }, lighting_fade_out_duration, Easing.OutQuint);

            bottomIcon.FadeColour(accentColour.Value, lighting_fade_out_duration, Easing.OutQuint);

            foreach (var circle in bottomIcon)
            {
                circle.TransformTo(nameof(EdgeEffect), new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Glow,
                    Colour = lightingColour,
                    Radius = 30,
                }, lighting_fade_out_duration, Easing.OutQuint);
            }
        }
    }
}