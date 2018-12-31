using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RatchetTexture;
using static DataFunctions;
using ImageMagick;
using static TextureParser;
using RatchetLevelEditor.Engine;

namespace RatchetLevelEditor
{
    public partial class TextureViewer : Form
    {
        public ProgressBar pBar = new ProgressBar();
        public Label texName = new Label();
        public BackgroundWorker bg;
        public Form exportProgress = new Form();
        public Form saveProgress = new Form();
        public int exportSelIndex;
        public Main main;

        public List<ListViewItem> virtualCache = new List<ListViewItem>();
        public TextureViewer(Main main)
        {
            this.main = main;
            InitializeComponent();
            CreateBitmap();
        }

        private void TextureViewer_Load(object sender, EventArgs e)
        {
            updateTextureList();
        }
        public void updateTextureList()
        {
            textureList.Items.Clear();
            texAmountLabel.Text = "Texture Count: " + DataStoreEngine.textures.Count();
            int index = 0;
            foreach (RatchetTexture_General tex in DataStoreEngine.textures)
                textureList.Items.Add("tex_" + index++);
            updateTextureGrid();
        }
        public void updateTextureImage(RatchetTexture_General tex)
        {

            textureImage.Image = getTextureImage(tex.ID);

            if (tex.height > textureImage.Height)
                textureImage.SizeMode = PictureBoxSizeMode.StretchImage;
            if (tex.width > textureImage.Width)
                textureImage.SizeMode = PictureBoxSizeMode.StretchImage;
            else
                textureImage.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void textureList_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateTextureImage(DataStoreEngine.textures[textureList.SelectedIndex]);
        }

        void CreateBitmap()
        {
            const int colWidth = 5;
            const int rowHeight = 5;
            System.Drawing.Bitmap checks = new System.Drawing.Bitmap(
                colWidth * 10, rowHeight * 10);
            // The checkerboard consists of 10 rows and 10 columns.
            // Each square in the checkerboard is 10 x 10 pixels.
            // The nested for loops are used to calculate the position
            // of each square on the bitmap surface, and to set the
            // pixels to black or white.

            // The two outer loops iterate through 
            //  each square in the bitmap surface.
            for (int columns = 0; columns < 10; columns++)
            {
                for (int rows = 0; rows < 10; rows++)
                {
                    // Determine whether the current sqaure
                    // should be black or white.
                    Color color;
                    if (columns % 2 == 0)
                        color = rows % 2 == 0 ? Color.LightGray : Color.White;
                    else
                        color = rows % 2 == 0 ? Color.White : Color.LightGray;
                    // The two inner loops iterate through
                    // each pixel in an individual square.
                    for (int j = columns * colWidth; j < (columns * colWidth) +
                        colWidth; j++)
                    {
                        for (int k = rows * rowHeight; k < (rows * rowHeight) +
                            rowHeight; k++)
                        {
                            // Set the pixel to the correct color.
                            checks.SetPixel(j, k, color);
                        }
                    }
                    textureImage.BackgroundImage = checks;
                }
            }
        }

        private void testbutt_Click(object sender, EventArgs e)
        {
            updateTextureGrid();
        }
        public void updateTextureGrid()
        {
            texListView.Items.Clear();
            texImages.Images.Clear();
            virtualCache.Clear();
            int index = 0;

            texListView.VirtualListSize = DataStoreEngine.textures.Count;
            //image4 = Microsoft.Xna.Framework.Graphics.Texture2D.FromFile()

            for (int i = 0; i < DataStoreEngine.textures.Count; i++)
            {
                virtualCache.Add(new ListViewItem("tex_" + i, i));
            }

            ThreadStart tstart = new ThreadStart(delegate ()
            {
                loadForGrid(null, -1, DataStoreEngine.textures.Count);
            });
            Thread thread = new Thread(tstart);
            thread.Start();

            /*for(int i = 0; i < textureAmount -1; i++) {
                //Console.WriteLine("test" + i);
                //texListView.Items.Add("a", "tex_" + i, i);
                //texImages.Images.Add(getTextureImage(i));
                //texImages.Images.Add(Properties.Resources.load);
            }
            for (int j = 0; j < textureAmount; j++ )
                texListView.Items.Add(null, "tex_" + j, j);*/
            //texListView.LargeImageList = texImages;
        }
        public void loadForGrid(Image image, int index, int test)
        {

            if (this.InvokeRequired)
            {
                for (int i = 0; i < test; i++)
                {
                    Image images = getTextureImage(DataStoreEngine.textures[i].ID);
                    this.Invoke(new MethodInvoker(delegate { loadForGrid(images, i, -1); }));
                }
                return;
            }
            texImages.Images.Add("tex_" + index, image);
            if (index >= DataStoreEngine.textures.Count - 1)
                texListView.Refresh();
        }

