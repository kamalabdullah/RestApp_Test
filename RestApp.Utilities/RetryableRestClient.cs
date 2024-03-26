using RestApp.DataDomain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestApp.Utilities
{
    public class RetryableRestClient : IRestClient
    {
        private readonly IRestClient _innerRestClient;
        private readonly ILogger _logger;
        private readonly IConfigurationOptions _configurationOptions;
        private readonly int _retryCount;
        private readonly TimeSpan _initialDelay;
        

        public RetryableRestClient(IRestClient innerRestClient, ILogger logger, IConfigurationOptions configurationOptions)
        {
            _innerRestClient = innerRestClient ?? throw new ArgumentNullException(nameof(innerRestClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configurationOptions = configurationOptions?? throw new ArgumentNullException(nameof(configurationOptions));
            _retryCount = 3;
            if (_configurationOptions.AppConfiguration != null)
            {
                _retryCount = _configurationOptions.AppConfiguration.CountOfRequestRetry;
                _initialDelay = TimeSpan.FromSeconds(_configurationOptions.AppConfiguration.InitialDelayForRetryIntervalInSecond);
            }
        }

        public async Task<TModel> Get<TModel>(string url)
        {
            return await RetryAsync(() => _innerRestClient.Get<TModel>(url));
        }

        public async Task<TModel> Put<TModel>(string url, TModel model)
        {
            return await RetryAsync(() => _innerRestClient.Put(url, model));
        }

        public async Task<TModel> Post<TModel>(string url, TModel model)
        {
            return await RetryAsync(() => _innerRestClient.Post(url, model));
        }

        public async Task<TModel> Delete<TModel>(int id)
        {
            return await RetryAsync(() => _innerRestClient.Delete<TModel>(id));
        }

        private async Task<T> RetryAsync<T>(Func<Task<T>> operation)
        {
            TimeSpan currentDelay = _initialDelay;
            bool _exceptionLogged = false;
            for (int attempt = 0; attempt <= _retryCount; attempt++)
            {
                try
                {
                    return await operation();
                }
                // If the API support retry after header option 
                catch (WebException ex) when (ex.Response is HttpWebResponse response &&
                                                response.Headers["Retry-After"] != null)
                {
                    if (int.TryParse(response.Headers["Retry-After"], out int secondsToWait))
                    {
                        if (!_exceptionLogged)
                        {
                            _logger.Error(ex);
                            _exceptionLogged = true;
                        }
                        await Task.Delay(TimeSpan.FromSeconds(secondsToWait));
                    }
                    else
                    {
                        if (!_exceptionLogged)
                        {
                            _logger.Error(ex);
                            _exceptionLogged = true;
                        }
                        await Task.Delay(currentDelay);
                    }
                }
                catch (WebException ex)
                {
                    if (!_exceptionLogged)
                    {
                        _logger.Error(ex);
                        _exceptionLogged = true;
                    }
                    await Task.Delay(currentDelay);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    throw ex;
                }
                // Increase the delay for the next attempt
                currentDelay += _initialDelay;
            }
            return default;
        }
    }
}
