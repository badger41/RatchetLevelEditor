using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataFunctions;

class EngineHeader
{
    public uint spawnablesPointer;
    //(0x04)Map render definitions
    //(0x08)null
    //(0x0C)null
    //(0x10)Skybox
    public uint collisionPointer;
    //(0x18)Player animations
    public uint levelModelsPointer;
    public uint levelModelsCount;
    public uint levelObjectPointer;
    public uint levelObjectCount;
    public uint sceneryModelsPointer;
    public uint sceneryModelsCount;
    public uint sceneryObjectsPointer;
    public uint sceneryObjectsCount;
    public uint terrainPointer;
    //(0x40)null
    //(0x44)null
    //(0x48)Sound config
    //(0x4C)Weapons pointer (rac1)
    //(0x50)Weapons count (rac1)
    public uint texturesPointer;
    public uint textureCount;
    //(0x5C)Lighting pointer
    public uint lightingLevel;
    //(0x64)Lighting config
    //(0x68)Texture config menu
    public uint textureConfigMenuCount;
    //(0x70)textureConfig2DGFX
    //(0x74)Sprite def

    public EngineHeader(FileStream fs, uint game)
    {
        switch (game)
        {
            case 1:
            case 2:
            case 3:
                getVals123(fs);
                break;

            case 4:
                getValsDL(fs);
                break;
        }

    }

    private void getVals123(FileStream fs)
    {
        byte[] engineHeader = ReadBlock(fs, 0, 0x78);
        spawnablesPointer = BAToUInt32(engineHeader, 0x00);
        collisionPointer = BAToUInt32(engineHeader, 0x14);
        levelModelsPointer = BAToUInt32(engineHeader, 0x1C);
        levelModelsCount = BAToUInt32(engineHeader, 0x20);
        levelObjectPointer = BAToUInt32(engineHeader, 0x24);
        levelObjectCount = BAToUInt32(engineHeader, 0x28);
        sceneryModelsPointer = BAToUInt32(engineHeader, 0x2C);
        sceneryModelsCount = BAToUInt32(engineHeader, 0x30);
        sceneryObjectsPointer = BAToUInt32(engineHeader, 0x34);
        sceneryObjectsCount = BAToUInt32(engineHeader, 0x38);
        terrainPointer = BAToUInt32(engineHeader, 0x3C);
        texturesPointer = BAToUInt32(engineHeader, 0x54);
        textureCount = BAToUInt32(engineHeader, 0x58);
        lightingLevel = BAToUInt32(engineHeader, 0x60);
        textureConfigMenuCount = BAToUInt32(engineHeader, 0x6C);
    }
    
    private void getValsDL(FileStream fs)
    {
        byte[] engineHeaderDL = ReadBlock(fs, 0, 0x90);
        uint spawnablesPointer = BAToUInt32(engineHeaderDL, 0x00);
        //(0x04)Map render definitions
        //(0x08)null
        //(0x0C)null
        //(0x14)Skybox
        collisionPointer = BAToUInt32(engineHeaderDL, 0x18);
        levelModelsPointer = BAToUInt32(engineHeaderDL, 0x20);
        levelModelsCount = BAToUInt32(engineHeaderDL, 0x24);
        levelObjectPointer = BAToUInt32(engineHeaderDL, 0x28);
        levelObjectCount = BAToUInt32(engineHeaderDL, 0x2C);
        sceneryModelsPointer = BAToUInt32(engineHeaderDL, 0x34);
        sceneryModelsCount = BAToUInt32(engineHeaderDL, 0x38);
        sceneryObjectsPointer = BAToUInt32(engineHeaderDL, 0x3C);
        sceneryObjectsCount = BAToUInt32(engineHeaderDL, 0x40);
        terrainPointer = BAToUInt32(engineHeaderDL, 0x48);
        //(0x44) ??
        //(0x54)Sound config (Menus + terrain)
        texturesPointer = BAToUInt32(engineHeaderDL, 0x60);
        uint textureCount = BAToUInt32(engineHeaderDL, 0x64);
        //(0x68)Lighting pointer
        lightingLevel = BAToUInt32(engineHeaderDL, 0x6C);
        textureConfigMenuCount = BAToUInt32(engineHeaderDL, 0x78);
        //(0x7C)textureConfig2DGFX
        //(0x80)Sprite def
        //(0x88) sound config (spawnables + everything else)
    }
}