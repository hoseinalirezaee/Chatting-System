using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileTransfer
{
    public class FileReciever
    {
        private int port = -1;

        /// <summary>
        /// Create an instance of <see cref="FileReciever"/> for listening on specified port.
        /// </summary>
        /// <param name="port">
        /// This port is used to listen for incoming cennection for receiving file.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Port number is not between <see cref="IPEndPoint.MinPort"/> and <see cref="IPEndPoint.MaxPort"/>.
        /// </exception>
        public FileReciever(int port)
        {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                throw new ArgumentException("Port number is not valid.");

            this.port = port;
        }

        /// <summary>
        /// Recieve a file from specified remoteEndPoint and save it in specified directoryPath.
        /// This method uses an instance of <see cref="TcpListener"/> and <see cref="TcpClient"/> to receive file.
        /// </summary>
        /// <returns>
        /// Returns true if specified remote end point connects and file recieved.
        /// Returns false if connected remote end point is not specified remote end point.
        /// </returns>
        /// <param name="remoteEndPoint">
        /// The end point that file should reach from
        /// </param>
        /// <param name="directoryPath">
        /// Directory where received file should save there
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// One of parameter is null.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// directoryPath is not found or is invalid.
        /// </exception>
        public bool RecieveFile(IPEndPoint remoteEndPoint, string directoryPath)
        {
            if (remoteEndPoint == null)
                throw new ArgumentNullException("remoteEndPoint", "This patameter cannot be null.");
            if (directoryPath == null)
                throw new ArgumentNullException("directoryPath", "This parameter cannot be null.");
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException(directoryPath + " not found or is invalid.");

            TcpClient recieverSocket = AcceptConnection(remoteEndPoint);
            if (recieverSocket == null)
                return false;

            var networkStream = recieverSocket.GetStream();

            var fileName = RetrieveFileNameFromStream(networkStream);
            string filePath = directoryPath + "\\" + fileName;
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);

            networkStream.CopyTo(fileStream);

            recieverSocket.Close();
            fileStream.Close();

            return true;
        }

        /// <summary>
        /// Recieve a file from specified remoteEndPoint and save it in specified instance of <see cref="DirectoryInfo"/>.
        /// This method uses an instance of <see cref="TcpListener"/> and <see cref="TcpClient"/> to receive file.
        /// </summary>
        /// <param name="remoteEndPoint">
        /// The end point that file should reach from
        /// </param>
        /// <param name="directory">
        /// An instance of <see cref="DirectoryInfo"/> that received file should save there
        /// </param>
        /// <returns>
        /// Returns true if specified remote end point connects and file recieved.
        /// Returns false if connected remote end point is not specified remote end point.
        /// </returns>
        public bool RecieveFile(IPEndPoint remoteEndPoint, DirectoryInfo directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory", "This parameter cannot be null.");

            var directoryPath = directory.FullName;
            return RecieveFile(remoteEndPoint, directoryPath);
        }

        private TcpClient AcceptConnection(IPEndPoint remoteEndPoint)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            TcpClient recieverSocket = listener.AcceptTcpClient();
            if (!recieverSocket.Client.RemoteEndPoint.Equals(remoteEndPoint as EndPoint))
                return null;
            return recieverSocket;
        }

        private string RetrieveFileNameFromStream(NetworkStream netStream)
        {
            BinaryReader reader = new BinaryReader(netStream);
            var fileNameSize = reader.ReadInt32();
            byte[] fileNameBytes = reader.ReadBytes(fileNameSize);
            var fileName = Encoding.Unicode.GetString(fileNameBytes);
            return fileName;
        }
    }
}
