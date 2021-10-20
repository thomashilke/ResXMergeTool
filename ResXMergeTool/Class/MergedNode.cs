using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Resources;

namespace ResXMergeTool
{
    public class MergedNode
    {
        public string Key => (BaseNode ?? LocalNode ?? RemoteNode)?.Name ?? throw new InvalidOperationException("At least one node must be defined.");

        public ResXDataNode BaseNode { get; set; }

        public string BaseValue => (string)BaseNode.GetValue((ITypeResolutionService)null);

        public string BaseComment => BaseNode.Comment;

        public ResXDataNode LocalNode { get; set; }

        public string LocalValue => (string)LocalNode.GetValue((ITypeResolutionService)null);

        public string LocalComment => LocalNode.Comment;

        public ResXDataNode RemoteNode { get; set; }

        public string RemoteValue => (string)RemoteNode.GetValue((ITypeResolutionService)null);

        public string RemoteComment => RemoteNode.Comment;

        public bool IsInConflict()
        {
            var remoteDelta = GetRemoteDelta();
            var localDelta = GetLocalDelta();

            return (localDelta,remoteDelta) switch
            {
                (NodeDelta.Deleted, NodeDelta.Modified) => true,
                (NodeDelta.Modified, NodeDelta.Deleted) => true,
                (NodeDelta.Modified, NodeDelta.Modified) => !RemoteValue.Equals(LocalValue),
                (NodeDelta.Added, NodeDelta.Added) => !RemoteValue.Equals(LocalValue),
                _ => false
            };
        }

        private NodeDelta GetDelta(ResXDataNode node)
        {
            if (BaseNode is null && node is not null)
            {
                return NodeDelta.Added;
            }
            else if (BaseNode is not null && node is null)
            {
                return NodeDelta.Deleted;
            }
            else if (BaseNode is null && node is null)
            {
                return NodeDelta.Unchanged;
            }
            else
            {
                return BaseValue.Equals((string)node.GetValue((ITypeResolutionService)null)) ? NodeDelta.Unchanged : NodeDelta.Modified;
            }
        }

        private NodeDelta GetRemoteDelta() => GetDelta(RemoteNode);
        private NodeDelta GetLocalDelta() => GetDelta(LocalNode);

        public IEnumerable<(string key, object value, string comment, string source, ResXSourceType sourceV)> GetRows()
        {
            var remoteDelta = GetRemoteDelta();
            var localDelta = GetLocalDelta();

            switch (localDelta)
            {
                case NodeDelta.Deleted:
                    switch (remoteDelta)
                    {
                        case NodeDelta.Deleted:
                            yield break;

                        case NodeDelta.Modified:
                            yield return (Key, BaseValue, BaseComment, "DEL LOCAL", ResXSourceType.LOCAL);
                            yield return (Key, RemoteValue, RemoteComment, $"MOD REMOTE (was '{BaseValue}')", ResXSourceType.REMOTE);
                            break;

                        case NodeDelta.Added:
                            throw new InvalidOperationException("Not possible, sir!");

                        case NodeDelta.Unchanged:
                            yield break;
                    }
                    break;

                case NodeDelta.Modified:
                    switch (remoteDelta)
                    {
                        case NodeDelta.Deleted:
                            yield return (Key, BaseValue, BaseComment, "DEL REMOTE", ResXSourceType.REMOTE);
                            yield return (Key, LocalValue, LocalComment, $"MOD LOCAL (was '{BaseValue}')", ResXSourceType.LOCAL);
                            break;

                        case NodeDelta.Modified:
                            if (RemoteValue.Equals(LocalValue))
                            {
                                yield return (Key, LocalValue, LocalComment, "MOD BOTH", ResXSourceType.LOCAL_REMOTE);
                            }
                            else
                            {
                                yield return (Key, RemoteValue, RemoteComment, $"MOD REMOTE (was '{BaseValue}')", ResXSourceType.REMOTE);
                                yield return (Key, LocalValue, LocalComment, $"MOD LOCAL (was '{BaseValue}')", ResXSourceType.LOCAL);
                            }
                            break;

                        case NodeDelta.Added:
                            throw new InvalidOperationException("Not possible, sir!");

                        case NodeDelta.Unchanged:
                            yield return (Key, LocalValue, LocalComment, $"MOD LOCAL (was '{BaseValue}')", ResXSourceType.LOCAL);
                            break;
                    }
                    break;

                case NodeDelta.Added:
                    switch (remoteDelta)
                    {
                        case NodeDelta.Deleted:
                        case NodeDelta.Modified:
                            throw new InvalidOperationException("Not possible, sir!");

                        case NodeDelta.Added:
                            if (RemoteValue.Equals(LocalValue))
                            {
                                yield return (Key, LocalValue, LocalComment, "ADD BOTH", ResXSourceType.LOCAL_REMOTE);
                            }
                            else
                            {
                                yield return (Key, RemoteValue, RemoteComment, "ADD REMOTE", ResXSourceType.REMOTE);
                                yield return (Key, LocalValue, LocalComment, "ADD LOCAL", ResXSourceType.LOCAL);
                            }
                            break;

                        case NodeDelta.Unchanged:
                            yield return (Key, LocalValue, LocalComment, "ADD LOCAL", ResXSourceType.LOCAL);
                            break;
                    }
                    break;

                case NodeDelta.Unchanged:
                    switch (remoteDelta)
                    {
                        case NodeDelta.Deleted:
                            yield break;

                        case NodeDelta.Modified:
                            yield return (Key, RemoteValue, RemoteComment, "MOD REMOTE", ResXSourceType.REMOTE);
                            break;

                        case NodeDelta.Added:
                            yield return (Key, RemoteValue, RemoteComment, "ADD REMOTE", ResXSourceType.LOCAL);
                            break;

                        case NodeDelta.Unchanged:
                            yield return (Key, LocalNode is not null ? LocalValue : RemoteValue, LocalNode is not null ? LocalComment : RemoteComment, "UNCH BOTH", ResXSourceType.ALL);
                            break;
                    }
                    break;
            }
        }
    }
}
