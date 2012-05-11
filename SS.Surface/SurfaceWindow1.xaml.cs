using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation.Controls;
using SS.Surface.Classes;
using SS.Surface.Projectiles;
using SS.Surface.ViewModels;

namespace SS.Surface
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1
    {
        private SurfaceWindowViewModel _vm;
        private static Dispatcher _dispatcher;
        private static Canvas _projectileField;
        private DispatcherTimer _gameTimer;
        private List<Projectile> _hitProjectiles;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            _dispatcher = Dispatcher.CurrentDispatcher;
            _hitProjectiles = new List<Projectile>();
            _projectileField = ProjectileField;
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            _gameTimer.Tick += GameTimeElapsed;
            _gameTimer.Start();

            _vm = new SurfaceWindowViewModel();
            DataContext = _vm;
        }

        void GameTimeElapsed(object sender, EventArgs eventArgs)
        {
            _hitProjectiles.Clear();

            foreach (var projectile in ProjectileField.Children.Cast<Projectile>())
            {
                var projectileX = Canvas.GetLeft(projectile) + (projectile.ActualWidth / 2);
                var projectileY = Canvas.GetTop(projectile) + (projectile.ActualHeight / 2);

                var elem = Utils.FindElementUnderPoint<ScatterViewItem>(PlayingField, new Point(projectileX, projectileY));

                if (elem != null)
                {
                    var svi = elem;
                    var projVM = (ProjectileViewModel)projectile.DataContext;
                    _vm.UserHit((User)svi.DataContext, projVM);
                    _hitProjectiles.Add(projectile);
                }
            }

            foreach (var hitProjectile in _hitProjectiles)
            {
                ProjectileField.Children.Remove(hitProjectile);
            }
        }

        public static void AddProjectile(int owner, Point origin, Vector direction, int damage, int projectileLife)
        {
            _dispatcher.BeginInvoke(new Action(
                () => _projectileField.AddProjectile(owner, origin, direction, damage, projectileLife)),
                DispatcherPriority.Send);
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

    }
}