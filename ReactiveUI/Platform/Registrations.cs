﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reactive.Concurrency;

#if UIKIT
using MonoTouch.UIKit;
using NSApplication = MonoTouch.UIKit.UIApplication;
#endif 

#if COCOA && !UIKIT
using MonoMac.AppKit;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// Ignore me. This class is a secret handshake between RxUI and RxUI.Xaml
    /// in order to register certain classes on startup that would be difficult
    /// to register otherwise.
    /// </summary>
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));

#if !WINRT && !WP8 && !WP81
            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
#endif

#if !MONO
            registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            registerFunction(() => new DependencyObjectObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new XamlDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new BooleanToVisibilityTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
#endif

#if ANDROID
            registerFunction(() => new AndroidDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new AndroidObservableForWidgets(), typeof(ICreatesObservableForProperty));
            registerFunction(() => AndroidCommandBinders.Instance.Value, typeof(ICreatesCommandBinding));
#endif

#if UIKIT
            registerFunction(() => UIKitObservableForProperty.Instance.Value, typeof(ICreatesObservableForProperty));
            registerFunction(() => UIKitCommandBinders.Instance.Value, typeof(ICreatesCommandBinding));
            registerFunction(() => DateTimeNSDateConverter.Instance.Value, typeof(IBindingTypeConverter));
#endif

#if COCOA
            registerFunction(() => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new CocoaDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
#endif

#if COCOA && !UIKIT
            registerFunction(() => new TargetActionCommandBinder(), typeof(ICreatesCommandBinding));
#endif

            RxApp.TaskpoolScheduler = System.Reactive.Concurrency.TaskPoolScheduler.Default;

#if COCOA
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler());
#endif

#if !MONO && !WINRT
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => DispatcherScheduler.Current);
#endif

#if WINRT
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => CoreDispatcherScheduler.Current);
#endif

#if WP8
            registerFunction(() => new PhoneServiceStateDriver(), typeof (ISuspensionDriver));
#elif WINRT
            registerFunction(() => new WinRTAppDataDriver(), typeof(ISuspensionDriver));
#elif UIKIT
            registerFunction(() => new AppSupportJsonSuspensionDriver(), typeof(ISuspensionDriver));
#elif ANDROID
            registerFunction(() => new BundleSuspensionDriver(), typeof(ISuspensionDriver));
#endif
        }
    }
}