using AikaEmu.GameServer.Managers;
using AikaEmu.GameServer.Models;
using AikaEmu.GameServer.Models.Account;
using AikaEmu.GameServer.Network.AuthServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Network.Packets.AG
{
    public class RequestEnterResult : AuthGamePacket
    {
        protected override void Read(PacketStream stream)
        {
            var accId = stream.ReadUInt32();
            var conId = stream.ReadUInt32();
            var result = stream.ReadByte();

            if (result == 1)
            {
                var accConnection = ConnectionsManager.Instance.GetConnection(conId);
                if (accConnection == null) return;

                var newAccount = new Account(accId, accConnection);
                if (!AccountsManager.Instance.AddAccount(newAccount)) return;

                accConnection.Account = newAccount;
                newAccount.SendCharacterList();
                Log.Debug("RequestEnterResult: success.");
            }
            else
            {
                Log.Debug("RequestEnterResult: failure.");
            }
        }
    }
}