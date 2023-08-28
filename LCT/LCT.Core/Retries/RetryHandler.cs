namespace LCT.Core.Retries
{
    public static class RetryHandler
    {
        public static async Task RetryActionAsync(Func<Task> action, int retries = 3)
        {
            bool success = false;
            int attempt = 0;
            while(!success && attempt < retries)
            {
                try
                {
                    await action.Invoke();
                    success = true;
                }
                catch(Exception ex)
                {
                    attempt++;
                }
            }
        }
    }
}
