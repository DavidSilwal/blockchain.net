﻿using Blockchain.NET.Blockchain.Network.Helpers;
using Blockchain.NET.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Settings
{
    public class NodeList
    {
        public List<NetworkNode> Nodes { get; set; } = new List<NetworkNode>();

        private static string _configName = "nodelist.json";
        private static string _rootPath = "Data";

        public NodeList()
        {
        }

        public void Save()
        {
            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);
            File.WriteAllText(Path.Combine(_rootPath, _configName), SerializeHelper.Serialize(this, Newtonsoft.Json.Formatting.Indented));
        }

        public static NodeList Load()
        {
            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);
            if (!File.Exists(Path.Combine(_rootPath, _configName)))
            {
                var nodesList = new NodeList();
                nodesList.Nodes.Add(new NetworkNode() { IPAddress = NetworkHelper.GetLocalIPAddress(), LastConnectionAttempt = DateTime.Now });
                nodesList.Save();
            }
            var nodeList = SerializeHelper.Deserialize<NodeList>(File.ReadAllText(Path.Combine(_rootPath, _configName)));
            return nodeList;
        }
    }
}
