using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace ResXMergeTool
{
    public class FileParser
    {
        private string _workingPath = Directory.GetCurrentDirectory();

        public FileParser(string path)
        {
            _workingPath = path;
        }

        public Dictionary<string, MergedNode> NodeDictionary { get; private set; }

        public IDictionary<string, int> SortOrder { get; private set; }

        public static ResXSourceType GetResXSourceTypeFromFileName(string file)
        {
            if (file.Contains(".resx", StringComparison.OrdinalIgnoreCase))
            {
                if (file.Contains("base", StringComparison.OrdinalIgnoreCase))
                    return ResXSourceType.BASE;
                else if (file.Contains("local", StringComparison.OrdinalIgnoreCase))
                    return ResXSourceType.LOCAL;
                else if (file.Contains("remote", StringComparison.OrdinalIgnoreCase))
                    return ResXSourceType.REMOTE;
                else
                    return ResXSourceType.UNKOWN;
            }
            else return ResXSourceType.UNKOWN;
        }

        /// <summary>
        /// Parses all given files and compares them with other read files
        /// Code by B.O.B. (https://www.codeproject.com/script/Membership/View.aspx?mid=3598385) under CPOL License (http://www.codeproject.com/info/cpol10.aspx)
        /// </summary>
        /// <param name="files"></param>
        public void ParseResXFiles(string baseVersion, string localVersion, string remoteVersion)
        {
            SortOrder = getSortOrder(localVersion);

            var nodeDict = new Dictionary<string, MergedNode>();
            processFile(baseVersion, ResXSourceType.BASE, nodeDict);
            processFile(localVersion, ResXSourceType.LOCAL, nodeDict);
            processFile(remoteVersion, ResXSourceType.REMOTE, nodeDict);

            NodeDictionary = nodeDict;

            void processFile(string fileName, ResXSourceType source, Dictionary<string, MergedNode> dictionary)
            {
                String file = Path.Combine(_workingPath, fileName);

                ResXResourceReader resx = new ResXResourceReader(file)
                {
                    UseResXDataNodes = true
                };

                IDictionaryEnumerator dict = resx.GetEnumerator();

                while (dict.MoveNext())
                {
                    ResXDataNode node = (ResXDataNode)dict.Value;
                    var key = node.Name;

                   if (dictionary.TryGetValue(key, out var mergedNode))
                    {
                        switch (source)
                        {
                            case ResXSourceType.BASE:
                                if (mergedNode.BaseNode is null)
                                {
                                    mergedNode.BaseNode = node;
                                }
                                break;
                            case ResXSourceType.LOCAL:
                                if (mergedNode.LocalNode is null)
                                {
                                    mergedNode.LocalNode = node;
                                }
                                break;
                            case ResXSourceType.REMOTE:
                                if (mergedNode.RemoteNode is null)
                                {
                                    mergedNode.RemoteNode = node;
                                }
                                break;
                        }
                    }
                   else
                    {
                        var newNode = new MergedNode();
                        switch (source)
                        {
                            case ResXSourceType.BASE:
                                newNode.BaseNode = node;
                                break;
                            case ResXSourceType.LOCAL:
                                newNode.LocalNode = node;
                                break;
                            case ResXSourceType.REMOTE:
                                newNode.RemoteNode = node;
                                break;
                        }
                        dictionary[key] = newNode;
                    }
                }
            }

            IDictionary<string, int> getSortOrder(string fileName)
            {
                String file = Path.Combine(_workingPath, fileName);

                ResXResourceReader resx = new ResXResourceReader(file)
                {
                    UseResXDataNodes = true
                };

                IDictionaryEnumerator dict = resx.GetEnumerator();

                int index = 0;
                var order = new Dictionary<string, int>();
                while (dict.MoveNext())
                {
                    ResXDataNode node = (ResXDataNode)dict.Value;
                    var key = node.Name;
                    order[key] = index;
                    index += 1;
                }

                return order;
            }
        }
    }
}
