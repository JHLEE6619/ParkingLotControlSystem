﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.Model;
using System.IO;

namespace Server.Controller
{
    public class FileReceiveServer
    {
        public async Task StartFileRcvServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 10000);
            Console.WriteLine(" 서버 시작 ");
            listener.Start();
            while (true)
            {
                TcpClient client =
                    await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                Task.Run(() => ServerMain(client));
            }
        }

        // 클라이언트 별로 다른 스레드가 실행하는 메소드
        private async Task ServerMain(Object client)
        {
            // 헤더 : 이미지식별자(int), 이미지 크기(int), 이미지 유형(byte) 
            // 바디 : 이미지 binary 데이터
            Console.WriteLine(" 클라이언트 연결됨 ");
            byte[] buf = new byte[1024];
            int imgId;
            long imgSize;
            byte imgType;
            byte[] imgBinary;
            int offset = 0;
            TcpClient tc = (TcpClient)client;
            NetworkStream stream = tc.GetStream();

            Dictionary<int, int> imgOffset = [];

            while (true)
            {
                int len = stream.Read(buf, 0, buf.Length);
                //await stream.ReadAsync(buf, 0, 5).ConfigureAwait(false);
                imgId = BitConverter.ToInt32(buf.AsSpan()[0..4]);
                imgSize = BitConverter.ToInt64(buf.AsSpan()[4..12]);
                imgType = buf[12];
                imgBinary = buf[13..^1];
                imgOffset.TryAdd(imgId, offset); // TryAdd 키가 없으면 추가하고 true 반환, 있으면 추가하지 않고 false 반환
                Write_img(imgBinary, imgId, imgType, imgOffset[imgId]);
                imgOffset[imgId] += 1011; // 실제로 읽은만큼 offset
            }
        }

        private void Write_img(byte[] imgBinary, int imgId, byte imgType, int offset)
        {
            string folderName = "";
            switch (imgType)
            {
                case 0: folderName = "Entrance"; break;
                case 1: folderName = "Exit"; break;
                case 2: folderName = "Parking Lot"; break;
            }
            string path = @$"/img/{folderName}";
            Console.WriteLine(Path.GetFullPath(path));

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
                Console.WriteLine("폴더 생성");
            }

            string saveDir = @$"/img/{folderName}/{imgId}.jpg";
            using (var stream = new FileStream(saveDir, FileMode.Create, FileAccess.Write))
            {              
                // 파일 쓰기
                stream.Write(imgBinary, offset, imgBinary.Length);
            }
        }
    }
}
