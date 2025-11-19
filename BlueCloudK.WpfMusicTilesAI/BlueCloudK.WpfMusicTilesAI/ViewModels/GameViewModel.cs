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
        private readonly ISettingsService _settingsService;
        private readonly DispatcherTimer _gameTimer;
        private double _fallSpeed = 300; // pixels per second (adjusted by speed setting and progression)
        private double _progressiveSpeedMultiplier = 1.0; // increases over time for difficulty
        private const double BASE_FALL_SPEED = 300; // base pixels per second
        private const double SPEED_INCREASE_RATE = 0.02; // 2% increase per 10 seconds
        private const double MAX_SPEED_MULTIPLIER = 2.5; // max 2.5x speed
        private const double HIT_ZONE_Y = 500; // Y position of hit zone
        private const double HIT_TOLERANCE = 50; // pixels tolerance for hitting notes
        private const double SPAWN_AHEAD_TIME = 2.0; // seconds to spawn notes before hit time

        [ObservableProperty]
        private BeatMap? _beatMap;

        [ObservableProperty]
        private Song? _song;

        [ObservableProperty]
        private ObservableCollection<Note> _activeNotes = new();

        [ObservableProperty]
        private ObservableCollection<HitFeedback> _hitFeedbacks = new();

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

        private double _previousTime = 0;

        public event Action? OnGameEnd;

        public GameViewModel(IAudioService audioService, ISettingsService settingsService)
        {
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            _gameTimer.Tick += GameLoop;

            // Note: Settings will be applied in Initialize() after audio is loaded
        }

        public void Initialize(BeatMap beatMap, Song song)
        {
            BeatMap = beatMap ?? throw new ArgumentNullException(nameof(beatMap));
            Song = song ?? throw new ArgumentNullException(nameof(song));

            System.Diagnostics.Debug.WriteLine($"=== GameViewModel Initialize ===");
            System.Diagnostics.Debug.WriteLine($"BeatMap has {beatMap.Notes.Count} total notes");
            System.Diagnostics.Debug.WriteLine($"Song: {song.Title}");
            System.Diagnostics.Debug.WriteLine($"First 5 notes:");
            foreach (var note in beatMap.Notes.Take(5))
            {
                System.Diagnostics.Debug.WriteLine($"  Time={note.Time:F2}s, Lane={note.Lane}, State={note.State}");
            }

            Score = 0;
            Combo = 0;
            MaxCombo = 0;
            CurrentTime = 0;
            _previousTime = 0;
            _frameCount = 0;
            _progressiveSpeedMultiplier = 1.0;
            ActiveNotes.Clear();
            HitFeedbacks.Clear();

            // Load and play audio file
            if (!string.IsNullOrEmpty(song.Url) && System.IO.File.Exists(song.Url))
            {
                System.Diagnostics.Debug.WriteLine($"Loading audio from: {song.Url}");
                _audioService.Load(song.Url);
                System.Diagnostics.Debug.WriteLine($"Audio duration: {_audioService.TotalDuration:F2}s");

                // Apply settings AFTER audio is loaded so Volume can be set
                ApplySettings();

                _audioService.Play();
                System.Diagnostics.Debug.WriteLine($"Audio IsPlaying: {_audioService.IsPlaying}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"WARNING: No audio file! Url={song.Url}, Exists={System.IO.File.Exists(song.Url ?? "")}");
            }

            // Start game
            System.Diagnostics.Debug.WriteLine("Starting game timer...");
            _gameTimer.Start();
            System.Diagnostics.Debug.WriteLine("=== GameViewModel Initialize COMPLETE ===");
        }

        /// <summary>
        /// Applies current settings from SettingsService
        /// </summary>
        public void ApplySettings()
        {
            var settings = _settingsService.Settings;

            // Apply speed multiplier to fall speed (settings.Speed * progressive multiplier)
            _fallSpeed = BASE_FALL_SPEED * settings.Speed * _progressiveSpeedMultiplier;
            System.Diagnostics.Debug.WriteLine($"Applied settings: Speed={settings.Speed}x, Progressive={_progressiveSpeedMultiplier:F2}x, FallSpeed={_fallSpeed}px/s");

            // Apply volume to audio service
            _audioService.Volume = settings.Volume;
            System.Diagnostics.Debug.WriteLine($"Applied volume: {settings.Volume}");

            // Note: Theme and effects would be applied through UI bindings or property changes
        }

        private int _frameCount = 0;

        private void GameLoop(object? sender, EventArgs e)
        {
            if (IsPaused || BeatMap == null)
                return;

            _frameCount++;

            // Sync game time with actual audio position for perfect rhythm timing
            double newTime = _audioService.CurrentPosition;
            double deltaTime = newTime - _previousTime;

            // Clamp delta time to prevent huge jumps (e.g., after pause) and handle zero deltas
            if (deltaTime > 0.05) // Max 50ms between frames
            {
                deltaTime = 0.016; // Use 16ms if jump is too large
                System.Diagnostics.Debug.WriteLine($"WARNING: Large deltaTime detected, clamping to 0.016");
            }
            else if (deltaTime <= 0) // Audio position hasn't updated yet
            {
                deltaTime = 0.016; // Use expected frame time
            }

            _previousTime = newTime;
            CurrentTime = newTime;

            // Progressive speed increase every 10 seconds (Magic Tiles difficulty)
            if (_frameCount % 600 == 0 && _progressiveSpeedMultiplier < MAX_SPEED_MULTIPLIER)
            {
                _progressiveSpeedMultiplier = Math.Min(_progressiveSpeedMultiplier + SPEED_INCREASE_RATE, MAX_SPEED_MULTIPLIER);
                ApplySettings(); // Recalculate fall speed with new multiplier
                System.Diagnostics.Debug.WriteLine($"SPEED INCREASED! Multiplier: {_progressiveSpeedMultiplier:F2}x, FallSpeed: {_fallSpeed:F1}px/s");
            }

            // Debug every 60 frames (1 second at 60fps)
            if (_frameCount % 60 == 0)
            {
                System.Diagnostics.Debug.WriteLine($"=== Frame {_frameCount} ===");
                System.Diagnostics.Debug.WriteLine($"CurrentTime: {CurrentTime:F3}s, deltaTime: {deltaTime:F4}s");
                System.Diagnostics.Debug.WriteLine($"ActiveNotes: {ActiveNotes.Count}, Total notes in beatmap: {BeatMap.Notes.Count}");
                System.Diagnostics.Debug.WriteLine($"Audio playing: {_audioService.IsPlaying}");
            }

            // Spawn notes that should appear on screen
            SpawnNotes();

            // Update positions of active notes with actual delta time
            UpdateNotes(deltaTime);

            // Update hit feedback animations
            UpdateHitFeedbacks(deltaTime);

            // Check if game is finished
            if (CurrentTime >= (BeatMap.Metadata.Duration + 3))
            {
                EndGame();
            }
        }

        private void SpawnNotes()
        {
            if (BeatMap == null)
            {
                System.Diagnostics.Debug.WriteLine("SpawnNotes: BeatMap is NULL!");
                return;
            }

            // Check candidate notes
            var activeNotes = BeatMap.Notes.Where(n => n.State == NoteState.Active).ToList();
            var notAlreadySpawned = activeNotes.Where(n => !ActiveNotes.Contains(n)).ToList();
            var withinSpawnWindow = notAlreadySpawned.Where(n => (n.Time - CurrentTime) <= 2.0).ToList();

            if (_frameCount % 60 == 0 && notAlreadySpawned.Any())
            {
                var nextNote = notAlreadySpawned.OrderBy(n => n.Time).First();
                System.Diagnostics.Debug.WriteLine($"Next note to spawn: Time={nextNote.Time:F2}s, Lane={nextNote.Lane}, TimeUntilSpawn={(nextNote.Time - CurrentTime - 2.0):F2}s");
            }

            var notesToSpawn = withinSpawnWindow;

            if (notesToSpawn.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"[{CurrentTime:F2}s] Spawning {notesToSpawn.Count} notes");
                foreach (var note in notesToSpawn)
                {
                    // Calculate initial Y based on time remaining until hit
                    double timeUntilHit = note.Time - CurrentTime;
                    // Note should travel at _fallSpeed to reach HIT_ZONE_Y exactly at note.Time
                    double distanceToTravel = timeUntilHit * _fallSpeed;
                    note.Y = HIT_ZONE_Y - distanceToTravel;

                    ActiveNotes.Add(note);
                    System.Diagnostics.Debug.WriteLine($"  Note spawned at lane {note.Lane}, time {note.Time:F2}s, Y={note.Y:F1} (timeUntil={timeUntilHit:F2}s, dist={distanceToTravel:F1}px)");
                }
            }
        }

        private void UpdateNotes(double deltaTime)
        {
            var notesToRemove = new System.Collections.Generic.List<Note>();

            foreach (var note in ActiveNotes)
            {
                // Update Y position using actual delta time
                var oldY = note.Y;
                note.Y += _fallSpeed * deltaTime;

                // Log first few updates to verify
                if (oldY < 0 && note.Y >= 0)
                {
                    System.Diagnostics.Debug.WriteLine($"  Note lane {note.Lane} entered screen at Y={note.Y:F1} (deltaTime={deltaTime:F3}s)");
                }

                // Check if note passed hit zone (missed)
                if (note.Y > HIT_ZONE_Y + 100 && note.State == NoteState.Active)
                {
                    note.State = NoteState.Missed;
                    Combo = 0;
                    notesToRemove.Add(note);
                    System.Diagnostics.Debug.WriteLine($"  Note lane {note.Lane} MISSED at Y={note.Y:F1}");

                    // Show miss feedback
                    ShowHitFeedback(note.Lane, "MISS", "#ff4444");
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

            if (ActiveNotes.Count > 0 && CurrentTime % 1.0 < 0.02) // Log every second
            {
                System.Diagnostics.Debug.WriteLine($"[{CurrentTime:F2}s] {ActiveNotes.Count} active notes on screen");
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
                string feedbackText;
                string feedbackColor;

                if (accuracy > 0.9)
                {
                    points = 150; // Perfect hit
                    feedbackText = "PERFECT!";
                    feedbackColor = "#00ff00";
                }
                else if (accuracy > 0.7)
                {
                    feedbackText = "GOOD";
                    feedbackColor = "#ffff00";
                }
                else
                {
                    feedbackText = "OK";
                    feedbackColor = "#ff9900";
                }

                Score += points * (Combo / 10 + 1); // Combo multiplier
                Combo++;

                if (Combo > MaxCombo)
                    MaxCombo = Combo;

                // Show hit feedback
                ShowHitFeedback(lane, feedbackText, feedbackColor);

                System.Diagnostics.Debug.WriteLine($"HIT! Lane {lane}, Accuracy: {accuracy:P0}, Score: +{points * (Combo / 10 + 1)}");
            }
            else
            {
                // Wrong tap: just break combo, don't end game
                if (Combo > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"WRONG TAP! Lane {lane}, No tile in hit zone - Combo broken!");
                    Combo = 0;
                }
            }
        }

        /// <summary>
        /// Shows visual feedback at the hit zone
        /// </summary>
        private void ShowHitFeedback(int lane, string text, string color)
        {
            var feedback = new HitFeedback(lane, text, color);
            HitFeedbacks.Add(feedback);
        }

        /// <summary>
        /// Updates hit feedback animations (fade out and move up)
        /// </summary>
        private void UpdateHitFeedbacks(double deltaTime)
        {
            var feedbacksToRemove = new System.Collections.Generic.List<HitFeedback>();

            foreach (var feedback in HitFeedbacks)
            {
                // Fade out over 0.8 seconds
                feedback.Opacity -= deltaTime / 0.8;

                // Move up slowly
                feedback.Y -= 50 * deltaTime;

                if (feedback.Opacity <= 0)
                {
                    feedbacksToRemove.Add(feedback);
                }
            }

            foreach (var feedback in feedbacksToRemove)
            {
                HitFeedbacks.Remove(feedback);
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
