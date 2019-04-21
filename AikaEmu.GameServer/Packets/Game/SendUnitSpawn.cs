using System;
using AikaEmu.GameServer.Managers.Configuration;
using AikaEmu.GameServer.Models;
using AikaEmu.GameServer.Models.Base;
using AikaEmu.GameServer.Models.Char.Inventory;
using AikaEmu.GameServer.Network;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Packets.Game
{
	public class SendUnitSpawn : GamePacket
	{
		private readonly BaseUnit _unit;
		private readonly bool _pran2;

		public SendUnitSpawn(BaseUnit unit, bool pran2 = false)
		{
			_unit = unit;
			_pran2 = pran2;

			Opcode = (ushort) GameOpcode.SendUnitSpawn;
			SetSenderIdWithUnit(_unit);
		}

		public override PacketStream Write(PacketStream stream)
		{
			if (_unit is Character character)
			{
				stream.Write(character.Name, 16);

				var equips = character.Inventory.GetItemsBySlotType(SlotType.Equipments);
				stream.Write(equips.ContainsKey(0) ? equips[0].ItemId : (ushort) 0);
				stream.Write(equips.ContainsKey(1) ? equips[1].ItemId : (ushort) 0);
				stream.Write(equips.ContainsKey(2) ? equips[2].ItemId : (ushort) 0);
				stream.Write(equips.ContainsKey(3) ? equips[3].ItemId : (ushort) 0);
				stream.Write(equips.ContainsKey(4) ? equips[4].ItemId : (ushort) 0);
				stream.Write(equips.ContainsKey(5) ? equips[5].ItemId : (ushort) 0);
				stream.Write(equips.ContainsKey(6) ? equips[6].ItemId : (ushort) 0);
				stream.Write(equips.ContainsKey(7) ? equips[7].ItemId : (ushort) 0);
				stream.Write((byte) 0); // accessories
				stream.Write((byte) 0); // accessories
				stream.Write((byte) (equips.ContainsKey(2) ? equips[2].Quantity << 4 : 0));
				stream.Write((byte) (equips.ContainsKey(3) ? equips[3].Quantity << 4 : (ushort) 0));
				stream.Write((byte) 0); // accessories
				stream.Write((byte) (equips.ContainsKey(4) ? equips[4].Quantity << 4 : (ushort) 0));
				stream.Write((byte) (equips.ContainsKey(5) ? equips[5].Quantity << 4 : (ushort) 0));
				stream.Write((byte) (equips.ContainsKey(6) ? equips[6].Quantity << 4 : (ushort) 0));
				stream.Write((byte) (equips.ContainsKey(7) ? equips[7].Quantity << 4 : (ushort) 0));
				stream.WriteCc(3);

				stream.Write(character.Position.CoordX);
				stream.Write(character.Position.CoordY);
				stream.Write((ushort) 0); // rotation (data+64 * 3,1415 / 180)
				stream.Write((ushort) 0);
				stream.Write(character.Hp);
				stream.Write(character.Hp); // TODO - MAX
				stream.Write(character.Mp);
				stream.Write(character.Mp); // TODO - MAX
				stream.Write((byte) 10); // unk - Can go up to 70
				stream.Write((byte) 45); // unk
				stream.Write((byte) 1); //spawnType
				stream.Write(character.BodyTemplate.Width);
				stream.Write(character.BodyTemplate.Chest);
				stream.Write(character.BodyTemplate.Leg);
				stream.Write((ushort) 0);
				stream.Write((ushort) 0);
				stream.Write((ushort) 0);

				// 96
				/*
				 96-216 buff? (120 bytes)
				 216-456 ?    (240 bytes)
				 */
				stream.Write("", 80);
				stream.Write((ushort) 0); // first? buff id
				stream.Write("", 38);

				stream.Write("", 240);

				stream.Write("", 32); // title
				stream.Write(0);

				stream.Write((ushort) 0); // unk (byte)
				stream.Write((ushort) 0); // unk
				stream.Write((ushort) 0); // unk (byte)
				stream.Write((ushort) 0); // unk (byte) >0 <64 unique behavior
				stream.Write(0);
				stream.Write(0); // test maybe title class
			}
			else if (_unit is Npc npc)
			{
				stream.Write(Convert.ToString(npc.NpcId), 16);
				// if 221 (Tower) different behavior
				stream.Write(DataManager.Instance.MobEffectsData.GetFace(npc.NpcId));

				stream.Write("", 26); // if Weapon == 1501 unique behavior

				stream.Write(npc.Position.CoordX + 0.4f);
				stream.Write(npc.Position.CoordY + 0.4f);

				stream.Write(180); // unk

				stream.Write(npc.Hp);
				stream.Write(npc.MaxHp);
				stream.Write(npc.Mp);
				stream.Write(npc.MaxMp);
				stream.Write((byte) 0); // unk - Can go up to 70
				stream.Write((byte) 40); // unk
				stream.Write((byte) 0); // spawnType (pran is usualy 2)
				stream.Write(npc.BodyTemplate.Width);
				stream.Write(npc.BodyTemplate.Chest);
				stream.Write(npc.BodyTemplate.Leg);
				stream.Write((byte) 160); // body?
				stream.Write((byte) 1); // unk
				stream.Write((byte) 1); // unk
				stream.Write("", 363);
				stream.Write("TODO", 32);
				stream.Write(0); // unk
				stream.Write(12);
				stream.Write("", 12);
			}
			else if (_unit is Pran pran)
			{
				stream.Write(pran.Name, 16);

				var equips = pran.Account.ActiveCharacter.Inventory.GetItemsBySlotType(SlotType.PranEquipments);
//				stream.Write(equips.ContainsKey(0) ? equips[0].ItemId : (ushort) 0);
				stream.Write((ushort) 105);
				stream.Write(equips.ContainsKey(1) ? equips[1].ItemId : (ushort) 9866); // TODO - Remove placeholder items
				stream.Write(equips.ContainsKey(2) ? equips[2].ItemId : (ushort) 6267);
				stream.Write(equips.ContainsKey(3) ? equips[3].ItemId : (ushort) 9909);
				stream.Write(equips.ContainsKey(4) ? equips[4].ItemId : (ushort) 0);
				stream.Write(equips.ContainsKey(5) ? equips[5].ItemId : (ushort) 0);
				stream.Write(equips.ContainsKey(6) ? equips[6].ItemId : (ushort) 151);
				stream.Write(equips.ContainsKey(7) ? equips[7].ItemId : (ushort) 0);
				stream.Write("", 12);

				stream.Write(pran.Position.CoordX);
				stream.Write(pran.Position.CoordY);
				stream.Write(0);

				stream.Write(pran.Hp);
				stream.Write(pran.MaxHp);
				stream.Write(pran.Mp);
				stream.Write(pran.MaxMp);

				stream.Write((byte) 0); // unk 
				stream.Write((byte) 61); // unk fixed value
				stream.Write((byte) (_pran2 ? 1 : 0)); // (pran is usualy 2) spawnType

				stream.Write(pran.BodyTemplate.Width);
				stream.Write(pran.BodyTemplate.Chest);
				stream.Write(pran.BodyTemplate.Leg);
				stream.Write(pran.ConnectionId);

				stream.Write("", 204);
				stream.Write(24);
				stream.Write("", 156);

				stream.Write(pran.Account.ActiveCharacter.Name + "'s Pran", 32);
				stream.Write("", 20);
			}


			return stream;
		}
	}
}