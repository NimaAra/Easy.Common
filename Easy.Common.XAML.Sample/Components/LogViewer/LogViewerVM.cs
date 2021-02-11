namespace Easy.Common.XAML.Sample.Components.LogViewer
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Easy.Common.Extensions;
    using Easy.Logger.Interfaces;

    public sealed class LogViewerVM : BindableBase
    {
        private readonly IEasyLogger _logger;
        private readonly IUDPListener<string> _logListener;
        private readonly SynchronizationContext _ctx;
        private CancellationTokenSource _cts;

        #region DataBinding

        private ObservableCollection<string> _entries;
        public ObservableCollection<string> Entries
        {
            get => _entries;
            set => SetField(ref _entries, value);
        }

        public ICommand StartListeningCommand => new CustomCommand(StartListener);
        public ICommand StopListeningCommand => new CustomCommand(StopListener);

        #endregion

        public LogViewerVM(ILogService logService, IUDPListener<string> logListener)
        {
            _logger = logService.GetLogger(GetType());
            _logListener = logListener;
            _ctx = SynchronizationContext.Current;
            _logListener.OnData += OnNewLogEntry;

            Init();
        }

        private void Init()
        {
            Entries = new ObservableCollection<string>();
            GenerateLogs();
        }

        private async void GenerateLogs()
        {
            while (true)
            {
                await Task.Delay(1.Seconds());
                _logger.Info("foo bar");
            }
        }

        private void OnNewLogEntry(object sender, string logMessage)
        {
            if (_cts.IsCancellationRequested) { return; }
            
            _ctx.Post(msg => Entries.Add((string)msg), logMessage.TrimEnd('\r', '\n'));
        }

        private void StartListener()
        {
            _entries.Clear();
            StopListener();

            _cts = new CancellationTokenSource();
            _logListener.Start(_cts.Token);
        }

        private void StopListener()
        {
            _cts?.Cancel();
        }
    }

    public interface IUDPListener<T>
    {
        event EventHandler<T> OnData;
        Task Start(CancellationToken cToken);
    }

    public sealed class UDPListener<T> : IUDPListener<T>
    {
        private readonly UdpClient _client;
        private readonly Func<byte[], T> _payloadConverter;
        private IPEndPoint _sender;

        public UDPListener(IPEndPoint endpoint, Func<byte[], T> payloadConverter)
        {
            _client = new UdpClient(1234);
            _payloadConverter = payloadConverter;
            _sender = new IPEndPoint(IPAddress.Loopback, 0);
        }

        public event EventHandler<T> OnData;

        public Task Start(CancellationToken cToken)
        {
            return Task.Factory.StartNew(() =>
            {
                while (!cToken.IsCancellationRequested)
                {
                    var result = _client.Receive(ref _sender);
                    HandleData(result);
                }
            }, cToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void HandleData(byte[] payload)
        {
            OnData?.Invoke(this, _payloadConverter(payload));
        }
    }
}