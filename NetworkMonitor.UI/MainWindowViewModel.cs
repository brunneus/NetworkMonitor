﻿using NetworkMonitor.Engine;
using NetworkMonitor.UI.ViewModels;
using NetworkMonitor.UI.Views;
using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.UI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private BackgroundWorker _worker = new BackgroundWorker();
        private List<Package> _originalPackages = new List<Package>();

        public MainWindowViewModel()
        {
            LoadDevices();
            FiltredGeneratedPackages = new ObservableCollection<Package>();

            _worker.DoWork += (s, e) => PackagesMonitor.Instance.BeginCapture(_worker, _selectedDevice);
            _worker.WorkerSupportsCancellation = true;
        }

        private void LoadDevices()
        {
            DetectedDevices = new List<ICaptureDevice>();

            var devices = CaptureDeviceList.Instance;

            foreach (var device in devices)
                DetectedDevices.Add(device);

            SelectedDevice = DetectedDevices.FirstOrDefault();
        }

        public List<ICaptureDevice> DetectedDevices { get; set; }

        private ICaptureDevice _selectedDevice;
        public ICaptureDevice SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedDevice"));
            }
        }

        private RelayCommand _beginCaptureCommand;
        public RelayCommand BeginCaptureCommand
        {
            get
            {
                return _beginCaptureCommand ?? (_beginCaptureCommand = new RelayCommand(BeginCapture));
            }
        }

        private RelayCommand _stopCaptureCommand;
        public RelayCommand StopCaptureCommand
        {
            get
            {
                return _stopCaptureCommand ?? (_stopCaptureCommand = new RelayCommand(StopCapture));
            }
        }

        private RelayCommand _showTCPPackageAnalysisCommand;
        public RelayCommand ShowTCPPackageAnalysisCommand
        {
            get
            {
                return _showTCPPackageAnalysisCommand ?? (_showTCPPackageAnalysisCommand = new RelayCommand(ShowTCPPackageAnalysis));
            }
        }

        private RelayCommand _clearCapturedPackagesCommand;
        public RelayCommand ClearCapturedPackagesCommand
        {
            get
            {
                if (_clearCapturedPackagesCommand == null)
                    _clearCapturedPackagesCommand = new RelayCommand(() =>
                    {
                        _originalPackages.Clear();
                        this.FiltredGeneratedPackages.Clear();
                    });

                return _clearCapturedPackagesCommand;
            }
        }

        private bool _showOnlyTCPPackages;
        public bool ShowOnlyTCPPackages
        {
            get
            {
                return _showOnlyTCPPackages;
            }
            set
            {
                _showOnlyTCPPackages = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ShowOnlyTCPPackages"));
                this.UpdateFiltredGeneratedPackages();
            }
        }

        private bool _capturingNetworkTraffic;
        public bool CapturingNetworkTraffic
        {
            get
            {
                return _capturingNetworkTraffic;
            }
            set
            {
                _capturingNetworkTraffic = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CapturingNetworkTraffic"));
            }
        }

        private Package _selectedPackage;
        public Package SelectedPackage
        {
            get
            {
                return _selectedPackage;
            }
            set
            {
                _selectedPackage = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedPackage"));
            }
        }

        public ObservableCollection<Package> FiltredGeneratedPackages { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void BeginCapture()
        {
            this.CapturingNetworkTraffic = true;
            PackagesMonitor.Instance.PackageGenerated += PackagesMonitor_PackageGenerated;
            _worker.RunWorkerAsync();
        }

        private void StopCapture()
        {
            this.CapturingNetworkTraffic = false;
            _worker.CancelAsync();
            PackagesMonitor.Instance.PackageGenerated -= PackagesMonitor_PackageGenerated;
        }

        private void PackagesMonitor_PackageGenerated(PackageGeneratedArgs generatedPackageArgs)
        {
            FiltredGeneratedPackages.AddOnUI(generatedPackageArgs.GeneratedPackage);
            _originalPackages.Add(generatedPackageArgs.GeneratedPackage);
        }

        private void UpdateFiltredGeneratedPackages()
        {
            FiltredGeneratedPackages.Clear();

            if (_showOnlyTCPPackages)
            {
                foreach (var tcpPackage in _originalPackages.Where(p => p.Protocol == IPProtocolType.TCP))
                    FiltredGeneratedPackages.Add(tcpPackage);
            }
            else
            {
                foreach (var package in _originalPackages)
                    FiltredGeneratedPackages.Add(package);
            }
        }

        private void ShowTCPPackageAnalysis()
        {
            var vm = new TCPConnectionAnalysisViewModel(_originalPackages.OfType<TCPPackage>(), this.SelectedPackage as TCPPackage);
            var view = new TCPConnectionAnalysisView() { DataContext = vm };
            view.ShowDialog();
        }

        private bool CanShowTCPPackageAnalysis()
        {
            return this.SelectedPackage.Protocol == IPProtocolType.TCP;
        }
    }
}
