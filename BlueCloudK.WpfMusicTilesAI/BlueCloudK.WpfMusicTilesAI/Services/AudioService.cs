using NAudio.Wave;
using System;

namespace BlueCloudK.WpfMusicTilesAI.Services
{
    /// <summary>
    /// Service for audio playback using NAudio
    /// </summary>
    public class AudioService : IAudioService
    {
        private IWavePlayer? _wavePlayer;
        private AudioFileReader? _audioFileReader;
        private bool _disposed;

        public double CurrentPosition => _audioFileReader?.CurrentTime.TotalSeconds ?? 0;

        public double TotalDuration => _audioFileReader?.TotalTime.TotalSeconds ?? 0;

        public float Volume
        {
            get => _audioFileReader?.Volume ?? 0;
            set
            {
                if (_audioFileReader != null)
                    _audioFileReader.Volume = Math.Clamp(value, 0f, 1f);
            }
        }

        public bool IsPlaying => _wavePlayer?.PlaybackState == PlaybackState.Playing;

        public void Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException("Audio file not found", filePath);

            // Dispose existing resources
            Stop();
            _audioFileReader?.Dispose();
            _wavePlayer?.Dispose();

            // Load new audio file
            _audioFileReader = new AudioFileReader(filePath);
            _wavePlayer = new WaveOutEvent();
            _wavePlayer.Init(_audioFileReader);
        }

        public void Play()
        {
            if (_wavePlayer == null)
                throw new InvalidOperationException("No audio file loaded");

            if (_wavePlayer.PlaybackState != PlaybackState.Playing)
            {
                _wavePlayer.Play();
            }
        }

        public void Pause()
        {
            if (_wavePlayer?.PlaybackState == PlaybackState.Playing)
            {
                _wavePlayer.Pause();
            }
        }

        public void Stop()
        {
            if (_wavePlayer != null)
            {
                _wavePlayer.Stop();
                if (_audioFileReader != null)
                {
                    _audioFileReader.Position = 0;
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _audioFileReader?.Dispose();
                _wavePlayer?.Dispose();
                _disposed = true;
            }
        }
    }
}
