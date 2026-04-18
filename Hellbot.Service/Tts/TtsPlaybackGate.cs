namespace Hellbot.Service.Tts
{
    public class TtsPlaybackGate : ITtsPlaybackGate
    {
        private readonly SemaphoreSlim _signal = new(1, 1);
        private readonly Lock _lock = new();

        private readonly Dictionary<string, DateTime> _activePauses = [];

        private bool _isClosed = false;

        public async Task WaitAsync(CancellationToken ct)
        {
            await _signal.WaitAsync(ct);
            _signal.Release();
        }

        public void Pause(string reason)
        {
            bool shouldClose = false;

            lock (_lock)
            {
                if (_activePauses.ContainsKey(reason))
                    return;

                _activePauses[reason] = DateTime.UtcNow;

                if (!_isClosed)
                {
                    _isClosed = true;
                    shouldClose = true;
                }
            }

            if (shouldClose)
            {
                _signal.Wait();
            }
        }

        public void Resume(string reason)
        {
            bool shouldOpen = false;

            lock (_lock)
            {
                if (!_activePauses.Remove(reason))
                    return;

                if (_isClosed && _activePauses.Count == 0)
                {
                    _isClosed = false;
                    shouldOpen = true;
                }
            }

            if (shouldOpen)
            {
                _signal.Release();
            }
        }

        public IReadOnlyDictionary<string, DateTime> GetActivePauses()
        {
            lock (_lock)
            {
                return new Dictionary<string, DateTime>(_activePauses);
            }
        }

        public bool IsPaused
        {
            get
            {
                lock (_lock)
                {
                    return _activePauses.Count > 0;
                }
            }
        }
    }
}
