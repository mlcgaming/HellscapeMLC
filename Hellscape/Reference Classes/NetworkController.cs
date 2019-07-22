using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Hellscape.Reference_Classes
{
    public static class NetworkController
    {
        static NetPeerConfiguration NetConfig;
        static NetServer NetServer;

        enum MessageType
        {
            ActorPlayerMove,
            ActorPlayerInteract,
            ActorPlayerRun,
            ActorPlayerWalk,
            ActorPlayerJump,
            SyncClients
        }

        static void InitializeNetwork()
        {
            NetConfig = new NetPeerConfiguration("HellscapeMLC") { Port = 25990 };
            NetServer = new NetServer(NetConfig);
            NetServer.Start();
        }

        static void ReadMessage()
        {
            NetIncomingMessage msg;
            while((msg = NetServer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                        {

                            break;
                        }
                    case NetIncomingMessageType.DebugMessage:
                        {

                            break;
                        }
                    case NetIncomingMessageType.WarningMessage:
                        {

                            break;
                        }
                    case NetIncomingMessageType.ErrorMessage:
                        {

                            break;
                        }
                    case NetIncomingMessageType.Data:
                        {
                            byte msgType = msg.ReadByte();
                            
                            switch (msgType)
                            {
                                case (byte)MessageType.ActorPlayerInteract:
                                    {
                                        // Read Interact from Other Player
                                        // Includes TileEntitySceneObject being Referenced

                                        break;
                                    }
                                case (byte)MessageType.ActorPlayerJump:
                                    {
                                        // Read Jump Key from Other Player
                                        // Includes VelocityX, VelocityY, Facing

                                        break;
                                    }
                                case (byte)MessageType.ActorPlayerMove:
                                    {
                                        // Read Movement from Other Player
                                        // Includes VelocityX, VelocityY, Facing

                                        break;
                                    }
                                case (byte)MessageType.ActorPlayerRun:
                                    {
                                        // Read Run Activation from Other Player

                                        break;
                                    }
                                case (byte)MessageType.ActorPlayerWalk:
                                    {
                                        // Read Run DeActivation from Other Player

                                        break;
                                    }
                                case (byte)MessageType.SyncClients:
                                    {
                                        // Syncs Position, Facing of Other Player
                                        string compaionLocationShortname = msg.ReadString();
                                        float companionX = msg.ReadFloat();
                                        float companionY = msg.ReadFloat();
                                        float companionVelocityX = msg.ReadFloat();
                                        float companionVelocityY = msg.ReadFloat();
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {

                            break;
                        }
                }

                NetServer.Recycle(msg);
            }
        }
    }
}
