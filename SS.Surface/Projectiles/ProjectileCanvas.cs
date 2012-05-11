using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using SS.Surface.ViewModels;

namespace SS.Surface.Projectiles
{
    public static class ProjectileCanvas
    {
        public static void AddProjectile(this Canvas canvas, int owner, Point origin, Vector direction, int damage, int projectileLife)
        {
            var projectile = new Projectile();
            var vm = new ProjectileViewModel();

            vm.Owner = owner;
            vm.Damage = damage;

            projectile.DataContext = vm;
            var muzzle = direction;
            muzzle.Normalize();
            origin = origin + (muzzle * 50);
            Canvas.SetLeft(projectile, origin.X);
            Canvas.SetTop(projectile, origin.Y);
            canvas.Children.Add(projectile);
            direction = direction * (projectileLife / 20.0);
            var endPoint = new Point(
                origin.X + direction.X, 
                origin.Y + direction.Y);
            DoubleAnimation animX = new DoubleAnimation(endPoint.X, TimeSpan.FromMilliseconds(projectileLife));
            DoubleAnimation animY = new DoubleAnimation(endPoint.Y, TimeSpan.FromMilliseconds(projectileLife));
            animX.Completed += (sender, args) => canvas.Children.Remove(projectile);
            projectile.BeginAnimation(Canvas.LeftProperty, animX);
            projectile.BeginAnimation(Canvas.TopProperty, animY);
        }
    }
}
