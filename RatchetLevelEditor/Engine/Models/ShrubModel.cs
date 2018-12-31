﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RatchetModel;

namespace RatchetLevelEditor.Engine.Models
{
    class ShrubModel
    {
        public float ind_00;
        public float ind_04;
        public float ind_08;
        public float ind_0C;

        public uint ptr_vert;
        public uint ptr_uv;
        public uint ptr_ind;
        public uint ptr_tex;

        public byte[] vertexData;
        public byte[] uVData;
        public byte[] indexData;
        public byte[] textureData;

        public uint ind_20;
        public int vertexCount;
        public int texCount; //Uint16 (0x28)
        public int ind_2A; //Uint16 (0x2A)
        public float ind_2C;

        public short ind_32; //Uint16 (0x30)
        public short modelId; //Uint16 (0x32)
        public uint ind_34;
        public uint ind_38;
        public uint rgba;

        public RatchetModel_General modelData;


    }
}
