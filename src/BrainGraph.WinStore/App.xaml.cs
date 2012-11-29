﻿using BrainGraph.WinStore.Screens.Edge;
using BrainGraph.WinStore.Screens.Experiment;
using BrainGraph.WinStore.Screens.Nodal;
using BrainGraph.WinStore.Screens.Selection;
using BrainGraph.WinStore.Screens.Sources;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using Ninject;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace BrainGraph.WinStore
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App
	{
		private IKernel _kernel;
		private WinRTContainer container;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();

			Application.Current.DebugSettings.IsBindingTracingEnabled = true;
			//Application.Current.DebugSettings.IsOverdrawHeatMapEnabled = true;
			Application.Current.DebugSettings.BindingFailed += DebugSettings_BindingFailed;
		}

		void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
		{
			int i = 0;
			string str = e.Message;
		}

		protected override void Configure()
		{
			base.Configure();

			container = new WinRTContainer(RootFrame);
			container.RegisterWinRTServices();

			_kernel = new StandardKernel();
			
			_kernel.Bind<INavigationService>().To<FrameAdapter>().InSingletonScope().WithConstructorArgument("frame", RootFrame);
			
			_kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
			_kernel.Bind<IRegionService>().To<RegionService>().InSingletonScope();
			_kernel.Bind<ISubjectDataService>().To<SubjectDataService>().InSingletonScope();
			_kernel.Bind<ISubjectFilterService>().To<SubjectFilterService>().InSingletonScope();
			_kernel.Bind<IComputeService>().To<ComputeService>().InSingletonScope();

			_kernel.Bind<MainMenuViewModel>().To<MainMenuViewModel>().InSingletonScope();
			
			_kernel.Bind<RegionsViewModel>().To<RegionsViewModel>().InSingletonScope();
			_kernel.Bind<SubjectsViewModel>().To<SubjectsViewModel>().InSingletonScope();
			_kernel.Bind<PermutationViewModel>().To<PermutationViewModel>().InSingletonScope();
			
			_kernel.Bind<RunExperimentViewModel>().To<RunExperimentViewModel>().InSingletonScope();

			_kernel.Bind<NodalStrengthViewModel>().To<NodalStrengthViewModel>().InSingletonScope();

			_kernel.Bind<EdgeSignificanceViewModel>().To<EdgeSignificanceViewModel>().InSingletonScope();
		}

		protected override object GetInstance(Type service, string key)
		{
			return _kernel.Get(service, key);
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return _kernel.GetAll(service);
		}

		protected override void BuildUp(object instance)
		{
			container.BuildUp(instance);
		}

		protected override Type GetDefaultView()
		{
			return typeof(MainMenuView);
		}

		///// <summary>
		///// Invoked when the application is launched normally by the end user.  Other entry points
		///// will be used when the application is launched to open a specific file, to display
		///// search results, and so forth.
		///// </summary>
		///// <param name="args">Details about the launch request and process.</param>
		//protected override void OnLaunched(LaunchActivatedEventArgs args)
		//{
		//	Frame rootFrame = Window.Current.Content as Frame;

		//	// Do not repeat app initialization when the Window already has content,
		//	// just ensure that the window is active
		//	if (rootFrame == null)
		//	{
		//		// Create a Frame to act as the navigation context and navigate to the first page
		//		rootFrame = new Frame();

		//		if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
		//		{
		//			//TODO: Load state from previously suspended application
		//		}

		//		// Place the frame in the current Window
		//		Window.Current.Content = rootFrame;
		//	}

		//	if (rootFrame.Content == null)
		//	{
		//		// When the navigation stack isn't restored navigate to the first page,
		//		// configuring the new page by passing required information as a navigation
		//		// parameter
		//		if (!rootFrame.Navigate(typeof(MainMenu), args.Arguments))
		//		{
		//			throw new Exception("Failed to create initial page");
		//		}
		//	}
		//	// Ensure the current window is active
		//	Window.Current.Activate();
		//}

		///// <summary>
		///// Invoked when application execution is being suspended.  Application state is saved
		///// without knowing whether the application will be terminated or resumed with the contents
		///// of memory still intact.
		///// </summary>
		///// <param name="sender">The source of the suspend request.</param>
		///// <param name="e">Details about the suspend request.</param>
		//private void OnSuspending(object sender, SuspendingEventArgs e)
		//{
		//	var deferral = e.SuspendingOperation.GetDeferral();
		//	//TODO: Save application state and stop any background activity
		//	deferral.Complete();
		//}
	}
}
