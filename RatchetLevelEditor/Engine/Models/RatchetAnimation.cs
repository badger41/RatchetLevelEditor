using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatchetLevelEditor.Engine.Models
{
    class RatchetAnimation_Player
    {
        public int index;
        public int unk_count;
        public byte[] rawData;
    }

    class RatchetAnimation_Moby
    {
        //0x00
        public uint anim_ind_00;
        public uint anim_ind_04;
        public uint anim_ind_08;
        public uint anim_ind_0C;

        //0x10
        public byte frameCount;
        public byte anim_ind_11;
        public byte soundCount;
        public byte anim_ind_13;

        //0x14
        public uint anim_ind_14;
        public float animationSpeed;

        public List<RatchetAnimationFrame> frames;

        public List<RatchetAnimationSoundToPlay> soundsToPlay;
    }

    class RatchetAnimationSoundToPlay
    {
        //number cannot exceed total number of sounds configured for the moby
        //Or no sound will play
        public short soundId;

        //number cannot exceed total number of frame parts or no sound will play
        //number is placed after the last frame pointer, regardless of how many frames
        public short soundFrameDelay;
    }

    class RatchetAnimationFrame
    {
        public float frameSpeed;
        public short frame_ind_04;
        public short frameIndex;
        public short sectionLength;
        public short sectionOffset;
        public short translationLength;
        public short translationOffset;
        public List<RatchetAnimationFrameData> frameData;
    }

    class RatchetAnimationFrameData
    {
        public short frame_data_ind_00;
        public short frame_data_ind_02;
        public short frame_data_ind_04;
        public short frame_data_ind_06;
        public short frame_data_ind_08;
        public short frame_data_ind_0A;
        public short frame_data_ind_0C;
        public short frame_data_ind_0E;
    }

    class RatchetAnimationSoundConfig
    {
        public uint config_offset_00;

        //How far away can you hear the sound
        public float distance;

        //Setting a value here will negate distance and volume
        //The animation will play a sound unless the moby is unrendered
        public uint masterVolume;

        public uint volume;
        public uint distortion;
        public uint distortion2; //unsure
        public short config_offset_18;
        public short listIndex;
        public uint config_offset_1C;
    }
}