        private void texListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (DataStoreEngine.textures != null && e.ItemIndex >= 0 && e.ItemIndex < virtualCache.Count)
            {
                //A cache hit, so get the ListViewItem from the cache instead of making a new one.
                e.Item = virtualCache[e.ItemIndex];
            }
            else
            {
                //A cache miss, so create a new ListViewItem and pass it back.
                int x = e.ItemIndex * e.ItemIndex;
                e.Item = new ListViewItem(x.ToString());
            }

        }
        //Convert RGB to BGR
        public Bitmap imgReverseRGB(Bitmap img)
        {
            try
            {
                Bitmap temp = (Bitmap)img;
                Bitmap bmap = (Bitmap)temp.Clone();
                for (int i = 0; i < bmap.Width; i++)
                {
                    for (int j = 0; j < bmap.Height; j++)
                    {
                        Color c = bmap.GetPixel(i, j);
                        bmap.SetPixel(i, j,
                        Color.FromArgb(c.A, c.B, c.G, c.R));
                    }
                }
                return (Bitmap)bmap.Clone();
            }
            catch (Exception protectedMemory)
            {
                Console.WriteLine("Attempted to overwrite protected memory? - Reverse Colors");
                return img;
            }
        }

        private void texListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection col = texListView.SelectedIndices;
            if(col.Count > 0)
            textureList.SelectedIndex = col[0];
        }

        private void exportAllFunction(object sender, EventArgs e)
        {
            exportSelIndex = int.Parse((string)((ToolStripMenuItem)sender).Tag);
            exportAllFolderSelect.SelectedPath = DataStoreGlobal.workingDirectory;
            if (exportAllFolderSelect.ShowDialog() == DialogResult.OK)
            {
                // Configure a BackgroundWorker to perform your long running operation.
                bg = new BackgroundWorker();
                bg.ProgressChanged += new ProgressChangedEventHandler(exportall_progress);
                bg.DoWork += new DoWorkEventHandler(exportall_working_background);
                bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(exportall_completed_background);
                bg.WorkerReportsProgress = true;
                bg.WorkerSupportsCancellation = true;

                // Start the worker.
                bg.RunWorkerAsync();

                // Display the loading form.
                texExportProgress();

                pBar.Value = 0;
            }
        }

        private void exportall_progress(object sender, ProgressChangedEventArgs e)
        {
            // The progress percentage is a property of e
            string[] extensions = new string[] { ".png", ".dxt5", ".dds" };
            pBar.Value = (int)(((double)e.ProgressPercentage / (double)DataStoreEngine.textures.Count) * 100);
            if (e.ProgressPercentage < DataStoreEngine.textures.Count - 1)
                texName.Text = "Exporting... tex_" + e.ProgressPercentage + extensions[exportSelIndex];
            else
                texName.Text = "Exporting... Done!";
        }

        private void exportall_working_background(object sender, DoWorkEventArgs e)
        {
            // Perform your long running operation here.
            // If you need to pass results on to the next
            // stage you can do so by assigning a value
            // to e.Result.
            //BackgroundWorker backgroundWorker = sender as BackgroundWorker;
            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;
            myEncoder = System.Drawing.Imaging.Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);
            switch (exportSelIndex)
            {
                case 0: //PNG
                default:
                    for (int i = 0; i < DataStoreEngine.textures.Count; i++)
                    {
                        getTextureImage(DataStoreEngine.textures[i].ID).Save(exportAllFolderSelect.SelectedPath + "/tex_" + i + ".png", ImageFormat.Png);
                        bg.ReportProgress(i);
                        if (bg.CancellationPending)
                            break;
                    }
                    break;
                case 1: //DXT5
                    for (int i = 0; i < DataStoreEngine.textures.Count; i++)
                    {
                        File.WriteAllBytes(exportAllFolderSelect.SelectedPath + "/tex_" + i + ".dxt3", DataStoreEngine.textures[i].texData);
                        bg.ReportProgress(i);
                        if (bg.CancellationPending)
                            break;
                    }
                    break;
                case 2: //DDS
                    for (int i = 0; i < DataStoreEngine.textures.Count; i++)
                    {
                        int width = DataStoreEngine.textures[i].width;
                        int height = DataStoreEngine.textures[i].height;
                        byte[] texData = DataStoreEngine.textures[i].texData;
                        IEnumerable<byte> ddsConcat = Constants.ddsHeader.Concat(texData);
                        byte[] ddsBuilt = ddsConcat.ToArray();
                        writeBytes(ddsBuilt, 0x0C, (uint)height << 16, 4);
                        writeBytes(ddsBuilt, 0x10, (uint)width << 16, 4);
                        File.WriteAllBytes(exportAllFolderSelect.SelectedPath + "/tex_" + i + ".dds", ddsBuilt);
                        bg.ReportProgress(i);
                        if (bg.CancellationPending)
                            break;
                    }
                    break;

            }
            // TODO: update e.Result with result, if required.

            // provide feedback to the other thread that the
            // cancellation was processed
            if (bg.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                // report end of processing
                bg.ReportProgress(DataStoreEngine.textures.Count());
            }
        }
        private void exportall_completed_background(object sender, RunWorkerCompletedEventArgs e)
        {
            // Retrieve the result pass from bg_DoWork() if any.
            // Note, you may need to cast it to the desired data type.
            //object result = e.Result;

            // Close the loading form.
            exportProgress.Hide();
            Console.WriteLine("Texture export completed!");
            bg = null;

            // Update any other UI controls that may need to be updated.
        }
        //Progress bar for exporting textures
        private void texExportProgress()
        {
            System.Drawing.Size size = new System.Drawing.Size(300, 80);
            exportProgress.Icon = main.Icon;

            exportProgress.StartPosition = FormStartPosition.CenterScreen;
            exportProgress.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            exportProgress.ClientSize = size;
            exportProgress.Text = "Export Progress";

            texName.Size = new System.Drawing.Size(size.Width - 10, 13);
            texName.Location = new System.Drawing.Point(1, 5);
            texName.Text = "Export tex name goes here";
            texName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            exportProgress.Controls.Add(texName);

            pBar.Size = new System.Drawing.Size(size.Width - 10, 23);
            pBar.Location = new System.Drawing.Point(5, 23);
            pBar.Minimum = 0;
            exportProgress.Controls.Add(pBar);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 50);
            cancelButton.Click += new EventHandler(Cancel_Click);
            exportProgress.Controls.Add(cancelButton);

            exportProgress.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.expPbWindow_Close);

            exportProgress.ShowDialog();
        }
        void Cancel_Click(object sender, EventArgs e)
        {
            bg.CancelAsync();
        }
        private void expPbWindow_Close(object sender, FormClosingEventArgs e)
        {
            exportProgress.Hide();
            e.Cancel = true; // this cancels the close event.
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (addTextureDialog.ShowDialog() == DialogResult.OK)
            {
                int width = 0;
                int height = 0;
                byte[] img = File.ReadAllBytes(addTextureDialog.FileName);

                string extension = Path.GetExtension(addTextureDialog.FileName).ToLower();
                Console.WriteLine(addTextureDialog.FileName);
                switch (extension)
                {
                    case ".bmp":
                    case ".png":
                    case ".jpg":
                        Console.WriteLine("Adding new PNG texture");
                        using (MagickImage image = new MagickImage(addTextureDialog.FileName))
                        {
                            image.Format = MagickFormat.Dxt5;
                            addNewTexture(removeHeader(image.ToByteArray()), image.Width, image.Height);
                        }
                        break;
                    case ".dxt5":
                       /* if (ShowSizeInputDialog(ref width, ref height) == DialogResult.OK)
                        {
                            addNewTexture(img, width, height);
                        }*/
                        break;
                    case ".dds":
                        Console.WriteLine("Adding new DDS texture");
                        width = ReadInt32(img, 0x10);
                        height = ReadInt32(img, 0x0C);
                        addNewTexture(removeHeader(img), width, height);
                        break;

                }
            }
        }
        //Removes DDS header
        public byte[] removeHeader(byte[] input)
        {
            byte[] newData = new byte[input.Length - 0x80];
            for (int i = 0; i < input.Length - 0x80; i++)
            {
                newData[i] = input[0x80 + i];
            }
            return newData;
        }
        public void addNewTexture(byte[] image, int width, int height)
        {
           // MessageBox.Show("Broken for now");
            byte[] zeros = new byte[0x10];
            byte[] texDef = Constants.textureTemplate;

            //Writes our image size
            writeBytes(texDef, 0x18, (uint)width << 16, 2);
            writeBytes(texDef, 0x1A, (uint)height << 16, 2);

            RatchetTexture_General newTex = new RatchetTexture_General()
            {
                ID = DataStoreEngine.textures.Count,
                height = height,
                width = width,
                texHeader = texDef,
                texData = image,
                reverseRGB = false

            };
            DataStoreEngine.textures.Add(newTex);
            updateTextureList();
            //Inserts 4 lines of 0s which indicates start of new texture
           // Form1.vramData = insertNew(Form1.vramData, zeros, Form1.vramData.Length);

            //Writes our new pointer for texture data
            //writeBytes(texDef, 0x00, (uint)Form1.vramData.Length, 4);

            //Writes our image size
            //writeBytes(texDef, 0x18, (uint)width << 16, 2);
            //writeBytes(texDef, 0x1A, (uint)height << 16, 2);

            //Inserts our newest texture def into engine file byte array
            //Form1.engineConfig = insertNew(Form1.engineConfig, texDef, (textureStart + (0x24 * (textureAmount))));

            //Inserts our new texture data into vram file byte array
            //Form1.vramData = insertNew(Form1.vramData, image, Form1.vramData.Length);

            //Increase any pointers that might have had data moved by the data insertion
            //updatePointers(Form1.engineConfig, 0x60, 0x20, 0x88);

            //Increase texture amount + 1
            //writeBytes(Form1.engineConfig, 0x64, (uint)textureAmount + 1, 4);

            //Refresh to show newest texture
            //updateTextureList(texFile);

        }

        private void textureReplaceCM_Click(object sender, EventArgs e)
        {

        }

        private void exportAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
