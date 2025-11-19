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

        private double _previousTime = 0;

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
            ActiveNotes.Clear();

            // Load and play audio file
            if (!string.IsNullOrEmpty(song.Url) && System.IO.File.Exists(song.Url))
            {
                System.Diagnostics.Debug.WriteLine($"Loading audio from: {song.Url}");
                _audioService.Load(song.Url);
                System.Diagnostics.Debug.WriteLine($"Audio duration: {_audioService.TotalDuration:F2}s");
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

        private int _frameCount = 0;

        private void GameLoop(object? sender, EventArgs e)
        {
            if (IsPaused || BeatMap == null)
                return;

            _frameCount++;

            // Sync game time with actual audio position for perfect rhythm timing
            double newTime = _audioService.CurrentPosition;
            double deltaTime = newTime - _previousTime;

            // Clamp delta time to prevent huge jumps (e.g., after pause)
            if (deltaTime > 0.1) deltaTime = 0.016; // Use 16ms if jump is too large
            if (deltaTime < 0) deltaTime = 0; // Ignore negative deltas

            _previousTime = newTime;
            CurrentTime = newTime;

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
                    note.Y = -100; // Start above screen
                    ActiveNotes.Add(note);
                    System.Diagnostics.Debug.WriteLine($"  Note spawned at lane {note.Lane}, time {note.Time:F2}s, Y={note.Y}");
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
                note.Y += FALL_SPEED * deltaTime;

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
