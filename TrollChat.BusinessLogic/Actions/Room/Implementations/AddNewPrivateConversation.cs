﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Room.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.Room.Implementations
{
    public class AddNewPrivateConversation : IAddNewPrivateConversation
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserRoomRepository userRoomRepository;
        private readonly IDomainRepository domainRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNewPrivateConversation(IRoomRepository roomRepository,
            IUserRepository userRepository,
            IUserRoomRepository userRoomRepository,
            IDomainRepository domainRepository, IUnitOfWork unitOfWork)
        {
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
            this.userRoomRepository = userRoomRepository;
            this.domainRepository = domainRepository;
            _unitOfWork = unitOfWork;
        }

        public RoomModel Invoke(Guid issuerUserId, List<Guid> users)
        {
            // Check if users wants to create a conversation with himself
            if (issuerUserId == Guid.Empty || users.Count <= 0 || users.Count > 8 || users.Any(x => x.Equals(issuerUserId) || x.Equals(Guid.Empty)) || users.Distinct().Count() != users.Count)
            {
                return null;
            }

            var issuerUser = userRepository.GetById(issuerUserId);

            if (issuerUser == null)
            {
                return null;
            }

            var privateConversationList = userRepository.GetPrivateConversationsTargets(issuerUserId).ToList();
            var listRoom = privateConversationList.Select(x => x.Room).Distinct();

            // Check if private conversation already exists
            foreach (var room in listRoom)
            {
                var userRoomsList = privateConversationList.Where(x => x.Room == room);
                var dict = new Dictionary<DataAccess.Models.UserRoom, bool>();
                var searchedCount = 0;

                // FIXME: Bug when creating multiple users priv conversation room when there's priv conversation to one of selected user list
                foreach (var connection in userRoomsList)
                {
                    dict.Add(connection, false);

                    if (users.Any(x => x.Equals(connection.User.Id)))
                    {
                        searchedCount += 1;
                        Debug.WriteLine("######### Znaleziono istniejące połączenie #########");
                        Debug.WriteLine(connection.User.Name);
                        dict[connection] = true;
                    }
                }

                if (searchedCount == users.Count)
                {
                    Debug.WriteLine("######### Jest tyle samo znalezień co userów do których chce sie podpiąć #########");
                }

                if (dict.All(x => x.Value))
                {
                    return null;
                }
            }

            // Create repository method for that?
            var userList = userRepository.GetAll().Where(x => users.Contains(x.Id)).ToList();

            var roomName = "";

            foreach (var item in userList)
            {
                roomName += item.Name + ", ";
            }

            roomName += issuerUser.Name;

            var userDomain = domainRepository.GetDomainByUserId(issuerUser.Id);

            var newRoom = new DataAccess.Models.Room
            {
                Name = roomName,
                Owner = AutoMapper.Mapper.Map<DataAccess.Models.User>(issuerUser),
                Domain = userDomain,
                IsPrivateConversation = true,
                IsPublic = false
            };

            roomRepository.Add(newRoom);

            var userRoom = new DataAccess.Models.UserRoom { User = issuerUser, Room = newRoom };
            userRoomRepository.Add(userRoom);

            foreach (var user in userList)
            {
                var userRoom2 = new DataAccess.Models.UserRoom { User = user, Room = newRoom };
                userRoomRepository.Add(userRoom2);
            }
            
            var returnRoom = AutoMapper.Mapper.Map<RoomModel>(newRoom);
            _unitOfWork.Save();

            return returnRoom;
        }
    }
}