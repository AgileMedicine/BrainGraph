﻿using BrainGraph.WinStore.Common;
using BrainGraph.WinStore.Common.Viz;
using BrainGraph.WinStore.Events;
using BrainGraph.WinStore.Models;
using BrainGraph.WinStore.Services;
using Caliburn.Micro;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace BrainGraph.WinStore.Screens.Sources
{
	public class RegionsViewModel : ViewModelBase
	{
		#region Private Service Vars
		private IEventAggregator _eventAggregator;
		private INavigationService _navService;
		private IRegionService _regionService;
		#endregion

		private const string SETTING_ROI_FILE_TOKEN = "ROIFileToken";

		public RegionsViewModel()
		{
			Regions = new BindableCollection<RegionViewModel>();

			Title = "Regions";
			PrimaryValue = "0";
			Subtitle = "Brain areas of interest defined in 3D.  Usually based on an imaging atlas.";
			IsReady = true;

			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				_eventAggregator = IoC.Get<IEventAggregator>();
				_navService = IoC.Get<INavigationService>();
				_regionService = IoC.Get<IRegionService>();
				_eventAggregator.Subscribe(this);

				Task.Factory.StartNew(async () =>
				{
					await OpenFileFromCache();

				}, new System.Threading.CancellationToken(), TaskCreationOptions.None, TaskScheduler.Default);
			}

			#region Sample Data
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2000, Index = 1, Name = "Region One", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2001, Index = 2, Name = "Region Two", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2002, Index = 3, Name = "Region Three", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2003, Index = 4, Name = "Region Four", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2004, Index = 5, Name = "Region Five", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2005, Index = 6, Name = "Region Six", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2006, Index = 7, Name = "Region Seven", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2007, Index = 8, Name = "Region Eight", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2008, Index = 9, Name = "Region Nine", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2009, Index = 10, Name = "Region Ten", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2010, Index = 51, Name = "Region Eleven", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2000, Index = 1, Name = "Region One", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2001, Index = 2, Name = "Region Two", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2002, Index = 3, Name = "Region Three", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2003, Index = 4, Name = "Region Four", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2004, Index = 5, Name = "Region Five", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2005, Index = 6, Name = "Region Six", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2006, Index = 7, Name = "Region Seven", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2007, Index = 8, Name = "Region Eight", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2008, Index = 9, Name = "Region Nine", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2009, Index = 10, Name = "Region Ten", Special = false, X = 10, Y = 10, Z = 10 } });
				Regions.Add(new RegionViewModel { Region = new ROI { Ident = 2010, Index = 51, Name = "Region Eleven", Special = false, X = 10, Y = 10, Z = 10 } });
			}
			#endregion
		}

		public async Task OpenFileFromCache()
		{
			Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
			if (roamingSettings.Values.ContainsKey(SETTING_ROI_FILE_TOKEN))
			{
				var token = roamingSettings.Values[SETTING_ROI_FILE_TOKEN] as string;
				StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
				LoadRegionFile(file);
			}
		}

		public async void OpenFileFromPicker(RoutedEventArgs args)
		{
			FileOpenPicker wkPicker = new FileOpenPicker();
			wkPicker.ViewMode = PickerViewMode.List;
			wkPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			wkPicker.FileTypeFilter.Add(".txt");

			var regionFile = await wkPicker.PickSingleFileAsync();
			if (regionFile != null)
			{
				var token = StorageApplicationPermissions.FutureAccessList.Add(regionFile);

				Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
				roamingSettings.Values[SETTING_ROI_FILE_TOKEN] = token;

				LoadRegionFile(regionFile);
			}
		}

		public async void LoadRegionFile(StorageFile file)
		{
			Regions.Clear();

			var regions = await _regionService.Load(file);

			foreach (var region in regions)
				Regions.Add(new RegionViewModel() { Region = region });

			this.PrimaryValue = regions.Count.ToString();

			//AXPlotModel = LoadPlotModel(r => r.X, r => r.Y);
			//SGPlotModel = LoadPlotModel(r => (100 - r.Y), r => r.Z);
			//CRPlotModel = LoadPlotModel(r => r.X, r => r.Z);
		}

		protected PlotModel LoadPlotModel(Func<ROI, double> horizSelector, Func<ROI, double> vertSelector)
		{
			var model = new PlotModel() { IsLegendVisible = false };
			model.PlotMargins = new OxyThickness(0, 0, 0, 0);
			model.PlotAreaBorderColor = OxyColors.White;
			model.PlotType = PlotType.Cartesian;

			var ba = new LinearAxis(AxisPosition.Bottom) { AxislineColor = OxyColors.White, TextColor = OxyColors.White, MajorGridlineColor = OxyColors.White, TicklineColor = OxyColors.White };
			var la = new LinearAxis(AxisPosition.Left) { AxislineColor = OxyColors.White, TextColor = OxyColors.White, TicklineColor = OxyColors.White };

			ba.MinimumPadding = 0.1;
			ba.MaximumPadding = 0.1;
			la.MinimumPadding = 0.1;
			la.MaximumPadding = 0.1;

			model.Axes.Add(ba);
			model.Axes.Add(la);

			var s1 = new BrainScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 7,
				MarkerFill = OxyColor.FromAColor(125, OxyColors.White),
			};

			foreach (var rvm in Regions)
			{
				s1.Points.Add(new BrainDataPoint(horizSelector(rvm.Region), vertSelector(rvm.Region), rvm.Region));
			}

			model.Series.Add(s1);

			return model;
		}

		public override Type ViewModelType { get { return typeof(RegionsViewModel); } }
		
		public BindableCollection<RegionViewModel> Regions { get; private set; }

		public PlotModel SGPlotModel { get { return _inlSGPlotModel; } set { _inlSGPlotModel = value; NotifyOfPropertyChange(() => SGPlotModel); } } private PlotModel _inlSGPlotModel;
		public PlotModel AXPlotModel { get { return _inlAXPlotModel; } set { _inlAXPlotModel = value; NotifyOfPropertyChange(() => AXPlotModel); } } private PlotModel _inlAXPlotModel;
		public PlotModel CRPlotModel { get { return _inlCRPlotModel; } set { _inlCRPlotModel = value; NotifyOfPropertyChange(() => CRPlotModel); } } private PlotModel _inlCRPlotModel;
	}
}
