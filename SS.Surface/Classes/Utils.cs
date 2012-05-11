using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Windows.Media.Imaging;

namespace SS.Surface.Classes
{
    public static class Utils
    {
        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T FindParent<T>(DependencyObject child)
          where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            UIElement parentUI = parentObject as UIElement;
            T parent = parentObject as T;
            if (parent != null)
            {
                if (parentUI != null)
                {
                    if (!(parentUI.Opacity == 0 || parentUI.Visibility != Visibility.Visible))
                    {
                        return parent;
                    }
                    else
                    {
                        return FindParent<T>(parentObject);
                    }
                }
                else
                {
                    return parent;
                }
            }
            else
            {
                //use recursion to proceed with next level
                return FindParent<T>(parentObject);
            }
        }

        /// <summary>
        /// Finds all children of type T for a control
        /// </summary>
        /// <typeparam name="T">DependencyObject</typeparam>
        /// <param name="depObj">The control whose children you want to find</param>
        /// <returns>An IEnumerable of type T</returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Do note, that for content element,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        public static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null) return null;
            ContentElement contentElement = child as ContentElement;

            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //if it's not a ContentElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

        /// <summary>
        /// Tries to locate a given item within the visual tree,
        /// starting with the dependency object at a given position. 
        /// </summary>
        /// <typeparam name="T">The type of the element to be found
        /// on the visual tree of the element at the given location.</typeparam>
        /// <param name="reference">The main element which is used to perform
        /// hit testing.</param>
        /// <param name="point">The position to be evaluated on the origin.</param>
        public static T FindElementUnderPoint<T>(UIElement reference, Point point)
          where T : DependencyObject
        {
            DependencyObject element = reference.InputHitTest(point) as DependencyObject;

            if (element == null) return null;
            else if (element is T) return (T)element;
            else return FindParent<T>(element);
        }

        public static BitmapImage ControlToBitmapImage(UIElement source, double scale)
        {
            double actualHeight = source.RenderSize.Height;
            double actualWidth = source.RenderSize.Width;

            double renderHeight = actualHeight * scale;
            double renderWidth = actualWidth * scale;

            if (actualHeight <= 0 || actualWidth <= 0)
                return null;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);

            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            BitmapImage bitmapImage = new BitmapImage();

            MemoryStream outputStream = new MemoryStream();
            pngEncoder.Save(outputStream);

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = outputStream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        /// <summary>
        /// Check if element that was clicked on is of specified parent type
        /// </summary>
        /// <typeparam name="T"> The type of element to be found.</typeparam>
        /// <param name="reference">The main element which is used to perform
        /// hit testing.</param>
        /// <param name="point">The position to be evaluated on the origin.</param>
        public static bool IsParentOfTypeFromPoint<T>(UIElement reference, Point point)
          where T : DependencyObject
        {
            return (FindElementUnderPoint<T>(reference, point) != null);
        }

        #region Methods to verify conditions (Require)

        /// <summary>
        ///     If <paramref name="truth"/> is false, throw an empty <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="truth">The 'truth' to evaluate.</param>
        [DebuggerStepThrough]
        public static void Require(bool truth)
        {
            if (!truth)
            {
                throw new InvalidOperationException(string.Empty);
            }
        }

