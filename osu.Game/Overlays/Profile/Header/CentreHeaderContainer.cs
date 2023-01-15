﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays.Profile.Header.Components;
using osuTK;

namespace osu.Game.Overlays.Profile.Header
{
    public partial class CentreHeaderContainer : CompositeDrawable
    {
        public readonly Bindable<UserProfileData?> User = new Bindable<UserProfileData?>();

        public CentreHeaderContainer()
        {
            Height = 60;
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background3
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Padding = new MarginPadding { Vertical = 10 },
                    Margin = new MarginPadding { Left = UserProfileOverlay.CONTENT_X_MARGIN },
                    Spacing = new Vector2(10, 0),
                    Children = new Drawable[]
                    {
                        new FollowersButton
                        {
                            User = { BindTarget = User }
                        },
                        new MappingSubscribersButton
                        {
                            User = { BindTarget = User }
                        },
                        new MessageUserButton
                        {
                            User = { BindTarget = User }
                        },
                    }
                },
                new Container
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    AutoSizeAxes = Axes.Both,
                    Margin = new MarginPadding { Right = UserProfileOverlay.CONTENT_X_MARGIN },
                    Children = new Drawable[]
                    {
                        new LevelBadge
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Size = new Vector2(40),
                            User = { BindTarget = User }
                        },
                        new Container
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Width = 200,
                            Height = 6,
                            Margin = new MarginPadding { Right = 50 },
                            Child = new LevelProgressBar
                            {
                                RelativeSizeAxes = Axes.Both,
                                User = { BindTarget = User }
                            }
                        },
                    }
                }
            };
        }
    }
}
