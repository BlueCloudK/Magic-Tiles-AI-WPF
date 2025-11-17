using BlueCloudK.WpfMusicTilesAI.Models;
using BlueCloudK.WpfMusicTilesAI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace BlueCloudK.WpfMusicTilesAI.ViewModels
{
    /// <summary>
    /// View model for the game screen
    /// </summary>
    public partial class GameViewModel : ObservableObject, IDisposable
    {
        private readonly IAudioService _audioService;
        private readonly DispatcherTimer _gameTimer;
        private const double FALL_SPEED = 300; // pixels per second
        private const double HIT_ZONE_Y = 500; // Y position of hit zone
        private const double HIT_TOLERANCE = 50; // pixels tolerance for hitting notes

        [ObservableProperty]
        private BeatMap? _beatMap;

        [ObservableProperty]
        private Song? _song;

        [ObservableProperty]
        private ObservableCollection<Note> _activeNotes = new();

        [ObservableProperty]
        private int _score;

        [ObservableProperty]
        private int _combo;

        [ObservableProperty]
        private int _maxCombo;

        [ObservableProperty]
        private bool _isPaused;

        [ObservableProperty]
        private double _currentTime;

        public event Action? OnGameEnd;

        public GameViewModel(IAudioService audioService)
        {
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));

            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            _gameTimer.Tick += GameLoop;
        }

        public void Initialize(BeatMap beatMap, Song song)
        {
            BeatMap = beatMap ?? throw new ArgumentNullException(nameof(beatMap));
            Song = song ?? throw new ArgumentNullException(nameof(song));

            Score = 0;
            Combo = 0;
            MaxCombo = 0;
            CurrentTime = 0;
            ActiveNotes.Clear();

            // Load and play audio file
            if (!string.IsNullOrEmpty(song.Url) && System.IO.File.Exists(song.Url))
            {
                _audioService.Load(song.Url);
                _audioService.Play();
            }

            // Start game
            _gameTimer.Start();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (IsPaused || BeatMap == null)
                return;

            CurrentTime += 0.016; // 16ms per frame

            // Spawn notes that should appear on screen
            SpawnNotes();

            // Update positions of active notes
            UpdateNotes();

            // Check if game is finished
            if (CurrentTime >= (BeatMap.Metadata.Duration + 3))
            {
                EndGame();
            }
        }

        private void SpawnNotes()
        {
            if (BeatMap == null)
                return;

            var notesToSpawn = BeatMap.Notes
                .Where(n => n.State == NoteState.Active &&
                           !ActiveNotes.Contains(n) &&
                           (n.Time - CurrentTime) <= 2.0) // Spawn 2 seconds before hit time
                .ToList();

            foreach (var note in notesToSpawn)
            {
                note.Y = -100; // Start above screen
                ActiveNotes.Add(note);
            }
        }

        private void UpdateNotes()
        {
            var notesToRemove = new System.Collections.Generic.List<Note>();

            foreach (var note in ActiveNotes)
            {
                // Update Y position
                note.Y += FALL_SPEED * 0.016;

                // Check if note passed hit zone (missed)
                if (note.Y > HIT_ZONE_Y + 100 && note.State == NoteState.Active)
                {
                    note.State = NoteState.Missed;
                    Combo = 0;
                    notesToRemove.Add(note);
                }
                else if (note.State == NoteState.Hit || note.State == NoteState.Missed)
                {
                    notesToRemove.Add(note);
                }
            }

            foreach (var note in notesToRemove)
            {
                ActiveNotes.Remove(note);
            }
        }

        /// <summary>
        /// Handle key press for a specific lane
        /// </summary>
        public void HandleKeyPress(int lane)
        {
            var note = ActiveNotes
                .Where(n => n.Lane == lane && n.State == NoteState.Active)
                .OrderBy(n => Math.Abs(n.Y - HIT_ZONE_Y))
                .FirstOrDefault();

            if (note != null && Math.Abs(note.Y - HIT_ZONE_Y) <= HIT_TOLERANCE)
            {
                note.State = NoteState.Hit;
                note.HitTime = CurrentTime;

                // Update score and combo
                int points = 100;
                double accuracy = 1 - (Math.Abs(note.Y - HIT_ZONE_Y) / HIT_TOLERANCE);
                if (accuracy > 0.9) points = 150; // Perfect hit

                Score += points * (Combo / 10 + 1); // Combo multiplier
                Combo++;

                if (Combo > MaxCombo)
                    MaxCombo = Combo;
            }
        }

        [RelayCommand]
        private void TogglePause()
        {
            IsPaused = !IsPaused;

            if (IsPaused)
                _audioService.Pause();
            else
                _audioService.Play();
        }

        [RelayCommand]
        private void EndGame()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== EndGame called ===");

                System.Diagnostics.Debug.WriteLine("Stopping game timer...");
                _gameTimer?.Stop();

                System.Diagnostics.Debug.WriteLine("Stopping audio...");
                _audioService?.Stop();

                System.Diagnostics.Debug.WriteLine("Invoking OnGameEnd event...");
                OnGameEnd?.Invoke();

                System.Diagnostics.Debug.WriteLine("EndGame completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in EndGame: {ex}");
            }
        }

        public void Dispose()
        {
            _gameTimer.Stop();
        }
    }
}
