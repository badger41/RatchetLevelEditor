using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;

namespace RatchetLevelEditor
{
    public class RatchetMoby
    {
        const uint MOBYELEMSIZERAC1 = 0x78;
        const uint MOBYELEMSIZERAC23 = 0x88;
        const uint MOBYELEMSIZERAC4 = 0x70;

        public uint length;
        public uint missionID;
        public uint unk1;
        public uint dataval;

        public uint dropamnt;
        public uint unk2;
        public uint modelID;
        public float size;

        public uint rend1;
        public uint rend2;
        public uint unk3;
        public uint unk4;

        public float x;
        public float y;
        public float z;
        public float rot1;

        public float rot2;
        public float rot3;
        public uint unk5;

        public uint unk6;
        public uint unk7;
        public uint unk8;
        public uint propIndex;

        public uint unk9;
        public uint unk10;
        public uint r;
        public uint g;

        public uint b;
        public uint light;
        public uint unk11;

        public uint unk12;
        public uint unk13;
        public uint unk14;
        public uint unk15;
        public uint unk16;


        public float z2;
        public int test1;
        public int cutScene;

        public List<MobyPVar> pVarConfig;
        public byte[] pVars;

        public RatchetMoby(uint racNum, byte[] mobyBlock, uint num)
        {
            switch (racNum)
            {
                case 1:
                    setValRac1(mobyBlock, num);
                    break;
                case 2:
                case 3:
                    setValRac23(mobyBlock, num);
                    break;
                case 4:
                    setValRac4(mobyBlock, num);
                    break;
            }
        }

