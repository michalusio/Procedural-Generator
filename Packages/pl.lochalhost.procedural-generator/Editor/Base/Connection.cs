using Packages.pl.lochalhost.procedural_generator.Editor.Packages.pl.lochalhost.procedural_generator.Editor.Base;
using Packages.pl.lochalhost.procedural_generator.Runtime;
using System;
using System.Linq;
using UnityEngine.UIElements;

namespace Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    public class Connection : ISerializable<Connection, SerializableConnection>
    {
        public NodeIn To { get; private set; }
        public NodeOut From { get; private set; }
        private readonly BezierElement bezierCurve = new BezierElement();
        private readonly RootElement root;

        private Connection(RootElement root)
        {
            this.root = root;
            root.Add(bezierCurve);
            bezierCurve.SendToBack();
            root.RegisterCallback<MouseMoveEvent>(MouseMove);
        }

        public void Remove()
        {
            root.UnregisterCallback<MouseMoveEvent>(MouseMove);
            bezierCurve.RemoveFromHierarchy();
            root.Window.Connections.Remove(this);
            To?.Node?.OnInputUnlinked(this);
            From?.Node?.OnOutputUnlinked(this);
            To?.Connections?.Remove(this);
            From?.Connections?.Remove(this);
        }

        public void MouseMove(MouseMoveEvent mme)
        {
            if (From == null)
            {
                bezierCurve.PositionA = mme.mousePosition;
            }
            if (To == null)
            {
                bezierCurve.PositionB = mme.mousePosition;
            }
            Update();
        }

        public void Update()
        {
            if (From != null)
            {
                var fromSocket = From.Children().ElementAt(1);
                bezierCurve.PositionA = fromSocket.worldBound.center;
            }
            if (To != null)
            {
                var toSocket = To.Children().ElementAt(0);
                bezierCurve.PositionB = toSocket.worldBound.center;
            }
            bezierCurve.MarkDirtyRepaint();
        }

        public void LinkTo(NodeIn ni)
        {
            if (To != null) throw new ApplicationException();
            To = ni;
            ni.Connections.Add(this);
            To.Node.OnInputLinked(this);
            From.Node.OnOutputLinked(this);
            Update();
            TryUpdateValue();
            root.UnregisterCallback<MouseMoveEvent>(MouseMove);
        }

        public void LinkFrom(NodeOut no)
        {
            if (From != null) throw new ApplicationException();
            From = no;
            no.Connections.Add(this);
            To.Node.OnInputLinked(this);
            From.Node.OnOutputLinked(this);
            Update();
            TryUpdateValue();
            root.UnregisterCallback<MouseMoveEvent>(MouseMove);
        }

        public static Connection MakeFrom(RootElement root, NodeOut nodeOut)
        {
            var con = new Connection(root)
            {
                From = nodeOut
            };
            nodeOut.Connections.Add(con);
            return con;
        }

        public static Connection MakeTo(RootElement root, NodeIn nodeIn)
        {
            var con = new Connection(root)
            {
                To = nodeIn
            };
            nodeIn.Connections.Add(con);
            return con;
        }

        private void TryUpdateValue()
        {
            if (From != null && To != null)
            {
                To.SetValue(From.Value);
                To.Node.Recalculate();
                root.Window.SetUnsavedChanges();
            }
        }

        public SerializableConnection Serialize()
        {
            return new SerializableConnection
            {
                FromNodeSocket = From.Node.Outputs.IndexOf(From),
                FromNodeIndex = root.Window.Nodes.IndexOf(From.Node),

                ToNodeIndex = root.Window.Nodes.IndexOf(To.Node),
                ToNodeSocket = To.Node.Inputs.IndexOf(To),
            };
        }
    }
}
