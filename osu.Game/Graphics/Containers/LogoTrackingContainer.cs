// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.MathUtils;
using osu.Game.Screens.Menu;
using osuTK;

namespace osu.Game.Graphics.Containers
{
    /// <summary>
    /// A container that handles tracking of an <see cref="OsuLogo"/> through different layout scenarios.
    /// </summary>
    public class LogoTrackingContainer : Container
    {
        public Facade LogoFacade { get; }

        protected OsuLogo Logo => logo;

        private OsuLogo logo;
        private Easing easing;
        private Vector2? startPosition;
        private double? startTime;
        private double duration;
        private bool tracking;

        public LogoTrackingContainer()
        {
            LogoFacade = new ExposedFacade();
        }

        /// <summary>
        /// Assign the logo that should track the Facade's position, as well as how it should transform to its initial position.
        /// </summary>
        /// <param name="logo">The instance of the logo to be used for tracking.</param>
        /// <param name="facadeScale">The scale of the facade. Does not actually affect the logo itself.</param>
        /// <param name="duration">The duration of the initial transform. Default is instant.</param>
        /// <param name="easing">The easing type of the initial transform.</param>
        public void StartTracking(OsuLogo logo, double duration = 0, Easing easing = Easing.None)
        {
            if (logo == null)
                throw new ArgumentNullException(nameof(logo));

            if (logo.IsTracking && tracking == false)
                throw new InvalidOperationException($"Cannot track an instance of {typeof(OsuLogo)} to multiple {typeof(LogoTrackingContainer)}s");

            if (this.logo != logo && this.logo != null)
            {
                // If we're replacing the logo to be tracked, the old one no longer has a tracking container
                this.logo.IsTracking = false;
            }

            this.logo = logo;
            this.logo.IsTracking = true;

            this.duration = duration;
            this.easing = easing;

            startTime = null;
            startPosition = null;

            tracking = true;
        }

        public void StopTracking()
        {
            if (logo != null)
                logo.IsTracking = false;

            tracking = false;
        }

        /// <summary>
        /// Gets the position that the logo should move to with respect to the <see cref="LogoFacade"/>.
        /// Manually performs a conversion of the Facade's position to the Logo's parent's relative space.
        /// </summary>
        /// <remarks>Will only be correct if the logo's <see cref="Drawable.RelativePositionAxes"/> are set to Axes.Both</remarks>
        protected Vector2 ComputeLogoTrackingPosition()
        {
            var absolutePos = Logo.Parent.ToLocalSpace(LogoFacade.ScreenSpaceDrawQuad.Centre);

            return new Vector2(absolutePos.X / Logo.Parent.RelativeToAbsoluteFactor.X,
                absolutePos.Y / Logo.Parent.RelativeToAbsoluteFactor.Y);
        }

        protected override void Update()
        {
            base.Update();

            if (Logo == null || !tracking)
                return;

            // Account for the scale of the actual OsuLogo, as SizeForFlow only accounts for the sprite scale.
            ((ExposedFacade)LogoFacade).SetSize(new Vector2(Logo.SizeForFlow * Logo.Scale.X));

            var localPos = ComputeLogoTrackingPosition();

            if (LogoFacade.Parent != null && Logo.Position != localPos && Logo.RelativePositionAxes == Axes.Both)
            {
                // If this is our first update since tracking has started, initialize our starting values for interpolation
                if (startTime == null || startPosition == null)
                {
                    startTime = Time.Current;
                    startPosition = Logo.Position;
                }

                if (duration != 0)
                {
                    double elapsedDuration = (double)(Time.Current - startTime);

                    var amount = (float)Interpolation.ApplyEasing(easing, Math.Min(elapsedDuration / duration, 1));

                    // Interpolate the position of the logo, where amount 0 is where the logo was when it first began interpolating, and amount 1 is the target location.
                    Logo.Position = Vector2.Lerp(startPosition.Value, localPos, amount);
                }
                else
                {
                    Logo.Position = localPos;
                }
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (Logo != null)
                Logo.IsTracking = false;

            base.Dispose(isDisposing);
        }

        private class ExposedFacade : Facade
        {
            public void SetSize(Vector2 size)
            {
                base.SetSize(size);
            }
        }

        /// <summary>
        /// A dummy object used to denote another object's location.
        /// </summary>
        public abstract class Facade : Drawable
        {
            public override Vector2 Size
            {
                get => base.Size;
                set => throw new InvalidOperationException($"Cannot set the Size of a {typeof(Facade)} outside of a {typeof(LogoTrackingContainer)}");
            }

            protected void SetSize(Vector2 size)
            {
                base.Size = size;
            }
        }
    }
}
