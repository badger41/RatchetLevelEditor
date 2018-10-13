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
    }
}