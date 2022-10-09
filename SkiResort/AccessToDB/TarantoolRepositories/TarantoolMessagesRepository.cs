using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;
using ProGaudi.Tarantool.Client.Model.UpdateOperations;

using BL;
using BL.Models;
using BL.IRepositories;
using AccessToDB.Converters;
using AccessToDB.Exceptions.MessageExceptions;

namespace AccessToDB.RepositoriesTarantool
{
    public class TarantoolMessagesRepository : IMessagesRepository
    {
        private ISpace _space;
        private IIndex _indexPrimary;
        private IIndex _indexSenderID;
        private IIndex _indexCheckedByID;
        private IBox _box;

        public TarantoolMessagesRepository(TarantoolContext context)
        {
            _space = context.messagesSpace;
            _indexPrimary = context.messagesIndexPrimary;
            _indexSenderID = context.messagesIndexSenderID;
            _indexCheckedByID = context.messagesIndexCheckedByID;
            _box = context.box;
        }


        public async Task<List<Message>> GetMessagesAsync(uint offset = 0u, uint limit = 0)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, MessageDB>
                (ValueTuple.Create(0u), new SelectOptions { Iterator = Iterator.Ge });

            List<Message> result = new();

            for (uint i = offset; i < (uint)data.Data.Length && (i < limit || limit == 0); i++)
            {
                result.Add(MessageConverter.DBToBL(data.Data[i]));
            }

            return result;
        }

        public async Task<List<Message>> GetMessagesBySenderIdAsync(uint senderID)
        {
            var data = await _indexSenderID.Select<ValueTuple<uint>, MessageDB>
                (ValueTuple.Create(senderID));

            List<Message> result = new();

            foreach (var item in data.Data)
            {
                Message message = MessageConverter.DBToBL(item);
                result.Add(message);
            }

            return result;
        }

        public async Task<List<Message>> GetMessagesByCheckerIdAsync(uint checkerId)
        {
            var data = await _indexCheckedByID.Select<ValueTuple<uint>, MessageDB>
                (ValueTuple.Create(checkerId));

            List<Message> result = new();

            foreach (var item in data.Data)
            {
                Message message = MessageConverter.DBToBL(item);
                result.Add(message);
            }

            return result;
        }

        public async Task<Message> GetMessageByIdAsync(uint MessageID)
        {
            var data = await _indexPrimary.Select<ValueTuple<uint>, MessageDB>
                (ValueTuple.Create(MessageID));

            if (data.Data.Length != 1)
            {
                throw new MessageNotFoundException();
            }

            return MessageConverter.DBToBL(data.Data[0]);
        }

        public async Task AddMessageAsync(uint messageID, uint senderID, uint checkedByID, string text)
        {
            try
            {
                await _space.Insert(new MessageDB(messageID, senderID, checkedByID, text));
            }
            catch (Exception ex)
            {
                throw new MessageAddException();
            }
        }

        public async Task<uint> AddMessageAutoIncrementAsync(uint senderID, uint checkedByID, string text)
        {
            try
            {
                var result = await _box.Call_1_6<MessageDBNoIndex, MessageDB>("auto_increment_messages", (new MessageDBNoIndex(senderID, checkedByID, text)));
                return MessageConverter.DBToBL(result.Data[0]).MessageID;
            }
            catch (Exception ex)
            {
                throw new MessageAddAutoIncrementException();
            }
        }
        public async Task UpdateMessageByIDAsync(uint messageID, uint newSenderID, uint newCheckedByID, string newText)
        {
            var response = await _space.Update<ValueTuple<uint>, MessageDB>(
                ValueTuple.Create(messageID), new UpdateOperation[] {
                    UpdateOperation.CreateAssign<uint>(1, newSenderID),
                    UpdateOperation.CreateAssign<uint>(2, newCheckedByID),
                    UpdateOperation.CreateAssign<string>(3, newText),
                });

            if (response.Data.Length != 1)
            {
                throw new MessageUpdateException();
            }
        }

        public async Task DeleteMessageByIDAsync(uint messageID)
        {
            var response = await _indexPrimary.Delete<ValueTuple<uint>, MessageDB>
                (ValueTuple.Create(messageID));

            if (response.Data.Length != 1)
            {
                throw new MessageDeleteException();
            }

        }
    }
}