        /// <summary>
        ///     If <paramref name="truth"/> is false, throw an 
        ///     <see cref="InvalidOperationException"/> with the provided <paramref name="message"/>.
        /// </summary>
        /// <param name="truth">The 'truth' to evaluate.</param>
        /// <param name="message">
        ///     The <see cref="Exception.Message"/> if 
        ///     <paramref name="truth"/> is false.
        /// </param>
        [DebuggerStepThrough]
        public static void Require(bool truth, string message)
        {
            RequireNotNullOrEmpty(message, "message");
            if (!truth)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        ///     If <paramref name="truth"/> is false, throws 
        ///     <paramref name="exception"/>.    
        /// </summary>
        /// <param name="truth">The 'truth' to evaluate.</param>
        /// <param name="exception">
        ///     The <see cref="Exception"/> to throw if <paramref name="truth"/> is false.
        /// </param>
        [DebuggerStepThrough]
        public static void Require(bool truth, Exception exception)
        {
            RequireNotNull(exception, "exception");
            if (!truth)
            {
                throw exception;
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if the
        ///     provided string is null.
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if the
        ///     provided string is empty.
        /// </summary>
        /// <param name="stringParameter">The object to test for null and empty.</param>
        /// <param name="parameterName">The string for the ArgumentException parameter, if thrown.</param>
        [DebuggerStepThrough]
        public static void RequireNotNullOrEmpty(string stringParameter, string parameterName)
        {
            if (stringParameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            else if (stringParameter.Length == 0)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if the
        ///     provided object is null.
        /// </summary>
        /// <param name="obj">The object to test for null.</param>
        /// <param name="parameterName">The string for the ArgumentNullException parameter, if thrown.</param>
        [DebuggerStepThrough]
        public static void RequireNotNull(object obj, string parameterName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentException"/> if the provided truth is false.
        /// </summary>
        /// <param name="truth">The value assumed to be true.</param>
        /// <param name="parameterName">The string for <see cref="ArgumentException"/>, if thrown.</param>
        [DebuggerStepThrough]
        public static void RequireArgument(bool truth, string parameterName)
        {
            Utils.RequireNotNullOrEmpty(parameterName, "parameterName");

            if (!truth)
            {
                throw new ArgumentException(parameterName);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentException"/> if the provided truth is false.
        /// </summary>
        /// <param name="truth">The value assumed to be true.</param>
        /// <param name="paramName">The paramName for the <see cref="ArgumentException"/>, if thrown.</param>
        /// <param name="message">The message for <see cref="ArgumentException"/>, if thrown.</param>
        [DebuggerStepThrough]
        public static void RequireArgument(bool truth, string paramName, string message)
        {
            Utils.RequireNotNullOrEmpty(paramName, "paramName");
            Utils.RequireNotNullOrEmpty(message, "message");

            if (!truth)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if the provided truth is false.
        /// </summary>
        /// <param name="truth">The value assumed to be true.</param>
        /// <param name="parameterName">The string for <see cref="ArgumentOutOfRangeException"/>, if thrown.</param>
        [DebuggerStepThrough]
        public static void RequireArgumentRange(bool truth, string parameterName)
        {
            Utils.RequireNotNullOrEmpty(parameterName, "parameterName");

            if (!truth)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if the provided truth is false.
        /// </summary>
        /// <param name="truth">The value assumed to be true.</param>
        /// <param name="paramName">The paramName for the <see cref="ArgumentOutOfRangeException"/>, if thrown.</param>
        /// <param name="message">The message for <see cref="ArgumentOutOfRangeException"/>, if thrown.</param>
        [DebuggerStepThrough]
        public static void RequireArgumentRange(bool truth, string paramName, string message)
        {
            Utils.RequireNotNullOrEmpty(paramName, "paramName");
            Utils.RequireNotNullOrEmpty(message, "message");

            if (!truth)
            {
                throw new ArgumentOutOfRangeException(message, paramName);
            }
        }

        #endregion

        /// <summary>
        ///     Wraps <see cref="Interlocked.CompareExchange{T}(ref T,T,T)"/> 
        ///     for atomically setting null fields.
        /// </summary>
        /// <typeparam name="T">The type of the field to set.</typeparam>
        /// <param name="location">
        ///     The field that, if null, will be set to <paramref name="objectToUse"/>.
        /// </param>
        /// <param name="value">
        ///     If <paramref name="location"/> is null, the object to set it to.
        /// </param>
        /// <returns>true if <paramref name="location"/> was null and has now been set; otherwise, false.</returns>
        public static bool InterlockedSetIfNotNull<T>(ref T location, T value) where T : class
        {
            Utils.RequireNotNull(value, "value");

            // Strictly speaking, this null check is not nessesary, but
            // while CompareExchange is fast, it's still much slower than a 
            // null check. 
            if (location == null)
            {
                // This is a paranoid method. In a multi-threaded environment, it's possible
                // for two threads to get through the null check before a value is set.
                // This makes sure than one and only one value is set to field.
                // This is super important if the field is used in locking, for instance.

                T valueWhenSet = Interlocked.CompareExchange<T>(ref location, value, null);
                return (valueWhenSet == null);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the provided <see cref="Exception"/> is considered 'critical'
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to evaluate for critical-ness.</param>
        /// <returns>true if the Exception is conisdered critical; otherwise, false.</returns>
        /// <remarks>
        /// These exceptions are consider critical:
        /// <list type="bullets">
        ///     <item><see cref="OutOfMemoryException"/></item>
        ///     <item><see cref="StackOverflowException"/></item>
        ///     <item><see cref="ThreadAbortException"/></item>
        ///     <item><see cref="SEHException"/></item>
        /// </list>
        /// </remarks>
        public static bool IsCriticalException(Exception exception)
        {
            Utils.RequireNotNull(exception, "exception");
            // Copied with respect from WPF WindowsBase->MS.Internal.CriticalExceptions.IsCriticalException
            // NullReferencException, SecurityException --> not going to consider these critical
            return exception is OutOfMemoryException ||
                    exception is StackOverflowException ||
                    exception is ThreadAbortException ||
                    exception is SEHException;

        } //*** static IsCriticalException

        public static ContentControl CloneContentControl(UIElement element)
        {
            ContentControl control = new ContentControl();

            ContentControl cc = element as ContentControl;
            if (cc != null)
            {
                control.Content = cc.Content;
                control.ContentTemplate = cc.ContentTemplate;
            }

            ContentPresenter cp = element as ContentPresenter;
            if (cp != null)
            {
                control.Content = cp.Content;
                control.ContentTemplate = cp.ContentTemplate;
            }

            return control;
        }

        public static UIElement CloneUIElement(UIElement src)
        {
            if (src != null)
            {
                string s = XamlWriter.Save(src);
                StringReader reader = new StringReader(s);
                XmlReader xmlReader = XmlTextReader.Create(reader, new XmlReaderSettings());

                return (UIElement)XamlReader.Load(xmlReader);
            }
            else
            {
                return null;
            }
        }

        public static UIElement LoadFromXaml(string project, string xamlFile)
        {
            Uri uri = new Uri("/" + project + ";component/" + xamlFile, UriKind.Relative);

            return (UIElement)Application.LoadComponent(uri);
        }

        public static T LoadXaml<T>(string xaml)
        {
            XmlReader xmlReader = XmlTextReader.Create(new StringReader(xaml), new XmlReaderSettings());

            return (T)XamlReader.Load(xmlReader);
        }

        public static UIElement LoadScreen(string screen)
        {
            return (UIElement)Application.LoadComponent(new Uri("/IQ.IQKiosk3D;component/Screens/" + screen, UriKind.Relative));
        }

        public static bool IsDesignMode { get { return DesignerProperties.GetIsInDesignMode(new DependencyObject()); } }

        public static bool IsAppInstanceAlreadyRunning()
        {
            var thisAppsName = Process.GetCurrentProcess().ProcessName;
            var processes = Process.GetProcessesByName(thisAppsName);

            // if there's another process with the same name running, wait 10 seconds and check again to see if it's gone (it might exit for the updater)
            if (processes.Length > 1)
            {
                Thread.Sleep(10000);
                return Process.GetProcessesByName(thisAppsName).Length > 1;
            }
            return false;
        }
    }
}