        void setValRac1(byte[] mobyBlock, uint num)
        {
            length = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x00);
            missionID = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x04);
            unk1 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x08);
            dataval = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x0C);

            dropamnt = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x10);
            unk2 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x14);
            modelID = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x18);
            size = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x1C);

            rend1 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x20);
            rend2 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x24);
            unk3 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x28);
            unk4 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x2C);

            x = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x30);
            y = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x34);
            z = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x38);
            rot1 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x3C);

            rot2 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x40);
            rot3 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x44);
            unk5 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x48);
            test1 = BAToInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x4C);

            z2 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x50);
            unk8 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x54);
            propIndex = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x58);
            unk9 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x5C);

            unk10 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x60);
            r = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x64);
            g = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x68);
            b = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x6C);

            light = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x70);
            cutScene = BAToInt32(mobyBlock, (num * MOBYELEMSIZERAC1) + 0x74);

            if ((int)propIndex > 0)
            {
                pVars = DataStore.pVarList[(int)propIndex];
                pVarConfig = DataStore.pVarMap.Where(x => x.modelIds.Contains(modelID)).FirstOrDefault()?.pVars;
            }
            else
            {
                pVars = new byte[0];
            }
        }

        void setValRac23(byte[] mobyBlock, uint num)
        {
            length = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x00);
            missionID = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x04);
            unk1 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x08);
            dataval = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x0C);

            unk2 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x10);
            dropamnt = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x14);
            unk3 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x18);
            unk4 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x1C);

            unk5 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x20);
            unk6 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x24);
            modelID = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x28);
            size = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x2C);

            rend1 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x30);
            rend2 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x34);
            unk7 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x38);
            unk8 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x3C);

            x = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x40);
            y = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x44);
            z = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x48);
            rot1 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x4C);

            rot2 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x50);
            rot3 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x54);
            unk9 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x58);
            unk10 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x5C);

            unk11 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x60);
            unk12 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x64);
            propIndex = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x68);
            unk14 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x6C);

            unk15 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x70);
            r = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x74);
            g = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x78);
            b = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x7C);

            light = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x80);
            unk16 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC23) + 0x84);

            z2 = 0;
            test1 = 0;
            cutScene = 0;
            if ((int)propIndex > 0)
            {
                pVars = DataStore.pVarList[(int)propIndex];
                pVarConfig = DataStore.pVarMap.Where(x => x.modelIds.Contains(modelID)).FirstOrDefault()?.pVars;
            }
            else
            {
                pVars = new byte[0];
            }
        }

        void setValRac4(byte[] mobyBlock, uint num)
        {
            length = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x00);
            missionID = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x04);
            dataval = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x08);
            unk1 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x0C);

            modelID = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x10);
            size = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x14);
            rend1 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x18);
            rend2 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x1C);

            unk2 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x20);
            unk3 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x24);
            x = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x28);
            y = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x2C);

            z = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x30);
            rot1 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x34);
            rot2 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x38);
            rot3 = BAToFloat(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x3C);

            unk4 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x40);
            unk5 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x44);
            unk6 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x48);
            unk7 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x4C);

            propIndex = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x50);
            unk8 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x54);
            unk9 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x58);
            r = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x5C);

            g = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x60);
            b = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x64);
            light = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x68);
            unk14 = BAToUInt32(mobyBlock, (num * MOBYELEMSIZERAC4) + 0x6C);

            z2 = 0;
            test1 = 0;
            cutScene = 0;
            if ((int)propIndex > 0)
            {
                pVars = DataStore.pVarList[(int)propIndex];
                pVarConfig = DataStore.pVarMap.Where(x => x.modelIds.Contains(modelID)).FirstOrDefault() ? .pVars;
            }
            else
            {
                pVars = new byte[0];
            }
        }

        public static void serialize(ref byte[] gameplay_ntsc, int index, int racNum)
        {
            //The 16 bytes before the actual start of the moby list
            //0x00 = moby count
            //0x04 = ??? but its required
            //0x08 - 0x0C = null
            byte[] mobyInitHeader = new byte[0x10];
            WriteUint32(ref mobyInitHeader, 0x00, (uint)DataStore.mobs.Count);
            WriteUint32(ref mobyInitHeader, 0x04, DataStore.mobyUnknownVal);

            int currentOffset = gameplay_ntsc.Length;
            Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + mobyInitHeader.Length));
            writeBytes(gameplay_ntsc, currentOffset, mobyInitHeader, mobyInitHeader.Length);


            switch (racNum)
            {
                case 1:
                    foreach (RatchetMoby mob in DataStore.mobs)
                    {
                        byte[] data = new byte[mob.length];

                        WriteUint32(ref data, 0x00, mob.length);
                        WriteUint32(ref data, 0x04, mob.missionID);
                        WriteUint32(ref data, 0x08, mob.unk1);
                        WriteUint32(ref data, 0x0C, mob.dataval);

                        WriteUint32(ref data, 0x10, mob.dropamnt);
                        WriteUint32(ref data, 0x14, mob.unk2);
                        WriteUint32(ref data, 0x18, mob.modelID);
                        WriteFloat(ref data, 0x1C, mob.size);

                        WriteUint32(ref data, 0x20, mob.rend1);
                        WriteUint32(ref data, 0x24, mob.rend2);
                        WriteUint32(ref data, 0x28, mob.unk3);
                        WriteUint32(ref data, 0x2C, mob.unk4);

                        WriteFloat(ref data, 0x30, mob.x);
                        WriteFloat(ref data, 0x34, mob.y);
                        WriteFloat(ref data, 0x38, mob.z);
                        WriteFloat(ref data, 0x3C, mob.rot1);

                        WriteFloat(ref data, 0x40, mob.rot2);
                        WriteFloat(ref data, 0x44, mob.rot3);
                        WriteUint32(ref data, 0x48, mob.unk5);
                        WriteInt32(ref data, 0x4C, mob.test1);

                        WriteFloat(ref data, 0x50, mob.z2);
                        WriteUint32(ref data, 0x54, mob.unk8);
                        WriteUint32(ref data, 0x58, mob.propIndex);
                        WriteUint32(ref data, 0x5C, mob.unk9);

                        WriteUint32(ref data, 0x60, mob.unk10);
                        WriteUint32(ref data, 0x64, mob.r);
                        WriteUint32(ref data, 0x68, mob.g);
                        WriteUint32(ref data, 0x6C, mob.b);

                        WriteUint32(ref data, 0x70, mob.light);
                        WriteInt32(ref data, 0x74, mob.cutScene);

                        currentOffset = gameplay_ntsc.Length;
                        Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + mob.length));
                        writeBytes(gameplay_ntsc, currentOffset, data, data.Length);

                    }
                    break;

                case 2:
                case 3:
                    foreach (RatchetMoby mob in DataStore.mobs)
                    {
                        byte[] data = new byte[mob.length];
                        WriteUint32(ref data, 0x00, mob.length);
                        WriteUint32(ref data, 0x04, mob.missionID);
                        WriteUint32(ref data, 0x08, mob.unk1);
                        WriteUint32(ref data, 0x0C, mob.dataval);

                        WriteUint32(ref data, 0x10, mob.unk2);
                        WriteUint32(ref data, 0x14, mob.dropamnt);
                        WriteUint32(ref data, 0x18, mob.unk3);
                        WriteUint32(ref data, 0x1C, mob.unk4);

                        WriteUint32(ref data, 0x20, mob.unk5);
                        WriteUint32(ref data, 0x24, mob.unk6);
                        WriteUint32(ref data, 0x28, mob.modelID);
                        WriteFloat(ref data, 0x2C, mob.size);

                        WriteUint32(ref data, 0x30, mob.rend1);
                        WriteUint32(ref data, 0x34, mob.rend2);
                        WriteUint32(ref data, 0x38, mob.unk7);
                        WriteUint32(ref data, 0x3C, mob.unk8);

                        WriteFloat(ref data, 0x40, mob.x);
                        WriteFloat(ref data, 0x44, mob.y);
                        WriteFloat(ref data, 0x48, mob.z);
                        WriteFloat(ref data, 0x4C, mob.rot1);

                        WriteFloat(ref data, 0x50, mob.rot2);
                        WriteFloat(ref data, 0x54, mob.rot3);
                        WriteUint32(ref data, 0x58, mob.unk9);
                        WriteUint32(ref data, 0x5C, mob.unk10);

                        WriteUint32(ref data, 0x60, mob.unk11);
                        WriteUint32(ref data, 0x64, mob.unk12);
                        WriteUint32(ref data, 0x68, mob.propIndex);
                        WriteUint32(ref data, 0x6C, mob.unk14);

                        WriteUint32(ref data, 0x70, mob.unk15);
                        WriteUint32(ref data, 0x74, mob.r);
                        WriteUint32(ref data, 0x78, mob.g);
                        WriteUint32(ref data, 0x7C, mob.b);

                        WriteUint32(ref data, 0x80, mob.light);
                        WriteUint32(ref data, 0x84, mob.unk16);

                        currentOffset = gameplay_ntsc.Length;
                        Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + mob.length));
                        writeBytes(gameplay_ntsc, currentOffset, data, data.Length);


                    }
                    break;

                case 4:
                    foreach (RatchetMoby mob in DataStore.mobs)
                    {
                        byte[] data = new byte[mob.length];
                        WriteUint32(ref data, 0x00, mob.length);
                        WriteUint32(ref data, 0x04, mob.missionID);
                        WriteUint32(ref data, 0x08, mob.dataval);
                        WriteUint32(ref data, 0x0C, mob.unk1);

                        WriteUint32(ref data, 0x10, mob.modelID);
                        WriteFloat(ref data, 0x14, mob.size);
                        WriteUint32(ref data, 0x18, mob.rend1);
                        WriteUint32(ref data, 0x1C, mob.rend2);

                        WriteUint32(ref data, 0x20, mob.unk2);
                        WriteUint32(ref data, 0x24, mob.unk3);
                        WriteFloat(ref data, 0x28, mob.x);
                        WriteFloat(ref data, 0x2C, mob.y);

                        WriteFloat(ref data, 0x30, mob.z);
                        WriteFloat(ref data, 0x34, mob.rot1);
                        WriteFloat(ref data, 0x38, mob.rot2);
                        WriteFloat(ref data, 0x3C, mob.rot3);

                        WriteUint32(ref data, 0x40, mob.unk4);
                        WriteUint32(ref data, 0x44, mob.unk5);
                        WriteUint32(ref data, 0x48, mob.unk6);
                        WriteUint32(ref data, 0x4C, mob.unk7);

                        WriteUint32(ref data, 0x50, mob.propIndex);
                        WriteUint32(ref data, 0x54, mob.unk8);
                        WriteUint32(ref data, 0x58, mob.unk9);
                        WriteUint32(ref data, 0x5C, mob.r);

                        WriteUint32(ref data, 0x60, mob.g);
                        WriteUint32(ref data, 0x64, mob.b);
                        WriteUint32(ref data, 0x68, mob.light);
                        WriteUint32(ref data, 0x6C, mob.unk14);

                        currentOffset = gameplay_ntsc.Length;

                        Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + mob.length));
                        writeBytes(gameplay_ntsc, currentOffset, data, data.Length);

                    }
                    break;
            }
            //Ensure that the next offset ends in 0
            uint resize = 0x00;
            while ((gameplay_ntsc.Length + resize) % 0x10 != 0)
                resize += 0x04;
            Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + resize));
        }

        public static void serializeMobyPvarSizes(ref byte[] gameplay_ntsc, int index)
        {
            int currentOffset = gameplay_ntsc.Length;

            //We have to ensure that pointers end on a 0, or else the game will not parse it
            //byte[] space = new byte[0x08];
            //Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + space.Length));
            //writeBytes(gameplay_ntsc, currentOffset, space, space.Length);

            //pVar sizes have to be stored as well
            int expectedSize = DataStore.pVarList.Count * 0x08;



            byte[] pVarSizes = new byte[expectedSize];
            uint pVarSizeOffset = 0;

            foreach (byte[] pvar in DataStore.pVarList)
            {

                byte[] pVarSize = new byte[0x08];
                //Write the current offset of the size config
                WriteUint32(ref pVarSize, 0x00, pVarSizeOffset);

                //Write the size of the current pvar
                WriteUint32(ref pVarSize, 0x04, (uint)pvar.Length);

                //Write the new pvar size data to appropriate index
                int pVarIndex = DataStore.pVarList.IndexOf(pvar);
                writeBytes(pVarSizes, pVarIndex * 0x08, pVarSize, 8);

                //Increment the size of the offset
                pVarSizeOffset += (uint)pvar.Length;
            }

            //Ensure that the next offset ends in 0
            uint resize = 0x00;
            while ((gameplay_ntsc.Length + pVarSizes.Length + resize) % 0x10 != 0)
                resize += 0x04;

            currentOffset = gameplay_ntsc.Length;
            Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + pVarSizes.Length));
            writeBytes(gameplay_ntsc, currentOffset, pVarSizes, pVarSizes.Length);
        }

        public static void serializeMobyPvars(ref byte[] gameplay_ntsc, int index)
        {
            int currentOffset = gameplay_ntsc.Length;
            byte[] pvarBlock = new byte[0];

            foreach (byte[] pvar in DataStore.pVarList)
            {
                //Add 0x08 to the size of the pvar size array
                Array.Resize(ref pvarBlock, (int)(pvarBlock.Length + pvar.Length));
                writeBytes(pvarBlock, pvarBlock.Length - pvar.Length, pvar, pvar.Length);

            }
            uint resize = 0x00;
            while ((gameplay_ntsc.Length + pvarBlock.Length + resize) % 0x10 != 0)
                resize += 0x04;

            Array.Resize(ref gameplay_ntsc, (int)(gameplay_ntsc.Length + pvarBlock.Length + resize));
            writeBytes(gameplay_ntsc, currentOffset, pvarBlock, pvarBlock.Length);
        }
    }
}