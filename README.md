# RetryableRestClient Class Documentation

## Overview

The `RetryableRestClient` class in the `RestApp.Utilities` project provides a wrapper around an existing `IRestClient` implementation with retry logic built-in. It retries requests that result in `System.Net.WebException` errors, up to a configurable number of times, with a configurable delay between retries.

## Features

- Retries failed requests automatically for a configurable number of times.
- Supports customizing the delay between retry attempts.
- Logs exceptions thrown during retries.
- Prevents excessive logging of exceptions by logging only once per request.
- Enables retry logic without modifying existing classes that use `IRestClient`.

## Usage

### Initialization

To use the `RetryableRestClient`, you need to inject in dependency injection of `IRestClient` and an implementation of `ILogger`. and `IConfigurationOptions` for retrieving app settings:

```csharp
var retryableRestClient = new RetryableRestClient(innerRestClient, logger, configurationOptions);
```
### Making Requests

You can use the `RetryableRestClient` to make requests just like you would with any other IRestClient implementation. For example:
```csharp
var result = await retryableRestClient.Get<string>("https://api.example.com/data");
```
### Customization
- To change the number of retry attempts, adjust the `CountOfRequestRetry` parameter in `appsettings.json` file.
- To customize the delay between retries, adjust the `InitialDelayForRetryIntervalInSecond` parameter in `appsettings.json` file.
### Unit Tests
The `RetryableRestClient` class is thoroughly tested using unit tests. The unit tests cover various scenarios, including:

- Successful request.
- Returns Default after all retry fails.
- Retry due to System.Net.WebException.
- Failure when all retry attempts are exhausted.
- Logging behavior during retries.
- Return success data after retry.
- Fast failed if not System.Net.WebException exception.
  
To run the unit tests, execute the test project under the tests folder that is included in the solution. The tests use a mocking framework (such as Moq) to mock dependencies and verify the behavior of the `RetryableRestClient` class.
### Dependencies
- .NET Core or .NET Framework
- Third-party libraries for HTTP requests (not implemented in-house)
### License
The `RetryableRestClient` class is licensed under the MIT License.
