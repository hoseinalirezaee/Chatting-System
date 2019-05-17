using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileTransfer
{
    /// <summary>
    /// <para>
    /// This class is used to send file using <see cref="System.Net.Sockets.TcpClient"/>.
    /// </para>
    /// <para>
    /// Each file is sent over a dedicated instance of <see cref="TcpClient"/>. In other words,
    /// when sending file is completed, the instance of <see cref="TcpClient"/> is closed.
    /// </para>
    /// </summary>
    public class FileSender
    {
        private IPEndPoint endPoint = null;

        public IPEndPoint LocalEndPoint
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Create an instance of <see cref="FileSender"/> with specified patameters.
        /// </summary>
        /// <param name="remoteEndPoint">
        /// Destination end point that file is going to send to.
        /// </param>
        /// <exception cref="ArgumentNullException"/>
        public FileSender(IPEndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
                throw new ArgumentNullException("senderSocket", "This parameter cannot be null");

            this.endPoint = remoteEndPoint;
        }

        /// <summary>
        /// Send a file specified with a path string.
        /// </summary>
        /// 
        /// <param name="path">Path to file</param>
        /// 
        /// <exception cref="FileNotFoundException">
        /// The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open,
        /// and the file specified by path does not exist. The file must already exist in
        /// these modes.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// path is null.
        /// </exception>
        /// 
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters,
        /// and file names must be less than 260 characters.
        /// </exception>
        /// 
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// path is an empty string (""), contains only white space, or contains one or more
        /// invalid characters. -or- path refers to a non-file device, such as "con:", "com1:",
        /// "lpt1", etc. in an NTFS enviroment.
        /// </exception>
        /// 
        /// <exception cref="NotSupportedException">
        /// path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a
        /// non-NTFS environment.
        /// </exception>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// mode contains an invalid value.
        /// </exception> 
        /// 
        /// <exception cref="UnauthorizedAccessException">
        /// The access requested is not permitted by the operating system for the specified
        /// path, such as when access is Write or ReadWrite and the file or directory is
        /// set for read-only access.
        /// </exception>
        /// 
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        public void SendFile(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            SendFile(fileStream);

        }

        /// <summary>
        /// Send specified instance of <see cref="FileInfo"/>.
        /// </summary>
        /// 
        /// <param name="file">An instance of <see cref="FileInfo"/> that should be sent.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// file parameter is null.
        /// </exception>
        /// 
        /// <exception cref="UnauthorizedAccessException">
        /// path is read-only or is directory.
        /// </exception>
        /// 
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        ///<exception cref="IOException">
        ///The file is already open.
        /// </exception> 
        public void SendFile(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file", "This parameter cannot be null.");

            var fileStream = file.OpenRead();

            SendFile(fileStream);
        }

        /// <summary>
        /// send bytes in specified instance of <see cref="FileStream"/>.
        /// </summary>
        /// 
        /// <param name="fileStream"></param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// fileStream parameter is null.
        /// </exception>
        public void SendFile(FileStream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentNullException("fileStream", "This parameter cannot be null.");

            var senderSocket = ConnectToEndPoint(this.endPoint);
            var networkStream = senderSocket.GetStream();

            var fileName = Path.GetFileName(fileStream.Name);
            var fileNameBytes = MarshallFileName(fileName);

            BinaryWriter writer = new BinaryWriter(networkStream);
            writer.Write(fileNameBytes.Length);
            writer.Write(fileNameBytes);
            writer.Flush();
            fileStream.CopyTo(networkStream);

            senderSocket.Close();
        }

        /// <exception cref="SocketException">
        /// An error occurred when accessing the socket.
        /// </exception>
        private TcpClient ConnectToEndPoint(IPEndPoint endPoint)
        {
            TcpClient senderSocket = new TcpClient();

            if (LocalEndPoint != null)
                senderSocket.Client.Bind(LocalEndPoint);
            senderSocket.Connect(endPoint);

            return senderSocket;
        }

        private byte[] MarshallFileName(string fileName)
        {
            var fileNameBytes = Encoding.Unicode.GetBytes(fileName);
            return fileNameBytes;
        }
    }
}
