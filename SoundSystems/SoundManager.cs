using System;
using System.Collections.Generic;
using System.Linq;
using ArchCore.Utils;
using UnityEngine;
using ArchCore.Utils.Executions;
using UnityEditor;

namespace ArchCore.SoundSystems
{
	public enum SoundPriorityType
	{
		Low = 0,
		Medium = 1,
		High = 2
	}



	public abstract class SoundManager<T> : MonoBehaviour where T : Enum
	{
		[SerializeField] private AudioClip[] clips;

		const int START_CHANNEL_COUNT = 3;
		const int MAX_CHANNEL_COUNT = 10;
		const float VOLUME = 0.5f;


		private List<AudioChannel> channels;
		private AudioChannel musicChannel;

		

		protected abstract int GetIndex(T value);
		
		private bool sfxOn;

		public virtual bool IsSfxOn
		{
			get => sfxOn;
			protected set
			{
				sfxOn = value;
				UpdateChannelsMute();
			}
		}

		private bool musicOn;

		public virtual bool IsMusicOn
		{
			get => musicOn;
			protected set
			{
				musicOn = value;
				UpdateChannelsMute();
			}
		}

		private AudioChannel CreateChannel()
		{
			AudioSource newSource = gameObject.AddComponent<AudioSource>();
			newSource.playOnAwake = false;
			newSource.loop = false;
			AudioChannel newChannel = new AudioChannel(newSource);
			newChannel.Mute = !sfxOn;
			return newChannel;
		}
		
		public AudioToken Play(T audioClip, SoundPriorityType priority = SoundPriorityType.Medium,
			bool loop = false)
		{

			AudioChannel free = channels.FirstOrDefault(source => !source.IsBusy);

			if (free == null)
			{
				if (channels.Count < MAX_CHANNEL_COUNT)
				{
					free = CreateChannel();
					channels.Add(free);
				}
				else
				{
					for (int i = 0; i < (int) priority; i++)
					{
						free = channels.FirstOrDefault(source => source.Priority == (SoundPriorityType) i);
						if (free != null) break;
					}

					if (free == null)
					{
						Debug.LogWarning("Too much audio effects at a time, ignoring " + audioClip);
						return AudioToken.ExpiredToken;
					}
				}
			}

			free.Volume = VOLUME;
			free.Mute = !sfxOn;
			
			return free.Play(clips[GetIndex(audioClip)], priority, loop);
		}

		public AudioToken MusicToken => musicChannel.Token ?? AudioToken.ExpiredToken;
		public AudioToken PlayMusic(T audioClip, bool loop = true)
		{
			musicChannel.Volume = VOLUME;
			musicChannel.Mute = !musicOn;
			return musicChannel.Play(clips[GetIndex(audioClip)], SoundPriorityType.High, loop);
		}

		public ActionExe PlayExe(T audioClip, SoundPriorityType priority = SoundPriorityType.Medium,
			bool loop = false)
		{
			return new ActionExe(() => Play(audioClip, priority, loop));
		}

		public AudioToken GetPlayingSound(T audioClip)
		{
			AudioClip cl = clips[GetIndex(audioClip)];
			var ch = channels.FirstOrDefault(c => c.Clip == cl);

			return ch?.Token;
		}


#if UNITY_EDITOR
		// private void OnValidate()
		// {
		// 	EnumGenerator.GenerateEnum("AudioClips", clips.Select(clip=>clip.name).ToArray());
		// }
		
		
		[SerializeField] private MonoScript audioClips;
		
		protected void GenerateEnum()
		{
			EnumGenerator.GenerateEnum(audioClips.name,clips.Select(clip => clip.name).ToArray(), AssetDatabase.GetAssetPath(audioClips));
		}
#endif

		private void Awake()
		{
			channels = new List<AudioChannel>();
			for (int i = 0; i < START_CHANNEL_COUNT; i++)
			{
				channels.Add(CreateChannel());
			}

			musicChannel = CreateChannel();

			UpdateChannelsMute();
		}



		protected void UpdateChannelsMute()
		{
			foreach (var channel in channels)
			{
				channel.Mute = !sfxOn;
			}

			musicChannel.Mute = !musicOn;
		}

	}
}