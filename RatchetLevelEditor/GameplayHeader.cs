using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;


class GameplayHeader
{
    public uint mobyElemSize;
    public uint gameNum;

    public uint mobyPointer;
    public uint pVarListPointer;
    public uint pVarPointer;
    public uint splinePointer;

    public GameplayHeader(FileStream fs, uint game)
    {
        gameNum = game;
        switch (game)
        {
            case 1:
                byte[] gameplayHeadBlock1 = ReadBlock(fs, 0, 0x78);
                mobyPointer = BAToUInt32(gameplayHeadBlock1, 0x44);
                pVarListPointer = BAToUInt32(gameplayHeadBlock1, 0x54);
                pVarPointer = BAToUInt32(gameplayHeadBlock1, 0x58);
                splinePointer = BAToUInt32(gameplayHeadBlock1, 0x70);
                mobyElemSize = 0x78;
                break;
            case 2:
            case 3:
                byte[] gameplayHeadBlock2 = ReadBlock(fs, 0, 0x80);
                mobyPointer = BAToUInt32(gameplayHeadBlock2, 0x4C);
                mobyElemSize = 0x88;
                pVarListPointer = BAToUInt32(gameplayHeadBlock2, 0x5C);
                pVarPointer = BAToUInt32(gameplayHeadBlock2, 0x60);
                splinePointer = BAToUInt32(gameplayHeadBlock2, 0x78);
                break;
            case 4:
                byte[] gameplayHeadBlock3 = ReadBlock(fs, 0, 0x78);
                mobyPointer = BAToUInt32(gameplayHeadBlock3, 0x30);
                mobyElemSize = 0x70;
                pVarListPointer = BAToUInt32(gameplayHeadBlock3, 0x40);
                pVarPointer = BAToUInt32(gameplayHeadBlock3, 0x44);
                splinePointer = BAToUInt32(gameplayHeadBlock3, 0x5C);
                break;
        }

    }
}
