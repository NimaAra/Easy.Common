namespace Easy.Common.XAML.Sample
{
    using System;
    using System.Net;
    using System.Text;
    using Easy.Common.Interfaces;
    using Easy.Common.XAML.Sample.Components.LogViewer;
    using Easy.Common.XAML.Sample.Components.Spinners;
    using Easy.Logger;
    using Easy.MessageHub;
    using SimpleInjector;

    public sealed class VMLocator
    {
        private readonly Container _container;

        public VMLocator()
        {
            _container = ConfigureContainer();
        }

        public MainVM MainVM => _container.GetInstance<MainVM>();
        public SpinnersVM SpinnersVM => _container.GetInstance<SpinnersVM>();
        public LogViewerVM LogViewerVM => _container.GetInstance<LogViewerVM>();

        private static Container ConfigureContainer()
        {
            var result = new Container();

            var lifeStyle = Lifestyle.Singleton;

            result.Register<IConfigReader>(() => new ConfigReader(), lifeStyle);
            result.Register<ILogService>(() => Log4NetService.Instance, lifeStyle);
            result.Register<IMessageHub>(() => MessageHub.Instance, lifeStyle);

            result.Register<IUDPListener<string>>(() =>
            {
                var configReader = result.GetInstance<IConfigReader>();
                configReader.TryRead("listening-port", out int listeningPort);
                
                var endpoint = new IPEndPoint(IPAddress.Loopback, 0);
                string LogPayloadConverter(byte[] payload) => Encoding.UTF8.GetString(payload);
                return new UDPListener<string>(endpoint, LogPayloadConverter);
            }, lifeStyle);

            #region ViewModels

            result.Register<MainVM>(lifeStyle);
            result.Register<SpinnersVM>(lifeStyle);
            result.Register<LogViewerVM>(lifeStyle);

            #endregion

            result.Verify();
            return result;
        }
    }
}