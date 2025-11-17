"""
Audio Analysis Script for Magic Tiles AI
Analyzes audio files to detect beats and create beat maps

Requirements:
pip install librosa numpy

Usage:
python analyze_audio.py <audio_file_path> <output_json_path>
"""

import sys
import json
import librosa
import numpy as np

def analyze_audio(audio_path):
    """Analyze audio file and extract beat information"""

    # Load audio file
    y, sr = librosa.load(audio_path)
    duration = librosa.get_duration(y=y, sr=sr)

    # Detect tempo and beats
    tempo, beat_frames = librosa.beat.beat_track(y=y, sr=sr)
    beat_times = librosa.frames_to_time(beat_frames, sr=sr)

    # Detect onset (note starts)
    onset_env = librosa.onset.onset_strength(y=y, sr=sr)
    onset_frames = librosa.onset.onset_detect(onset_envelope=onset_env, sr=sr)
    onset_times = librosa.frames_to_time(onset_frames, sr=sr)

    # Spectral analysis for difficulty estimation
    spectral_centroids = librosa.feature.spectral_centroid(y=y, sr=sr)[0]
    avg_spectral_centroid = np.mean(spectral_centroids)

    # Estimate difficulty based on beat density
    beat_density = len(beat_times) / duration
    difficulty = "Easy"
    if beat_density > 2.5:
        difficulty = "Hard"
    elif beat_density > 1.5:
        difficulty = "Medium"

    return {
        "duration": float(duration),
        "tempo": float(tempo),
        "beat_times": beat_times.tolist(),
        "onset_times": onset_times.tolist(),
        "difficulty": difficulty,
        "beat_count": len(beat_times)
    }

def create_beat_map(analysis, audio_path, title="Unknown"):
    """Create beat map in the format expected by Magic Tiles AI"""

    notes = []
    lanes = [0, 1, 2, 3]  # 4 lanes

    # Create notes from detected beats
    for i, time in enumerate(analysis['onset_times']):
        lane = lanes[i % len(lanes)]  # Distribute across lanes
        note = {
            "Time": float(time),
            "Lane": lane,
            "Duration": 0.0  # Regular tap note
        }
        notes.append(note)

    beat_map = {
        "Metadata": {
            "Title": title,
            "Artist": "Auto-detected",
            "Difficulty": analysis['difficulty'],
            "Duration": analysis['duration'],
            "BPM": analysis['tempo']
        },
        "Notes": notes
    }

    return beat_map

def main():
    if len(sys.argv) < 3:
        print("Usage: python analyze_audio.py <audio_file> <output_json>")
        sys.exit(1)

    audio_path = sys.argv[1]
    output_path = sys.argv[2]
    title = sys.argv[3] if len(sys.argv) > 3 else "Unknown"

    try:
        print(f"Analyzing: {audio_path}")
        analysis = analyze_audio(audio_path)

        print(f"Tempo: {analysis['tempo']:.2f} BPM")
        print(f"Duration: {analysis['duration']:.2f} seconds")
        print(f"Beats detected: {analysis['beat_count']}")
        print(f"Difficulty: {analysis['difficulty']}")

        beat_map = create_beat_map(analysis, audio_path, title)

        with open(output_path, 'w') as f:
            json.dump(beat_map, f, indent=2)

        print(f"Beat map saved to: {output_path}")

    except Exception as e:
        print(f"Error: {str(e)}", file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
