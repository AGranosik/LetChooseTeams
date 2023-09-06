using Polly;
using Polly.Retry;

namespace LCT.Core.RetryPolicies
{
    public static class ConnectionPolicy
    {
        public static AsyncRetryPolicy AsyncRetry
            => Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

        public static AsyncRetryPolicy AsyncRetryForever 
            => Policy.Handle<Exception>()
                .WaitAndRetryForeverAsync(retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
    }
}
