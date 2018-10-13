using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatchetLevelEditor
{
	public enum RCGame {
		RatchetAndClank = 1,
		GoingCommando = 2,
		UpYourArsenal = 3,
		Deadlocked = 4
	};

	class Constants
    {
        public static string[,] errorCodes = {
                                                 {"Unknown or unsupported file type: %s.","Unsupported File"},
                                                 {"Cannot find file: %s.","Cannot Find File"},
                                                 {"Missing texture definitions, texture data unreadable.", "Missing Data"},
                                                 {"Input values must have 4 byte alignment.", "Alignment Error"},
                                                 {"Saving textures of this file type are not supported yet.","Type Not Supported"},
                                                 {"Map field cannot be empty. Select a map.", "Empty Directory"},
                                             };
        public static string[,] fileNames = {
                                                    {"engine.ps3", "gameplay_ntsc", "vram.ps3"},
                                                    {"gameplay_ntsc", "engine.ps3","vram.ps3"},
                                                    {"vram.ps3", "engine.ps3", "gameplay_ntsc"},
                                                    {"hud.ps3", "hud.vram", null},
                                                    {"boot.ps3", "boot.vram", null},
                                                    {"eurostile_boldextendedtwo_pages_e.ps3", "eurostile_boldextendedtwo_pages_e.vram", null},
                                                    {"eurostile_boldextendedtwo_pages_j.ps3","eurostile_boldextendedtwo_pages_j.vram", null},
                                                    {"eurostile_demi_pages_e.ps3","eurostile_demi_pages_e.vram", null},
                                                    {"data.ps3", "data.vram", null},
                                                    {"gameplay_mission_data[%d].dat", null, null},
                                                    {"gadgets.ps3","gadgets.vram",null },
                                                    {"skin%d.ps3","skin%d.vram", null },
                                                    {"mobyload%d.ps3",null, null },
                                             };
        public static byte[] textureTemplate = {
                                                      0x00, 0x00 ,0x00, 0x00, //vram Pointer
                                                      0x00, 0x01, 0x88, 0x29, //Always ends in 8829, the 01 might be transparency?
                                                      0x00, 0x01, 0x03, 0x03, //03 can also be 01, not sure of usage
                                                      0x80, 0x00, 0x00, 0x00, //Changes sometimes, unsure of usage
                                                      0x00, 0x00, 0xAA, 0xE4,
                                                      0x02, 0x06, 0x3E, 0x80,
                                                      0x04, 0x00, 0x04, 0x00, //Size
                                                      0x00, 0x10, 0x00, 0x00,
                                                      0x00, 0xFF, 0x00, 0x00,
                                               };
        public static byte[] ddsHeader = {
                                                      0x44, 0x44, 0x53, 0x20,
                                                      0x7C, 0x00, 0x00, 0x00,
                                                      0x07, 0x10, 0x0A, 0x00,
                                                      0x00, 0x01, 0x00, 0x00, //height
                                                      0x00, 0x01, 0x00, 0x00, //width
                                                      0x00, 0x04, 0x00, 0x00, //idk but needed
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x09, 0x00, 0x00, 0x00, //idk but needed
                                                      0x52, 0x41, 0x54, 0x43,
                                                      0x48, 0x45, 0x54, 0x4C,
                                                      0x45, 0x64, 0x54, 0x4C,
                                                      0x45, 0x44, 0x49, 0x54,
                                                      0x4F, 0x52, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x20, 0x00, 0x00, 0x00,
                                                      0x04, 0x00, 0x00, 0x00,
                                                      0x44, 0x58, 0x54, 0x35, //DXT5
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x08, 0x10, 0x40, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                                      0x00, 0x00, 0x00, 0x00,
                                         };
    }
}
