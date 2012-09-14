using IMAPI2;
using IMAPI2.Interop;
using Microsoft.Win32.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace IMAPI2
{
    public class ImageMaster : IDisposable
    {
        private const string ClientName = "IMAPI2 Wrapper";

        private ReadOnlySelectableCollection<DiscRecorder> _recorders;
        private bool _isWriting;
        private long _mediaCapacity;
        private bool _mediaLoaded;        
        private PhysicalMedia _media;
        private ReadOnlyCollection<MediaState> _mediaStates;
        private List<IMediaNode> _nodes;
        private bool _recorderLoaded;
        private Thread _writerThread;

        public delegate void FormatEraseUpdateEventHandler(ImageMaster sender, FormatEraseUpdateEventArgs e);
        public delegate void FormatWriteUpdateEventHandler(ImageMaster sender, FormatWriteUpdateEventArgs e);
        public delegate void ImageUpdateEventHandler(ImageMaster sender, ImageUpdateEventArgs e);

        public event FormatEraseUpdateEventHandler FormatEraseUpdate;
        public event FormatWriteUpdateEventHandler FormatWriteUpdate;
        public event ImageUpdateEventHandler ImageUpdate;

        public PhysicalMedia Media
        {
            get { return _media; }
        }
        public ReadOnlyCollection<MediaState> MediaStates
        {
            get { return _mediaStates; }
        }
        public long MediaCapacity
        {
            get { return _mediaCapacity; }
        }
        public List<IMediaNode> Nodes
        {
            get { return _nodes; }
        }
        public ReadOnlySelectableCollection<DiscRecorder> Recorders
        {
            get { return _recorders; }
        }
        public string VolumeLabel
        {
            get;
            set;
        }

        public ImageMaster()
        {
            List<DiscRecorder> recorders = new List<DiscRecorder>();            
            _media = PhysicalMedia.Unknown;
            _nodes = new List<IMediaNode>();
            _mediaStates = new ReadOnlyCollection<MediaState>(new List<MediaState> {MediaState.Unknown});

            MsftDiscMaster2 discMaster = null;
            try
            {
                discMaster = new MsftDiscMaster2();

                if (!discMaster.IsSupportedEnvironment)
                    throw new NotSupportedException(
                        "Either the environment does not contain one or more optical devices or the " +
                        "execution context has no permission to access the devices.");

                foreach (string uniqueRecorderId in discMaster)
                {
                    recorders.Add(new DiscRecorder(uniqueRecorderId));
                }
                _recorders = new ReadOnlySelectableCollection<DiscRecorder>(recorders);
            }
            catch (COMException ex)
            {
                throw new NotSupportedException("IMAP2 not found on this system. It will need to be installed (ErrorCode = " + ex.ErrorCode + ").", ex);
            }
            finally
            {
                if (discMaster != null) Marshal.ReleaseComObject(discMaster);
            }
        }

        private void _AddNode(IFsiDirectoryItem root, IMediaNode node)
        {
            IStream stream = null;

            try
            {
                if (node is DirectoryNode)
                {
                    root.AddTree(node.Path, true);
                }
                else
                {
                    Shell.SHCreateStreamOnFile(node.Path, Shell.STGM_READ | Shell.STGM_SHARE_DENY_WRITE, ref stream);
                    if (stream != null)
                        root.AddFile(node.Name, stream);
                }
            }
            finally
            {
                if (stream != null)
                {
                    Marshal.FinalReleaseComObject(stream);
                }
            }
        }

        private void _CreateImage(IDiscRecorder2 discRecorder, object[] multisessionInterfaces, out IStream dataStream)
        {
            MsftFileSystemImage image = null;
            string volumeLabel = this.VolumeLabel;
            try
            {
                if (String.IsNullOrEmpty(volumeLabel))
                    volumeLabel = DateTime.Now.ToShortDateString();

                image = new MsftFileSystemImage();
                image.ChooseImageDefaults(discRecorder);
                image.FileSystemsToCreate = FsiFileSystems.FsiFileSystemJoliet | FsiFileSystems.FsiFileSystemISO9660;
                image.VolumeName = volumeLabel;

                image.Update += _fileSystemImage_Update;

                // If multisessions, then import previous sessions               
                if (multisessionInterfaces != null)
                {
                    image.MultisessionInterfaces = multisessionInterfaces;
                    image.ImportFileSystem();
                }

                IFsiDirectoryItem rootNode = image.Root;
                foreach (IMediaNode node in this.Nodes)
                {
                    _AddNode(rootNode, node);
                }

                image.Update -= _fileSystemImage_Update;

                dataStream = image.CreateResultImage().ImageStream;
            }
            finally
            {
                if (image != null)
                    Marshal.ReleaseComObject(image);
            }
        }

        public void FormatMedia(bool quick, bool eject)
        {
            if (!_mediaLoaded)
                throw new InvalidOperationException("LoadMedia must be called first.");

            IDiscRecorder2 recorder = _recorders.SelectedItem.Internal;
            MsftDiscFormat2Erase discFormatErase = null;

            try
            {
                discFormatErase = new MsftDiscFormat2Erase
                {
                    Recorder = recorder,
                    ClientName = ImageMaster.ClientName,
                    FullErase = !quick
                };

                discFormatErase.Update += _discFormatErase_Update;
                discFormatErase.EraseMedia();

                if (eject) recorder.EjectMedia();
            }
            finally
            {
                if (discFormatErase != null) Marshal.ReleaseComObject(discFormatErase);
            }
        }

        public void LoadMedia()
        {
            long mediaStateFlags;
            var mediaStates = new List<MediaState>();

            if (!_recorderLoaded)
                throw new InvalidOperationException("LoadRecorder must be called first.");
            if (_recorders.SelectedIndex == -1)
                throw new InvalidOperationException("No DiscRecorder selected on the DiscRecorders list.");

            MsftDiscRecorder2 recorder = _recorders.SelectedItem.Internal;
            MsftFileSystemImage image = null;
            MsftDiscFormat2Data format = null;

            try
            {
                //
                // Create and initialize the IDiscFormat2Data
                //
                format = new MsftDiscFormat2Data();
                if (!format.IsCurrentMediaSupported(recorder))
                    throw new MediaNotSupportedException("There is no media in the device.");

                //
                // Get the media type in the recorder
                //
                format.Recorder = recorder;
                _media = (PhysicalMedia)format.CurrentPhysicalMediaType;
                                
                mediaStateFlags = (long)format.CurrentMediaStatus;
                foreach (MediaState state in Enum.GetValues(typeof(MediaState)))
                {
                    if (((long)mediaStateFlags & (long)state) > 0)
                        mediaStates.Add(state);
                }
                if (mediaStates.Count == 0) mediaStates.Add(MediaState.Unknown);
                _mediaStates = new ReadOnlyCollection<MediaState>(mediaStates);

                if ((mediaStateFlags & (long)IMAPI_FORMAT2_DATA_MEDIA_STATE.IMAPI_FORMAT2_DATA_MEDIA_STATE_WRITE_PROTECTED) > 0)
                    throw new MediaNotSupportedException("The media in the device is write protected.");
                //
                // Create a file system and select the media type
                //
                image = new MsftFileSystemImage();                
                image.ChooseImageDefaultsForMediaType((IMAPI_MEDIA_PHYSICAL_TYPE)_media);

                //
                // See if there are other recorded sessions on the disc
                //
                if (!format.MediaHeuristicallyBlank)
                {
                    image.MultisessionInterfaces = format.MultisessionInterfaces;
                    image.ImportFileSystem();
                }

                _mediaCapacity = 2048 * image.FreeMediaBlocks;
                _mediaLoaded = true;
            }
            finally
            {
                if (format != null) Marshal.ReleaseComObject(format);
                if (image != null) Marshal.ReleaseComObject(image);
            }
        }

        public void LoadRecorder()
        {
            if (_recorders.SelectedIndex == -1)
                throw new InvalidOperationException("No DiscRecorder selected from the DiscRecorders list.");

            var recorder = _recorders.SelectedItem.Internal;

            //
            // Verify recorder is supported
            //
            IDiscFormat2Data discFormatData = null;
            try
            {
                discFormatData = new MsftDiscFormat2Data();
                switch (discFormatData.IsRecorderSupported(recorder))
                {
                    case 0x80004003:
                        throw new RecorderNotSupportedException("Pointer is not valid.");
                    case 0xC0AA0202:
                        throw new RecorderNotSupportedException("The selected recorder is not supported on this system.");
                }
                _recorderLoaded = true;
                _mediaLoaded = false;
            }
            finally
            {
                if (discFormatData != null)
                {
                    Marshal.ReleaseComObject(discFormatData);
                }
            }
        }

        public void WriteImage(BurnVerificationLevel verification, bool finalize, bool eject)
        {
            if (!_recorderLoaded)
                throw new InvalidOperationException("LoadMedia must be called first.");

            MsftDiscRecorder2 recorder = null;
            MsftDiscFormat2Data discFormatData = null;

            try
            {
                recorder = _recorders.SelectedItem.Internal;

                discFormatData = new MsftDiscFormat2Data
                    {
                        Recorder = recorder,
                        ClientName = ClientName,
                        ForceMediaToBeClosed = finalize
                    };

                //
                // Set the verification level
                //
                var burnVerification = (IBurnVerification)discFormatData;
                burnVerification.BurnVerificationLevel = IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_NONE;

                //
                // Check if media is blank, (for RW media)
                //
                object[] multisessionInterfaces = null;
                if (!discFormatData.MediaHeuristicallyBlank)
                    multisessionInterfaces = discFormatData.MultisessionInterfaces;

                //
                // Create the file system
                //
                IStream fileSystem;
                _CreateImage(recorder, multisessionInterfaces, out fileSystem);

                discFormatData.Update += _discFormatWrite_Update;

                //
                // Write the data
                //
                try
                {
                    discFormatData.Write(fileSystem);
                }
                finally
                {
                    if (fileSystem != null)
                    {
                        Marshal.FinalReleaseComObject(fileSystem);
                    }
                }

                discFormatData.Update -= _discFormatWrite_Update;

                if (eject)
                    recorder.EjectMedia();
            }
            finally
            {
                if (recorder != null) Marshal.ReleaseComObject(recorder);
                if (discFormatData != null) Marshal.ReleaseComObject(discFormatData);
                _isWriting = false;
            }
        }

        void _discFormatWrite_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress)
        {
            if (FormatWriteUpdate != null) FormatWriteUpdate(this, new FormatWriteUpdateEventArgs((IDiscFormat2DataEventArgs)progress));
        }
        void _discFormatErase_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, int elapsedSeconds, int estimatedTotalSeconds)
        {
            if (FormatEraseUpdate != null) FormatEraseUpdate(this, new FormatEraseUpdateEventArgs(elapsedSeconds, estimatedTotalSeconds));
        }
        private void _fileSystemImage_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender,
            [In, MarshalAs(UnmanagedType.BStr)]string currentFile, [In] int copiedSectors, [In] int totalSectors)
        {
            if (ImageUpdate != null) ImageUpdate(this, new ImageUpdateEventArgs(currentFile, copiedSectors, totalSectors));
        }

        void IDisposable.Dispose()
        {
            if ((_writerThread != null) && (_writerThread.IsAlive)) _writerThread.Join();
            foreach (DiscRecorder recorder in _recorders)
            {
                Marshal.ReleaseComObject(recorder.Internal);
            }
        }
    }
}
