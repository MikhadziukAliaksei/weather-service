using Moq;
using WeatherService.Application.Services;
using WeatherService.Domain.Interfaces;
using WeatherService.Domain.Weather;

namespace WeatherService.Tests;

[TestFixture]
public class WeatherTrackerTests
{
    private Mock<IDataClient<WeatherParameter, Weather>> _dataClientMock;
    private Mock<IDataProcessor> _dataProcessorMock;
    private Mock<IDataViewer> _dataViewerMock;
    private CancellationTokenSource _cancellationTokenSource;

    [SetUp]
    public void Setup()
    {
        _dataClientMock = new Mock<IDataClient<WeatherParameter, Weather>>();
        _dataProcessorMock = new Mock<IDataProcessor>();
        _dataViewerMock = new Mock<IDataViewer>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    [TearDown]
    public void Teardown()
    {
        _cancellationTokenSource.Dispose();
    }

    [Test]
    public async Task StartWeatherTrackingAsync_ShouldFetchWeatherData_WhenGivenListOfCities()
    {
        // Arrange
        var cities = new List<string> { "Seattle", "Portland" };
        var cancellationToken = _cancellationTokenSource.Token;
        var weatherTracker =
            new WeatherTracker(_dataClientMock.Object, _dataViewerMock.Object, _dataProcessorMock.Object);

        var seattleWeather = new Weather
            { City = "Seattle", Temperature = 60, Precipitation = -1, Summary = "hot", WindSpeed = 6 };
        var portlandWeather = new Weather
            { City = "Portland", Temperature = 55, Precipitation = -1, Summary = "hot", WindSpeed = 6 };

        _dataClientMock
            .Setup(x => x.GetDataAsync(It.IsAny<WeatherParameter>(), cancellationToken))
            .ReturnsAsync((WeatherParameter parameter, CancellationToken token) =>
            {
                if (parameter.CityName == "Seattle") return seattleWeather;
                if (parameter.CityName == "Portland") return portlandWeather;
                throw new ArgumentException($"Unexpected city name: {parameter.CityName}");
            });
        _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));

        // Act
        try
        {
            await weatherTracker.StartWeatherTrackingAsync(cities, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        
        // Assert
        _dataClientMock.Verify(x => x.GetDataAsync(It.IsAny<WeatherParameter>(), cancellationToken), Times.AtLeastOnce);
        _dataProcessorMock.Verify(x => x.StoreDataAsync(It.IsAny<IEnumerable<Weather>>()), Times.AtLeastOnce);
        _dataViewerMock.Verify(x => x.DisplayDataAsync(It.IsAny<IEnumerable<Weather>>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task StartWeatherTrackingAsync_ShouldStopFetchingWeatherData_WhenCancellationRequested()
    {
        // Arrange
        var cities = new List<string> { "Seattle", "Portland" };
        var cancellationToken = _cancellationTokenSource.Token;
        var weatherTracker =
            new WeatherTracker(_dataClientMock.Object, _dataViewerMock.Object, _dataProcessorMock.Object);

        var seattleWeather = new Weather
            { City = "Seattle", Temperature = 60, Precipitation = -1, Summary = "hot", WindSpeed = 6 };
        var portlandWeather = new Weather
            { City = "Portland", Temperature = 55, Precipitation = -1, Summary = "hot", WindSpeed = 6 };

        _dataClientMock
            .Setup(x => x.GetDataAsync(It.IsAny<WeatherParameter>(), cancellationToken))
            .ReturnsAsync((WeatherParameter parameter, CancellationToken token) =>
            {
                if (parameter.CityName == "Seattle") return seattleWeather;
                if (parameter.CityName == "Portland") return portlandWeather;
                throw new ArgumentException($"Unexpected city name: {parameter.CityName}");
            });

        // Act
        var trackingTask = weatherTracker.StartWeatherTrackingAsync(cities, cancellationToken);
        _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));

        Assert.ThrowsAsync<OperationCanceledException>(async () => { await trackingTask; });


        // Assert
        _dataClientMock.Verify(x => x.GetDataAsync(It.IsAny<WeatherParameter>(), cancellationToken), Times.AtLeastOnce);
        _dataProcessorMock.Verify(x => x.StoreDataAsync(It.IsAny<IEnumerable<Weather>>()), Times.AtLeastOnce);
        _dataViewerMock.Verify(x => x.DisplayDataAsync(It.IsAny<IEnumerable<Weather>>()), Times.AtLeastOnce);
    }
    
    [Test]
    public async Task StartWeatherTrackingAsync_ShouldNotFetchWeatherData_WhenCancellationRequestedImmediately()
    {
        // Arrange
        var weatherTracker =
            new WeatherTracker(_dataClientMock.Object, _dataViewerMock.Object, _dataProcessorMock.Object);
        var cts = new CancellationTokenSource();

        // Act
        try
        {
            cts.Cancel();
            await weatherTracker.StartWeatherTrackingAsync(new List<string>(), cts.Token);
        }
        catch (TaskCanceledException)
        {
            //Do nothing
        }
        

        // Assert
        _dataClientMock.Verify(x => x.GetDataAsync(It.IsAny<WeatherParameter>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _dataProcessorMock.Verify(x => x.StoreDataAsync(It.IsAny<IEnumerable<Weather>>()), Times.Never);
        _dataViewerMock.Verify(x => x.DisplayDataAsync(It.IsAny<IEnumerable<Weather>>()), Times.Never);
    }
}